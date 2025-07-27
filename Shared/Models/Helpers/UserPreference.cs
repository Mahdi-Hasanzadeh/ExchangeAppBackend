using Shared.Models.Currency;
using System.ComponentModel.DataAnnotations;

namespace Shared.Models.Helpers
{
    public class UserPreference
    {
        [Key]
        public int Id { get; set; }

        //public UserEntity UserEntity { get; set; }
        [Required]
        public int UserId { get; set; }

        public int LastUsedAccountNumber { get; set; } = 0;

        public CurrencyEntity CurrencyEntity { get; set; }
        public int? BaseCurrencyId { get; set; }

    }
}
