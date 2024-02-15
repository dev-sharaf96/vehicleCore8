using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Tameenk.Models
{
    public class VerifyCodeCheckoutViewModel
    {
        public string UserId { get; set; }

        //[Required]
        public string Code { get; set; }

        public string PhoneNumber { get; set; }

        public string ReferenceId { get; set; }

    }
}