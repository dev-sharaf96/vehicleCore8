using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tameenk.PaymentNotificationsApi.Domain.WafierAndWaffer
{
    public class ShippingAddresses
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public string ShippingAddressAddress { get; set; }
        public string ShippingAddressAddressLine1 { get; set; }
        public string ShippingAddressAddressLine2 { get; set; }
        [Required]
        public string ShippingAddressCity { get; set; }
        [ForeignKey("ShippingAddressCity")]
        public virtual ShipperCities ShipperCities { get; set; }
        [Required]
        public string ShippingAddressStreetName { get; set; }
        [Required]
        public string ShippingAddressArea { get; set; }
        public string ShippingAddressWasel { get; set; }
        [Required]
        public string ShippingAddressHouseNumber { get; set; }
        [Required]
        [DataType(DataType.PhoneNumber)]
        //[RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid Phone number")]
        public string ShippingAddressMobileNumber { get; set; }
        [Required]
        public DateTime ShippingAddressCreatedDate { get; set; }
        [Required]
        [MaxLength(128)]
        public string ShippingAddressUserID { get; set; }
        // [Required]
        public int? CustomersID { get; set; }
        public virtual Customers Customers { get; set; }
    }
}