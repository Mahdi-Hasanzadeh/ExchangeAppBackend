using API.DataContext;
using API.Services;
using API.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Shared;
using Shared.Contract;
using Shared.Contract.User;
using Shared.DTOs;
using Shared.DTOs.UserDTOs;
using Shared.Models;
using Shared.Roles;
using Shared.View_Model;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Xml.Linq;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly ILogger<UserController> _logger;
        //private readonly IGenericRepository<UserEntity> _genericRepository;
        private readonly IUserRepository _userRepo;
        private readonly IConfiguration _configuration;

        public UserController(ILogger<UserController> logger,
            //IGenericRepository<UserEntity> genericRepository,
            IUserRepository userRepo, IConfiguration configuration)
        {
            _logger = logger;
            //_genericRepository = genericRepository;
            _userRepo = userRepo;
           _configuration = configuration;
        }

        [HttpGet("{id:int}", Name = "GetUserById")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<UserEntity>>> GetUserById(int id)
        {
            try
            {
                var user = await _userRepo.GetUserByIdAsync(id);

                if (user == null)
                {
                    _logger.LogInformation("user not found");
                    return NotFound(new ApiResponse<UserEntity>(false, "User not found", nameof(ResourceStringsError.UserNotFound)));
                }

                return Ok(new ApiResponse<UserEntity>(true, "User found", null, user));
            }

            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<UserEntity>(false, "Something gone wrong", nameof(ResourceStringsError.ServerError)));
            }

        }

        [HttpPost("register", Name = "RegisterUser")]
        public async Task<ActionResult<ApiResponse<UserEntity>>> RegisterUser(RegisterViewModel newUser)
        {
            var apiResponse = new ApiResponse<UserEntity>(false);
            try
            {

                // Check for duplicate username
                var existingUser = await _userRepo.GetUserByUsernameAsync(newUser.Username);
                if (existingUser != null)
                {
                    apiResponse.Message = "نام کاربری وجود دارد";
                    apiResponse.ErrorCode = nameof(ResourceStringsError.UsernameAlreadyExist);
                    return Ok(apiResponse);
                }

                // Optional: Check for email existence if required
                // var existingEmailUser = await _userRepo.GetUserByEmailAsync(newUser.Email);
                // if (existingEmailUser != null) { ... }

                // Hash the password
                string hashedPassword = newUser.Password.HashPassword();

                var userEntity = new UserEntity
                {
                    Username = newUser.Username,
                    Password = hashedPassword,
                    Role = UserRole.SuperAdmin,
                    CreatedDate = DateTime.Now,
                    ValidUntil = DateTime.Now.AddDays(100)
                };

                var added = await _userRepo.AddUserAsync(userEntity);
                if (!added)
                {
                    apiResponse.Message = "خطا در ثبت کاربر";
                    apiResponse.ErrorCode = nameof(ResourceStringsError.ServerError);
                    return Ok(apiResponse);
                }

                await _userRepo.SaveAsync();

                // Return user info without password
                userEntity.Password = string.Empty;

                apiResponse.Success = true;
                apiResponse.Data = userEntity;
                apiResponse.Message = "کاربر با موفقیت ثبت شد";
                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                apiResponse.Message = ex.Message;
                apiResponse.Success = false;
                return StatusCode(StatusCodes.Status500InternalServerError, apiResponse);
                //return StatusCode(StatusCodes.Status500InternalServerError,
                //    new ApiResponse<UserEntity>(false, ex.Message, nameof(ResourceStringsError.ServerError)));
            }
        }

        [HttpPost("registerSubUser", Name = "RegisterSubUser")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<bool>>> RegisterSubUser(SubUserDTO subUser)
        {
            var apiResponse = new ApiResponse<bool>(false);
            try
            {
                var currentUserId = User.GetUserId();

                var subUserExist = await _userRepo.GetUserByUsernameAsync(subUser.Username);
                if (subUserExist != null)
                {
                    apiResponse.Message = "نام کاربری وجود دارد";
                    apiResponse.ErrorCode = nameof(ResourceStringsError.UsernameAlreadyExist);
                    return Ok(apiResponse);
                }


                //var subUserExist = await _userRepo.GetUserByUsernameAsync(subUser.Username);

                //if (subUserExist != null)
                //{
                //    apiResponse.Message = "نام کاربری وجود دارد";
                //    apiResponse.ErrorCode = nameof(ResourceStringsError.UsernameAlreadyExist);
                //    return Ok(apiResponse);
                //}

                var ownerInfo = await _userRepo.GetUserByIdAsync(currentUserId);

                string hashedPassword = subUser.Password.HashPassword();

                subUser.Password = hashedPassword;

                var userEntity = new UserEntity()
                {
                    Username = subUser.Username,
                    Password = subUser.Password,
                    Role = subUser.Role,
                    Email = subUser.Email,
                    Image = subUser.Image,
                    ParentUserId = currentUserId,
                    CreatedDate = DateTime.Now
                };

                var addedUser = await _userRepo.AddUserAsync(userEntity);
                if (!addedUser)
                {
                    apiResponse.Message = "خطا در ثبت کاربر";
                    return Ok(apiResponse);
                }

                await _userRepo.SaveAsync();
                apiResponse.Success = true;
                apiResponse.Message = "کاربر با موفقیت ثبت شد";
                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                apiResponse.Message = ex.Message;
                apiResponse.Success = false;
                return StatusCode(StatusCodes.Status500InternalServerError, apiResponse);
            }
        }

        [HttpPost("login", Name = "LoginUser")]
        public async Task<ActionResult<ApiResponse<UserEntity>>> LoginUser([FromBody] LoginViewModel loginModel)
        {
            var apiResponse = new ApiResponse<UserEntity>(false);
            try
            {

                var user = await _userRepo.GetUserByUsernameAsync(loginModel.Username);
                if (user == null)
                {
                    apiResponse.Message = "کاربر یافت نشد";
                    apiResponse.ErrorCode = nameof(ResourceStringsError.UserNotFound);
                    return NotFound(apiResponse);
                }

                if (user.Username != loginModel.Username || !loginModel.Password.VerifyPassword(user.Password))
                {
                    apiResponse.Message = "نام کاربری یا رمز عبور اشتباه است";
                    apiResponse.ErrorCode = nameof(ResourceStringsError.InvalidCredentials);
                    return BadRequest(apiResponse);
                }

                bool isUserIsSubUser = false;
                UserEntity parentUser = null;

                if (user.ParentUserId != null)
                {
                    isUserIsSubUser = true;
                    parentUser = await _userRepo.GetUserByIdAsync((int)user.ParentUserId);
                }

                string connectionString = isUserIsSubUser ? parentUser?.ConnectionString! : user?.ConnectionString!;

                string isFirstTimeLogin = isUserIsSubUser ? "false" : user?.isFirstTimeLogin.ToString()!;

                string id = isUserIsSubUser ? parentUser?.Id.ToString()! : user?.Id.ToString()!;

                var claims = new List<Claim>
                     {
                        new Claim(ClaimTypes.Name,user?.Username!),
                        new Claim(ClaimTypes.Role,user?.Role!.ToString()!),
                        new Claim(ClaimTypes.NameIdentifier,id),
                        //new Claim("ConnectionString", connectionString), // Store connection string in JWT
                        new Claim("IsFirstTimeLogin",isFirstTimeLogin),
                };

                var issuer = _configuration["JwtSettings:Issuer"];
                var audience = _configuration["JwtSettings:Audience"];
                var secretKey = _configuration["JwtSettings:SecretKey"];

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);


                var jwtToken = new JwtSecurityToken(
                    issuer: issuer,
                    audience: audience,
                    claims: claims,
                    expires: DateTime.Now.AddHours(8),
                    signingCredentials: creds);
                var token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
                //if (user.isFirstTimeLogin)
                //{
                //    user.isFirstTimeLogin = false;
                //    await _userRepo.SaveAsync();
                //}
                return Ok(new ApiResponse<string>(true, "Token created", null, token));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<UserEntity>(false, ex.Message, nameof(ResourceStringsError.ServerError)));
            }
        }

        [HttpPut("update", Name = "UpdateUser")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<object>>> UpdateUser([FromBody] UpdateInfoViewModel updateInfoModel)
        {
            try
            {
                var userInfo = await _userRepo.GetUserByIdAsync(updateInfoModel.Id);
                if (userInfo == null)
                {
                    return NotFound(new ApiResponse<UserEntity>(false, "User not found",
                        nameof(ResourceStringsError.UserNotFound)));
                }
                //if (userInfo.Username == updateInfoModel.Username
                //    && userInfo.Email == updateInfoModel.Email
                //    && userInfo.PictureUrl == updateInfoModel.PictureUrl
                //    )
                //{
                //    return Ok(new ApiResponse<UserEntity>(false, "Nothing Changed",
                //                            nameof(ResourceStringsError.NoChangesToUserAccount)));
                //}
                if (userInfo.Username != updateInfoModel.Username)
                {
                    var user = await _userRepo.GetUserByUsernameAsync(updateInfoModel.Username);
                    if (user != null)
                    {
                        return BadRequest(new ApiResponse<UserEntity>(false, "Username is not available",
                            nameof(ResourceStringsError.UsernameAlreadyExist)));
                    }
                }
                //if (userInfo.Email != updateInfoModel.Email)
                //{
                //    var user = await _userRepo.GetUserByEmailAsync(updateInfoModel.Email);
                //    if (user != null)
                //    {
                //        return BadRequest(new ApiResponse<UserEntity>(false, "Email address is not available",
                //            nameof(ResourceStringsError.EmailAlreadyExist)));
                //    }
                //}

                //if (!updateInfoModel.Password.VerifyPassword(userInfo.Password))
                //{
                //    return BadRequest(new ApiResponse<UserEntity>(false, "Username or Password is wrong",
                //        nameof(ResourceStringsError.InvalidCredentials)));
                //}


                userInfo.Password = updateInfoModel.Password.HashPassword();
                userInfo.Username = updateInfoModel.Username;
                userInfo.Email = updateInfoModel.Email;
                userInfo.PictureUrl = updateInfoModel.PictureUrl;

                await _userRepo.SaveAsync();

                return Ok(new ApiResponse<UserEntity>(true, "User account updated", nameof(ResourceStringsSuccessMessage.AccountUpdated)));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<UserEntity>(false, ex.Message, nameof(ResourceStringsError.ServerError)));
            }
        }

        [HttpGet("getSubUsers/{parentId:int}", Name = "GetSubUsers")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<List<SubUserDTO>>>> GetSubUsers(int parentId)
        {
            var apiResponse = new ApiResponse<List<SubUserDTO>>(false);
            try
            {
                var subUsers = await _userRepo.GetAllUsersByParentUserIdAsync(parentId);
                if (subUsers == null)
                {
                    apiResponse.Message = "اطلاعاتی وجود ندارد";

                    return NotFound(apiResponse);
                }

                var subUserDtos = subUsers.Select(u => new SubUserDTO
                {
                    Id = u.Id,
                    Username = u.Username,
                    Role = u.Role,
                    Email = u.Email,
                    Image = u.Image,
                    ParentUserId = u.ParentUserId,
                    CreatedDate = u.CreatedDate,
                    LastModifiedDate = u.LastModifiedDate
                }).ToList();

                apiResponse.Success = true;
                apiResponse.Message = "اطلاعات با موفقیت دریافت شد.";
                apiResponse.Data = subUserDtos;

                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                apiResponse.Success = false;
                apiResponse.Message = ex.Message;
                return StatusCode(StatusCodes.Status500InternalServerError, apiResponse);
            }
        }

        [HttpPut("updateSubUser/{userId:int}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<bool>>> UpdateSubUser(int userId, EditSubUserDTO subUserDTO)
        {
            var apiResponse = new ApiResponse<bool>(false);
            try
            {
                var currentUserId = User.GetUserId();
                var subUser = await _userRepo.GetUserByIdAsync(userId);
                if (subUser == null)
                {
                    apiResponse.Message = "کاربر یافت نشد";
                    apiResponse.ErrorCode = nameof(ResourceStringsError.UserNotFound);
                    return Ok(apiResponse);
                }
                if (subUser.ParentUserId != currentUserId)
                {
                    apiResponse.Message = "شما اجازه ویرایش این کاربر را ندارید";
                    return Ok(apiResponse);
                }
                if (subUser.Username != subUserDTO.Username)
                {
                    var user = await _userRepo.GetUserByUsernameAsync(subUserDTO.Username);
                    if (user != null)
                    {
                        apiResponse.Message = "نام کاربری وجود دارد";
                        apiResponse.ErrorCode = nameof(ResourceStringsError.UsernameAlreadyExist);
                        return Ok(apiResponse);
                    }
                }

                subUser.Username = subUserDTO.Username;
                subUser.Email = subUserDTO.Email;
                subUser.Image = subUserDTO.Image;
                subUser.Role = subUserDTO.Role;
                subUser.LastModifiedDate = DateTime.Now;
                if (!string.IsNullOrEmpty(subUserDTO.Password))
                {
                    subUser.Password = subUserDTO.Password.HashPassword();
                }

                await _userRepo.SaveAsync();
                apiResponse.Success = true;
                apiResponse.Message = "کاربر با موفقیت ویرایش شد";
                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                apiResponse.Message = ex.Message;
                return StatusCode(StatusCodes.Status500InternalServerError, apiResponse);
            }
        }

        [HttpDelete("deleteSubUser/{userId:int}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<bool>>> RemoveSubUser(int userId)
        {
            var apiResponse = new ApiResponse<bool>(false);
            try
            {
                var currentUserId = User.GetUserId();
                var subUser = await _userRepo.GetUserByIdAsync(userId);
                if (subUser == null)
                {
                    apiResponse.Message = "کاربر یافت نشد";
                    return Ok(apiResponse);
                }
                if (subUser.ParentUserId != currentUserId)
                {
                    apiResponse.Message = "شما اجازه حذف این کاربر را ندارید";
                    return Ok(apiResponse);
                }
                var isDeleted = await _userRepo.DeleteUserById(userId);
                if (!isDeleted)
                {
                    apiResponse.Message = "خطا در حذف کاربر";
                    return Ok(apiResponse);
                }

                await _userRepo.SaveAsync();
                apiResponse.Success = true;
                apiResponse.Message = "کاربر با موفقیت حذف شد";
                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                apiResponse.Message = ex.Message;
                return StatusCode(StatusCodes.Status500InternalServerError, apiResponse);
            }
        }

        #region This code is neede for multi-tenant app

        //[HttpPost("register", Name = "RegisterUser")]
        //public async Task<ActionResult<ApiResponse<UserEntity>>> RegisterUser(RegisterViewModel newUser)
        //{
        //    try
        //    {
        //        await _userRepo.BeginTransaction();
        //        _logger.LogInformation("Regitser User in invoked");
        //        var user = await _userRepo.GetUserByUsernameAsync(newUser.Username);
        //        if (user != null)
        //        {
        //            return Ok(new ApiResponse<UserEntity>(false, "Username already exist", nameof(ResourceStringsError.UsernameAlreadyExist)));
        //        }


        //        //if (!string.IsNullOrEmpty(newUser.Email))
        //        //{
        //        //    user = await _userRepo.GetUserByEmailAsync(newUser.Email);
        //        //    if (user != null)
        //        //    {
        //        //        return Ok(new ApiResponse<UserEntity>(false, "Email already exist", nameof(ResourceStringsError.EmailAlreadyExist)));
        //        //    }
        //        //}



        //        string hashedPassword = newUser.Password.HashPassword();

        //        // Connection string using the provided server and admin credentials
        //        var connectionString = $"Server={newUser.ServerName};User Id={newUser.Username};Password={newUser.Password};Trusted_Connection=true;TrustServerCertificate=true";

        //        // Create a new database for the user
        //        var dbName = $"ExchangeDB_{newUser.Username}";
        //        //using (var connection = new SqlConnection(connectionString))
        //        //{
        //        //    await connection.OpenAsync();


        //        //    var createDbCommand = new SqlCommand($"CREATE DATABASE {dbName}", connection);

        //        //    await createDbCommand.ExecuteNonQueryAsync();
        //        //}

        //        // Now, change the connection string to point to the newly created database
        //        var userDbConnectionString = $"Server={newUser.ServerName};Database={dbName};User Id={newUser.Username};Password={newUser.Password};Trusted_Connection=true;TrustServerCertificate=true";

        //        // Use the DbContext to apply migrations to the newly created database
        //        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        //        optionsBuilder.UseSqlServer(userDbConnectionString);
        //        UserDatabaseService userDatabaseService = new UserDatabaseService();
        //        userDatabaseService.SetConnectionString(userDbConnectionString);
        //        using (var dbContext = new AppDbContext(optionsBuilder.Options, userDatabaseService))
        //        {
        //            //// Ensure the database exists (without recreating it)
        //            //await dbContext.Database.EnsureCreatedAsync();

        //            // Apply migrations (only if needed)
        //            //await dbContext.Database.BeginTransactionAsync();
        //            await dbContext.Database.MigrateAsync();

        //            // add data to database

        //        }

        //        // Change the Password to hashedPassword for security
        //        newUser.Password = hashedPassword;

        //        //newUser.Role = string.IsNullOrEmpty(newUser.Role) ? nameof(Roles.Guest) : newUser.Role;

        //        var userEntity = new UserEntity()
        //        {
        //            Username = newUser.Username,
        //            Password = newUser.Password,
        //            Role = UserRole.SuperAdmin,
        //            ServerAddress = newUser.ServerName,
        //            Databasename = $"ExchangeDB_{newUser.Username}",
        //            ConnectionString = userDbConnectionString,
        //            CreatedDate = DateTime.Now,
        //            ValidUntil = DateTime.Now.AddDays(1),
        //        };

        //        var addedUser = await _userRepo.AddUserAsync(userEntity);
        //        if (!addedUser)
        //        {
        //            await _userRepo.RollbackTransaction();
        //        }

        //        await _userRepo.SaveAsync();
        //        await _userRepo.CommitTransaction();
        //        UserEntity createdUser = new UserEntity()
        //        {
        //            Password = string.Empty,
        //            ConnectionString = userDbConnectionString,
        //            Username = newUser.Username,
        //        };
        //        return Ok(new ApiResponse<UserEntity>
        //            (true, "User created successfully", null, createdUser));
        //    }
        //    catch (Exception ex)
        //    {
        //        await _userRepo.RollbackTransaction();
        //        _logger.LogError(ex.Message);
        //        return StatusCode(StatusCodes.Status500InternalServerError,
        //            new ApiResponse<UserEntity>(false, ex.Message, nameof(ResourceStringsError.ServerError)));
        //    }
        //}

        #endregion

        private static List<ClaimDto> GenerateClaims(UserEntity? user)
        {
            var claims = new List<Claim>
                     {
                        new Claim(ClaimTypes.Name,user?.Username!),
                        new Claim(ClaimTypes.Role,user?.Role!.ToString()),
                        new Claim(ClaimTypes.NameIdentifier,user?.Id.ToString()!)
                     };
            var claimDtos = claims.Select(c => new ClaimDto { Type = c.Type, Value = c.Value }).ToList();
            return claimDtos;
        }
    }
}
