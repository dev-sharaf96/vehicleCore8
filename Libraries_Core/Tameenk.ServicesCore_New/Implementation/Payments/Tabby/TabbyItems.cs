using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Tameenk.Services.Implementation.Payments.Tabby
{
    public class TabbyItems
    {
        public List<BuyerHistoryData> BuyerHistoryData { get; set; }
        public UserData Buyer { get; set; } 
        public int TotalPolicies { get; set; }
    }


    public class UserData
    {
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public DateTime RegisterDate { get; set; }
    }
 
    public class BuyerHistoryData
    {
        public decimal TotalPrice { get; set; }
        public DateTime CreatedDate { get; set; }
        public string StatusName { get; set; }
    }
}
