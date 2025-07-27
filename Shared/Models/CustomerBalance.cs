using Shared.Models.Currency;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Shared.Models
{
    public class CustomerBalance
    {
        [Key]
        public int Id { get; set; }

        public int CustomerId { get; set; }

        public int CurrencyId { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,4)")]
        public decimal Balance { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; } = new byte[0];

        // Navigation properties
        [JsonIgnore]

        public CurrencyEntity CurrencyEntity { get; set; }
        [JsonIgnore]

        public CustomerAccount CustomerAccount { get; set; }

        //[JsonIgnore]

        //public UserEntity UserEntity { get; set; }
        public int UserId { get; set; }

    }

}
