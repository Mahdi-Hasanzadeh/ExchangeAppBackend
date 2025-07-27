using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs.CustomerDTOs
{
    public class CustomerDTO
    {
        public CustomerDTO(int customerId, int userId)
        {
            CustomerId = customerId;
            UserId = userId;
        }

        public int CustomerId { get; }
        public int UserId { get; }
    }
}
