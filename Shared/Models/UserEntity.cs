using Shared.Models.Currency;
using Shared.Models.Helpers;
using Shared.Roles;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Shared.Models
{
    public class UserEntity
    {
        [Key]
        public int Id { get; set; }


        [Required]
        public required string Username { get; set; }

        [Required]
        public required string Password { get; set; }

        [Required]
        public UserRole Role { get; set; } = UserRole.User; // Default to User


        [JsonIgnore]// Navigation property to the parent user
        public UserEntity? ParentUser { get; set; }

        [JsonIgnore] // Navigation property to sub-users (employees)
        public ICollection<UserEntity> SubUsers { get; set; } = new List<UserEntity>();

        public string Email { get; set; } = string.Empty;

        public string? PictureUrl { get; set; } = string.Empty;

        public byte[]? Image { get; set; }

        public int? ParentUserId { get; set; } // ParentUserId for sub-users (employees)

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime? LastModifiedDate { get; set; }

        public DateTime? ValidUntil { get; set; }

        public string? ConnectionString { get; set; }

        public string? ServerAddress { get; set; }

        public string? Databasename { get; set; }

        public bool isFirstTimeLogin { get; set; } = true;

        //// Navigation property to CustomerAccounts
        //[JsonIgnore]
        //public ICollection<CustomerAccount> CustomerAccounts { get; set; } = new List<CustomerAccount>();

        //[JsonIgnore]
        //public ICollection<CurrencyEntity> Currency { get; set; } = new List<CurrencyEntity>();
        //[JsonIgnore]
        //public ICollection<CurrencyExchangeRate> CurrencyExchangeRates { get; set; } = new List<CurrencyExchangeRate>();
        //[JsonIgnore]
        //public UserPreference UserPreference { get; set; } // Navigation property

    }

}
