using API.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared;
using Shared.Contract;
using Shared.Contract.CustomerAccount;
using Shared.DTOs;
using Shared.DTOs.CurrencyDTOs;
using Shared.Enums;
using Shared.Models;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class CustomerTransactionController : ControllerBase
    {
        private readonly ILogger<CustomerTransactionController> _logger;
        private readonly IGenericRepository<CustomerTransactionHistory> _genericRepository;
        private readonly ICustomerAccountRepo _customerAccountRepo;
        private readonly IAuthorizationService _authorizationService;
        private readonly ICurrencyRepository _currencyRepository;

        public CustomerTransactionController
            (ILogger<CustomerTransactionController> logger,
            IGenericRepository<CustomerTransactionHistory> genericRepository,
            ICustomerAccountRepo customerAccountRepo,
            IAuthorizationService authorizationService,
            ICurrencyRepository currencyRepository
            )
        {
            _logger = logger;
            _genericRepository = genericRepository;
            _customerAccountRepo = customerAccountRepo;
            _authorizationService = authorizationService;
            _currencyRepository = currencyRepository;
        }

        [HttpDelete("{transactionId:int}", Name = "DeleteCustomerTransactionById")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<bool>>> RemoveCustomerTransactionById(int transactionId)
        {
            var apiResponse = new ApiResponse<bool>(false);
            try
            {
                var currentUserId = User.GetUserId();

                bool isOwner = await _authorizationService.IsUserOwnerOfTransaction(transactionId, currentUserId);

                if (!isOwner)
                {
                    apiResponse.Message = "شما اجازه دسترسی به این اطلاعات را ندارید";
                    return Unauthorized(apiResponse);
                }

                var transaction = await _genericRepository.GetByIdAsync(transactionId);

                if (transaction == null)
                {
                    apiResponse.Message = "تراکنش مشتری یافت نشد.";
                    return Ok(apiResponse);
                }

                // update customer balance

                await _genericRepository.BeginTransactionAsync();

                var result = await _customerAccountRepo.UpdateBalanceAfterRemoveTransaction
                    (currentUserId, transaction.CustomerId, transaction.CurrencyId, transaction.Amount, transaction.DealType);

                if (!result.Item1)
                {
                    apiResponse.Message = result.Item2;
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }

                if (!await _genericRepository.DeleteByIdAsync(transactionId))
                {
                    apiResponse.Message = "تراکنش مشتری حذف نشد.";
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
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
                    .CalculateCustomerBalanceInUSD(currentUserId, transaction.CustomerId, exchangeRates.ToList());

                if (totalbalance == null)
                {
                    apiResponse.Message = "مشتری یافت نشد.";
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }

                var borrowamount = await _customerAccountRepo.GetBorrowAmountOfACustomer(currentUserId, transaction.CustomerId);

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


                // check if the deal type is deposit, so withdraw from treasury account
                var dealType = !(transaction.DealType == DealType.Deposit);

                // update treasury account
                var TreasuryUpdated = await _customerAccountRepo
                       .UpdateTreasuryAccount(currentUserId, Math.Abs(transaction.Amount), transaction.CurrencyId, dealType);
                if (!TreasuryUpdated)
                {
                    apiResponse.Message = "خطار در بروز رسانی حساب صندوق!";
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }

                if (transaction.DealType == DealType.Deposit)
                {
                    var isBorrowAllowed = await _customerAccountRepo.CheckTreasuryAccountBasedOnBorrowAmount(currentUserId);
                    if (!isBorrowAllowed.Item1)
                    {
                        apiResponse.Message = isBorrowAllowed.Item2;
                        await _genericRepository.RollbackTransactionAsync();
                        return Ok(apiResponse);
                    }
                }

                apiResponse.Success = true;
                await _customerAccountRepo.SaveAsync();
                await _genericRepository.CommitTransactionAsync();
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

        [HttpGet("transactionsTotal", Name = "Get the total transactions of a User")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<int>>> GetTransactionsTotal()
        {
            var apiResponse = new ApiResponse<int>(false);
            try
            {
                var currentUserId = User.GetUserId();
                var result = await _customerAccountRepo.GetTotalOfTransactions(currentUserId);
                if (result == null)
                {
                    apiResponse.Message = "دریافت تعداد تراکنش با مشکل مواجه شد.";
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

        [HttpPost("dailyTransactions", Name = "GetdailyTransactions")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<IEnumerable<TransactionHistoryDTO>>>>
        GetDailyTransactions([FromQuery] int? currencyId, [FromQuery] DateTime? FromDate, [FromQuery] DateTime? ToDate)
        {
            var apiResponse = new ApiResponse<IEnumerable<TransactionHistoryDTO>>(false);
            try
            {
                // This method extract the userId from token
                var currentUserId = User.GetUserId();

                //// Check if the current user is the owner of the customer
                //bool isOwner = await _authorizationService.IsUserOwnerOfCustomerAsync(id, currentUserId);
                //if (!isOwner)
                //{
                //    apiResponse.Message = "شما اجازه دسترسی به این اطلاعات را ندارید";
                //    return Unauthorized(apiResponse);
                //}

                var transactions = await _customerAccountRepo.GetDailyTransactions(currentUserId, FromDate, ToDate, currencyId);
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


        [HttpGet("totalDepositAndWithdrawPerCurrency", Name = "TotalDepositAndWithdrawPerCurrency")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<List<CurrencyDepositWithdrawDTO>>>> CalculatetotalDepositAndWithdrawPerCurrency()
        {
            var apiResponse = new ApiResponse<List<CurrencyDepositWithdrawDTO>>(false);
            try
            {
                var currentUserId = User.GetUserId();
                var result = await _customerAccountRepo.CalculateDepositWithdrawPerCurrency(currentUserId);
                if (result == null)
                {
                    apiResponse.Message = "محاسبه مجموع رسید و برداشت با مشکل مواجه شد.";
                    return Ok(apiResponse);
                }
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


        [HttpGet("totalBuySellPerCurrency", Name = "TotalBuySellPerCurrency")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<List<CurrencyBuySellDTO>>>> CalculateTotalBuySellPerCurrency()
        {
            var apiResponse = new ApiResponse<List<CurrencyBuySellDTO>>(false);
            try
            {
                var currentUserId = User.GetUserId();
                var result = await _customerAccountRepo.CalculateBuySellCurrencyPerCurrency(currentUserId);
                if (result == null)
                {
                    apiResponse.Message = "محاسبه مجموع خرید و فروش ارز با مشکل مواجه شد.";
                    return Ok(apiResponse);
                }
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




        //TODO: Fix it, we have problem with this method to how to calcluate for each currency
        [HttpGet("lossAndBenefit", Name = "GetLossAndBenefitOfCustomer")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<List<ProfitLossSummaryDTO>>>> CalculateLossAndBenefitsOfACustomer()
        {
            var apiResponse = new ApiResponse<List<ProfitLossSummaryDTO>>(false);
            try
            {
                var currentUserId = User.GetUserId();


                var result = await _customerAccountRepo.CalculateProfitLossInAsync(currentUserId);
                if (result == null)
                {
                    apiResponse.Message = "محاسبه سود و زیان با مشکل مواجه شد.";
                    return Ok(apiResponse);
                }
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


    }
}
