using Shared.DTOs.CurrencyDTOs;
using Shared.Models.Currency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Contract
{
    public interface ICurrencyRepository
    {
        public Task<bool> UpdateCurrency(int userId, int currencyId, CurrencyDTO currencyDTO);

        public Task<bool> UpdateCurrencyAtivation(int userId, int currencyId, CurrencyActivationDTO currencyActivationDTO);

        public Task<IEnumerable<CurrencyDetailDTO>> GetAllCurrencyDetails(int baseCurrencyId, int userId);

        public Task<IEnumerable<CurrencyDetailDTOForAllRates>> GetAllCurrencyDetails(int userId);

        public Task<bool> DisableCurrencyExchangeRate(int userId,int exchangeRateId);

        public Task<CurrencyDTO> GetBaseCurrency(int userId);

        public Task<List<CurrencyEntity>> GetAllCurrenciesByUserId(int userId);
    }
}
