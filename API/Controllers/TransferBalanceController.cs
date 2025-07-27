using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.Contract.CustomerAccount;
using Shared.Contract;
using Shared.Models;
using Shared.DTOs;
using Shared.DTOs.TransactionsDTOs;
using API.Repositories.CustomerAccount;
using Shared.Enums;
using Shared;
using Shared.Contract.Transactions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransferBalanceController : ControllerBase
    {
        private readonly ILogger<TransferBalanceController> _logger;
        private readonly IGenericRepository<TransferBetweenAccountHistory> _genericRepository;
        private readonly ICustomerAccountRepo _customerAccountRepo;
        private readonly IAuthorizationService _authorizationService;
        private readonly ICurrencyRepository _currencyRepository;
        private readonly ITransactions _transactionsRepo;
        private readonly IGenericRepository<CustomerTransactionHistory> _customerTransactionHistoryGeneric;

        public TransferBalanceController
            (ILogger<TransferBalanceController> logger,
            IGenericRepository<TransferBetweenAccountHistory> genericRepository,
            ICustomerAccountRepo customerAccountRepo,
            IAuthorizationService authorizationService,
            ICurrencyRepository currencyRepository,
            ITransactions transactionsRepo,
            IGenericRepository<CustomerTransactionHistory> customerTransactionHistoryGeneric
            )
        {
            _logger = logger;
            _genericRepository = genericRepository;
            _customerAccountRepo = customerAccountRepo;
            _authorizationService = authorizationService;
            _currencyRepository = currencyRepository;
            _transactionsRepo = transactionsRepo;
            _customerTransactionHistoryGeneric = customerTransactionHistoryGeneric;
        }

        [HttpPost("add", Name = "Transfer money between accounts")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<int>>> AddTransferTransation([FromBody] TransferBetweenAccountHistoryDTO transferDTO)
        {
            _logger.LogError("adding transfer transactions");

            var apiResponse = new ApiResponse<int>(false);
            try
            {

                // This method extract the userId from token
                var currentUserId = User.GetUserId();

                bool isOwner = await _authorizationService.IsUserOwnerOfCustomerAsync
                    (transferDTO.SenderId, currentUserId);

                if (!isOwner)
                {
                    apiResponse.Message = "شما اجازه دسترسی به این اطلاعات را ندارید";
                    return Unauthorized(apiResponse);
                }

                await _genericRepository.BeginTransactionAsync();

                // add commision transaction

                transferDTO.UserId = currentUserId;

                if (transferDTO.CommisionType != CommisionType.NoComission)
                {
                    // add commision to its account (commision account)

                    if (transferDTO.CommisionType != CommisionType.Cash)
                    {
                        transferDTO.CommisionCurrencyId = transferDTO.CurrencyId;
                    }

                    // deposit commisionAmout to commision account
                    var isDeposited = await _customerAccountRepo.DepositeByCustomerIdAsync
                     (currentUserId, (int)transferDTO.CommisionAccountId, transferDTO.CommisionCurrencyId, transferDTO.TransactionFeeAmount);

                    if (!isDeposited)
                    {
                        apiResponse.Message = "کمشن به حساب مربوطه واریز نشد.";
                        await _genericRepository.RollbackTransactionAsync();
                        return Ok(apiResponse);
                    }

                    //update treasury account for commision if commision is cash
                    if (transferDTO.CommisionType == CommisionType.Cash)
                    {
                        var treasuryAccountUpdated = await _customerAccountRepo
                            .UpdateTreasuryAccount(currentUserId, transferDTO.TransactionFeeAmount, transferDTO.CommisionCurrencyId, true);

                        if (!treasuryAccountUpdated)
                        {
                            apiResponse.Message = "حساب صندوق بروز رسانی نشد.";
                            await _genericRepository.RollbackTransactionAsync();
                            return Ok(apiResponse);
                        }
                    }

                    // add commision transactions to the transactionTable
                    // for example: 10 USD for commision
                    var transferTransactionDTO = new CustomerTransactionHistoryDTO()
                    {
                        Amount = transferDTO.TransactionFeeAmount,
                        CurrencyId = transferDTO.CommisionCurrencyId,
                        CreatedDate = DateTime.Now,
                        CustomerId = (int)transferDTO.CommisionAccountId,
                        DealType = DealType.Deposit,
                        DepositOrWithdrawBy = transferDTO.TransactionFeeRecievedBy,
                        Description = transferDTO.TransactionFeeDescription,
                        TransactionType = TransactionType.Transfer,
                        UserId = transferDTO.UserId,
                    };

                    var commisionTransactionId = await _customerAccountRepo.AddCustomerTransactionAsync(transferTransactionDTO);

                    if (commisionTransactionId == 0)
                    {
                        apiResponse.Message = "کمشن به حساب مربوطه واریز نشد.";
                        await _genericRepository.RollbackTransactionAsync();
                        return Ok(apiResponse);
                    }
                    transferDTO.TransactionFeeAccountId = commisionTransactionId;
                }

                // add sender transaction
                // sended amount:5000 
                // commision amount: 10
                // total: 5010 to withdraw from the sender amount

                var WithdrawAmount = transferDTO.SendedAmount;
                if (transferDTO.CommisionType == CommisionType.FromSender)
                {
                    WithdrawAmount += transferDTO.TransactionFeeAmount;
                }
                // Withdraw
                var result = await _customerAccountRepo.WithdrawByCustomerIdAsync
                    (currentUserId, transferDTO.SenderId, transferDTO.CurrencyId, WithdrawAmount);
                if (!result.Item1)
                {
                    apiResponse.Message = result.Item2;
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }
                await _customerAccountRepo.SaveAsync();

                // calculate the total balance in USD
                //await _genericCurrencyEntity.GetAllAsync();
                // first check balance that if the user have money in his account

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
                    .CalculateCustomerBalanceInUSD(currentUserId, transferDTO.SenderId, exchangeRates.ToList());

                if (totalBalance == null)
                {
                    apiResponse.Message = "مشتری یافت نشد.";
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }


                var BorrowAmount = await _customerAccountRepo.GetBorrowAmountOfACustomer(currentUserId, transferDTO.SenderId);

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

                // adding sender transaction to transactionTable
                var transactionDTO = new CustomerTransactionHistoryDTO()
                {
                    Amount = -WithdrawAmount,
                    CurrencyId = transferDTO.CurrencyId,
                    CreatedDate = DateTime.Now,
                    CustomerId = transferDTO.SenderId,
                    DealType = DealType.Withdraw,
                    DepositOrWithdrawBy = transferDTO.SendBy,
                    Description = transferDTO.SenderDescription,
                    TransactionType = TransactionType.Transfer,
                    UserId = transferDTO.UserId
                    //DocumentNumber = transferDTO.

                };

                var senderTransactionId = await _customerAccountRepo.AddCustomerTransactionAsync(transactionDTO);

                if (senderTransactionId == 0)
                {
                    apiResponse.Message = "برداشت پول از حساب فرستنده با مشکل مواجه شد.";
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }


                // add reciever transaction
                // Deposit
                // recieved amount 5000
                // Commision:10
                // total 4090
                var depositAmount = transferDTO.RecievedAmount;
                if (transferDTO.CommisionType == CommisionType.FromReciever)
                {
                    depositAmount -= transferDTO.TransactionFeeAmount;
                }

                var deposit = await _customerAccountRepo.DepositeByCustomerIdAsync
                    (currentUserId, transferDTO.RecieverId, transferDTO.CurrencyId, depositAmount);
                if (!deposit)
                {
                    apiResponse.Message = "واریز پول به حساب گیرنده انجام نشد.";
                    await _genericRepository.RollbackTransactionAsync();
                    return BadRequest(apiResponse);
                }
                transactionDTO = new CustomerTransactionHistoryDTO()
                {
                    Amount = depositAmount, // the amount recieved with commision if commision is fromReciever
                    CurrencyId = transferDTO.CurrencyId,
                    CreatedDate = DateTime.Now,
                    CustomerId = transferDTO.RecieverId,
                    DealType = DealType.Deposit,
                    DepositOrWithdrawBy = transferDTO.RecievedBy,
                    Description = transferDTO.RecieverDescription,
                    TransactionType = TransactionType.Transfer,
                    UserId = currentUserId
                };

                var recieverTransactionId = await _customerAccountRepo.AddCustomerTransactionAsync(transactionDTO);

                if (recieverTransactionId == 0)
                {
                    apiResponse.Message = "واریز پول به حساب گیرنده انجام نشد.";
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }

                // add Transfer transaction
                transferDTO.SenderTransactionId = senderTransactionId;
                transferDTO.RecieverTransactionId = recieverTransactionId;
                var transfer = await _genericRepository.AddEntityAsync(transferDTO.ToTransferBetweenAccountHistory());

                if (transfer == null)
                {
                    apiResponse.Message = "تراکنش انتقال ثبت نشد.";
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }
                await _genericRepository.SaveAsync();
                await _genericRepository.CommitTransactionAsync();
                apiResponse.Success = true;
                apiResponse.Data = transfer.Id;
                _logger.LogError("adding transfer transactions: ENDEDDDDDDDD");

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

        [HttpGet("{customerId:int}", Name = "GetCustmerTransferTransaction")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<IEnumerable<TransferSummaryDTO>>>> GetCustomerTransactions(int customerId)
        {
            var apiResponse = new ApiResponse<IEnumerable<TransferSummaryDTO>>(false);
            try
            {
                _logger.LogError("getting transfer transactions");
                var currentUserId = User.GetUserId();

                //TODO, the below method query the database twice, so we can use view to reduce it.

                bool isOwner = await _authorizationService.IsUserOwnerOfCustomerAsync(customerId, currentUserId);

                if (!isOwner)
                {
                    apiResponse.Message = "شما اجازه دسترسی به این اطلاعات را ندارید";
                    return Unauthorized(apiResponse);
                }

                var result = await _transactionsRepo.GetTransferTransactionsByCustomerIdAsync(customerId, currentUserId);
                if (result == null)
                {
                    apiResponse.Message = "دریافت تراکنش های مشتری با مشکل مواجه شد.";
                    return Ok(apiResponse);
                }

                apiResponse.Success = true;
                apiResponse.Data = result;
                _logger.LogError("getting transfer transactions: ENDED");
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

        [HttpPut("updateDetails/{transferId:int}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<bool>>> UpdateTransferTransactionDetails(int transferId, TransferBetweenAccountHistoryDTO transferHistory)
        {
            var apiResponse = new ApiResponse<bool>(false);
            try
            {
                var currentUserId = User.GetUserId();

                var transferTransaction = await _genericRepository.GetByIdAsync(transferHistory.Id);

                if (transferTransaction == null)
                {
                    apiResponse.Message = "تراکنش مشتری یافت نشد.";
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }

                bool isOwner = await _authorizationService.IsUserOwnerOfTransaction(transferTransaction.SenderTransactionId, currentUserId);

                if (!isOwner)
                {
                    apiResponse.Message = "شما اجازه دسترسی به این اطلاعات را ندارید";
                    return Unauthorized(apiResponse);
                }


                var senderTrasactionId = transferHistory.SenderTransactionId;
                var recieverTransactionId = transferHistory.RecieverTransactionId;

                await _genericRepository.BeginTransactionAsync();

                var senderTransaction = await _customerTransactionHistoryGeneric.GetByIdAsync(senderTrasactionId);
                if (senderTransaction == null)
                {
                    apiResponse.Message = "تراکنش مشتری (فرستنده) یافت نشد.";
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }

                senderTransaction.Description = transferHistory.SenderDescription;
                senderTransaction.UpdatedDate = DateTime.Now;
                senderTransaction.CreatedDate = transferHistory.CreatedDate;
                senderTransaction.DepositOrWithdrawBy = transferHistory.SendBy;

                var recieverTransaction = await _customerTransactionHistoryGeneric.GetByIdAsync(recieverTransactionId);
                if (recieverTransaction == null)
                {
                    apiResponse.Message = "تراکنش مشتری (گیرنده) یافت نشد.";
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }

                senderTransaction.Description = transferHistory.SenderDescription;
                senderTransaction.UpdatedDate = DateTime.Now;
                senderTransaction.CreatedDate = transferHistory.CreatedDate;
                senderTransaction.DepositOrWithdrawBy = transferHistory.SendBy;



                transferTransaction.SenderDescription = transferHistory.SenderDescription;
                transferTransaction.RecieverDescription = transferHistory.RecieverDescription;
                transferTransaction.CreatedDate = transferTransaction.CreatedDate;
                transferTransaction.LastUpdatedDate = DateTime.Now;
                transferTransaction.SendBy = transferHistory.SendBy;
                transferTransaction.RecievedBy = transferHistory.RecievedBy;

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

        [HttpPut("{transferId:int}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<bool>>> UpdateTransferTransaction(int transferId, TransferBetweenAccountHistoryDTO transferDTO)
        {
            var apiResponse = new ApiResponse<bool>(false);
            try
            {
                _logger.LogInformation($"updating transfer transaction STARTED: id:{transferId}");
                var currentUserId = User.GetUserId();
                var oldTransaction = await _genericRepository.GetByIdAsync(transferDTO.Id);

                if (oldTransaction == null)
                {
                    apiResponse.Message = "تراکنش مشتری یافت نشد.";
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }

                bool isOwner = await _authorizationService.IsUserOwnerOfTransaction(oldTransaction.SenderTransactionId, currentUserId);

                if (!isOwner)
                {
                    apiResponse.Message = "شما اجازه دسترسی به این اطلاعات را ندارید";
                    return Unauthorized(apiResponse);
                }

                // first: find the old transaction

                // second: Rollback transaction
                //1: rollback treasury balance, if commisionType is not NoCommision, rollback treasury
                //2: rollback commision acount, if commisionType is not NoCommision, rollback commisionAccount
                //3:rollback sender balance(check if the Commision is from sender)
                //4:roolback reciever balance(check if the commision is from reciever)

                // update transaction
                //1: update treasury balance, if the commision type is not NoCommision
                //2: update commisionAccount, if commisionType is not NoCommision,
                //3:update senderBalance(check if the commision is from sender)
                //4: update senderTransaction
                //5:update recieverBalance(check if the commision is from reciever)
                //6: update recieverTransaction
                //7:update transferTransaction


                await _genericRepository.BeginTransactionAsync();

                // rollback commisionAccount and treasury balance
                if (oldTransaction.CommisionType != CommisionType.NoComission)
                {
                    int commisionAccountId = (int)oldTransaction.CommisionAccountId;
                    int commisionCurrencyId = (int)oldTransaction.CommisionCurrencyId;
                    var commisionAmount = oldTransaction.TransactionFeeAmount;

                    // withdraw the commision amount from the Commision account
                    var isDeposited = await _customerAccountRepo.WithdrawByCustomerIdAsync
                     (currentUserId, commisionAccountId, commisionCurrencyId, commisionAmount);

                    if (!isDeposited.Item1)
                    {
                        apiResponse.Message = isDeposited.Item2;
                        await _genericRepository.RollbackTransactionAsync();
                        return Ok(apiResponse);
                    }
                    if (oldTransaction.CommisionType == CommisionType.Cash)
                    {
                        //update treasury account for commision
                        var treasuryAccountUpdated = await _customerAccountRepo
                            .UpdateTreasuryAccount(currentUserId, commisionAmount, commisionCurrencyId, false);

                        if (!treasuryAccountUpdated)
                        {
                            apiResponse.Message = "حساب صندوق بروز رسانی نشد.";
                            await _genericRepository.RollbackTransactionAsync();
                            return Ok(apiResponse);
                        }
                    }

                }

                // rollback sender balance
                // sendedAmount:5000
                // commisionAmount:10
                var DepositAmount = oldTransaction.SendedAmount;
                if (oldTransaction.CommisionType == CommisionType.FromSender)
                {
                    DepositAmount += oldTransaction.TransactionFeeAmount;
                }

                // Deposit money back to sender account
                var result = await _customerAccountRepo.DepositeByCustomerIdAsync
                    (currentUserId, oldTransaction.SenderId, oldTransaction.CurrencyId, DepositAmount);

                if (!result)
                {
                    apiResponse.Message = "خطا در بروز رسانی حساب فرستنده";
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }

                // rollback reciever balance
                // recieverAmount:5000
                // commisionAmount:10
                //
                var WithdrawAmount = oldTransaction.RecievedAmount;
                if (oldTransaction.CommisionType == CommisionType.FromReciever)
                {
                    WithdrawAmount -= oldTransaction.TransactionFeeAmount;
                }

                // Withdraw
                // Withdraw money from reciever account
                var withdrawResult = await _customerAccountRepo.WithdrawByCustomerIdAsync
                    (currentUserId, oldTransaction.RecieverId, oldTransaction.CurrencyId, WithdrawAmount);

                if (!withdrawResult.Item1)
                {
                    apiResponse.Message = withdrawResult.Item2;
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }
                //Rollback finished

                // update transaction from scratch

                // update treasury and commision account
                if (oldTransaction.CommisionType != CommisionType.NoComission && transferDTO.CommisionType == CommisionType.NoComission)
                {
                    // Set all details to null
                    var commisionTransaction = await _customerTransactionHistoryGeneric.GetByIdAsync((int)oldTransaction.TransactionFeeAccountId);

                    if (commisionTransaction == null)
                    {
                        apiResponse.Message = "تراکنش حساب کمشن یافت نشد.";
                        await _genericRepository.RollbackTransactionAsync();
                        return Ok(apiResponse);
                    }

                    commisionTransaction.Amount = 0;
                    //commisionTransaction.CurrencyId = transferDTO.CommisionCurrencyId;
                    commisionTransaction.CreatedDate = transferDTO.CreatedDate;
                    commisionTransaction.UpdatedDate = DateTime.Now;
                    commisionTransaction.DepositOrWithdrawBy = string.Empty;
                    commisionTransaction.Description = string.Empty;
                }
                else if (transferDTO.CommisionType != CommisionType.NoComission)
                {
                    if (transferDTO.CommisionType != CommisionType.Cash)
                    {
                        transferDTO.CommisionCurrencyId = transferDTO.CurrencyId;
                    }

                    // update commision account
                    var isDeposited = await _customerAccountRepo.DepositeByCustomerIdAsync
                     (currentUserId, (int)transferDTO.CommisionAccountId, transferDTO.CommisionCurrencyId, transferDTO.TransactionFeeAmount);

                    if (!isDeposited)
                    {
                        apiResponse.Message = "کمشن به حساب مربوطه واریز نشد.";
                        await _genericRepository.RollbackTransactionAsync();
                        return Ok(apiResponse);
                    }

                    //update treasury account for commision
                    if (transferDTO.CommisionType == CommisionType.Cash)
                    {
                        var treasuryAccountUpdated = await _customerAccountRepo
                        .UpdateTreasuryAccount(currentUserId, transferDTO.TransactionFeeAmount, transferDTO.CommisionCurrencyId, true);

                        if (!treasuryAccountUpdated)
                        {
                            apiResponse.Message = "حساب صندوق بروز رسانی نشد.";
                            await _genericRepository.RollbackTransactionAsync();
                            return Ok(apiResponse);
                        }
                    }

                    if (oldTransaction.CommisionType == CommisionType.NoComission && oldTransaction.TransactionFeeAccountId == null)
                    {
                        var transferTransactionDTO = new CustomerTransactionHistoryDTO()
                        {
                            Amount = transferDTO.TransactionFeeAmount,
                            CurrencyId = transferDTO.CommisionCurrencyId,
                            CreatedDate = DateTime.Now,
                            CustomerId = (int)transferDTO.CommisionAccountId,
                            DealType = DealType.Deposit,
                            DepositOrWithdrawBy = transferDTO.TransactionFeeRecievedBy,
                            Description = transferDTO.TransactionFeeDescription,
                            TransactionType = TransactionType.Transfer,
                            UserId = currentUserId,
                        };

                        var commisionTransactionId = await _customerAccountRepo.AddCustomerTransactionAsync(transferTransactionDTO);

                        if (commisionTransactionId == 0)
                        {
                            apiResponse.Message = "کمشن به حساب مربوطه واریز نشد.";
                            await _genericRepository.RollbackTransactionAsync();
                            return Ok(apiResponse);
                        }
                        transferDTO.TransactionFeeAccountId = commisionTransactionId;
                    }
                    else
                    {
                        var commisionTransaction = await _customerTransactionHistoryGeneric.GetByIdAsync((int)oldTransaction.TransactionFeeAccountId);

                        if (commisionTransaction == null)
                        {
                            apiResponse.Message = "تراکنش حساب کمشن یافت نشد.";
                            await _genericRepository.RollbackTransactionAsync();
                            return Ok(apiResponse);
                        }

                        transferDTO.TransactionFeeAccountId = commisionTransaction.TransactionId;

                        commisionTransaction.Amount = transferDTO.TransactionFeeAmount;
                        commisionTransaction.CurrencyId = transferDTO.CommisionCurrencyId;
                        commisionTransaction.CreatedDate = transferDTO.CreatedDate;
                        commisionTransaction.UpdatedDate = DateTime.Now;
                        commisionTransaction.DepositOrWithdrawBy = transferDTO.TransactionFeeRecievedBy;
                        commisionTransaction.Description = transferDTO.TransactionFeeDescription;
                        commisionTransaction.CustomerId = (int)transferDTO.CommisionAccountId;
                    }
                }

                // update sender transaction

                var senderTrasactionId = oldTransaction.SenderTransactionId;
                var senderTransaction = await _customerTransactionHistoryGeneric.GetByIdAsync(senderTrasactionId);
                if (senderTransaction == null)
                {
                    apiResponse.Message = "تراکنش مشتری (فرستنده) یافت نشد.";
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }

                WithdrawAmount = transferDTO.SendedAmount;
                if (transferDTO.CommisionType == CommisionType.FromSender)
                {
                    WithdrawAmount += transferDTO.TransactionFeeAmount;
                }
                // Withdraw
                withdrawResult = await _customerAccountRepo.WithdrawByCustomerIdAsync
                    (currentUserId, transferDTO.SenderId, transferDTO.CurrencyId, WithdrawAmount);
                if (!withdrawResult.Item1)
                {
                    apiResponse.Message = withdrawResult.Item2;
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }

                //senderTransaction.Amount = -transferDTO.SendedAmount;
                senderTransaction.Amount = -WithdrawAmount;
                senderTransaction.CurrencyId = transferDTO.CurrencyId;
                senderTransaction.CreatedDate = transferDTO.CreatedDate;
                senderTransaction.UpdatedDate = DateTime.Now;
                senderTransaction.DepositOrWithdrawBy = transferDTO.SendBy;
                senderTransaction.Description = transferDTO.SenderDescription;


                // update reciever transaction

                var recieverTrasactionId = oldTransaction.RecieverTransactionId;
                var recieverTrasaction = await _customerTransactionHistoryGeneric.GetByIdAsync(recieverTrasactionId);
                if (recieverTrasaction == null)
                {
                    apiResponse.Message = "تراکنش مشتری (گیرنده) یافت نشد.";
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }

                DepositAmount = transferDTO.RecievedAmount;
                if (transferDTO.CommisionType == CommisionType.FromReciever)
                {
                    DepositAmount -= transferDTO.TransactionFeeAmount;
                }
                // Deposit
                result = await _customerAccountRepo.DepositeByCustomerIdAsync
                    (currentUserId, transferDTO.RecieverId, transferDTO.CurrencyId, DepositAmount);
                if (!result)
                {
                    apiResponse.Message = "واریز پول به حساب گیرنده انجام نشد.";
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }

                //recieverTrasaction.Amount = transferDTO.RecievedAmount;
                recieverTrasaction.Amount = DepositAmount;
                recieverTrasaction.CurrencyId = transferDTO.CurrencyId;
                recieverTrasaction.CreatedDate = transferDTO.CreatedDate;
                recieverTrasaction.UpdatedDate = DateTime.Now;
                recieverTrasaction.DepositOrWithdrawBy = transferDTO.RecievedBy;
                recieverTrasaction.Description = transferDTO.RecieverDescription;
                recieverTrasaction.CustomerId = transferDTO.RecieverId;
                await _customerAccountRepo.SaveAsync();

                // check if the reciever has balance to withdraw


                // calculate the total balance in USD
                //await _genericCurrencyEntity.GetAllAsync();

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
                    .CalculateCustomerBalanceInUSD(currentUserId, oldTransaction.RecieverId, exchangeRates.ToList());

                if (totalBalance == null)
                {
                    apiResponse.Message = "مشتری یافت نشد.";
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }

                var BorrowAmount = await _customerAccountRepo.GetBorrowAmountOfACustomer(currentUserId, oldTransaction.RecieverId);

                if (BorrowAmount == null)
                {
                    apiResponse.Message = "مشتری یافت نشد.";
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }

                if (totalBalance < -(BorrowAmount))
                {
                    apiResponse.Message = "مشتری(گیرنده) به سقف قرضه خود رسیده است. تراکنش ثبت نشد";
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }

                totalBalance = await _customerAccountRepo
                   .CalculateCustomerBalanceInUSD(currentUserId, oldTransaction.SenderId, exchangeRates.ToList());

                if (totalBalance == null)
                {
                    apiResponse.Message = "مشتری یافت نشد.";
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }

                BorrowAmount = await _customerAccountRepo.GetBorrowAmountOfACustomer(currentUserId, oldTransaction.SenderId);

                if (BorrowAmount == null)
                {
                    apiResponse.Message = "مشتری یافت نشد.";
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }

                if (totalBalance < -(BorrowAmount))
                {
                    apiResponse.Message = "مشتری(فرستنده) به سقف قرضه خود رسیده است. تراکنش ثبت نشد";
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }

                // update transfer transaction

                oldTransaction.RecieverId = transferDTO.RecieverId;
                oldTransaction.RecievedAmount = transferDTO.RecievedAmount;
                oldTransaction.SendedAmount = transferDTO.SendedAmount;
                oldTransaction.CurrencyId = transferDTO.CurrencyId;
                oldTransaction.CreatedDate = transferDTO.CreatedDate;
                oldTransaction.LastUpdatedDate = DateTime.Now;
                oldTransaction.SendBy = transferDTO.SendBy;
                oldTransaction.RecievedBy = transferDTO.RecievedBy;
                oldTransaction.TransactionFeeRecievedBy = transferDTO.TransactionFeeRecievedBy;
                oldTransaction.SenderDescription = transferDTO.SenderDescription;
                oldTransaction.RecieverDescription = transferDTO.RecieverDescription;
                oldTransaction.TransactionFeeDescription = transferDTO.TransactionFeeDescription;
                oldTransaction.CommisionType = transferDTO.CommisionType;

                if (transferDTO.CommisionType == CommisionType.NoComission)
                {
                    oldTransaction.TransactionFeeAmount = 0;
                    oldTransaction.CommisionCurrencyId = null;
                    oldTransaction.CommisionAccountId = null;
                    //oldTransaction.TransactionFeeAccountId = null;
                }
                else
                {
                    oldTransaction.TransactionFeeAmount = transferDTO.TransactionFeeAmount;
                    oldTransaction.CommisionCurrencyId = transferDTO.CommisionCurrencyId;
                    oldTransaction.CommisionAccountId = transferDTO.CommisionAccountId;
                    oldTransaction.TransactionFeeAccountId = transferDTO.TransactionFeeAccountId;
                }

                await _genericRepository.SaveAsync();
                await _genericRepository.CommitTransactionAsync();
                apiResponse.Success = true;
                apiResponse.Data = true;
                _logger.LogInformation($"updating transfer transaction ENDED: id:{transferId}");
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

        [HttpDelete("{transferId:int}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteTransferTransaction(int transferId)
        {
            var apiResponse = new ApiResponse<bool>(false);
            try
            {
                _logger.LogInformation($"Deleting Transfer Transactions:STARTED: {transferId}");

                var currentUserId = User.GetUserId();

                var oldTransaction = await _genericRepository.GetByIdAsync(transferId);
                if (oldTransaction == null)
                {
                    apiResponse.Message = "تراکنش مشتری یافت نشد.";
                    return Ok(apiResponse);
                }
                bool isOwner = await _authorizationService.IsUserOwnerOfTransaction(oldTransaction.SenderTransactionId, currentUserId);

                if (!isOwner)
                {
                    apiResponse.Message = "شما اجازه دسترسی به این اطلاعات را ندارید";
                    return Unauthorized(apiResponse);
                }

                await _genericRepository.BeginTransactionAsync();

                // rollback commisionAccount and treasury balance
                if (oldTransaction.CommisionType != CommisionType.NoComission)
                {
                    int commisionAccountId = (int)oldTransaction.CommisionAccountId;
                    int commisionCurrencyId = (int)oldTransaction.CommisionCurrencyId;
                    var commisionAmount = oldTransaction.TransactionFeeAmount;

                    var isDeposited = await _customerAccountRepo.WithdrawByCustomerIdAsync
                     (currentUserId, commisionAccountId, commisionCurrencyId, commisionAmount);

                    if (!isDeposited.Item1)
                    {
                        apiResponse.Message = isDeposited.Item2;
                        await _genericRepository.RollbackTransactionAsync();
                        return Ok(apiResponse);
                    }

                    //update treasury account for commision
                    if (oldTransaction.CommisionType == CommisionType.Cash)
                    {
                        var treasuryAccountUpdated = await _customerAccountRepo
                            .UpdateTreasuryAccount(currentUserId, commisionAmount, commisionCurrencyId, false);
                        if (!treasuryAccountUpdated)
                        {
                            apiResponse.Message = "حساب صندوق بروز رسانی نشد.";
                            await _genericRepository.RollbackTransactionAsync();
                            return Ok(apiResponse);
                        }
                    }

                    var isDeleted = await _customerTransactionHistoryGeneric.DeleteByIdAsync((int)oldTransaction.TransactionFeeAccountId);
                    if (!isDeleted)
                    {
                        apiResponse.Message = "تراکنش حذف نشد.";
                        await _genericRepository.RollbackTransactionAsync();
                        return Ok(apiResponse);
                    }
                }



                // rollback sender balance
                //5000 + 10
                var DepositAmount = oldTransaction.SendedAmount;
                if (oldTransaction.CommisionType == CommisionType.FromSender)
                {
                    DepositAmount += oldTransaction.TransactionFeeAmount;
                }

                // Deposit
                var result = await _customerAccountRepo.DepositeByCustomerIdAsync
                    (currentUserId, oldTransaction.SenderId, oldTransaction.CurrencyId, DepositAmount);

                if (!result)
                {
                    apiResponse.Message = "خطا در بروز رسانی حساب فرستنده";
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }

                var isRemoved = await _customerTransactionHistoryGeneric.DeleteByIdAsync(oldTransaction.SenderTransactionId);
                if (!isRemoved)
                {
                    apiResponse.Message = "تراکنش حذف نشد.";
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }

                // rollback reciever balance
                // 5000 10
                var WithdrawAmount = oldTransaction.RecievedAmount;
                if (oldTransaction.CommisionType == CommisionType.FromReciever)
                {
                    WithdrawAmount -= oldTransaction.TransactionFeeAmount;
                }

                // Withdraw
                var withdrawResult = await _customerAccountRepo.WithdrawByCustomerIdAsync
                    (currentUserId, oldTransaction.RecieverId, oldTransaction.CurrencyId, WithdrawAmount);

                if (!withdrawResult.Item1)
                {
                    apiResponse.Message = withdrawResult.Item2;
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }

                isRemoved = await _customerTransactionHistoryGeneric.DeleteByIdAsync(oldTransaction.RecieverTransactionId);
                if (!isRemoved)
                {
                    apiResponse.Message = "تراکنش حذف نشد.";
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }

                // delete transfer transaction
                isRemoved = await _genericRepository.DeleteByIdAsync(transferId);
                if (!isRemoved)
                {
                    apiResponse.Message = "تراکنش حذف نشد.";
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }

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
                    .CalculateCustomerBalanceInUSD(currentUserId, oldTransaction.RecieverId, exchangeRates.ToList());

                if (totalBalance == null)
                {
                    apiResponse.Message = "مشتری یافت نشد.";
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }

                var BorrowAmount = await _customerAccountRepo.GetBorrowAmountOfACustomer(currentUserId, oldTransaction.RecieverId);

                if (BorrowAmount == null)
                {
                    apiResponse.Message = "مشتری یافت نشد.";
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }

                if (totalBalance < -(BorrowAmount))
                {
                    apiResponse.Message = "مشتری(گیرنده) به سقف قرضه خود رسیده است. تراکنش ثبت نشد";
                    await _genericRepository.RollbackTransactionAsync();
                    return Ok(apiResponse);
                }

                await _genericRepository.SaveAsync();
                await _genericRepository.CommitTransactionAsync();
                apiResponse.Success = true;
                apiResponse.Data = true;
                apiResponse.Message = "تراکنش حذف شد.";
                _logger.LogInformation($"Deleting Transfer Transactions:ENDED: {transferId}");
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
