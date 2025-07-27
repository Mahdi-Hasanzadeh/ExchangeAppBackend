using API.DataContext;
using Microsoft.EntityFrameworkCore;
using Shared;
using Shared.Contract;
using Shared.DTOs.CurrencyDTOs;
using Shared.Models.Currency;

namespace API.Repositories
{
    public class CurrencyRepo : ICurrencyRepository
    {
        private readonly AppDbContext _context;

        public CurrencyRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> DisableCurrencyExchangeRate(int userId, int exchangeRateId)
        {
            try
            {
                int affectedRows = await _context.CurrencyExchangeRates
                     .Where(c => c.UserId == userId && c.Id == exchangeRateId)
                     .AsNoTracking()
                     .ExecuteUpdateAsync(setters => setters.SetProperty(s => s.IsActive, false)
                     .SetProperty(s => s.EndDate, DateTime.Now)
                     );

                if (affectedRows > 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<IEnumerable<CurrencyDetailDTOForAllRates>> GetAllCurrencyDetails(int userId)
        {
            try
            {
                var result = (from baseCurrency in _context.Currencies.AsNoTracking().Where(c => c.UserId == userId)
                              from targetCurrency in _context.Currencies.AsNoTracking().Where(c => c.UserId == userId)
                              join rate in _context.CurrencyExchangeRates.AsNoTracking()
                                  .Where(r => r.UserId == userId && r.IsActive) // Filter for active user rates
                                  on new { BaseId = baseCurrency.CurrencyId, TargetId = targetCurrency.CurrencyId }
                                  equals new { BaseId = rate.BaseCurrencyId, TargetId = rate.TargetCurrencyId }
                                  into rateGroup
                              from rate in rateGroup.DefaultIfEmpty() // Left Join to include missing rates
                              select new CurrencyDetailDTOForAllRates
                              {
                                  BaseCurrencyId = baseCurrency.CurrencyId,
                                  TargetCurrencyId = targetCurrency.CurrencyId,
                                  Name = targetCurrency.Name,
                                  BuyValue = rate != null ? rate.Buy : 0, // If rate is null, set Buy to 0
                                  SellValue = rate != null ? rate.Sell : 0, // If rate is null, set Sell to 0
                                  UserId = userId,
                                  Unit = baseCurrency.Unit,
                                  CurrencyExchangeRateId = rate != null ? rate.Id : 0,

                              }).ToList();
                return result;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<CurrencyEntity>> GetAllCurrenciesByUserId(int userId)
        {
            try
            {
                return await _context.Currencies.AsNoTracking().Where(c => c.UserId == userId).ToListAsync();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<IEnumerable<CurrencyDetailDTO>> GetAllCurrencyDetails(int baseCurrencyId, int userId)
        {
            try
            {
                var exchangeRates = await _context.CurrencyExchangeRates.AsNoTracking()
                .Where(rate => rate.UserId == userId &&
                rate.IsActive == true &&
                rate.BaseCurrencyId == baseCurrencyId)  // baseCurrencyId for USD
                .ToListAsync();

                var currencies = await GetAllCurrenciesByUserId(userId);

                var currencyList = currencies.Select(currency => new CurrencyDetailDTO
                {
                    BaseCurrencyId = baseCurrencyId,
                    TargetCurrencyId = currency.CurrencyId,
                    Name = currency.Name,
                    ImageURL = currency.ImageURL,
                    Image = currency.Image,
                    BuyValue = exchangeRates
                    .FirstOrDefault(rate => rate.TargetCurrencyId == currency.CurrencyId)?.Buy ?? 0,
                    SellValue = exchangeRates
                    .FirstOrDefault(rate => rate.TargetCurrencyId == currency.CurrencyId)?.Sell ?? 0,
                    UserId = userId,
                    CurrencyExchangeRateId = exchangeRates
                    .FirstOrDefault(rate => rate.TargetCurrencyId == currency.CurrencyId)?.Id ?? 0
                }).ToList();

                return currencyList;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<CurrencyDTO> GetBaseCurrency(int userId)
        {
            try
            {
                // TODO: IS USD OKAY IN HERE?
                var currency = await _context.Currencies.FirstOrDefaultAsync
                    (c => c.UserId == userId && c.Code == "USD");
                if (currency == null)
                {
                    return null;
                }

                return currency.ToCurrencyDTO();

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<CurrencyEntity> GetCurrencyById(int currencyId)
        {
            try
            {
                var currency = await _context.Currencies.AsNoTracking().FirstOrDefaultAsync(c => c.CurrencyId == currencyId);
                if (currency == null) return null;

                return currency;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<bool> UpdateCurrency(int userId, int currencyId, CurrencyDTO currencyDTO)
        {
            try
            {

                int affectedRows = await _context.Currencies
                      .Where(c => c.UserId == userId && c.CurrencyId == currencyId)
                      .ExecuteUpdateAsync(setters => setters
                      .SetProperty(p => p.Name, currencyDTO.Name)
                      .SetProperty(p => p.Code, currencyDTO.Code)
                      .SetProperty(p => p.Symbol, currencyDTO.Symbol)
                      .SetProperty(p => p.Unit, currencyDTO.Unit)
                      .SetProperty(p => p.Image, currencyDTO.Image)
                      .SetProperty(p => p.ImageURL, currencyDTO.ImageURL)
                      );

                if (affectedRows > 0)
                {
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> UpdateCurrencyAtivation(int userId, int currencyId, CurrencyActivationDTO currencyActivationDTO)
        {
            try
            {
                int affectedRows = await _context.Currencies
                     .Where(c => c.UserId == userId && c.CurrencyId == currencyId)
                     .ExecuteUpdateAsync(setters => setters
                     .SetProperty(p => p.IsActive, currencyActivationDTO.IsActive)
                     );

                if (affectedRows > 0)
                {
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
