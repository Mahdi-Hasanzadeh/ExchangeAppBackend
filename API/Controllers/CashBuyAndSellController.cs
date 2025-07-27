using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.Contract.CustomerAccount;
using Shared.Contract.Transactions;
using Shared.Contract;
using Shared.Models;
using Shared.DTOs.TransactionsDTOs;
using Shared.DTOs;
using Shared.Enums;
using Shared;
using API.Repositories.Transactions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using API.DataContext;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CashBuyAndSellController : ControllerBase
    {
        private readonly ILogger<CashBuyAndSellController> _logger;
        private readonly IGenericRepository<BuyAndSellTransaction> _genericRepository;
        private readonly ICustomerAccountRepo _customerAccountRepo;
        private readonly IAuthorizationService _authorizationService;
        private readonly ICurrencyRepository _currencyRepository;
        private readonly IGenericRepository<CustomerTransactionHistory> _customerTransactionHistoryGeneric;

        public CashBuyAndSellController(ILogger<CashBuyAndSellController> logger,
            IGenericRepository<BuyAndSellTransaction> genericRepository,
            ICustomerAccountRepo customerAccountRepo,
            IAuthorizationService authorizationService,
            ICurrencyRepository currencyRepository,
            IGenericRepository<CustomerTransactionHistory> customerTransactionHistoryGeneric
            )
        {
            _logger = logger;
            _genericRepository = genericRepository;
            _customerAccountRepo = customerAccountRepo;
            _authorizationService = authorizationService;
            _currencyRepository = currencyRepository;
            _customerTransactionHistoryGeneric = customerTransactionHistoryGeneric;
        }

        [HttpPost(Name = "AddTransaction")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<int>>> AddTransferTransation(BuyAndSellTransactionDTO newTransactionDTO)
        {
            _logger.LogError("Cash Buy/Sell currency Started ");

            var apiResponse = new ApiResponse<int>(false);
            try
            {
                var currentUserId = User.GetUserId();

                int exchangeAccountId = await _customerAccountRepo.IsCurrencyExchangeAccountExist(currentUserId);
                if (exchangeAccountId == 0)
                {
                    apiResponse.Message = "حساب تبادله ارز در سیستم وجود ندارد.";
                    return Ok(apiResponse);
                }

                bool isOwner = await _authorizationService.IsUserOwnerOfCustomerAsync
                    (exchangeAccountId, currentUserId);
                if (!isOwner)
                {
                    apiResponse.Message = "شما اجازه دسترسی به این اطلاعات را ندارید";
                    return Ok(apiResponse);
                }

                newTransactionDTO.CurrencyExchangeAccountId = exchangeAccountId;

                await _genericRepository.BeginTransactionAsync();

                // add buy and sell transaction to transactions history

                // add sell transaction(converted Amount)
                int sellTransactionId = 0;
                int buyTransactionId = 0;

                if (newTransactionDTO.TransactionType == TransactionType.Buy)
                {
                    // Buy
                    // Withdraw from exchange account
                    var result = await _customerAccountRepo.WithdrawByCustomerIdAsync
                        (currentUserId, exchangeAccountId, newTransactionDTO.TargetCurrencyId, newTransactionDTO.ConvertedAmount);
                    if (!result.Item1)
                    {
                        apiResponse.Message = result.Item2;
                        await _genericRepository.RollbackTransactionAsync();
                        return Ok(apiResponse);
                    }

                    var transactionDTO = new CustomerTransactionHistoryDTO()
                    {
                        Amount = -newTransactionDTO.ConvertedAmount,
                        CurrencyId = newTransactionDTO.TargetCurrencyId,
                        CreatedDate = newTransactionDTO.CreatedDate,
                        CustomerId = exchangeAccountId,
                        DealType = DealType.Withdraw,
                        Description = newTransactionDTO.Description,
                        TransactionType = TransactionType.Sell,
                        DepositOrWithdrawBy = string.Empty,
                        UserId = newTransactionDTO.UserId
                        //DocumentNumber = transferDTO.

                    };

                    sellTransactionId = await _customerAccountRepo.AddCustomerTransactionAsync(transactionDTO);

                    if (sellTransactionId == 0)
                    {
                        apiResponse.Message = "برداشت پول از حساب تبادله ارز با مشکل مواجه شد.";
                        await _genericRepository.RollbackTransactionAsync();
                        return Ok(apiResponse);
                    }

                    // add buy transaction
                    // Deposit
                    var deposit = await _customerAccountRepo.DepositeByCustomerIdAsync
                        (currentUserId, exchangeAccountId, newTransactionDTO.SourceCurrencyId, newTransactionDTO.Amount);
                    if (!deposit)
                    {
                        apiResponse.Message = "واریز پول به حساب تبادله ارز انجام نشد.";
                        await _genericRepository.RollbackTransactionAsync();
                        return BadRequest(apiResponse);
                    }

                    transactionDTO = new CustomerTransactionHistoryDTO()
                    {
                        Amount = newTransactionDTO.Amount,
                        CurrencyId = newTransactionDTO.SourceCurrencyId,
                        CreatedDate = newTransactionDTO.CreatedDate,
                        CustomerId = exchangeAccountId,
                        DealType = DealType.Deposit,
                        Description = newTransactionDTO.Description,
                        TransactionType = TransactionType.Buy,
                        DepositOrWithdrawBy = string.Empty,
                        UserId = newTransactionDTO.UserId
                    };

                    buyTransactionId = await _customerAccountRepo.AddCustomerTransactionAsync(transactionDTO);
                    if (buyTransactionId == 0)
                    {
                        apiResponse.Message = "واریز پول به حساب تبادله ارز انجام نشد.";
                        await _genericRepository.RollbackTransactionAsync();
                        return Ok(apiResponse);
                    }
                }
                else
                {
                    // Sell
                    // Withdraw from exchange account
                    var result = await _customerAccountRepo.WithdrawByCustomerIdAsync
                        (currentUserId, exchangeAccountId, newTransactionDTO.SourceCurrencyId, newTransactionDTO.Amount);
                    if (!result.Item1)
                    {
                        apiResponse.Message = result.Item2;
                        await _genericRepository.RollbackTransactionAsync();
                        return Ok(apiResponse);
                    }

                    var transactionDTO = new CustomerTransactionHistoryDTO()
                    {
                        Amount = -newTransactionDTO.Amount,
                        CurrencyId = newTransactionDTO.SourceCurrencyId,
                        CreatedDate = newTransactionDTO.CreatedDate,
                        CustomerId = exchangeAccountId,
                        DealType = DealType.Withdraw,
                        Description = newTransactionDTO.Description,
                        TransactionType = TransactionType.Sell,
                        DepositOrWithdrawBy = string.Empty,
                        UserId = newTransactionDTO.UserId
                        //DocumentNumber = transferDTO.
                    };

                    sellTransactionId = await _customerAccountRepo.AddCustomerTransactionAsync(transactionDTO);

                    if (sellTransactionId == 0)
                    {
                        apiResponse.Message = "برداشت پول از حساب تبادله ارز با مشکل مواجه شد.";
                        await _genericRepository.RollbackTransactionAsync();
                        return Ok(apiResponse);
                    }

                    // add buy transaction
                    // Deposit
                    var deposit = await _customerAccountRepo.DepositeByCustomerIdAsync
                        (currentUserId, exchangeAccountId, newTransactionDTO.TargetCurrencyId, newTransactionDTO.ConvertedAmount);
                    if (!deposit)
                    {
                        apiResponse.Message = "واریز پول به حساب تبادله ارز انجام نشد.";
                        await _genericRepository.RollbackTransactionAsync();
                        return BadRequest(apiResponse);
                    }

                    transactionDTO = new CustomerTransactionHistoryDTO()
                    {
                        Amount = newTransactionDTO.ConvertedAmount,
                        CurrencyId = newTransactionDTO.TargetCurrencyId,
                        CreatedDate = newTransactionDTO.CreatedDate,
                        CustomerId = exchangeAccountId,
                        DealType = DealType.Deposit,
                        Description = newTransactionDTO.Description,
                        TransactionType = TransactionType.Buy,
                        DepositOrWithdrawBy = string.Empty,
                        UserId = newTransactionDTO.UserId
                    };

                    buyTransactionId = await _customerAccountRepo.AddCustomerTransactionAsync(transactionDTO);
                    if (buyTransactionId == 0)
                    {
                        apiResponse.Message = "واریز پول به حساب تبادله ارز انجام نشد.";
                        await _genericRepository.RollbackTransactionAsync();
                        return Ok(apiResponse);
                    }

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
                    .CalculateCustomerBalanceInUSD(currentUserId, exchangeAccountId, exchangeRates.ToList());

                if (totalBalance == null)
                {
                    apiResponse.Message = "مشتری یافت نشد.";
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }

                var BorrowAmount = await _customerAccountRepo.GetBorrowAmountOfACustomer(currentUserId, exchangeAccountId);

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

                // update treasury account

                var treasuryAccountUpdated = await _customerAccountRepo
                    .UpdateTreasuryAccount(currentUserId, newTransactionDTO.Amount, newTransactionDTO.SourceCurrencyId,
                    newTransactionDTO.TransactionType == TransactionType.Buy);
                if (!treasuryAccountUpdated)
                {
                    apiResponse.Message = "حساب صندوق بروز رسانی نشد.";
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }

                treasuryAccountUpdated = await _customerAccountRepo
                    .UpdateTreasuryAccount(currentUserId, newTransactionDTO.ConvertedAmount, newTransactionDTO.TargetCurrencyId,
                    newTransactionDTO.TransactionType != TransactionType.Buy);
                if (!treasuryAccountUpdated)
                {
                    apiResponse.Message = "حساب صندوق بروز رسانی نشد.";
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }


                var check = await _customerAccountRepo.CheckTreasuryAccountBasedOnBorrowAmount(currentUserId);
                if (!check.Item1)
                {
                    apiResponse.Message = check.Item2;
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }


                // add buy/sell transaction

                newTransactionDTO.BuyTransactionId = buyTransactionId;
                newTransactionDTO.SellTransactionId = sellTransactionId;
                newTransactionDTO.BuyAndSellType = CurrencyBuyAndSellType.Cash;


                var transaction = await _genericRepository.AddEntityAsync(newTransactionDTO.ToBuyAndSellTransaction());

                if (transaction == null)
                {
                    apiResponse.Message = "تراکنش انتقال ثبت نشد.";
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }
                await _genericRepository.SaveAsync();
                await _genericRepository.CommitTransactionAsync();
                apiResponse.Success = true;
                apiResponse.Data = transaction.Id;
                _logger.LogError("adding buy/sell transaction: ENDEDDDDDDDD");

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

        //Get Cash Buy and Sell transactions of the app's owner
        [HttpGet("{userId:int}", Name = "GetAllBuyAndSellTransactions")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<IEnumerable<BuyAndSellTransactionDTO>>>> GetBuyAndSellTransactions(int userId)
        {
            var apiResponse = new ApiResponse<IEnumerable<BuyAndSellTransactionDTO>>(false);
            try
            {
                _logger.LogError("Getting Buy & Sell Transactions");
                var currentUserId = User.GetUserId();


                var result = await _customerAccountRepo.GetConvertedDTOById(currentUserId);

                if (result == null)
                {
                    apiResponse.Message = "دریافت تراکنش های خرید و فروش با مشکل مواجه شد.";
                    return Ok(apiResponse);
                }

                apiResponse.Success = true;
                apiResponse.Data = result;
                _logger.LogError("Getting Buy & Sell transactions: ENDED");
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

        [HttpDelete("{transactionId:int}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteBuyAndSellTransaction(int transactionId)
        {
            var apiResponse = new ApiResponse<bool>(false);
            try
            {
                _logger.LogInformation($"Deleting Buy&Sell Transaction:STARTED: {transactionId}");
                var currentUserId = User.GetUserId();

                var oldTransaction = await _genericRepository.GetByIdAsync(transactionId);
                if (oldTransaction == null)
                {
                    apiResponse.Message = "تراکنش خرید/فروش یافت نشد.";
                    return Ok(apiResponse);
                }
                bool isOwner = await _authorizationService
                    .IsUserOwnerOfTransaction(oldTransaction.BuyTransactionId, currentUserId);

                if (!isOwner)
                {
                    apiResponse.Message = "شما اجازه دسترسی به این اطلاعات را ندارید";
                    return Unauthorized(apiResponse);
                }

                await _genericRepository.BeginTransactionAsync();

                if (oldTransaction.TransactionType == TransactionType.Buy)
                {
                    // sourceCurrency should be withdraw from account
                    var isWithdrawed = await _customerAccountRepo.WithdrawByCustomerIdAsync
                     (currentUserId, oldTransaction.CurrencyExchangeAccountId, oldTransaction.SourceCurrencyId, oldTransaction.Amount);

                    if (!isWithdrawed.Item1)
                    {
                        apiResponse.Message = isWithdrawed.Item2;
                        await _genericRepository.RollbackTransactionAsync();
                        return Ok(apiResponse);
                    }

                    // targetCurrency should be deposit to account

                    // Deposit
                    var isDeposited = await _customerAccountRepo.DepositeByCustomerIdAsync
                        (currentUserId, oldTransaction.CurrencyExchangeAccountId, oldTransaction.TargetCurrencyId, oldTransaction.ConvertedAmount);

                    if (!isDeposited)
                    {
                        apiResponse.Message = "خطا در بروز رسانی حساب تبادله ارز";
                        await _genericRepository.RollbackTransactionAsync();
                        return Ok(apiResponse);
                    }

                }
                else
                {
                    // targetCurrency should be Withdraw from account
                    var isWithdrawed = await _customerAccountRepo.WithdrawByCustomerIdAsync
                     (currentUserId, oldTransaction.CurrencyExchangeAccountId, oldTransaction.TargetCurrencyId, oldTransaction.ConvertedAmount);

                    if (!isWithdrawed.Item1)
                    {
                        apiResponse.Message = isWithdrawed.Item2;
                        await _genericRepository.RollbackTransactionAsync();
                        return Ok(apiResponse);
                    }

                    // sourceCurrency Should be deposit
                    // Deposit
                    var isDeposited = await _customerAccountRepo.DepositeByCustomerIdAsync
                        (currentUserId, oldTransaction.CurrencyExchangeAccountId, oldTransaction.SourceCurrencyId, oldTransaction.Amount);

                    if (!isDeposited)
                    {
                        apiResponse.Message = "خطا در بروز رسانی حساب تبادله ارز";
                        await _genericRepository.RollbackTransactionAsync();
                        return Ok(apiResponse);
                    }
                }

                // check exchange account for borrow
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
                    .CalculateCustomerBalanceInUSD(currentUserId, oldTransaction.CurrencyExchangeAccountId, exchangeRates.ToList());

                if (totalBalance == null)
                {
                    apiResponse.Message = "مشتری یافت نشد.";
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }

                var BorrowAmount = await _customerAccountRepo.GetBorrowAmountOfACustomer(currentUserId, oldTransaction.CurrencyExchangeAccountId);

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

                // Delete buy and sell transaction

                var isRemoved = await _customerTransactionHistoryGeneric.DeleteByIdAsync(oldTransaction.BuyTransactionId);
                if (!isRemoved)
                {
                    apiResponse.Message = "تراکنش حذف نشد.";
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }

                isRemoved = await _customerTransactionHistoryGeneric.DeleteByIdAsync(oldTransaction.SellTransactionId);
                if (!isRemoved)
                {
                    apiResponse.Message = "تراکنش حذف نشد.";
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }

                // updateTreasuryAccount
                var treasuryAccountUpdated = await _customerAccountRepo
                    .UpdateTreasuryAccount(currentUserId, oldTransaction.Amount, oldTransaction.SourceCurrencyId,
                    oldTransaction.TransactionType == TransactionType.Buy ? false : true);
                if (!treasuryAccountUpdated)
                {
                    apiResponse.Message = "حساب صندوق بروز رسانی نشد.";
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }

                treasuryAccountUpdated = await _customerAccountRepo
                    .UpdateTreasuryAccount(currentUserId, oldTransaction.ConvertedAmount, oldTransaction.TargetCurrencyId,
                    oldTransaction.TransactionType == TransactionType.Buy ? true : false);
                if (!treasuryAccountUpdated)
                {
                    apiResponse.Message = "حساب صندوق بروز رسانی نشد.";
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }

                var check = await _customerAccountRepo.CheckTreasuryAccountBasedOnBorrowAmount(currentUserId);
                if (!check.Item1)
                {
                    apiResponse.Message = check.Item2;
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }

                // delete BuyAndSellTransaction

                isRemoved = await _genericRepository.DeleteByIdAsync(oldTransaction.Id);
                if (!isRemoved)
                {
                    apiResponse.Message = "تراکنش حذف نشد.";
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }

                await _genericRepository.SaveAsync();
                await _genericRepository.CommitTransactionAsync();
                apiResponse.Success = true;
                apiResponse.Data = true;
                apiResponse.Message = "تراکنش حذف شد.";
                _logger.LogInformation($"Deleting Buy & Sell Transaction:ENDED: {transactionId}");
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

        [HttpPut("updateDetails/{transactionId:int}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<bool>>> UpdateTransferTransactionDetails(int transactionId, BuyAndSellTransactionDTO updatedTransaction)
        {
            var apiResponse = new ApiResponse<bool>(false);
            try
            {
                var currentUserId = User.GetUserId();

                var transaction = await _genericRepository.GetByIdAsync(updatedTransaction.Id);

                if (transaction == null)
                {
                    apiResponse.Message = "تراکنش یافت نشد.";
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }


                bool isOwner = await _authorizationService.IsUserOwnerOfTransaction(transaction.BuyTransactionId, currentUserId);

                if (!isOwner)
                {
                    apiResponse.Message = "شما اجازه دسترسی به این اطلاعات را ندارید";
                    return Unauthorized(apiResponse);
                }
                await _genericRepository.BeginTransactionAsync();

                var buyTransactionId = transaction.BuyTransactionId;
                var sellTransactionId = transaction.SellTransactionId;


                var buyTransaction = await _customerTransactionHistoryGeneric.GetByIdAsync(buyTransactionId);
                if (buyTransaction == null)
                {
                    apiResponse.Message = "تراکنش یافت نشد.";
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }

                buyTransaction.Description = updatedTransaction.Description;
                buyTransaction.CreatedDate = updatedTransaction.CreatedDate;
                buyTransaction.UpdatedDate = DateTime.Now;

                var sellTransaction = await _customerTransactionHistoryGeneric.GetByIdAsync(sellTransactionId);
                if (sellTransaction == null)
                {
                    apiResponse.Message = "تراکنش یافت نشد.";
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }

                sellTransaction.Description = updatedTransaction.Description;
                sellTransaction.CreatedDate = updatedTransaction.CreatedDate;
                sellTransaction.UpdatedDate = DateTime.Now;


                // update BuyAndSellTransaction table
                transaction.Description = updatedTransaction.Description;
                transaction.CreatedDate = updatedTransaction.CreatedDate;
                transaction.UpdatedDate = DateTime.Now;

                apiResponse.Success = true;
                apiResponse.Message = "تراکنش با موفقیت بروز رسانی شد.";
                await _genericRepository.SaveAsync();
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

        [HttpPut("{transactionId:int}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<bool>>> UpdateTransferTransaction(int transactionId, BuyAndSellTransactionDTO updatedTransaction)
        {
            var apiResponse = new ApiResponse<bool>(false);
            try
            {
                _logger.LogInformation($"updating Buy&Sell transaction STARTED: id:{transactionId}");

                var currentUserId = User.GetUserId();

                var oldTransaction = await _genericRepository.GetByIdAsync(transactionId);
                if (oldTransaction == null)
                {
                    apiResponse.Message = "تراکنش خرید/فروش یافت نشد.";
                    return Ok(apiResponse);
                }

                int exchangeAccountId = await _customerAccountRepo.IsCurrencyExchangeAccountExist(currentUserId);
                if (exchangeAccountId == 0)
                {
                    apiResponse.Message = "حساب تبادله ارز در سیستم وجود ندارد.";
                    return Ok(apiResponse);
                }

                bool isOwner = await _authorizationService
                    .IsUserOwnerOfTransaction(oldTransaction.BuyTransactionId, currentUserId);

                await _genericRepository.BeginTransactionAsync();

                // rollback based on transaction type
                if (oldTransaction.TransactionType == TransactionType.Buy)
                {
                    // sourceCurrency should be withdraw from account
                    var isWithdrawed = await _customerAccountRepo.WithdrawByCustomerIdAsync
                     (currentUserId, oldTransaction.CurrencyExchangeAccountId, oldTransaction.SourceCurrencyId, oldTransaction.Amount);

                    if (!isWithdrawed.Item1)
                    {
                        apiResponse.Message = isWithdrawed.Item2;
                        await _genericRepository.RollbackTransactionAsync();
                        return Ok(apiResponse);
                    }

                    // targetCurrency should be deposit to account

                    // Deposit
                    var isDeposited = await _customerAccountRepo.DepositeByCustomerIdAsync
                        (currentUserId, oldTransaction.CurrencyExchangeAccountId, oldTransaction.TargetCurrencyId, oldTransaction.ConvertedAmount);

                    if (!isDeposited)
                    {
                        apiResponse.Message = "خطا در بروز رسانی حساب تبادله ارز";
                        await _genericRepository.RollbackTransactionAsync();
                        return Ok(apiResponse);
                    }

                }
                else
                {
                    // targetCurrency should be Withdraw from account
                    var isWithdrawed = await _customerAccountRepo.WithdrawByCustomerIdAsync
                     (currentUserId, oldTransaction.CurrencyExchangeAccountId, oldTransaction.TargetCurrencyId, oldTransaction.ConvertedAmount);

                    if (!isWithdrawed.Item1)
                    {
                        apiResponse.Message = isWithdrawed.Item2;
                        await _genericRepository.RollbackTransactionAsync();
                        return Ok(apiResponse);
                    }

                    // sourceCurrency Should be deposit
                    // Deposit
                    var isDeposited = await _customerAccountRepo.DepositeByCustomerIdAsync
                        (currentUserId, oldTransaction.CurrencyExchangeAccountId, oldTransaction.SourceCurrencyId, oldTransaction.Amount);

                    if (!isDeposited)
                    {
                        apiResponse.Message = "خطا در بروز رسانی حساب تبادله ارز";
                        await _genericRepository.RollbackTransactionAsync();
                        return Ok(apiResponse);
                    }
                }

                // rollback TreasuryAccount
                var treasuryAccountUpdated = await _customerAccountRepo
                    .UpdateTreasuryAccount(currentUserId, oldTransaction.Amount, oldTransaction.SourceCurrencyId,
                    oldTransaction.TransactionType == TransactionType.Buy ? false : true);
                if (!treasuryAccountUpdated)
                {
                    apiResponse.Message = "حساب صندوق بروز رسانی نشد.";
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }

                treasuryAccountUpdated = await _customerAccountRepo
                    .UpdateTreasuryAccount(currentUserId, oldTransaction.ConvertedAmount, oldTransaction.TargetCurrencyId,
                    oldTransaction.TransactionType == TransactionType.Buy ? true : false);
                if (!treasuryAccountUpdated)
                {
                    apiResponse.Message = "حساب صندوق بروز رسانی نشد.";
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }


                // update transaction

                var buyTransactionId = oldTransaction.BuyTransactionId;
                var sellTransactionId = oldTransaction.SellTransactionId;


                var buyTransaction = await _customerTransactionHistoryGeneric.GetByIdAsync(buyTransactionId);
                if (buyTransaction == null)
                {
                    apiResponse.Message = "تراکنش یافت نشد.";
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }

                var sellTransaction = await _customerTransactionHistoryGeneric.GetByIdAsync(sellTransactionId);
                if (sellTransaction == null)
                {
                    apiResponse.Message = "تراکنش یافت نشد.";
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }


                if (updatedTransaction.TransactionType == TransactionType.Buy)
                {
                    // Buy
                    // Withdraw from exchange account
                    var isWithdrawed = await _customerAccountRepo.WithdrawByCustomerIdAsync
                        (currentUserId, exchangeAccountId, updatedTransaction.TargetCurrencyId, updatedTransaction.ConvertedAmount);
                    if (!isWithdrawed.Item1)
                    {
                        apiResponse.Message = isWithdrawed.Item2;
                        await _genericRepository.RollbackTransactionAsync();
                        return Ok(apiResponse);
                    }

                    sellTransaction.Amount = -updatedTransaction.ConvertedAmount;
                    sellTransaction.CurrencyId = updatedTransaction.TargetCurrencyId;
                    sellTransaction.CreatedDate = updatedTransaction.CreatedDate;
                    sellTransaction.CustomerId = exchangeAccountId;
                    sellTransaction.DealType = DealType.Withdraw;
                    sellTransaction.Description = updatedTransaction.Description;
                    sellTransaction.TransactionType = TransactionType.Sell;
                    sellTransaction.UpdatedDate = DateTime.Now;

                    // add buy transaction
                    // Deposit
                    var deposit = await _customerAccountRepo.DepositeByCustomerIdAsync
                        (currentUserId, exchangeAccountId, updatedTransaction.SourceCurrencyId, updatedTransaction.Amount);
                    if (!deposit)
                    {
                        apiResponse.Message = "واریز پول به حساب تبادله ارز انجام نشد.";
                        await _genericRepository.RollbackTransactionAsync();
                        return BadRequest(apiResponse);
                    }

                    buyTransaction.Amount = updatedTransaction.Amount;
                    buyTransaction.CurrencyId = updatedTransaction.SourceCurrencyId;
                    buyTransaction.CreatedDate = updatedTransaction.CreatedDate;
                    buyTransaction.CustomerId = exchangeAccountId;
                    buyTransaction.DealType = DealType.Deposit;
                    buyTransaction.Description = updatedTransaction.Description;
                    buyTransaction.TransactionType = TransactionType.Buy;
                    buyTransaction.UpdatedDate = DateTime.Now;

                }
                else
                {
                    // Sell
                    // Withdraw from exchange account
                    var isWithdrawed = await _customerAccountRepo.WithdrawByCustomerIdAsync
                        (currentUserId, exchangeAccountId, updatedTransaction.SourceCurrencyId, updatedTransaction.Amount);
                    if (!isWithdrawed.Item1)
                    {
                        apiResponse.Message = isWithdrawed.Item2;
                        await _genericRepository.RollbackTransactionAsync();
                        return Ok(apiResponse);
                    }

                    sellTransaction.Amount = -updatedTransaction.Amount;
                    sellTransaction.CurrencyId = updatedTransaction.SourceCurrencyId;
                    sellTransaction.CreatedDate = updatedTransaction.CreatedDate;
                    sellTransaction.CustomerId = exchangeAccountId;
                    sellTransaction.DealType = DealType.Withdraw;
                    sellTransaction.Description = updatedTransaction.Description;
                    sellTransaction.TransactionType = TransactionType.Sell;
                    sellTransaction.UpdatedDate = DateTime.Now;

                    // add buy transaction
                    // Deposit
                    var deposit = await _customerAccountRepo.DepositeByCustomerIdAsync
                        (currentUserId, exchangeAccountId, updatedTransaction.TargetCurrencyId, updatedTransaction.ConvertedAmount);
                    if (!deposit)
                    {
                        apiResponse.Message = "واریز پول به حساب تبادله ارز انجام نشد.";
                        await _genericRepository.RollbackTransactionAsync();
                        return BadRequest(apiResponse);
                    }

                    buyTransaction.Amount = updatedTransaction.ConvertedAmount;
                    buyTransaction.CurrencyId = updatedTransaction.TargetCurrencyId;
                    buyTransaction.CreatedDate = updatedTransaction.CreatedDate;
                    buyTransaction.CustomerId = exchangeAccountId;
                    buyTransaction.DealType = DealType.Deposit;
                    buyTransaction.Description = updatedTransaction.Description;
                    buyTransaction.TransactionType = TransactionType.Buy;
                    buyTransaction.UpdatedDate = DateTime.Now;
                }
                await _customerAccountRepo.SaveAsync();

                // update treasury account


                treasuryAccountUpdated = await _customerAccountRepo
                    .UpdateTreasuryAccount(currentUserId, updatedTransaction.Amount, updatedTransaction.SourceCurrencyId,
                    updatedTransaction.TransactionType == TransactionType.Buy);
                if (!treasuryAccountUpdated)
                {
                    apiResponse.Message = "حساب صندوق بروز رسانی نشد.";
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }

                treasuryAccountUpdated = await _customerAccountRepo
                    .UpdateTreasuryAccount(currentUserId, updatedTransaction.ConvertedAmount, updatedTransaction.TargetCurrencyId,
                    updatedTransaction.TransactionType != TransactionType.Buy);
                if (!treasuryAccountUpdated)
                {
                    apiResponse.Message = "حساب صندوق بروز رسانی نشد.";
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }


                // check exchange account for borrow

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
                    .CalculateCustomerBalanceInUSD(currentUserId, exchangeAccountId, exchangeRates.ToList());

                if (totalBalance == null)
                {
                    apiResponse.Message = "مشتری یافت نشد.";
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }

                var BorrowAmount = await _customerAccountRepo.GetBorrowAmountOfACustomer(currentUserId, exchangeAccountId);

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

                // check treasury account for borrow
                var check = await _customerAccountRepo.CheckTreasuryAccountBasedOnBorrowAmount(currentUserId);
                if (!check.Item1)
                {
                    apiResponse.Message = check.Item2;
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }

                oldTransaction.Description = updatedTransaction.Description;
                oldTransaction.CreatedDate = updatedTransaction.CreatedDate;
                oldTransaction.UpdatedDate = DateTime.Now;
                oldTransaction.Amount = updatedTransaction.Amount;
                oldTransaction.TransactionType = updatedTransaction.TransactionType;
                oldTransaction.SourceCurrencyId = updatedTransaction.SourceCurrencyId;
                oldTransaction.TargetCurrencyId = updatedTransaction.TargetCurrencyId;
                oldTransaction.Rate = updatedTransaction.Rate;
                oldTransaction.ConvertedAmount = updatedTransaction.ConvertedAmount;

                await _genericRepository.SaveAsync();
                await _genericRepository.CommitTransactionAsync();
                apiResponse.Success = true;
                apiResponse.Data = true;
                _logger.LogInformation($"updating Buy & Sell transaction ENDED: id:{transactionId}");
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

        // Get Buy and sell transactions of a user
        [HttpGet("customerTransaction/{customerId}", Name = "GetAllBuyAndSellTransactionsOfACustomer")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<IEnumerable<BuyAndSellTransactionDTO>>>> GetBuyAndSellTransactionsOfACustomer(int customerId)
        {
            var apiResponse = new ApiResponse<IEnumerable<BuyAndSellTransactionDTO>>(false);
            try
            {
                _logger.LogError("Getting Buy & Sell Transactions of a customer");
                var currentUserId = User.GetUserId();

                var result = await _customerAccountRepo.GetAllBuyAndSellTransactionsAsync(currentUserId, customerId);

                if (result == null)
                {
                    apiResponse.Message = "دریافت تراکنش های خرید و فروش با مشکل مواجه شد.";
                    return Ok(apiResponse);
                }

                apiResponse.Success = true;
                apiResponse.Data = result;
                _logger.LogError("Getting Buy & Sell transactions of a customer: ENDED");
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

        [HttpPost("customerTransaction/{customerId}", Name = "AddBuyAndSellTransactionForAUser")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<int>>> AddCustomerBuyAndSellTransation(int customerId, BuyAndSellTransactionDTO newTransactionDTO)
        {
            _logger.LogError("Buy/Sell currency for customer Started ");

            var apiResponse = new ApiResponse<int>(false);
            try
            {
                var currentUserId = User.GetUserId();

                int exchangeAccountId = await _customerAccountRepo.IsCurrencyExchangeAccountExist(currentUserId);

                if (exchangeAccountId == 0)
                {
                    apiResponse.Message = "حساب تبادله ارز در سیستم وجود ندارد.";
                    return Ok(apiResponse);
                }

                bool isOwner = await _authorizationService.IsUserOwnerOfCustomerAsync
                    (exchangeAccountId, currentUserId);
                if (!isOwner)
                {
                    apiResponse.Message = "شما اجازه دسترسی به این اطلاعات را ندارید";
                    return Ok(apiResponse);
                }

                newTransactionDTO.CurrencyExchangeAccountId = exchangeAccountId;

                await _genericRepository.BeginTransactionAsync();

                // add buy and sell transaction to transactions history

                // add sell transaction(converted Amount)
                int sellTransactionId = 0;
                int buyTransactionId = 0;
                int customerSellTransactionId = 0;
                int customerBuyTransactionId = 0;

                if (newTransactionDTO.TransactionType == TransactionType.Buy)
                {
                    // 1000USD Buy => Amount
                    // 73000 AFN Sell => ConvertedAmount
                    // Buy=> customer want to buy the source currency and deposit to its account

                    // Withdraw Amount from exchange account

                    // Exchange Account Section
                    //1: withdraw source currency with amoun
                    //2: deposit targetCurrency with convertedAmount

                    var result = await _customerAccountRepo.WithdrawByCustomerIdAsync
                        (currentUserId, exchangeAccountId, newTransactionDTO.SourceCurrencyId, newTransactionDTO.Amount);
                    if (!result.Item1)
                    {
                        apiResponse.Message = result.Item2;
                        await _genericRepository.RollbackTransactionAsync();
                        return Ok(apiResponse);
                    }

                    // exchange account
                    var transactionDTO = new CustomerTransactionHistoryDTO()
                    {
                        Amount = -newTransactionDTO.Amount,
                        CurrencyId = newTransactionDTO.SourceCurrencyId,
                        CustomerId = exchangeAccountId,
                        DealType = DealType.Withdraw,
                        TransactionType = TransactionType.Sell,
                        CreatedDate = newTransactionDTO.CreatedDate,
                        Description = newTransactionDTO.Description,
                        //Description = string.Format("فروش{0}به نرخ {1}")
                        DepositOrWithdrawBy = string.Empty,
                        UserId = newTransactionDTO.UserId
                    };

                    sellTransactionId = await _customerAccountRepo.AddCustomerTransactionAsync(transactionDTO);

                    if (sellTransactionId == 0)
                    {
                        apiResponse.Message = "برداشت پول از حساب تبادله ارز با مشکل مواجه شد.";
                        await _genericRepository.RollbackTransactionAsync();
                        return Ok(apiResponse);
                    }

                    // Deposit the Converted
                    var deposit = await _customerAccountRepo.DepositeByCustomerIdAsync
                        (currentUserId, exchangeAccountId, newTransactionDTO.TargetCurrencyId, newTransactionDTO.ConvertedAmount);

                    if (!deposit)
                    {
                        apiResponse.Message = "واریز پول به حساب تبادله ارز انجام نشد.";
                        await _genericRepository.RollbackTransactionAsync();
                        return BadRequest(apiResponse);
                    }

                    transactionDTO = new CustomerTransactionHistoryDTO()
                    {
                        Amount = newTransactionDTO.ConvertedAmount,
                        CurrencyId = newTransactionDTO.TargetCurrencyId,
                        CreatedDate = newTransactionDTO.CreatedDate,
                        CustomerId = exchangeAccountId,
                        DealType = DealType.Deposit,
                        Description = newTransactionDTO.Description,
                        TransactionType = TransactionType.Buy,
                        DepositOrWithdrawBy = string.Empty,
                        UserId = newTransactionDTO.UserId
                    };

                    buyTransactionId = await _customerAccountRepo.AddCustomerTransactionAsync(transactionDTO);
                    if (buyTransactionId == 0)
                    {
                        apiResponse.Message = "واریز پول به حساب تبادله ارز انجام نشد.";
                        await _genericRepository.RollbackTransactionAsync();
                        return Ok(apiResponse);
                    }

                    // Exchange Account Section Ended

                    //Customer Account Section

                    // withdraw the converted Amount from customer account
                    result = await _customerAccountRepo.WithdrawByCustomerIdAsync
                        (currentUserId, customerId, newTransactionDTO.TargetCurrencyId, newTransactionDTO.ConvertedAmount);
                    if (!result.Item1)
                    {
                        apiResponse.Message = result.Item2;
                        await _genericRepository.RollbackTransactionAsync();
                        return Ok(apiResponse);
                    }

                    transactionDTO = new CustomerTransactionHistoryDTO()
                    {
                        Amount = -newTransactionDTO.ConvertedAmount,
                        CurrencyId = newTransactionDTO.TargetCurrencyId,
                        CustomerId = customerId,
                        DealType = DealType.Withdraw,
                        TransactionType = TransactionType.Sell,
                        CreatedDate = newTransactionDTO.CreatedDate,
                        Description = newTransactionDTO.Description,
                        //Description = string.Format("فروش{0}به نرخ {1}")
                        DepositOrWithdrawBy = string.Empty,
                        UserId = newTransactionDTO.UserId
                    };

                    customerSellTransactionId = await _customerAccountRepo.AddCustomerTransactionAsync(transactionDTO);

                    if (customerSellTransactionId == 0)
                    {
                        apiResponse.Message = "برداشت پول از حساب مشتری با مشکل مواجه شد.";
                        await _genericRepository.RollbackTransactionAsync();
                        return Ok(apiResponse);
                    }


                    // Add buy amount to customer account
                    // Deposit the Converted
                    deposit = await _customerAccountRepo.DepositeByCustomerIdAsync
                        (currentUserId, customerId, newTransactionDTO.SourceCurrencyId, newTransactionDTO.Amount);

                    if (!deposit)
                    {
                        apiResponse.Message = "واریز پول به حساب مشتری انجام نشد.";
                        await _genericRepository.RollbackTransactionAsync();
                        return BadRequest(apiResponse);
                    }

                    transactionDTO = new CustomerTransactionHistoryDTO()
                    {
                        Amount = newTransactionDTO.Amount,
                        CurrencyId = newTransactionDTO.SourceCurrencyId,
                        CreatedDate = newTransactionDTO.CreatedDate,
                        CustomerId = customerId,
                        DealType = DealType.Deposit,
                        Description = newTransactionDTO.Description,
                        TransactionType = TransactionType.Buy,
                        DepositOrWithdrawBy = string.Empty,
                        UserId = newTransactionDTO.UserId
                    };

                    customerBuyTransactionId = await _customerAccountRepo.AddCustomerTransactionAsync(transactionDTO);
                    if (customerBuyTransactionId == 0)
                    {
                        apiResponse.Message = "واریز پول به حساب مشتری انجام نشد.";
                        await _genericRepository.RollbackTransactionAsync();
                        return Ok(apiResponse);
                    }

                    //Customer Account Section Ended
                }
                else
                {
                    // Sell

                    // Exchange Account Section

                    //1: deposit source currency with amount
                    //2: withdraw targetCurrency with convertedAmount

                    var result = await _customerAccountRepo.WithdrawByCustomerIdAsync
                        (currentUserId, exchangeAccountId, newTransactionDTO.TargetCurrencyId, newTransactionDTO.ConvertedAmount);
                    if (!result.Item1)
                    {
                        apiResponse.Message = result.Item2;
                        await _genericRepository.RollbackTransactionAsync();
                        return Ok(apiResponse);
                    }

                    // exchange account
                    var transactionDTO = new CustomerTransactionHistoryDTO()
                    {
                        Amount = -newTransactionDTO.ConvertedAmount,
                        CurrencyId = newTransactionDTO.TargetCurrencyId,
                        CustomerId = exchangeAccountId,
                        DealType = DealType.Withdraw,
                        TransactionType = TransactionType.Sell,
                        CreatedDate = newTransactionDTO.CreatedDate,
                        Description = newTransactionDTO.Description,
                        //Description = string.Format("فروش{0}به نرخ {1}")
                        DepositOrWithdrawBy = string.Empty,
                        UserId = newTransactionDTO.UserId
                    };

                    sellTransactionId = await _customerAccountRepo.AddCustomerTransactionAsync(transactionDTO);

                    if (sellTransactionId == 0)
                    {
                        apiResponse.Message = "برداشت پول از حساب تبادله ارز با مشکل مواجه شد.";
                        await _genericRepository.RollbackTransactionAsync();
                        return Ok(apiResponse);
                    }

                    // Deposit the Amount
                    var deposit = await _customerAccountRepo.DepositeByCustomerIdAsync
                        (currentUserId, exchangeAccountId, newTransactionDTO.SourceCurrencyId, newTransactionDTO.Amount);

                    if (!deposit)
                    {
                        apiResponse.Message = "واریز پول به حساب تبادله ارز انجام نشد.";
                        await _genericRepository.RollbackTransactionAsync();
                        return BadRequest(apiResponse);
                    }

                    transactionDTO = new CustomerTransactionHistoryDTO()
                    {
                        Amount = newTransactionDTO.Amount,
                        CurrencyId = newTransactionDTO.SourceCurrencyId,
                        CreatedDate = newTransactionDTO.CreatedDate,
                        CustomerId = exchangeAccountId,
                        DealType = DealType.Deposit,
                        Description = newTransactionDTO.Description,
                        TransactionType = TransactionType.Buy,
                        DepositOrWithdrawBy = string.Empty,
                        UserId = newTransactionDTO.UserId
                    };

                    buyTransactionId = await _customerAccountRepo.AddCustomerTransactionAsync(transactionDTO);
                    if (buyTransactionId == 0)
                    {
                        apiResponse.Message = "واریز پول به حساب تبادله ارز انجام نشد.";
                        await _genericRepository.RollbackTransactionAsync();
                        return Ok(apiResponse);
                    }

                    // Exchange Account Section Ended

                    //Customer Account Section

                    // withdraw the Amount from customer account
                    result = await _customerAccountRepo.WithdrawByCustomerIdAsync
                        (currentUserId, customerId, newTransactionDTO.SourceCurrencyId, newTransactionDTO.Amount);
                    if (!result.Item1)
                    {
                        apiResponse.Message = result.Item2;
                        await _genericRepository.RollbackTransactionAsync();
                        return Ok(apiResponse);
                    }

                    transactionDTO = new CustomerTransactionHistoryDTO()
                    {
                        Amount = -newTransactionDTO.Amount,
                        CurrencyId = newTransactionDTO.SourceCurrencyId,
                        CustomerId = customerId,
                        DealType = DealType.Withdraw,
                        TransactionType = TransactionType.Sell,
                        CreatedDate = newTransactionDTO.CreatedDate,
                        Description = newTransactionDTO.Description,
                        //Description = string.Format("فروش{0}به نرخ {1}")
                        DepositOrWithdrawBy = string.Empty,
                        UserId = newTransactionDTO.UserId
                    };

                    customerSellTransactionId = await _customerAccountRepo.AddCustomerTransactionAsync(transactionDTO);

                    if (customerSellTransactionId == 0)
                    {
                        apiResponse.Message = "برداشت پول از حساب مشتری با مشکل مواجه شد.";
                        await _genericRepository.RollbackTransactionAsync();
                        return Ok(apiResponse);
                    }

                    // Deposit the Converted
                    deposit = await _customerAccountRepo.DepositeByCustomerIdAsync
                        (currentUserId, customerId, newTransactionDTO.TargetCurrencyId, newTransactionDTO.ConvertedAmount);

                    if (!deposit)
                    {
                        apiResponse.Message = "واریز پول به حساب مشتری انجام نشد.";
                        await _genericRepository.RollbackTransactionAsync();
                        return BadRequest(apiResponse);
                    }

                    transactionDTO = new CustomerTransactionHistoryDTO()
                    {
                        Amount = newTransactionDTO.ConvertedAmount,
                        CurrencyId = newTransactionDTO.TargetCurrencyId,
                        CreatedDate = newTransactionDTO.CreatedDate,
                        CustomerId = customerId,
                        DealType = DealType.Deposit,
                        Description = newTransactionDTO.Description,
                        TransactionType = TransactionType.Buy,
                        DepositOrWithdrawBy = string.Empty,
                        UserId = newTransactionDTO.UserId
                    };

                    customerBuyTransactionId = await _customerAccountRepo.AddCustomerTransactionAsync(transactionDTO);
                    if (customerBuyTransactionId == 0)
                    {
                        apiResponse.Message = "واریز پول به حساب مشتری انجام نشد.";
                        await _genericRepository.RollbackTransactionAsync();
                        return Ok(apiResponse);
                    }

                    //Customer Account Section Ended
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
                    .CalculateCustomerBalanceInUSD(currentUserId, exchangeAccountId, exchangeRates.ToList());

                if (totalBalance == null)
                {
                    apiResponse.Message = "حساب تبادله ارز یافت نشد.";
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }

                var BorrowAmount = await _customerAccountRepo.GetBorrowAmountOfACustomer(currentUserId, exchangeAccountId);

                if (BorrowAmount == null)
                {
                    apiResponse.Message = "حساب تبادله ارز یافت نشد.";
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }

                if (totalBalance < -(BorrowAmount))
                {
                    apiResponse.Message = "حساب تبادله ارز به سقف قرضه خود رسیده است. تراکنش ثبت نشد";
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }


                // check currency balance

                totalBalance = await _customerAccountRepo
                    .CalculateCustomerBalanceInUSD(currentUserId, customerId, exchangeRates.ToList());

                if (totalBalance == null)
                {
                    apiResponse.Message = "حساب مشتری یافت نشد.";
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }

                BorrowAmount = await _customerAccountRepo.GetBorrowAmountOfACustomer(currentUserId, customerId);

                if (BorrowAmount == null)
                {
                    apiResponse.Message = "حساب مشتری یافت نشد.";
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }

                if (totalBalance < -(BorrowAmount))
                {
                    apiResponse.Message = "مشتری به سقف قرضه خود رسیده است. تراکنش ثبت نشد";
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }


                var check = await _customerAccountRepo.CheckTreasuryAccountBasedOnBorrowAmount(currentUserId);
                if (!check.Item1)
                {
                    apiResponse.Message = check.Item2;
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }
                // add buy/sell transaction

                newTransactionDTO.BuyTransactionId = buyTransactionId;
                newTransactionDTO.SellTransactionId = sellTransactionId;
                newTransactionDTO.CustomerId = customerId;
                newTransactionDTO.CustomerSellTransactionId = customerSellTransactionId;
                newTransactionDTO.CustomerBuyTransactionId = customerBuyTransactionId;
                newTransactionDTO.BuyAndSellType = CurrencyBuyAndSellType.NonCash;

                var transaction = await _genericRepository.AddEntityAsync(newTransactionDTO.ToBuyAndSellTransaction());

                if (transaction == null)
                {
                    apiResponse.Message = "تراکنش خرید/فروش ارز ثبت نشد.";
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }
                await _genericRepository.SaveAsync();
                await _genericRepository.CommitTransactionAsync();
                apiResponse.Success = true;
                apiResponse.Data = transaction.Id;
                _logger.LogError("adding buy/sell transaction: ENDEDDDDDDDD");

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

        [HttpDelete("customerTransaction/{transactionId}", Name = "DeleteBuy&sellTransactionsOfAUser")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteCustomerBuyAndSellTransation(int transactionId)
        {
            var apiResponse = new ApiResponse<bool>(false);
            try
            {
                _logger.LogInformation($"Deleting Buy&Sell Transaction:STARTED: {transactionId}");
                var currentUserId = User.GetUserId();

                var oldTransaction = await _genericRepository.GetByIdAsync(transactionId);
                if (oldTransaction == null)
                {
                    apiResponse.Message = "تراکنش خرید/فروش یافت نشد.";
                    return Ok(apiResponse);
                }
                bool isOwner = await _authorizationService
                    .IsUserOwnerOfTransaction(oldTransaction.BuyTransactionId, currentUserId);

                if (!isOwner)
                {
                    apiResponse.Message = "شما اجازه دسترسی به این اطلاعات را ندارید";
                    return Unauthorized(apiResponse);
                }

                await _genericRepository.BeginTransactionAsync();

                if (oldTransaction.TransactionType == TransactionType.Buy)
                {
                    // Exchange Account Section
                    //1: deposit source currency with amount
                    //2: withdraw targetCurrency with convertedAmount

                    var isWithdrawed = await _customerAccountRepo.WithdrawByCustomerIdAsync
                     (currentUserId, oldTransaction.CurrencyExchangeAccountId, oldTransaction.TargetCurrencyId, oldTransaction.ConvertedAmount);

                    if (!isWithdrawed.Item1)
                    {
                        apiResponse.Message = isWithdrawed.Item2;
                        await _genericRepository.RollbackTransactionAsync();
                        return Ok(apiResponse);
                    }

                    // Deposit
                    var isDeposited = await _customerAccountRepo.DepositeByCustomerIdAsync
                        (currentUserId, oldTransaction.CurrencyExchangeAccountId, oldTransaction.SourceCurrencyId, oldTransaction.Amount);

                    if (!isDeposited)
                    {
                        apiResponse.Message = "خطا در بروز رسانی حساب تبادله ارز";
                        await _genericRepository.RollbackTransactionAsync();
                        return Ok(apiResponse);
                    }

                    //Customer Account Section
                    // Deposit the converted Amount to customer account
                    // withdraw buy amount from customer account

                    // Deposit
                    isDeposited = await _customerAccountRepo.DepositeByCustomerIdAsync
                        (currentUserId, (int)oldTransaction.CustomerId, oldTransaction.TargetCurrencyId, oldTransaction.ConvertedAmount);

                    if (!isDeposited)
                    {
                        apiResponse.Message = "خطا در بروز رسانی حساب مشتری";
                        await _genericRepository.RollbackTransactionAsync();
                        return Ok(apiResponse);
                    }

                    isWithdrawed = await _customerAccountRepo.WithdrawByCustomerIdAsync
                     (currentUserId, (int)oldTransaction.CustomerId, oldTransaction.SourceCurrencyId, oldTransaction.Amount);

                    if (!isWithdrawed.Item1)
                    {
                        apiResponse.Message = isWithdrawed.Item2;
                        await _genericRepository.RollbackTransactionAsync();
                        return Ok(apiResponse);
                    }

                }
                else
                {
                    // Exchange Account Section

                    //1: withdraw source currency with amount
                    //2: deposit targetCurrency with convertedAmount

                    var isWithdrawed = await _customerAccountRepo.WithdrawByCustomerIdAsync
                     (currentUserId, oldTransaction.CurrencyExchangeAccountId, oldTransaction.SourceCurrencyId, oldTransaction.Amount);

                    if (!isWithdrawed.Item1)
                    {
                        apiResponse.Message = isWithdrawed.Item2;
                        await _genericRepository.RollbackTransactionAsync();
                        return Ok(apiResponse);
                    }

                    // Deposit
                    var isDeposited = await _customerAccountRepo.DepositeByCustomerIdAsync
                        (currentUserId, oldTransaction.CurrencyExchangeAccountId, oldTransaction.TargetCurrencyId, oldTransaction.ConvertedAmount);

                    if (!isDeposited)
                    {
                        apiResponse.Message = "خطا در بروز رسانی حساب تبادله ارز";
                        await _genericRepository.RollbackTransactionAsync();
                        return Ok(apiResponse);
                    }

                    //Customer Account Section

                    // Deposit the Amount to customer account
                    // Withdraw the Converted
                    isDeposited = await _customerAccountRepo.DepositeByCustomerIdAsync
                        (currentUserId, (int)oldTransaction.CustomerId, oldTransaction.SourceCurrencyId, oldTransaction.Amount);

                    if (!isDeposited)
                    {
                        apiResponse.Message = "خطا در بروز رسانی حساب تبادله ارز";
                        await _genericRepository.RollbackTransactionAsync();
                        return Ok(apiResponse);
                    }

                    isWithdrawed = await _customerAccountRepo.WithdrawByCustomerIdAsync
                     (currentUserId, (int)oldTransaction.CustomerId, oldTransaction.TargetCurrencyId, oldTransaction.ConvertedAmount);

                    if (!isWithdrawed.Item1)
                    {
                        apiResponse.Message = isWithdrawed.Item2;
                        await _genericRepository.RollbackTransactionAsync();
                        return Ok(apiResponse);
                    }

                }

                await _customerAccountRepo.SaveAsync();

                // check exchange account for borrow
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
                    .CalculateCustomerBalanceInUSD(currentUserId, oldTransaction.CurrencyExchangeAccountId, exchangeRates.ToList());

                if (totalBalance == null)
                {
                    apiResponse.Message = "مشتری یافت نشد.";
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }

                var BorrowAmount = await _customerAccountRepo.GetBorrowAmountOfACustomer(currentUserId, oldTransaction.CurrencyExchangeAccountId);

                if (BorrowAmount == null)
                {
                    apiResponse.Message = "مشتری یافت نشد.";
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }

                if (totalBalance < -(BorrowAmount))
                {
                    apiResponse.Message = "حساب تبادله ارز به سقف قرضه خود رسیده است. تراکنش ثبت نشد";
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }


                // customer balance check

                totalBalance = await _customerAccountRepo
                    .CalculateCustomerBalanceInUSD(currentUserId, (int)oldTransaction.CustomerId, exchangeRates.ToList());

                if (totalBalance == null)
                {
                    apiResponse.Message = "مشتری یافت نشد.";
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }

                BorrowAmount = await _customerAccountRepo.GetBorrowAmountOfACustomer(currentUserId, (int)oldTransaction.CustomerId);

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

                // Delete buy and sell transaction

                var isRemoved = await _customerTransactionHistoryGeneric.DeleteByIdAsync(oldTransaction.BuyTransactionId);
                if (!isRemoved)
                {
                    apiResponse.Message = "تراکنش حذف نشد.";
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }

                isRemoved = await _customerTransactionHistoryGeneric.DeleteByIdAsync(oldTransaction.SellTransactionId);
                if (!isRemoved)
                {
                    apiResponse.Message = "تراکنش حذف نشد.";
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }

                isRemoved = await _customerTransactionHistoryGeneric.DeleteByIdAsync((int)oldTransaction.CustomerBuyTransactionId);
                if (!isRemoved)
                {
                    apiResponse.Message = "تراکنش حذف نشد.";
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }

                isRemoved = await _customerTransactionHistoryGeneric.DeleteByIdAsync((int)oldTransaction.CustomerSellTransactionId);
                if (!isRemoved)
                {
                    apiResponse.Message = "تراکنش حذف نشد.";
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }



                // delete BuyAndSellTransaction

                isRemoved = await _genericRepository.DeleteByIdAsync(oldTransaction.Id);
                if (!isRemoved)
                {
                    apiResponse.Message = "تراکنش حذف نشد.";
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }

                await _genericRepository.SaveAsync();
                await _genericRepository.CommitTransactionAsync();
                apiResponse.Success = true;
                apiResponse.Data = true;
                apiResponse.Message = "تراکنش حذف شد.";
                _logger.LogInformation($"Deleting Buy & Sell Transaction:ENDED: {transactionId}");
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

        [HttpPut("customerTransaction/updateDetails/{transactionId:int}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<bool>>> UpdateCustomerTransferTransactionDetails(int transactionId, BuyAndSellTransactionDTO updatedTransaction)
        {
            var apiResponse = new ApiResponse<bool>(false);
            try
            {
                var currentUserId = User.GetUserId();

                var transaction = await _genericRepository.GetByIdAsync(updatedTransaction.Id);

                if (transaction == null)
                {
                    apiResponse.Message = "تراکنش یافت نشد.";
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }


                bool isOwner = await _authorizationService.IsUserOwnerOfTransaction(transaction.BuyTransactionId, currentUserId);

                if (!isOwner)
                {
                    apiResponse.Message = "شما اجازه دسترسی به این اطلاعات را ندارید";
                    return Unauthorized(apiResponse);
                }
                await _genericRepository.BeginTransactionAsync();

                var buyTransactionId = transaction.BuyTransactionId;
                var sellTransactionId = transaction.SellTransactionId;
                var customerBuyTransactionId = transaction.CustomerBuyTransactionId;
                var customerSellTransactionId = transaction.CustomerSellTransactionId;

                var customerBuyTransaction = await _customerTransactionHistoryGeneric.GetByIdAsync((int)customerBuyTransactionId);
                if (customerBuyTransaction == null)
                {
                    apiResponse.Message = "تراکنش یافت نشد.";
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }

                customerBuyTransaction.Description = updatedTransaction.Description;
                customerBuyTransaction.CreatedDate = updatedTransaction.CreatedDate;
                customerBuyTransaction.UpdatedDate = DateTime.Now;


                var customerSellTransaction = await _customerTransactionHistoryGeneric.GetByIdAsync((int)customerSellTransactionId);
                if (customerSellTransaction == null)
                {
                    apiResponse.Message = "تراکنش یافت نشد.";
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }

                customerSellTransaction.Description = updatedTransaction.Description;
                customerSellTransaction.CreatedDate = updatedTransaction.CreatedDate;
                customerSellTransaction.UpdatedDate = DateTime.Now;


                var buyTransaction = await _customerTransactionHistoryGeneric.GetByIdAsync(buyTransactionId);
                if (buyTransaction == null)
                {
                    apiResponse.Message = "تراکنش یافت نشد.";
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }

                buyTransaction.Description = updatedTransaction.Description;
                buyTransaction.CreatedDate = updatedTransaction.CreatedDate;
                buyTransaction.UpdatedDate = DateTime.Now;

                var sellTransaction = await _customerTransactionHistoryGeneric.GetByIdAsync(sellTransactionId);
                if (sellTransaction == null)
                {
                    apiResponse.Message = "تراکنش یافت نشد.";
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }

                sellTransaction.Description = updatedTransaction.Description;
                sellTransaction.CreatedDate = updatedTransaction.CreatedDate;
                sellTransaction.UpdatedDate = DateTime.Now;


                // update BuyAndSellTransaction table
                transaction.Description = updatedTransaction.Description;
                transaction.CreatedDate = updatedTransaction.CreatedDate;
                transaction.UpdatedDate = DateTime.Now;

                apiResponse.Success = true;
                apiResponse.Message = "تراکنش با موفقیت بروز رسانی شد.";
                await _genericRepository.SaveAsync();
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

        [HttpPut("customerTransaction/{transactionId:int}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<bool>>> UpdateCustomerTransferTransaction(int transactionId, BuyAndSellTransactionDTO updatedTransaction)
        {
            var apiResponse = new ApiResponse<bool>(false);
            try
            {
                _logger.LogInformation($"updating customer Buy&Sell transaction STARTED: id:{transactionId}");

                var currentUserId = User.GetUserId();

                int exchangeAccountId = await _customerAccountRepo.IsCurrencyExchangeAccountExist(currentUserId);
                if (exchangeAccountId == 0)
                {
                    apiResponse.Message = "حساب تبادله ارز در سیستم وجود ندارد.";
                    return Ok(apiResponse);
                }

                var oldTransaction = await _genericRepository.GetByIdAsync(transactionId);
                if (oldTransaction == null)
                {
                    apiResponse.Message = "تراکنش خرید/فروش یافت نشد.";
                    return Ok(apiResponse);
                }

                bool isOwner = await _authorizationService
                    .IsUserOwnerOfTransaction(oldTransaction.BuyTransactionId, currentUserId);

                await _genericRepository.BeginTransactionAsync();

                // rollback based on transaction type
                if (oldTransaction.TransactionType == TransactionType.Buy)
                {
                    // Exchange Account Section
                    //1: deposit source currency with amount
                    //2: withdraw targetCurrency with convertedAmount

                    var isWithdrawed = await _customerAccountRepo.WithdrawByCustomerIdAsync
                     (currentUserId, oldTransaction.CurrencyExchangeAccountId, oldTransaction.TargetCurrencyId, oldTransaction.ConvertedAmount);

                    if (!isWithdrawed.Item1)
                    {
                        apiResponse.Message = isWithdrawed.Item2;
                        await _genericRepository.RollbackTransactionAsync();
                        return Ok(apiResponse);
                    }

                    // Deposit
                    var isDeposited = await _customerAccountRepo.DepositeByCustomerIdAsync
                        (currentUserId, oldTransaction.CurrencyExchangeAccountId, oldTransaction.SourceCurrencyId, oldTransaction.Amount);

                    if (!isDeposited)
                    {
                        apiResponse.Message = "خطا در بروز رسانی حساب تبادله ارز";
                        await _genericRepository.RollbackTransactionAsync();
                        return Ok(apiResponse);
                    }

                    //Customer Account Section
                    // Deposit the converted Amount to customer account
                    // withdraw buy amount from customer account

                    // Deposit
                    isDeposited = await _customerAccountRepo.DepositeByCustomerIdAsync
                        (currentUserId, (int)oldTransaction.CustomerId, oldTransaction.TargetCurrencyId, oldTransaction.ConvertedAmount);

                    if (!isDeposited)
                    {
                        apiResponse.Message = "خطا در بروز رسانی حساب مشتری";
                        await _genericRepository.RollbackTransactionAsync();
                        return Ok(apiResponse);
                    }

                    isWithdrawed = await _customerAccountRepo.WithdrawByCustomerIdAsync
                     (currentUserId, (int)oldTransaction.CustomerId, oldTransaction.SourceCurrencyId, oldTransaction.Amount);

                    if (!isWithdrawed.Item1)
                    {
                        apiResponse.Message = isWithdrawed.Item2;
                        await _genericRepository.RollbackTransactionAsync();
                        return Ok(apiResponse);
                    }

                }
                else
                {
                    // Exchange Account Section

                    //1: withdraw source currency with amount
                    //2: deposit targetCurrency with convertedAmount

                    var isWithdrawed = await _customerAccountRepo.WithdrawByCustomerIdAsync
                     (currentUserId, oldTransaction.CurrencyExchangeAccountId, oldTransaction.SourceCurrencyId, oldTransaction.Amount);

                    if (!isWithdrawed.Item1)
                    {
                        apiResponse.Message = isWithdrawed.Item2;
                        await _genericRepository.RollbackTransactionAsync();
                        return Ok(apiResponse);
                    }

                    // Deposit
                    var isDeposited = await _customerAccountRepo.DepositeByCustomerIdAsync
                        (currentUserId, oldTransaction.CurrencyExchangeAccountId, oldTransaction.TargetCurrencyId, oldTransaction.ConvertedAmount);

                    if (!isDeposited)
                    {
                        apiResponse.Message = "خطا در بروز رسانی حساب تبادله ارز";
                        await _genericRepository.RollbackTransactionAsync();
                        return Ok(apiResponse);
                    }

                    //Customer Account Section

                    // Deposit the Amount to customer account
                    // Withdraw the Converted
                    isDeposited = await _customerAccountRepo.DepositeByCustomerIdAsync
                        (currentUserId, (int)oldTransaction.CustomerId, oldTransaction.SourceCurrencyId, oldTransaction.Amount);

                    if (!isDeposited)
                    {
                        apiResponse.Message = "خطا در بروز رسانی حساب تبادله ارز";
                        await _genericRepository.RollbackTransactionAsync();
                        return Ok(apiResponse);
                    }

                    isWithdrawed = await _customerAccountRepo.WithdrawByCustomerIdAsync
                     (currentUserId, (int)oldTransaction.CustomerId, oldTransaction.TargetCurrencyId, oldTransaction.ConvertedAmount);

                    if (!isWithdrawed.Item1)
                    {
                        apiResponse.Message = isWithdrawed.Item2;
                        await _genericRepository.RollbackTransactionAsync();
                        return Ok(apiResponse);
                    }

                }

                await _customerAccountRepo.SaveAsync();

                // update transaction

                var buyTransactionId = oldTransaction.BuyTransactionId;
                var sellTransactionId = oldTransaction.SellTransactionId;
                var customerBuyTransactionId = oldTransaction.CustomerBuyTransactionId;
                var customerSellTransactionId = oldTransaction.CustomerSellTransactionId;

                var customerId = (int)oldTransaction.CustomerId;

                var buyTransaction = await _customerTransactionHistoryGeneric.GetByIdAsync(buyTransactionId);
                if (buyTransaction == null)
                {
                    apiResponse.Message = "تراکنش یافت نشد.";
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }

                var sellTransaction = await _customerTransactionHistoryGeneric.GetByIdAsync(sellTransactionId);
                if (sellTransaction == null)
                {
                    apiResponse.Message = "تراکنش یافت نشد.";
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }

                var customerBuyTransaction = await _customerTransactionHistoryGeneric.GetByIdAsync((int)customerBuyTransactionId);
                if (customerBuyTransaction == null)
                {
                    apiResponse.Message = "تراکنش یافت نشد.";
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }

                var customerSellTransaction = await _customerTransactionHistoryGeneric.GetByIdAsync((int)customerSellTransactionId);
                if (customerSellTransaction == null)
                {
                    apiResponse.Message = "تراکنش یافت نشد.";
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }

                if (updatedTransaction.TransactionType == TransactionType.Buy)
                {
                    // Exchange Account Section

                    //1: withdraw source currency with amount
                    //2: deposit targetCurrency with convertedAmount


                    // Buy
                    // Exchange Account Section

                    //1: withdraw source currency with amount from exchange account(Sell money)
                    //2: deposit targetCurrency with convertedAmount to exchange account(buy money)
                    var result = await _customerAccountRepo.WithdrawByCustomerIdAsync
                        (currentUserId, exchangeAccountId, updatedTransaction.SourceCurrencyId, updatedTransaction.Amount);

                    if (!result.Item1)
                    {
                        apiResponse.Message = result.Item2;
                        await _genericRepository.RollbackTransactionAsync();
                        return Ok(apiResponse);
                    }

                    sellTransaction.Amount = -updatedTransaction.Amount;
                    sellTransaction.CurrencyId = updatedTransaction.SourceCurrencyId;
                    sellTransaction.CreatedDate = updatedTransaction.CreatedDate;
                    sellTransaction.CustomerId = exchangeAccountId;
                    sellTransaction.DealType = DealType.Withdraw;
                    sellTransaction.Description = updatedTransaction.Description;
                    sellTransaction.TransactionType = TransactionType.Sell;
                    sellTransaction.UpdatedDate = DateTime.Now;

                    // add buy transaction
                    // Deposit
                    var deposit = await _customerAccountRepo.DepositeByCustomerIdAsync
                        (currentUserId, exchangeAccountId, updatedTransaction.TargetCurrencyId, updatedTransaction.ConvertedAmount);
                    if (!deposit)
                    {
                        apiResponse.Message = "واریز پول به حساب تبادله ارز انجام نشد.";
                        await _genericRepository.RollbackTransactionAsync();
                        return BadRequest(apiResponse);
                    }

                    buyTransaction.Amount = updatedTransaction.ConvertedAmount;
                    buyTransaction.CurrencyId = updatedTransaction.TargetCurrencyId;
                    buyTransaction.CreatedDate = updatedTransaction.CreatedDate;
                    buyTransaction.CustomerId = exchangeAccountId;
                    buyTransaction.DealType = DealType.Deposit;
                    buyTransaction.Description = updatedTransaction.Description;
                    buyTransaction.TransactionType = TransactionType.Buy;
                    buyTransaction.UpdatedDate = DateTime.Now;

                    //Customer Account Section

                    // withdraw the converted Amount from customer account
                    result = await _customerAccountRepo.WithdrawByCustomerIdAsync
                        (currentUserId, customerId, updatedTransaction.TargetCurrencyId, updatedTransaction.ConvertedAmount);
                    if (!result.Item1)
                    {
                        apiResponse.Message = result.Item2;
                        await _genericRepository.RollbackTransactionAsync();
                        return Ok(apiResponse);
                    }

                    customerSellTransaction.Amount = -updatedTransaction.ConvertedAmount;
                    customerSellTransaction.CurrencyId = updatedTransaction.TargetCurrencyId;
                    customerSellTransaction.CustomerId = customerId;
                    customerSellTransaction.DealType = DealType.Withdraw;
                    customerSellTransaction.TransactionType = TransactionType.Sell;
                    customerSellTransaction.CreatedDate = updatedTransaction.CreatedDate;
                    customerSellTransaction.Description = updatedTransaction.Description;
                    customerSellTransaction.UpdatedDate = DateTime.Now;

                    // Add buy amount to customer account
                    // Deposit the Amount to customer account
                    deposit = await _customerAccountRepo.DepositeByCustomerIdAsync
                        (currentUserId, customerId, updatedTransaction.SourceCurrencyId, updatedTransaction.Amount);

                    if (!deposit)
                    {
                        apiResponse.Message = "واریز پول به حساب مشتری انجام نشد.";
                        await _genericRepository.RollbackTransactionAsync();
                        return BadRequest(apiResponse);
                    }

                    customerBuyTransaction.Amount = updatedTransaction.Amount;
                    customerBuyTransaction.CurrencyId = updatedTransaction.SourceCurrencyId;
                    customerBuyTransaction.CreatedDate = updatedTransaction.CreatedDate;
                    customerBuyTransaction.CustomerId = customerId;
                    customerBuyTransaction.Description = updatedTransaction.Description;
                    customerBuyTransaction.DealType = DealType.Deposit;
                    customerBuyTransaction.TransactionType = TransactionType.Buy;
                    customerBuyTransaction.UpdatedDate = DateTime.Now;

                    //Customer Account Section Ended
                }
                else
                {
                    // Sell
                    // Exchange Account Section

                    //1: deposit source currency with amount
                    //2: withdraw targetCurrency with convertedAmount
                    var isWithdrawed = await _customerAccountRepo.WithdrawByCustomerIdAsync
                        (currentUserId, exchangeAccountId, updatedTransaction.TargetCurrencyId, updatedTransaction.ConvertedAmount);
                    if (!isWithdrawed.Item1)
                    {
                        apiResponse.Message = isWithdrawed.Item2;
                        await _genericRepository.RollbackTransactionAsync();
                        return Ok(apiResponse);
                    }

                    sellTransaction.Amount = -updatedTransaction.ConvertedAmount;
                    sellTransaction.CurrencyId = updatedTransaction.TargetCurrencyId;
                    sellTransaction.CreatedDate = updatedTransaction.CreatedDate;
                    sellTransaction.CustomerId = exchangeAccountId;
                    sellTransaction.DealType = DealType.Withdraw;
                    sellTransaction.Description = updatedTransaction.Description;
                    sellTransaction.TransactionType = TransactionType.Sell;
                    sellTransaction.UpdatedDate = DateTime.Now;

                    // add buy transaction
                    // Deposit
                    var deposit = await _customerAccountRepo.DepositeByCustomerIdAsync
                        (currentUserId, exchangeAccountId, updatedTransaction.SourceCurrencyId, updatedTransaction.Amount);
                    if (!deposit)
                    {
                        apiResponse.Message = "واریز پول به حساب تبادله ارز انجام نشد.";
                        await _genericRepository.RollbackTransactionAsync();
                        return BadRequest(apiResponse);
                    }

                    buyTransaction.Amount = updatedTransaction.Amount;
                    buyTransaction.CurrencyId = updatedTransaction.SourceCurrencyId;
                    buyTransaction.CreatedDate = updatedTransaction.CreatedDate;
                    buyTransaction.CustomerId = exchangeAccountId;
                    buyTransaction.DealType = DealType.Deposit;
                    buyTransaction.Description = updatedTransaction.Description;
                    buyTransaction.TransactionType = TransactionType.Buy;
                    buyTransaction.UpdatedDate = DateTime.Now;

                    //Customer Account Section

                    // withdraw the Amount from customer account
                    var result = await _customerAccountRepo.WithdrawByCustomerIdAsync
                        (currentUserId, customerId, updatedTransaction.SourceCurrencyId, updatedTransaction.Amount);
                    if (!result.Item1)
                    {
                        apiResponse.Message = result.Item2;
                        await _genericRepository.RollbackTransactionAsync();
                        return Ok(apiResponse);
                    }

                    customerSellTransaction.Amount = -updatedTransaction.Amount;
                    customerSellTransaction.CurrencyId = updatedTransaction.SourceCurrencyId;
                    customerSellTransaction.CustomerId = customerId;
                    customerSellTransaction.DealType = DealType.Withdraw;
                    customerSellTransaction.TransactionType = TransactionType.Sell;
                    customerSellTransaction.CreatedDate = updatedTransaction.CreatedDate;
                    customerSellTransaction.Description = updatedTransaction.Description;
                    //customerSellTransaction.//Description = string.Format("فروش{0}به نرخ {1}")
                    customerSellTransaction.UpdatedDate = DateTime.Now;

                    // Deposit the Converted
                    deposit = await _customerAccountRepo.DepositeByCustomerIdAsync
                        (currentUserId, customerId, updatedTransaction.TargetCurrencyId, updatedTransaction.ConvertedAmount);

                    if (!deposit)
                    {
                        apiResponse.Message = "واریز پول به حساب مشتری انجام نشد.";
                        await _genericRepository.RollbackTransactionAsync();
                        return BadRequest(apiResponse);
                    }

                    customerBuyTransaction.Amount = updatedTransaction.ConvertedAmount;
                    customerBuyTransaction.CurrencyId = updatedTransaction.TargetCurrencyId;
                    customerBuyTransaction.CreatedDate = updatedTransaction.CreatedDate;
                    customerBuyTransaction.CustomerId = customerId;
                    customerBuyTransaction.DealType = DealType.Deposit;
                    customerBuyTransaction.Description = updatedTransaction.Description;
                    customerBuyTransaction.TransactionType = TransactionType.Buy;
                    customerBuyTransaction.UpdatedDate = DateTime.Now;

                    //Customer Account Section Ended

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
                    .CalculateCustomerBalanceInUSD(currentUserId, exchangeAccountId, exchangeRates.ToList());

                if (totalBalance == null)
                {
                    apiResponse.Message = "حساب تبادله ارز یافت نشد.";
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }

                var BorrowAmount = await _customerAccountRepo.GetBorrowAmountOfACustomer(currentUserId, exchangeAccountId);

                if (BorrowAmount == null)
                {
                    apiResponse.Message = "حساب تبادله ارز یافت نشد.";
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }

                if (totalBalance < -(BorrowAmount))
                {
                    apiResponse.Message = "حساب تبادله ارز به سقف قرضه خود رسیده است. تراکنش ثبت نشد";
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }


                // check currency balance

                totalBalance = await _customerAccountRepo
                    .CalculateCustomerBalanceInUSD(currentUserId, customerId, exchangeRates.ToList());

                if (totalBalance == null)
                {
                    apiResponse.Message = "حساب مشتری یافت نشد.";
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }

                BorrowAmount = await _customerAccountRepo.GetBorrowAmountOfACustomer(currentUserId, customerId);

                if (BorrowAmount == null)
                {
                    apiResponse.Message = "حساب مشتری یافت نشد.";
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }

                if (totalBalance < -(BorrowAmount))
                {
                    apiResponse.Message = "مشتری به سقف قرضه خود رسیده است. تراکنش ثبت نشد";
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }


                var check = await _customerAccountRepo.CheckTreasuryAccountBasedOnBorrowAmount(currentUserId);
                if (!check.Item1)
                {
                    apiResponse.Message = check.Item2;
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }

                // update old transaction

                oldTransaction.Description = updatedTransaction.Description;
                oldTransaction.CreatedDate = updatedTransaction.CreatedDate;
                oldTransaction.UpdatedDate = DateTime.Now;
                oldTransaction.Amount = updatedTransaction.Amount;
                oldTransaction.TransactionType = updatedTransaction.TransactionType;
                oldTransaction.SourceCurrencyId = updatedTransaction.SourceCurrencyId;
                oldTransaction.TargetCurrencyId = updatedTransaction.TargetCurrencyId;
                oldTransaction.Rate = updatedTransaction.Rate;
                oldTransaction.ConvertedAmount = updatedTransaction.ConvertedAmount;

                await _genericRepository.SaveAsync();
                await _genericRepository.CommitTransactionAsync();
                apiResponse.Success = true;
                apiResponse.Data = true;
                _logger.LogInformation($"updating Buy & Sell transaction ENDED: id:{transactionId}");
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
