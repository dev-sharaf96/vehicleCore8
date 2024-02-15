using System.Collections.Generic;

namespace Tameenk.Core.Domain.Entities
{
    public class CheckoutCarImage : BaseEntity
    {
        public CheckoutCarImage()
        {
            CheckoutDetails = new HashSet<CheckoutDetail>();
            CheckoutDetails1 = new HashSet<CheckoutDetail>();
            CheckoutDetails2 = new HashSet<CheckoutDetail>();
            CheckoutDetails3 = new HashSet<CheckoutDetail>();
            CheckoutDetails4 = new HashSet<CheckoutDetail>();
        }

        public int ID { get; set; }
        
        public byte[] ImageData { get; set; }
        public string ImageURL { get; set; }

        public ICollection<CheckoutDetail> CheckoutDetails { get; set; }

        public ICollection<CheckoutDetail> CheckoutDetails1 { get; set; }

        public ICollection<CheckoutDetail> CheckoutDetails2 { get; set; }

        public ICollection<CheckoutDetail> CheckoutDetails3 { get; set; }

        public ICollection<CheckoutDetail> CheckoutDetails4 { get; set; }
    }
}
