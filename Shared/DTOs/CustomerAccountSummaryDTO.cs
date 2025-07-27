using Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs
{
    public class CustomerAccountSummaryDTO
    {
        public CustomerAccountSummaryDTO(int customerId, string customerName, int accountNumber, string customerLastname, eAccountType accountType)
        {
            CustomerId = customerId;
            CustomerName = customerName;
            AccountNumber = accountNumber;
            CustomerLastname = customerLastname;
            AccountType = accountType;
        }

        public int CustomerId { get; set; }
        public string CustomerName { get; set; }

        public int AccountNumber { get; set; }

        public string CustomerLastname { get; set; }

        public eAccountType AccountType { get; set; }
    }
}
