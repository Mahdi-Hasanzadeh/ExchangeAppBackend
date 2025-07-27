using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared;
using Shared.Contract;
using Shared.Contract.CustomerAccount;
using Shared.DTOs;
using Shared.DTOs.CurrencyDTOs;
using Shared.Models.Currency;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CurrencyController : ControllerBase
    {
        private readonly ILogger<CurrencyController> _logger;
        private readonly IGenericRepository<CurrencyEntity> _currencyGenericRepo;
        private readonly IGenericRepository<CurrencyExchangeRate> _genericCurrencyExchangeRate;
        private readonly IAuthorizationService _authorizationService;
        private readonly ICurrencyRepository _currencyRepository;
        private readonly ICustomerAccountRepo _customerAccountRepo;

        public CurrencyController(
            ILogger<CurrencyController> logger,
            IGenericRepository<CurrencyEntity> currencyGenericRepo,
            IGenericRepository<CurrencyExchangeRate> genericCurrencyExchangeRate,
            IAuthorizationService authorizationService,
            ICurrencyRepository currencyRepository,
            ICustomerAccountRepo customerAccountRepo
            )
        {
            _logger = logger;
            _currencyGenericRepo = currencyGenericRepo;
            _genericCurrencyExchangeRate = genericCurrencyExchangeRate;
            _authorizationService = authorizationService;
            _currencyRepository = currencyRepository;
            _customerAccountRepo = customerAccountRepo;
        }

        [HttpGet(Name = "GetAllCurrencies")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<IEnumerable<CurrencyDTO>>>> GetAllCurrencies()
        {
            _logger.LogError("getting all currencies");
            var apiResponse = new ApiResponse<IEnumerable<CurrencyDTO>>(false);
            try
            {
                //This method extract the userId from token
                var currentUserId = User.GetUserId();

                //TODO
                // for now we use 1 for currentUserId

                //int currentUserId = 1;

                var result = await _currencyRepository.GetAllCurrenciesByUserId(currentUserId);

                if (result == null)
                {
                    apiResponse.Message = "ارز وجود ندارد";
                    return NotFound(apiResponse);
                }

                _logger.LogError("getting all currencies ENDED");


                apiResponse.Success = true;
                apiResponse.Data = result.ToCurrencyDTOs();
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

        [HttpPost(Name = "AddNewCurrency")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<int>>> AddCurrency(CurrencyDTO currencyDTO)
        {


            var apiResponse = new ApiResponse<int>(false);
            try
            {

                // This method extract the userId from token
                var currentUserId = User.GetUserId();

                //TODO
                // for now we use 1 for currentUserId

                //int currentUserId = 1;


                // check if the treasurey account exist
                var treasuryAccountId = await _customerAccountRepo.GetTreasureAccountId(currentUserId);
                if (treasuryAccountId == null)
                {
                    apiResponse.Message = "حساب صندوق در سیستم وجود ندارد،لطفا حساب صندوق را بسازید.";
                    return Ok(apiResponse);
                }

                var newCurrency = currencyDTO.ToCurrencyEntity();
                await _currencyGenericRepo.BeginTransactionAsync();

                var result = await _currencyGenericRepo.AddAsync(newCurrency);

                if (!result || result == null)
                {
                    apiResponse.Message = "ارز اضافه نشد";
                    await _currencyGenericRepo.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }
                await _currencyGenericRepo.SaveAsync();
                // create a balance for currency for the treasury account


                result = await _customerAccountRepo.CreateTreasuryBalanceForCurrency(newCurrency.CurrencyId, (int)treasuryAccountId, currentUserId);
                if (!result || result == null)
                {
                    apiResponse.Message = "ارز اضافه نشد";
                    await _currencyGenericRepo.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }

                await _currencyGenericRepo.SaveAsync();

                await _currencyGenericRepo.CommitTransactionAsync();

                apiResponse.Success = true;
                apiResponse.Data = newCurrency.CurrencyId;
                apiResponse.Message = "ارز با موفقیت اضافه شد.";
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

        [HttpPut("{currencyId:int}", Name = "UpdateCurrency")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<bool>>> UpdateCurrency(int currencyId, CurrencyDTO currencyDTO)
        {
            var apiResponse = new ApiResponse<bool>(false);
            try
            {

                // This method extract the userId from token
                var currentUserId = User.GetUserId();

                //TODO
                // for now we use 1 for currentUserId

                //int currentUserId = 1;


                // Check if the current user is the owner of the currency
                bool isOwner = await _authorizationService.IsUserOwnerOfCurrency(currencyId, currentUserId);
                if (!isOwner)
                {
                    apiResponse.Message = "شما اجازه دسترسی به این اطلاعات را ندارید";
                    return Unauthorized(apiResponse);
                }

                var result = await _currencyRepository.UpdateCurrency(currentUserId, currencyId, currencyDTO);

                if (!result || result == null)
                {
                    apiResponse.Message = "ارز بروز رسانی نشد";
                    return BadRequest(apiResponse);
                }
                await _currencyGenericRepo.SaveAsync();

                apiResponse.Success = true;
                apiResponse.Message = "ارز با موفقیت بروز رسانی شد.";
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

        [HttpPut("activation/{currencyId:int}", Name = "UpdateActivationCurrency")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<bool>>> UpdateCurrencyActivation(int currencyId, CurrencyActivationDTO currencyActivationDTO)
        {
            var apiResponse = new ApiResponse<bool>(false);
            try
            {

                // This method extract the userId from token
                var currentUserId = User.GetUserId();

                //TODO
                // for now we use 1 for currentUserId

                //int currentUserId = 1;


                // Check if the current user is the owner of the currency
                bool isOwner = await _authorizationService.IsUserOwnerOfCurrency(currencyId, currentUserId);
                if (!isOwner)
                {
                    apiResponse.Message = "شما اجازه دسترسی به این اطلاعات را ندارید";
                    return Unauthorized(apiResponse);
                }

                var result = await _currencyRepository.UpdateCurrencyAtivation(currentUserId, currencyId, currencyActivationDTO);

                if (!result || result == null)
                {
                    apiResponse.Message = "ارز بروز رسانی نشد";
                    return BadRequest(apiResponse);
                }
                await _currencyGenericRepo.SaveAsync();

                apiResponse.Success = true;
                apiResponse.Message = "ارز با موفقیت بروز رسانی شد.";
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

        //id in here is currencyId
        [HttpDelete("{currencyId:int}", Name = "DeleteCurrencyById")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<int>>> DeleteCurrency(int currencyId)
        {
            var apiResponse = new ApiResponse<bool>(false);
            try
            {
                // This method extract the userId from token
                var currentUserId = User.GetUserId();

                //TODO
                // for now we use 1 for currentUserId

                //int currentUserId = 1;


                // Check if the current user is the owner of the customer
                bool isOwner = await _authorizationService.IsUserOwnerOfCurrency(currencyId, currentUserId);
                if (!isOwner)
                {
                    apiResponse.Message = "شما اجازه دسترسی به این اطلاعات را ندارید";
                    return Unauthorized(apiResponse);
                }
                var result = await _currencyGenericRepo.DeleteByIdAsync(currencyId);

                if (!result || result == null)
                {
                    apiResponse.Message = "ارز حذف نشد";
                    return BadRequest(apiResponse);
                }
                await _currencyGenericRepo.SaveAsync();

                apiResponse.Success = true;
                apiResponse.Message = "ارز با موفقیت حذف شد.";
                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                apiResponse.Success = false;
                apiResponse.Message = "خطای در سرور رخ داده است.ارز حذف نشد.";
                return StatusCode(StatusCodes.Status500InternalServerError, apiResponse);
            }

        }

        [HttpGet("currencyDetail", Name = "GetAllCurrenciesDetail")]

        public async Task<ActionResult<ApiResponse<IEnumerable<CurrencyDetailDTOForAllRates>>>> GetAllCurrenciesDetail()
        {
            var apiResponse = new ApiResponse<IEnumerable<CurrencyDetailDTOForAllRates>>(false);
            _logger.LogError("getting all currencies details");

            try
            {
                // This method extract the userId from token
                var currentUserId = User.GetUserId();

                //TODO
                // for now we use 1 for currentUserId

                //int currentUserId = 1;

                var result = await _currencyRepository.GetAllCurrencyDetails(currentUserId);

                if (result == null)
                {
                    apiResponse.Message = "جزییات ارز وجود ندارد";
                    return NotFound(apiResponse);
                }
                _logger.LogError("getting all currencies details ended");

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

        [HttpGet("currencyDetail/{baseCurrencyId:int}", Name = "GetAllCurrenciesDetailBasedOnBaseCurrency")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<IEnumerable<CurrencyDetailDTO>>>> GetCurrenciesDetailByBaseCurrencyId(int baseCurrencyId)
        {
            var apiResponse = new ApiResponse<IEnumerable<CurrencyDetailDTO>>(false);
            _logger.LogError("getting all currencies details based on baseCurrencyId");

            try
            {
                // This method extract the userId from token
                var currentUserId = User.GetUserId();

                //TODO
                // for now we use 1 for currentUserId

                //int currentUserId = 1;

                var result = await _currencyRepository.GetAllCurrencyDetails(baseCurrencyId, currentUserId);

                if (result == null)
                {
                    apiResponse.Message = "جزییات ارز وجود ندارد";
                    return NotFound(apiResponse);
                }
                _logger.LogError("getting all currencies details based on baseCurrencyId ended");

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

        [HttpPut("currencyRate/{oldExchangeRateId}", Name = "UpdateCurrencyExchangeRate")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<int>>> UpdateCurrencyExchangeRate(int oldExchangeRateId, CurrencyExchangeRateDTO currencyExchangeRateDTO)
        {
            await _currencyGenericRepo.BeginTransactionAsync();
            var apiResponse = new ApiResponse<int>(false);
            try
            {

                // This method extract the userId from token
                var currentUserId = User.GetUserId();

                // Check if the current user is the owner of the currency
                bool isOwner = await _authorizationService.IsUserOwnerOfCurrencyExchangeRate
                    (oldExchangeRateId, currentUserId);
                if (!isOwner)
                {
                    apiResponse.Message = "شما اجازه دسترسی به این اطلاعات را ندارید";
                    await _currencyGenericRepo.RollbackTransactionAsync();
                    return Unauthorized(apiResponse);
                }
                var result = await _currencyRepository.DisableCurrencyExchangeRate(currentUserId, oldExchangeRateId);
                if (!result)
                {
                    apiResponse.Message = "نرخ ارز بروز رسانی نشد.";
                    await _currencyGenericRepo.RollbackTransactionAsync();
                    return BadRequest(apiResponse);
                }

                // Add new Currency Exchange Rate
                var newRate = currencyExchangeRateDTO.ToCurrencyExchangeRate();

                var isAdded = await _genericCurrencyExchangeRate.AddAsync(newRate);
                if (!isAdded)
                {
                    apiResponse.Message = "نرخ ارز بروز رسانی نشد.";
                    await _currencyGenericRepo.RollbackTransactionAsync();
                    return BadRequest(apiResponse);
                }

                await _currencyGenericRepo.SaveAsync();
                await _currencyGenericRepo.CommitTransactionAsync();
                apiResponse.Success = true;
                apiResponse.Data = newRate.Id;
                apiResponse.Message = "نرخ ارز با موفقیت بروز رسانی شد.";
                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                apiResponse.Success = false;
                apiResponse.Message = "خطای در سرور رخ داده است. نرح ارز بروز رسانی نشد.";
                return StatusCode(StatusCodes.Status500InternalServerError, apiResponse);
            }
        }

        [HttpPost("currencyRate", Name = "AddCurrencyExchangeRate")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<int>>> AddCurrencyExchangeRate
            (CurrencyExchangeRateDTO currencyExchangeRateDTO)
        {
            var apiResponse = new ApiResponse<int>(false);
            try
            {

                // This method extract the userId from token
                //var currentUserId = User.GetUserId();

                //TODO
                // for now we use 1 for currentUserId


                // TODO: check that we need user authentication here or not
                //int currentUserId = 1;


                //// Check if the current user is the owner of the currency
                //bool isOwner = await _authorizationService.IsUserOwnerOfCurrencyExchangeRate
                //    (oldExchangeRateId, currentUserId);

                //if (!isOwner)
                //{
                //    apiResponse.Message = "شما اجازه دسترسی به این اطلاعات را ندارید";
                //    await _currencyGenericRepo.RollbackTransactionAsync();
                //    return Unauthorized(apiResponse);
                //}

                // Add new Currency Exchange Rate
                var newRate = currencyExchangeRateDTO.ToCurrencyExchangeRate();

                var isAdded = await _genericCurrencyExchangeRate.AddAsync(newRate);
                if (!isAdded)
                {
                    apiResponse.Message = "نرخ ارز اضافه نشد.";
                    return BadRequest(apiResponse);
                }

                await _currencyGenericRepo.SaveAsync();
                apiResponse.Success = true;
                apiResponse.Data = newRate.Id;
                apiResponse.Message = "نرخ ارز با موفقیت اضافه شد.";
                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                apiResponse.Success = false;
                apiResponse.Message = "خطای در سرور رخ داده است. نرح ارز بروز رسانی نشد.";
                return StatusCode(StatusCodes.Status500InternalServerError, apiResponse);
            }
        }

    }
}
