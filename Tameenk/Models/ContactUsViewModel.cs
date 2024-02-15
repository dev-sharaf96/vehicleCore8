using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Tameenk.Resources.ContactUs;

namespace Tameenk.Models
{
    public class ContactUsViewModel
    {
        [Required(ErrorMessageResourceName = "required", ErrorMessageResourceType = typeof(ContactUsResource))]
        public string Name { get; set; }

        [Required(ErrorMessageResourceName = "required", ErrorMessageResourceType = typeof(ContactUsResource))]
        [EmailAddress(ErrorMessageResourceName = "InvalidEmail", ErrorMessageResourceType = typeof(ContactUsResource))]
        //[DataType(DataType.EmailAddress, ErrorMessageResourceName = "InvalidEmail", ErrorMessageResourceType =typeof(ContactUsResource))]
        public string Email { get; set; }

        [Required(ErrorMessageResourceName = "required", ErrorMessageResourceType = typeof(ContactUsResource))]
        public string Mobile { get; set; }


        public string Address { get; set; }

        [Required(ErrorMessageResourceName = "required", ErrorMessageResourceType = typeof(ContactUsResource))]
        public string Message { get; set; }

        [Required(ErrorMessageResourceName = "Captcha_required", ErrorMessageResourceType = typeof(ContactUsResource))]
        [RegularExpression(@"^[0-9]{4}$", ErrorMessageResourceName = "Captcha_invalid", ErrorMessageResourceType = typeof(ContactUsResource))]
        public string CaptchaInput { get; set; }
        public string CaptchaToken { get; set; }

        public string IdentityUrl { get; set; }
    }
}