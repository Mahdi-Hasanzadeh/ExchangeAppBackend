using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared;
using Shared.Contract;
using Shared.Contract.CustomerAccount;
using Shared.Contract.User;
using Shared.DTOs;
using Shared.DTOs.CurrencyDTOs;
using Shared.DTOs.CustomerDTOs;
using Shared.DTOs.GeneralLedgerDTOs;
using Shared.Enums;
using Shared.Models;
using Shared.Models.Currency;
using Shared.View_Model.CustomerAccount;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerAccountController : ControllerBase
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IGenericRepository<CustomerAccount> _genericRepository;
        private readonly IGenericRepository<CustomerBalance> _genericRepositoryCustomerBalance;
        private readonly IGenericRepository<CurrencyEntity> _genericCurrencyEntity;
        private readonly ILogger _logger;
        private readonly ICustomerAccountRepo _customerAccountRepo;
        private readonly ICurrencyRepository _currencyRepository;
        private readonly IGenericRepository<CustomerTransactionHistory> _transactionGenericRepository;
        private readonly IGenericRepository<CurrencyExchangeRate> _genericCurrencyExchangeRate;
        private readonly IUserRepository userRepo;

        public CustomerAccountController(
            IAuthorizationService authorizationService,
            IGenericRepository<CustomerAccount> genericRepository,
            IGenericRepository<CustomerBalance> genericRepositoryCustomerBalance,
            IGenericRepository<CurrencyEntity> genericCurrencyEntity,
            ILogger<CustomerAccountController> logger,
            ICustomerAccountRepo customerAccountRepo,
            ICurrencyRepository currencyRepository,
            IGenericRepository<CustomerTransactionHistory> transactionGenericRepository,
            IGenericRepository<CurrencyExchangeRate> genericCurrencyExchangeRate,
            IUserRepository userRepo
            )
        {
            _genericRepositoryCustomerBalance = genericRepositoryCustomerBalance;
            _genericCurrencyEntity = genericCurrencyEntity;
            _authorizationService = authorizationService;
            _genericRepository = genericRepository;
            _logger = logger;
            _customerAccountRepo = customerAccountRepo;
            _currencyRepository = currencyRepository;
            _transactionGenericRepository = transactionGenericRepository;
            _genericCurrencyExchangeRate = genericCurrencyExchangeRate;
            this.userRepo = userRepo;
        }


        [HttpPost(Name = "CreateCustomerAccount")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<CustomerAccountSummaryDTO>>> CreateAccountForCustomer(CreateCustomerAccountViewModel newCustomerAccount)
        {
            var apiResponse = new ApiResponse<CustomerAccountSummaryDTO>(false);
            if (newCustomerAccount.AccountType == eAccountType.Treasury)
            {
                // check the treasury exist
                var exist = await _customerAccountRepo.IsTreasuryAccountExist(newCustomerAccount.UserId);
                if (exist)
                {
                    apiResponse.Message = "حساب صندوق در سیستم وجود دارد.";
                    return Ok(apiResponse);
                }
            }

            if (newCustomerAccount.AccountType == eAccountType.CurrencyExchange)
            {
                // check the currencyExchange account exist
                var accountId = await _customerAccountRepo.IsCurrencyExchangeAccountExist(newCustomerAccount.UserId);
                if (accountId != 0)
                {
                    apiResponse.Message = "حساب تبادله اسعار در سیستم وجود دارد.";
                    return Ok(apiResponse);
                }
            }

            try
            {

                await _genericRepository.BeginTransactionAsync(); // Start transaction

                int lastUsedAccountNumber;

                var result = await _customerAccountRepo.GetLastUsedAccountNumber(newCustomerAccount.UserId);

                if (result == null)
                {
                    // initialize the table

                    await _customerAccountRepo.InitializeAccountNumberSequenceTable(newCustomerAccount.UserId);
                    await _customerAccountRepo.SaveAsync();
                    lastUsedAccountNumber = 0;
                }
                else
                {
                    lastUsedAccountNumber = (int)result;
                }

                int accountNumber = lastUsedAccountNumber + 1;

                // Update the account number sequence
                await _customerAccountRepo.UpdateLastUsedAccountNumber(accountNumber, newCustomerAccount.UserId);

                var customer = new CustomerAccount()
                {
                    Name = newCustomerAccount.Name,
                    Lastname = newCustomerAccount.Lastname,
                    AccountNumber = accountNumber,
                    AccountType = newCustomerAccount.AccountType,
                    BorrowAmount = newCustomerAccount.BorrowAmount,
                    Mobile = newCustomerAccount.Mobile,
                    UserId = newCustomerAccount.UserId,
                    IDCardNumber = newCustomerAccount.IDCardNumber,
                    PassportNumber = newCustomerAccount.PassportNumber,
                    Image = newCustomerAccount.Image
                };

                var isAccountCreated = await _genericRepository.AddAsync(customer);

                if (!isAccountCreated)
                {
                    await _genericRepository.RollbackTransactionAsync();
                    apiResponse.Message = "حساب مشتری ساخته نشد";
                    return BadRequest(apiResponse);
                }

                await _genericRepository.SaveAsync();
                await _genericRepository.CommitTransactionAsync(); // Commit transaction
                apiResponse.Success = true;
                apiResponse.Message = "حساب مشتری با موفقیت ساخته شد.";
                apiResponse.Data = customer.ToCustomerAccountSummaryDTO();
                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                await _genericRepository.RollbackTransactionAsync(); // Rollback transaction on error
                apiResponse.Success = false;

                if (ex.InnerException?.Message.Contains("duplicate") ?? false)
                {
                    apiResponse.Message = "شماره حساب وجود دارد.";
                    return StatusCode(StatusCodes.Status500InternalServerError, apiResponse);
                }
                //else if (ex.InnerException?.Message.Contains("conflicted with the FOREIGN KEY") ?? false)
                //{
                //    apiResponse.Message = "حساب اصلی کاربر وجود ندارد";

                //    return StatusCode(StatusCodes.Status500InternalServerError, apiResponse);
                //}
                apiResponse.Message = "خطای در سرور رخ داده است";
                return StatusCode(StatusCodes.Status500InternalServerError, apiResponse);
            }
        }


        //Customer Info of a user
        [HttpGet("customers/{customerId:int}", Name = "Get single customer info of a user")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<CustomerAccountDTO>>> GetCustomerInfo(int customerId)
        {
            _logger.LogError("getting complete Customer info ");
            var apiResponse = new ApiResponse<CustomerAccountDTO>(false);
            try
            {
                var currentUserId = User.GetUserId();
                var customer = await _customerAccountRepo.GetCustomerInfoByCustomerIdAsync(currentUserId, customerId);
                if (customer == null)
                {
                    apiResponse.Message = "اطلاعات مشتری یافت نشد.";
                    return Ok(apiResponse);
                }
                _logger.LogError("getting complete Customer info Ended");
                apiResponse.Success = true;
                apiResponse.Data = customer;
                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<CustomerAccountSummaryDTO>(false, "خطای در سرور رخ داده است"));
            }

        }

        [HttpPut("customers/{customerId:int}", Name = "Update single customer info of a user")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<bool>>> UpdateCustomerInfo(int customerId, CustomerAccountDTO updatedCustomer)
        {
            var apiResponse = new ApiResponse<bool>(false);
            try
            {
                _logger.LogError($"updating single customer info,customerId:{customerId}");
                var currentUserId = User.GetUserId();
                var oldCustomer = await _genericRepository.GetByIdAsync(customerId);
                if (oldCustomer == null)
                {
                    apiResponse.Message = "اطلاعات مشتری یافت نشد.";
                    return Ok(apiResponse);
                }

                if (oldCustomer.BorrowAmount != updatedCustomer.BorrowAmount)
                {
                    // check borrowAmount of the user
                    // Fetch the exchange rates
                    var baseCurrency = await _currencyRepository.GetBaseCurrency(currentUserId);
                    if (baseCurrency == null)
                    {
                        apiResponse.Message = "ارز پایه در سیستم مشخص نشده است.";
                        await _genericRepository.RollbackTransactionAsync();
                        return Ok(apiResponse);
                    }

                    var exchangeRates = await _currencyRepository
                        .GetAllCurrencyDetails(baseCurrency.CurrencyId, currentUserId);
                    if (exchangeRates == null)
                    {
                        apiResponse.Message = "نرخ تبادله برای ارز پایه مشخص نشده است.";
                        await _genericRepository.RollbackTransactionAsync();
                        return Ok(apiResponse);
                    }

                    // check the user can borrow money or not
                    var totalbalance = await _customerAccountRepo
                        .CalculateCustomerBalanceInUSD(currentUserId, oldCustomer.Id, exchangeRates.ToList());

                    if (totalbalance == null)
                    {
                        apiResponse.Message = "مشتری یافت نشد.";
                        await _genericRepository.RollbackTransactionAsync();
                        return Ok(apiResponse);
                    }

                    if (totalbalance < -(updatedCustomer.BorrowAmount))
                    {
                        apiResponse.Message = "مشتری به سقف قرضه خود رسیده است. اطلاعات بروزرسانی نشد.";
                        return Ok(apiResponse);
                    }
                }

                oldCustomer.Name = updatedCustomer.Firstname;
                oldCustomer.Lastname = updatedCustomer.Lastname;
                oldCustomer.BorrowAmount = updatedCustomer.BorrowAmount;
                oldCustomer.IDCardNumber = updatedCustomer.IDCardNumber;
                oldCustomer.PassportNumber = updatedCustomer.PassportNumber;
                oldCustomer.Image = updatedCustomer.Image;
                oldCustomer.Mobile = updatedCustomer.Mobile;
                oldCustomer.LastModifiedDate = DateTime.Now;
                oldCustomer.CreatedDate = updatedCustomer.CreatedDate;

                _logger.LogError("updating Customer info Ended");
                await _genericRepository.SaveAsync();
                apiResponse.Success = true;
                apiResponse.Data = true;
                apiResponse.Message = "اطلاعات مشتری بروزرسانی شد.";
                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                apiResponse.Success = false;
                return StatusCode(StatusCodes.Status500InternalServerError, apiResponse);
            }
        }

        //Customer Info of a user
        [HttpGet("{userId:int}", Name = "Get customers info of a user")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<IEnumerable<CustomerAccountSummaryDTO>>>> GetAllCustomersInfo(int userId)
        {
            _logger.LogError("getting complete Customer info ");
            var apiResponse = new ApiResponse<IEnumerable<CustomerAccountDTO>>(false);
            try
            {
                var customers = await _customerAccountRepo.GetAllCustomersInfoAsync(userId);
                _logger.LogError("getting complete Customer info Ended");
                apiResponse.Success = true;
                apiResponse.Data = customers;
                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<CustomerAccountSummaryDTO>(false, "خطای در سرور رخ داده است"));
            }

        }

        // id in here is userId
        [HttpGet("CustomersSummary/{id:int}", Name = "Get customers of a user")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<IEnumerable<CustomerAccountSummaryDTO>>>> GetAllCustomers(int id)
        {
            _logger.LogError("getting Customer info like name and account number");
            try
            {
                var customers = await _customerAccountRepo.GetAllCustomersAsync(id);
                _logger.LogError("getting Customer info Ended");
                return Ok(new ApiResponse<IEnumerable<CustomerAccountSummaryDTO>>
                    (true, "Operation Successfully", null, customers));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<CustomerAccountSummaryDTO>(false, "خطای در سرور رخ داده است"));
            }

        }

        [HttpGet("OfficeAccountSummary/{id:int}", Name = "Get accounts of a user")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<IEnumerable<CustomerAccountSummaryDTO>>>> GetAllAccountsOfAUser(int id)
        {
            _logger.LogError("getting Account info of a user like name and account number");
            var apiResponse = new ApiResponse<IEnumerable<CustomerAccountSummaryDTO>>(false);
            try
            {
                var customers = await _customerAccountRepo.GetAllAccountOfAUserAsync(id);
                _logger.LogError("getting Customer info Ended");
                apiResponse.Success = true;
                apiResponse.Data = customers;

                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                apiResponse.Success = false;
                apiResponse.Message = "خطای در سرور رخ داده است.";
                return StatusCode(StatusCodes.Status500InternalServerError, apiResponse);
            }

        }

        [HttpGet("account/personal/{userId:int}", Name = "Get personal accounts of a user")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<IEnumerable<PersonalAccountDTO>>>> GetPersonalAccounts(int userId)
        {
            var apiResponse = new ApiResponse<IEnumerable<PersonalAccountDTO>>(false);
            try
            {
                var accounts = await _customerAccountRepo.GetPersonalAccountOfUser(userId);
                if (accounts == null)
                {
                    apiResponse.Message = "دریافت حسابات شخصی با مشکل مواجه شد.";
                    return Ok(apiResponse);
                }

                apiResponse.Success = true;
                apiResponse.Message = "موفق";
                apiResponse.Data = accounts;
                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                apiResponse.Success = false;
                apiResponse.Message = "دریافت حسابات شخصی با مشکل مواجه شد.";
                return StatusCode(StatusCodes.Status500InternalServerError, apiResponse);
            }
        }

        [HttpPost("addTransaction", Name = "Add Customer Transaction")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<int>>> AddTransaction(CustomerTransactionHistoryDTO transactionDTO)
        {
            var apiResponse = new ApiResponse<int>(false);
            try
            {
                var currentUserId = User.GetUserId();

                // Check if the current user is the owner of the customer
                bool isOwner = await _authorizationService.IsUserOwnerOfCustomerAsync(transactionDTO.CustomerId, currentUserId);
                if (!isOwner)
                {
                    apiResponse.Message = "شما اجازه دسترسی به این اطلاعات را ندارید";
                    return Unauthorized(apiResponse);
                }


                await _genericRepository.BeginTransactionAsync();

                if (transactionDTO.DealType == DealType.Deposit)
                {
                    // Deposit
                    var result = await _customerAccountRepo.DepositeByCustomerIdAsync
                        (currentUserId, transactionDTO.CustomerId, transactionDTO.CurrencyId, transactionDTO.Amount);
                    if (!result)
                    {
                        apiResponse.Message = "تراکنش مشتری ثبت نشد.";
                        await _genericRepository.RollbackTransactionAsync();
                        return Ok(apiResponse);
                    }
                }
                else
                {
                    // Withdraw
                    var result = await _customerAccountRepo.WithdrawByCustomerIdAsync
                        (currentUserId, transactionDTO.CustomerId, transactionDTO.CurrencyId, transactionDTO.Amount);
                    if (!result.Item1)
                    {
                        apiResponse.Message = result.Item2;
                        await _genericRepository.RollbackTransactionAsync();
                        return Ok(apiResponse);
                    }
                    await _customerAccountRepo.SaveAsync();

                    // calculate the total balance in USD

                    var baseCurrency = await _currencyRepository.GetBaseCurrency(currentUserId);
                    if (baseCurrency == null)
                    {
                        apiResponse.Message = "ارز پایه در سیستم مشخص نشده است.";
                        await _genericRepository.RollbackTransactionAsync();
                        return Ok(apiResponse);
                    }

                    var exchangeRates = await _currencyRepository
                        .GetAllCurrencyDetails(baseCurrency.CurrencyId, currentUserId);
                    if (exchangeRates == null)
                    {
                        apiResponse.Message = "نرخ تبادله برای ارز پایه مشخص نشده است.";
                        await _genericRepository.RollbackTransactionAsync();
                        return Ok(apiResponse);
                    }

                    var totalBalance = await _customerAccountRepo
                        .CalculateCustomerBalanceInUSD(currentUserId, transactionDTO.CustomerId, exchangeRates.ToList());

                    if (totalBalance == null)
                    {
                        apiResponse.Message = "مشتری یافت نشد.";
                        await _genericRepository.RollbackTransactionAsync();
                        return Ok(apiResponse);
                    }


                    var BorrowAmount = await _customerAccountRepo.GetBorrowAmountOfACustomer(currentUserId, transactionDTO.CustomerId);

                    if (BorrowAmount == null)
                    {
                        apiResponse.Message = "مشتری یافت نشد.";
                        await _genericRepository.RollbackTransactionAsync();
                        return Ok(apiResponse);
                    }

                    if (totalBalance < -(BorrowAmount))
                    {
                        apiResponse.Message = "مشتری به سقف قرضه خود رسیده است. تراکنش ثبت نشد";
                        await _genericRepository.RollbackTransactionAsync();
                        return Ok(apiResponse);
                    }
                }


                // update treasury account based on currency and deal Type

                var treasuryAccountUpdated = await _customerAccountRepo.UpdateTreasuryAccount(currentUserId, transactionDTO.Amount, transactionDTO.CurrencyId, transactionDTO.DealType == DealType.Deposit);
                if (!treasuryAccountUpdated)
                {
                    apiResponse.Message = "حساب صندوق بروز رسانی نشد.";
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }

                var isWithdraw = transactionDTO.DealType == DealType.Withdraw;

                // check the treasury account for the borrow amount
                if (isWithdraw)
                {
                    var result = await _customerAccountRepo.CheckTreasuryAccountBasedOnBorrowAmount(currentUserId);
                    if (!result.Item1)
                    {
                        apiResponse.Message = result.Item2;
                        await _genericRepository.RollbackTransactionAsync();
                        return Ok(apiResponse);
                    }
                }

                if (isWithdraw)
                {
                    transactionDTO.Amount = -transactionDTO.Amount;
                }
                transactionDTO.UserId = currentUserId;
                var transactionId = await _customerAccountRepo.AddCustomerTransactionAsync(transactionDTO);

                if (transactionId == 0)
                {
                    apiResponse.Message = "تراکنش مشتری ثبت نشد";
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }

                await _genericRepository.CommitTransactionAsync();
                await _customerAccountRepo.SaveAsync();
                apiResponse.Success = true;
                apiResponse.Data = transactionId;
                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.InnerException.ToString());
                apiResponse.Success = false;
                apiResponse.Message = "خطای در سرور رخ داده است";
                //await _genericRepository.RollbackTransactionAsync(); ;
                return StatusCode(StatusCodes.Status500InternalServerError, apiResponse);
            }
        }

        //// id in the URL is the transactionId
        [HttpGet("cutomerTransactions/{customerId:int}", Name = "GetCustomerTransactionsByid")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<IEnumerable<CustomerTransactionHistoryDTO>>>> GetCustomerTransactionsById(
            int customerId)
        {
            var apiResponse = new ApiResponse<IEnumerable<CustomerTransactionHistoryDTO>>(false);
            try
            {
                // This method extract the userId from token
                var currentUserId = User.GetUserId();

                // Check if the current user is the owner of the customer
                bool isOwner = await _authorizationService.IsUserOwnerOfCustomerAsync(customerId, currentUserId);
                if (!isOwner)
                {
                    apiResponse.Message = "شما اجازه دسترسی به این اطلاعات را ندارید";
                    return Unauthorized(apiResponse);
                }

                var transactions = await _customerAccountRepo.GetAllTransactionsByCustomerId(currentUserId, customerId);
                if (transactions == null)
                {
                    apiResponse.Message = "تراکنشی وجود ندارد";
                    return BadRequest(apiResponse);

                }

                apiResponse.Success = true;
                apiResponse.Data = transactions;
                return Ok(apiResponse);

            }
            catch (Exception ex)
            {
                apiResponse.Success = false;
                apiResponse.Message = "خطای در سرور رخ داده است";
                return StatusCode(StatusCodes.Status500InternalServerError, apiResponse);
            }
        }

        //// id in the URL is the transactionId
        [HttpGet("filterCutomerTransactions/{customerId:int}", Name = "FilterCustomerTransactionsByid")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<IEnumerable<CustomerTransactionHistoryDTO>>>> FilterCustomerTransactionsById(int customerId,
            [FromQuery] int? currencyId,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate)
        {
            var apiResponse = new ApiResponse<IEnumerable<CustomerTransactionHistoryDTO>>(false);
            try
            {
                // This method extract the userId from token
                var currentUserId = User.GetUserId();

                // Check if the current user is the owner of the customer
                bool isOwner = await _authorizationService.IsUserOwnerOfCustomerAsync(customerId, currentUserId);
                if (!isOwner)
                {
                    apiResponse.Message = "شما اجازه دسترسی به این اطلاعات را ندارید";
                    return Unauthorized(apiResponse);
                }

                var transactions = await _customerAccountRepo.FilterAllTransactionsByCustomerId(currentUserId, customerId, currencyId, fromDate, toDate);
                if (transactions == null)
                {
                    apiResponse.Message = "تراکنشی وجود ندارد";
                    return BadRequest(apiResponse);

                }

                apiResponse.Success = true;
                apiResponse.Data = transactions;
                return Ok(apiResponse);

            }
            catch (Exception ex)
            {
                apiResponse.Success = false;
                apiResponse.Message = "خطای در سرور رخ داده است";
                return StatusCode(StatusCodes.Status500InternalServerError, apiResponse);
            }
        }

        //id in url below is customerId
        [HttpGet("customerBalance/{customerId:int}", Name = "GetCustomerBalanceById")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<IEnumerable<CustomerBalanceDetailsDTO>>>> GetCustomerBalanceById(int customerId)
        {
            var apiResponse = new ApiResponse<IEnumerable<CustomerBalanceDetailsDTO>>(false);
            try
            {
                _logger.LogError($"getting customer Balance:{customerId}");
                // This method extract the userId from token
                var currentUserId = User.GetUserId();

                // Check if the current user is the owner of the customer
                bool isOwner = await _authorizationService.IsUserOwnerOfCustomerAsync(customerId, currentUserId);
                if (!isOwner)
                {
                    apiResponse.Message = "شما اجازه دسترسی به این اطلاعات را ندارید";
                    return Unauthorized(apiResponse);
                }

                var result = await _customerAccountRepo.GetCustomerBalanceById(currentUserId, customerId);

                if (!result.Item1)
                {
                    apiResponse.Message = result.Item2;
                    return Ok(apiResponse);
                }

                _logger.LogError($"getting customer Balance:{customerId} ENDED");

                apiResponse.Success = true;
                apiResponse.Data = result.Item3;
                return Ok(apiResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                apiResponse.Success = false;
                apiResponse.Message = "خطای در سرور رخ داده است";
                return StatusCode(StatusCodes.Status500InternalServerError, apiResponse);
            }
        }


        [HttpGet("balances", Name = "Get balances of all customers by currencyId")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<IEnumerable<GeneralListDTO>>>>
            GetAllCustomersBalances([FromQuery] int? currencyId, [FromQuery] string? customerName, [FromQuery] bool? isDeptor)
        {
            var apiResponse = new ApiResponse<IEnumerable<GeneralListDTO>>(false);
            try
            {
                _logger.LogError($"getting balances of all customers by currencyId");
                // This method extract the userId from token
                var currentUserId = User.GetUserId();

                // Check if the current user is the owner of the customer
                //bool isOwner = await _authorizationService.IsUserOwnerOfCustomerAsync(customerId, currentUserId);
                //if (!isOwner)
                //{
                //    apiResponse.Message = "شما اجازه دسترسی به این اطلاعات را ندارید";
                //    return Unauthorized(apiResponse);
                //}

                var result = await _customerAccountRepo.GetCustomerBalancesByUserIdAsync(currentUserId, currencyId, isDeptor, customerName);

                if (result == null)
                {
                    apiResponse.Message = "دریافت لیست کل با مشکل مواجه شد.";
                    return Ok(apiResponse);
                }

                _logger.LogError($"getting balances of all customers by currencyId ENDED");

                apiResponse.Success = true;
                apiResponse.Data = result;
                return Ok(apiResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                apiResponse.Success = false;
                apiResponse.Message = "خطای در سرور رخ داده است";
                return StatusCode(StatusCodes.Status500InternalServerError, apiResponse);
            }
        }


        [HttpGet("treasuryBalance/{userId:int}", Name = "Get balances of the treasury account")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<IEnumerable<CustomerBalanceDetailsDTO>>>> GetTreasuryBalaneAccount(int userId)
        {
            var apiResponse = new ApiResponse<IEnumerable<CustomerBalanceDetailsDTO>>(false);
            try
            {

                // This method extract the userId from token
                var currentUserId = User.GetUserId();

                //// Check if the current user is the owner of the customer
                //bool isOwner = await _authorizationService.IsUserOwnerOfCustomerAsync(customerId, currentUserId);
                //if (!isOwner)
                //{
                //    apiResponse.Message = "شما اجازه دسترسی به این اطلاعات را ندارید";
                //    return Unauthorized(apiResponse);
                //}

                var treasuryAccountId = await _customerAccountRepo.GetTreasureAccountId(userId);
                if (treasuryAccountId == null)
                {
                    apiResponse.Message = "حساب صندوق یافت نشد.";
                    return Ok(apiResponse);
                }

                var result = await _customerAccountRepo.GetCustomerBalanceById(userId, (int)treasuryAccountId);

                if (!result.Item1)
                {
                    apiResponse.Message = result.Item2;
                    return Ok(apiResponse);
                }

                apiResponse.Success = true;
                apiResponse.Data = result.Item3;
                return Ok(apiResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                apiResponse.Success = false;
                apiResponse.Message = "خطای در سرور رخ داده است";
                return StatusCode(StatusCodes.Status500InternalServerError, apiResponse);
            }
        }

        [HttpGet("totalCustomer", Name = "Get the total customers of a User")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<int>>> GetCustomersTotal()
        {
            var apiResponse = new ApiResponse<int>(false);
            try
            {
                var currentUserId = User.GetUserId();
                var result = await _customerAccountRepo.GetTotalOfCustomerAsync(currentUserId);
                if (result == null)
                {
                    apiResponse.Message = "دریافت تعداد مشتریان با مشکل مواجه شد.";
                    return Ok(apiResponse);
                }

                apiResponse.Success = true;
                apiResponse.Data = (int)result;
                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                apiResponse.Success = false;
                apiResponse.Message = "خطای در سرور رخ داده است";
                return StatusCode(StatusCodes.Status500InternalServerError, apiResponse);
            }
        }

        #region Update Transactions


        [HttpPut("updateTransaction/{transactionId:int}", Name = "Update Customer Transaction")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<int>>> UpdateTransaction(int transactionId, CustomerTransactionHistoryDTO transactionDTO)
        {
            var apiResponse = new ApiResponse<int>(false);
            try
            {
                // This method extract the userId from token
                var currentUserId = User.GetUserId();

                // Check if the current user is the owner of the customer
                bool isOwner = await _authorizationService.IsUserOwnerOfCustomerAsync(transactionDTO.CustomerId, currentUserId);
                if (!isOwner)
                {
                    apiResponse.Message = "شما اجازه دسترسی به این اطلاعات را ندارید";
                    return Unauthorized(apiResponse);
                }

                // Retrieve the existing transaction from the database
                var existingTransaction = await _transactionGenericRepository.GetByIdAsync(transactionId);
                if (existingTransaction == null)
                {
                    apiResponse.Message = "تراکنش یافت نشد";
                    return NotFound(apiResponse);
                }

                await _genericRepository.BeginTransactionAsync();


                bool dealTypeChanged = existingTransaction.DealType != transactionDTO.DealType;
                bool currencyChanged = existingTransaction.CurrencyId != transactionDTO.CurrencyId;

                decimal previousAmount = existingTransaction.Amount;
                int previousCurrencyId = existingTransaction.CurrencyId;
                DealType previousDealType = existingTransaction.DealType;

                // 1️⃣ Rollback the old transaction before applying new changes
                if (previousDealType == DealType.Withdraw)
                {
                    // If the original transaction was a withdrawal, we add back the money (like a refund)
                    var result = await _customerAccountRepo
                        .DepositeByCustomerIdAsync(currentUserId, existingTransaction.CustomerId, previousCurrencyId, previousAmount);
                    if (!result)
                    {
                        apiResponse.Message = "خطا در حذف تراکنش قدیمی!";
                        await _genericRepository.RollbackTransactionAsync();
                        return Ok(apiResponse);
                    }
                    // update the treasury account based on currency
                    var isTreasuryUpdated = await _customerAccountRepo
                        .UpdateTreasuryAccount(currentUserId, previousAmount, previousCurrencyId, true);
                    if (!isTreasuryUpdated)
                    {
                        apiResponse.Message = "خطار در بروز رسانی حساب صندوق!";
                        await _genericRepository.RollbackTransactionAsync();
                        return Ok(apiResponse);
                    }
                }
                else
                {
                    // If the original transaction was a deposit, we remove the money
                    var result = await _customerAccountRepo.WithdrawByCustomerIdAsync(currentUserId, existingTransaction.CustomerId, previousCurrencyId, previousAmount);

                    if (!result.Item1)
                    {
                        apiResponse.Message = result.Item2;
                        await _genericRepository.RollbackTransactionAsync();
                        return Ok(apiResponse);
                    }

                    // update treasury Account based on currency
                    var isTreasuryUpdated = await _customerAccountRepo
                        .UpdateTreasuryAccount(currentUserId, previousAmount, previousCurrencyId, false);
                    if (!isTreasuryUpdated)
                    {
                        apiResponse.Message = "خطار در بروز رسانی حساب صندوق!";
                        await _genericRepository.RollbackTransactionAsync();
                        return Ok(apiResponse);
                    }
                }

                await _customerAccountRepo.SaveAsync(); // Save rollback before applying new changes

                // 2️⃣ Apply new currency change (if applicable)
                if (currencyChanged)
                {
                    existingTransaction.CurrencyId = transactionDTO.CurrencyId;
                }

                // 4️⃣ Adjust balance based on the new deal type
                if (transactionDTO.DealType == DealType.Withdraw)
                {
                    // Withdraw the amount from the updated currency
                    var result = await _customerAccountRepo.WithdrawByCustomerIdAsync(currentUserId, existingTransaction.CustomerId, existingTransaction.CurrencyId, transactionDTO.Amount);

                    if (!result.Item1)
                    {
                        apiResponse.Message = result.Item2;
                        await _genericRepository.RollbackTransactionAsync();
                        return Ok(apiResponse);
                    }
                }
                else
                {
                    // Deposit the amount in the updated currency
                    var result = await _customerAccountRepo.DepositeByCustomerIdAsync(currentUserId, existingTransaction.CustomerId, existingTransaction.CurrencyId, transactionDTO.Amount);

                    if (!result)
                    {
                        apiResponse.Message = "خطا در حذف تراکنش قدیمی!";
                        await _genericRepository.RollbackTransactionAsync();
                        return Ok(apiResponse);
                    }
                }

                // Fetch the exchange rates
                var baseCurrency = await _currencyRepository.GetBaseCurrency(currentUserId);
                if (baseCurrency == null)
                {
                    apiResponse.Message = "ارز پایه در سیستم مشخص نشده است.";
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }

                var exchangeRates = await _currencyRepository
                    .GetAllCurrencyDetails(baseCurrency.CurrencyId, currentUserId);
                if (exchangeRates == null)
                {
                    apiResponse.Message = "نرخ تبادله برای ارز پایه مشخص نشده است.";
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }

                // check the user can borrow money or not
                var totalbalance = await _customerAccountRepo
                    .CalculateCustomerBalanceInUSD(currentUserId, transactionDTO.CustomerId, exchangeRates.ToList());

                if (totalbalance == null)
                {
                    apiResponse.Message = "مشتری یافت نشد.";
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }

                var borrowamount = await _customerAccountRepo.GetBorrowAmountOfACustomer(currentUserId, transactionDTO.CustomerId);

                if (borrowamount == null)
                {
                    apiResponse.Message = "مشتری یافت نشد.";
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }

                if (totalbalance < -(borrowamount))
                {
                    apiResponse.Message = "مشتری به سقف قرضه خود رسیده است. تراکنش ثبت نشد";
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }

                // update treasury account after updating transaction
                var TreasuryUpdated = await _customerAccountRepo
                        .UpdateTreasuryAccount(currentUserId, transactionDTO.Amount, transactionDTO.CurrencyId, transactionDTO.DealType == DealType.Deposit);
                if (!TreasuryUpdated)
                {
                    apiResponse.Message = "خطار در بروز رسانی حساب صندوق!";
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }


                var isBorrowAllowed = await _customerAccountRepo.CheckTreasuryAccountBasedOnBorrowAmount(currentUserId);
                if (!isBorrowAllowed.Item1)
                {
                    apiResponse.Message = isBorrowAllowed.Item2;
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }

                existingTransaction.Amount = transactionDTO.DealType == DealType.Withdraw ?
                    -transactionDTO.Amount : transactionDTO.Amount;

                // 3️⃣ Apply new deal type
                existingTransaction.DealType = transactionDTO.DealType;
                existingTransaction.UpdatedDate = DateTime.Now;
                existingTransaction.CreatedDate = transactionDTO.CreatedDate;
                existingTransaction.CustomerId = transactionDTO.CustomerId;
                existingTransaction.DocumentNumber = transactionDTO.DocumentNumber;
                existingTransaction.DepositOrWithdrawBy = transactionDTO.DepositOrWithdrawBy;
                existingTransaction.Description = transactionDTO.Description;
                existingTransaction.TransactionType = TransactionType.Normal;


                await _customerAccountRepo.SaveAsync();
                await _genericRepository.CommitTransactionAsync();
                apiResponse.Success = true;
                apiResponse.Data = transactionId;
                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                apiResponse.Success = false;
                apiResponse.Message = "خطای در سرور رخ داده است";
                //await _genericRepository.RollbackTransactionAsync(); ;
                return StatusCode(StatusCodes.Status500InternalServerError, apiResponse);
            }
        }

        [HttpPut("updateTransactionDetails/{transactionId:int}", Name = "Update Customer Transaction detail(documentNumber,description,Date)")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<int>>> UpdateTransactionDetails(int transactionId, CustomerTransactionHistoryDTO transactionDTO)
        {
            var apiResponse = new ApiResponse<int>(false);
            try
            {

                // TODO: Implement this feature to all controller to return info base on the userId(owner of the app)

                // This method extract the userId from token
                var currentUserId = User.GetUserId();

                //TODO for now we use 1 for currentUserId

                //int currentUserId = 1;


                // Check if the current user is the owner of the customer
                bool isOwner = await _authorizationService.IsUserOwnerOfCustomerAsync(transactionDTO.CustomerId, currentUserId);
                if (!isOwner)
                {
                    apiResponse.Message = "شما اجازه دسترسی به این اطلاعات را ندارید";
                    return Unauthorized(apiResponse);
                }

                var result = await _customerAccountRepo.UpdateTransactionDetail(currentUserId, transactionDTO);
                if (!result)
                {
                    apiResponse.Message = "تراکنش بروز رسانی نشد.";
                    return Ok(apiResponse);
                }

                await _customerAccountRepo.SaveAsync();
                apiResponse.Success = true;
                apiResponse.Data = transactionId;
                return Ok(apiResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                apiResponse.Success = false;
                apiResponse.Message = "خطای در سرور رخ داده است";
                //await _genericRepository.RollbackTransactionAsync(); ;
                return StatusCode(StatusCodes.Status500InternalServerError, apiResponse);
            }
        }

        [HttpPut("updateTransactionAmount/{transactionId:int}", Name = "Update Customer Transaction Amount")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<int>>> UpdateTransactionAmount(int transactionId, CustomerTransactionHistoryDTO transactionDTO)
        {
            var apiResponse = new ApiResponse<int>(false);
            try
            {

                // TODO: Implement this feature to all controller to return info base on the userId(owner of the app)

                // This method extract the userId from token
                var currentUserId = User.GetUserId();

                //TODO for now we use 1 for currentUserId

                //int currentUserId = 1;


                // Check if the current user is the owner of the customer
                bool isOwner = await _authorizationService.IsUserOwnerOfCustomerAsync(transactionDTO.CustomerId, currentUserId);
                if (!isOwner)
                {
                    apiResponse.Message = "شما اجازه دسترسی به این اطلاعات را ندارید";
                    return Unauthorized(apiResponse);
                }

                await _genericRepository.BeginTransactionAsync();
                // update amount in transaction table
                var result = await _customerAccountRepo.UpdateTransactionAmount(currentUserId, transactionDTO);
                if (!result)
                {
                    apiResponse.Message = "تراکنش بروز رسانی نشد.";
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }

                // update amount in balance table

                result = await _customerAccountRepo.UpdateCustomerBalanceAmount(currentUserId, transactionDTO.CustomerId, transactionDTO.CurrencyId, transactionDTO.NewAmountToUpdateBalance, transactionDTO.DealType);
                if (!result)
                {
                    apiResponse.Message = "تراکنش بروز رسانی نشد.";
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }

                if (transactionDTO.DealType == DealType.Withdraw)
                {
                    await _customerAccountRepo.SaveAsync();
                    var baseCurrency = await _currencyRepository.GetBaseCurrency(currentUserId);
                    if (baseCurrency == null)
                    {
                        apiResponse.Message = "ارز پایه در سیستم مشخص نشده است.";
                        await _genericRepository.RollbackTransactionAsync();
                        return Ok(apiResponse);
                    }

                    var exchangeRates = await _currencyRepository
                        .GetAllCurrencyDetails(baseCurrency.CurrencyId, currentUserId);
                    if (exchangeRates == null)
                    {
                        apiResponse.Message = "نرخ تبادله برای ارز پایه مشخص نشده است.";
                        await _genericRepository.RollbackTransactionAsync();
                        return Ok(apiResponse);
                    }

                    var totalBalance = await _customerAccountRepo
                        .CalculateCustomerBalanceInUSD(currentUserId, transactionDTO.CustomerId, exchangeRates.ToList());

                    if (totalBalance == null)
                    {
                        apiResponse.Message = "مشتری یافت نشد.";
                        await _genericRepository.RollbackTransactionAsync();
                        return Ok(apiResponse);
                    }


                    var BorrowAmount = await _customerAccountRepo.GetBorrowAmountOfACustomer(currentUserId, transactionDTO.CustomerId);

                    if (BorrowAmount == null)
                    {
                        apiResponse.Message = "مشتری یافت نشد.";
                        await _genericRepository.RollbackTransactionAsync();
                        return Ok(apiResponse);
                    }

                    if (totalBalance < -(BorrowAmount))
                    {
                        apiResponse.Message = "مشتری به سقف قرضه خود رسیده است. تراکنش ثبت نشد";
                        await _genericRepository.RollbackTransactionAsync();
                        return Ok(apiResponse);
                    }
                }

                await _customerAccountRepo.SaveAsync();
                await _genericRepository.CommitTransactionAsync();
                apiResponse.Success = true;
                apiResponse.Data = transactionId;
                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                apiResponse.Success = false;
                apiResponse.Message = "خطای در سرور رخ داده است";
                //await _genericRepository.RollbackTransactionAsync(); ;
                return StatusCode(StatusCodes.Status500InternalServerError, apiResponse);
            }
        }


        #endregion

        // Create initila Accounts for user
        [HttpPost("createInitialAccount", Name = "create initial account for user")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<bool>>> CreateInitialAccounts(List<CurrencyImage> images)
        {
            var apiResponse = new ApiResponse<bool>(false);
            try
            {
                var currentUserId = User.GetUserId();

                var user = await userRepo.GetUserByIdAsync(currentUserId);
                if (!user.isFirstTimeLogin)
                {
                    apiResponse.Message = "alreadyExist";
                    apiResponse.Success = true;
                    return Ok(apiResponse);
                }

                using HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0"); // Mimic a browser request
                var imageUrl = "https://upload.wikimedia.org/wikipedia/en/a/a4/Flag_of_the_United_States.svg";
                byte[] usaImage = await client.GetByteArrayAsync(imageUrl);

                imageUrl = "https://upload.wikimedia.org/wikipedia/commons/b/b7/Flag_of_Europe.svg";
                byte[] euroImage = await client.GetByteArrayAsync(imageUrl);

                imageUrl = "https://upload.wikimedia.org/wikipedia/en/a/ae/Flag_of_the_United_Kingdom.svg";
                byte[] poundImage = await client.GetByteArrayAsync(imageUrl);

                imageUrl = "https://upload.wikimedia.org/wikipedia/commons/b/b4/Flag_of_Turkey.svg";
                byte[] turkeyImage = await client.GetByteArrayAsync(imageUrl);

                imageUrl = "https://upload.wikimedia.org/wikipedia/commons/c/ca/Flag_of_Iran.svg";
                byte[] iranImage = await client.GetByteArrayAsync(imageUrl);

                imageUrl = "https://upload.wikimedia.org/wikipedia/commons/3/32/Flag_of_Pakistan.svg";
                byte[] pakistanImage = await client.GetByteArrayAsync(imageUrl);

                imageUrl = "https://www.countryflags.com/wp-content/uploads/afghanistan-flag-png-large.png";
                byte[] afnImage = await client.GetByteArrayAsync(imageUrl);


                await _genericRepository.BeginTransactionAsync(); // Start transaction

                await _customerAccountRepo.InitializeAccountNumberSequenceTable(currentUserId);
                await _customerAccountRepo.SaveAsync();

                int lastUsedAccountNumber;

                lastUsedAccountNumber = 0;
                int accountNumber = lastUsedAccountNumber + 1;

                var treasuyAccount = new CustomerAccount()
                {
                    Name = "صندوق",
                    AccountNumber = accountNumber,
                    AccountType = eAccountType.Treasury,
                    BorrowAmount = 50000,
                    UserId = currentUserId,
                };

                var isAccountCreated = await _genericRepository.AddAsync(treasuyAccount);
                if (!isAccountCreated)
                {
                    await _genericRepository.RollbackTransactionAsync();
                    apiResponse.Message = "حساب دفتری ساخته نشد";
                    return BadRequest(apiResponse);
                }

                accountNumber += 1;

                var account = new CustomerAccount()
                {
                    Name = "کمشن",
                    AccountNumber = accountNumber,
                    AccountType = eAccountType.Incremental,
                    BorrowAmount = 50000,
                    UserId = currentUserId,
                };

                isAccountCreated = await _genericRepository.AddAsync(account);

                if (!isAccountCreated)
                {
                    await _genericRepository.RollbackTransactionAsync();
                    apiResponse.Message = "حساب دفتری ساخته نشد";
                    return BadRequest(apiResponse);
                }

                accountNumber += 1;

                account = new CustomerAccount()
                {
                    Name = "تبادله ارز",
                    AccountNumber = accountNumber,
                    AccountType = eAccountType.CurrencyExchange,
                    BorrowAmount = 50000,
                    UserId = currentUserId,
                };

                isAccountCreated = await _genericRepository.AddAsync(account);

                if (!isAccountCreated)
                {
                    await _genericRepository.RollbackTransactionAsync();
                    apiResponse.Message = "حساب دفتری ساخته نشد";
                    return BadRequest(apiResponse);
                }

                accountNumber += 1;

                account = new CustomerAccount()
                {
                    Name = "مصارفات",
                    AccountNumber = accountNumber,
                    AccountType = eAccountType.Decremental,
                    BorrowAmount = 50000,
                    UserId = currentUserId,
                };

                isAccountCreated = await _genericRepository.AddAsync(account);

                if (!isAccountCreated)
                {
                    await _genericRepository.RollbackTransactionAsync();
                    apiResponse.Message = "حساب دفتری ساخته نشد";
                    return BadRequest(apiResponse);
                }

                accountNumber += 1;

                account = new CustomerAccount()
                {
                    Name = "عواید متفرقه",
                    AccountNumber = accountNumber,
                    AccountType = eAccountType.Incremental,
                    BorrowAmount = 50000,
                    UserId = currentUserId,
                };

                isAccountCreated = await _genericRepository.AddAsync(account);

                if (!isAccountCreated)
                {
                    await _genericRepository.RollbackTransactionAsync();
                    apiResponse.Message = "حساب دفتری ساخته نشد";
                    return BadRequest(apiResponse);
                }

                // Update the account number sequence
                await _customerAccountRepo.UpdateLastUsedAccountNumber(accountNumber, currentUserId);

                // create currency for user

                //var newCurrency = currencyDTO.ToCurrencyEntity();

                var i = images;

                var usaCurrency = new CurrencyEntity()
                {
                    Name = "دالر",
                    Code = "USD",
                    IsActive = true,
                    Symbol = "$",
                    Unit = 1,
                    UserId = currentUserId,
                    Image = usaImage
                };

                var result = await _genericCurrencyEntity.AddAsync(usaCurrency);

                if (!result || result == null)
                {
                    apiResponse.Message = "ارز اضافه نشد";
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }

                var afnCurrency = new CurrencyEntity()
                {
                    Name = "افغانی",
                    Code = "AFN",
                    IsActive = true,
                    Symbol = "؋",
                    Unit = 1,
                    UserId = currentUserId,
                    Image = afnImage
                };

                result = await _genericCurrencyEntity.AddAsync(afnCurrency);

                if (!result || result == null)
                {
                    apiResponse.Message = "ارز اضافه نشد";
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }

                // Adding Toman Currency
                var tomanCurrency = new CurrencyEntity()
                {
                    Name = "تومان",
                    Code = "IRR",
                    IsActive = true,
                    Symbol = "ريال",
                    Unit = 1000,
                    UserId = currentUserId,
                    Image = iranImage
                };

                result = await _genericCurrencyEntity.AddAsync(tomanCurrency);

                if (!result || result == null)
                {
                    apiResponse.Message = "ارز اضافه نشد";
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }

                // Adding Kaldar Currency

                var kaldarCurrency = new CurrencyEntity()
                {
                    Name = "کالدار",
                    Code = "PKR",
                    IsActive = true,
                    Symbol = "Rs",
                    Unit = 1000,
                    UserId = currentUserId,
                    Image = pakistanImage
                };

                result = await _genericCurrencyEntity.AddAsync(kaldarCurrency);

                if (!result || result == null)
                {
                    apiResponse.Message = "ارز اضافه نشد";
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }

                await _genericCurrencyEntity.SaveAsync();

                // create a balance for currency for the treasury account
                result = await _customerAccountRepo.CreateTreasuryBalanceForCurrency(afnCurrency.CurrencyId, (int)treasuyAccount.Id, currentUserId);
                if (!result || result == null)
                {
                    apiResponse.Message = "ارز اضافه نشد";
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }

                result = await _customerAccountRepo.CreateTreasuryBalanceForCurrency(usaCurrency.CurrencyId, (int)treasuyAccount.Id, currentUserId);
                if (!result || result == null)
                {
                    apiResponse.Message = "ارز اضافه نشد";
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }

                result = await _customerAccountRepo.CreateTreasuryBalanceForCurrency(tomanCurrency.CurrencyId, (int)treasuyAccount.Id, currentUserId);
                if (!result || result == null)
                {
                    apiResponse.Message = "ارز اضافه نشد";
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }

                result = await _customerAccountRepo.CreateTreasuryBalanceForCurrency(kaldarCurrency.CurrencyId, (int)treasuyAccount.Id, currentUserId);
                if (!result || result == null)
                {
                    apiResponse.Message = "ارز اضافه نشد";
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }

                // USD => USD rate
                var newRate = new CurrencyExchangeRate()
                {
                    BaseCurrencyId = usaCurrency.CurrencyId,
                    TargetCurrencyId = usaCurrency.CurrencyId,
                    Buy = 1,
                    Sell = 1,
                    UserId = currentUserId,
                    Unit = 1,
                    EffectiveDate = DateTime.Now,
                };

                var isAdded = await _genericCurrencyExchangeRate.AddAsync(newRate);
                if (!isAdded)
                {
                    apiResponse.Message = "نرخ ارز اضافه نشد.";
                    return BadRequest(apiResponse);
                }
                // AFN => AFN rate
                newRate = new CurrencyExchangeRate()
                {
                    BaseCurrencyId = afnCurrency.CurrencyId,
                    TargetCurrencyId = afnCurrency.CurrencyId,
                    Buy = 1,
                    Sell = 1,
                    UserId = currentUserId,
                    Unit = 1,
                    EffectiveDate = DateTime.Now,
                };

                isAdded = await _genericCurrencyExchangeRate.AddAsync(newRate);
                if (!isAdded)
                {
                    apiResponse.Message = "نرخ ارز اضافه نشد.";
                    return BadRequest(apiResponse);
                }

                // TOMAN => TOMAN rate
                newRate = new CurrencyExchangeRate()
                {
                    BaseCurrencyId = tomanCurrency.CurrencyId,
                    TargetCurrencyId = tomanCurrency.CurrencyId,
                    Buy = 1,
                    Sell = 1,
                    UserId = currentUserId,
                    Unit = 1000,
                    EffectiveDate = DateTime.Now,
                };

                isAdded = await _genericCurrencyExchangeRate.AddAsync(newRate);
                if (!isAdded)
                {
                    apiResponse.Message = "نرخ ارز اضافه نشد.";
                    return BadRequest(apiResponse);
                }

                // Kaldar => Kaldar rate
                newRate = new CurrencyExchangeRate()
                {
                    BaseCurrencyId = kaldarCurrency.CurrencyId,
                    TargetCurrencyId = kaldarCurrency.CurrencyId,
                    Buy = 1,
                    Sell = 1,
                    UserId = currentUserId,
                    Unit = 1000,
                    EffectiveDate = DateTime.Now,
                };

                isAdded = await _genericCurrencyExchangeRate.AddAsync(newRate);
                if (!isAdded)
                {
                    apiResponse.Message = "نرخ ارز اضافه نشد.";
                    return BadRequest(apiResponse);
                }

                // USD => AFN rate
                newRate = new CurrencyExchangeRate()
                {
                    BaseCurrencyId = usaCurrency.CurrencyId,
                    TargetCurrencyId = afnCurrency.CurrencyId,
                    Buy = 73.10m,
                    Sell = 73.20m,
                    UserId = currentUserId,
                    Unit = 1,
                    EffectiveDate = DateTime.Now,
                };

                isAdded = await _genericCurrencyExchangeRate.AddAsync(newRate);
                if (!isAdded)
                {
                    apiResponse.Message = "نرخ ارز اضافه نشد.";
                    return BadRequest(apiResponse);
                }

                // USD => Toman rate
                newRate = new CurrencyExchangeRate()
                {
                    BaseCurrencyId = usaCurrency.CurrencyId,
                    TargetCurrencyId = tomanCurrency.CurrencyId,
                    Buy = 92000,
                    Sell = 93000,
                    UserId = currentUserId,
                    Unit = 1,
                    EffectiveDate = DateTime.Now,
                };

                isAdded = await _genericCurrencyExchangeRate.AddAsync(newRate);
                if (!isAdded)
                {
                    apiResponse.Message = "نرخ ارز اضافه نشد.";
                    return BadRequest(apiResponse);
                }

                // AFN => Toman rate
                newRate = new CurrencyExchangeRate()
                {
                    BaseCurrencyId = afnCurrency.CurrencyId,
                    TargetCurrencyId = tomanCurrency.CurrencyId,
                    Buy = 1250,
                    Sell = 1252,
                    UserId = currentUserId,
                    Unit = 1,
                    EffectiveDate = DateTime.Now,
                };

                isAdded = await _genericCurrencyExchangeRate.AddAsync(newRate);
                if (!isAdded)
                {
                    apiResponse.Message = "نرخ ارز اضافه نشد.";
                    return BadRequest(apiResponse);
                }


                // Toman => AFN rate
                newRate = new CurrencyExchangeRate()
                {
                    BaseCurrencyId = tomanCurrency.CurrencyId,
                    TargetCurrencyId = afnCurrency.CurrencyId,
                    Buy = 0.80m,
                    Sell = 0.81m,
                    UserId = currentUserId,
                    Unit = 1000,
                    EffectiveDate = DateTime.Now,
                };

                isAdded = await _genericCurrencyExchangeRate.AddAsync(newRate);
                if (!isAdded)
                {
                    apiResponse.Message = "نرخ ارز اضافه نشد.";
                    return BadRequest(apiResponse);
                }


                // Kaldar => AFN rate
                newRate = new CurrencyExchangeRate()
                {
                    BaseCurrencyId = kaldarCurrency.CurrencyId,
                    TargetCurrencyId = afnCurrency.CurrencyId,
                    Buy = 255,
                    Sell = 260,
                    UserId = currentUserId,
                    Unit = 1000,
                    EffectiveDate = DateTime.Now,
                };

                isAdded = await _genericCurrencyExchangeRate.AddAsync(newRate);
                if (!isAdded)
                {
                    apiResponse.Message = "نرخ ارز اضافه نشد.";
                    return BadRequest(apiResponse);
                }

                // USD => Kaldar rate
                newRate = new CurrencyExchangeRate()
                {
                    BaseCurrencyId = usaCurrency.CurrencyId,
                    TargetCurrencyId = kaldarCurrency.CurrencyId,
                    Buy = 279,
                    Sell = 280,
                    UserId = currentUserId,
                    Unit = 1,
                    EffectiveDate = DateTime.Now,
                };

                isAdded = await _genericCurrencyExchangeRate.AddAsync(newRate);
                if (!isAdded)
                {
                    apiResponse.Message = "نرخ ارز اضافه نشد.";
                    return BadRequest(apiResponse);
                }

                user.isFirstTimeLogin = false;
                await userRepo.SaveAsync();

                apiResponse.Success = true;
                apiResponse.Data = true;
                apiResponse.Message = "حسابات اولیه با موفقیت ساخته شد.";
                await _genericRepository.SaveAsync();
                //await _customerAccountRepo.SaveAsync();
                await _genericRepository.CommitTransactionAsync();
                //apiResponse.Data = (int)result;

                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                await _genericRepository.RollbackTransactionAsync();
                apiResponse.Success = false;
                apiResponse.Message = "خطای در سرور رخ داده است";
                return StatusCode(StatusCodes.Status500InternalServerError, apiResponse);
            }
        }

    }
}
