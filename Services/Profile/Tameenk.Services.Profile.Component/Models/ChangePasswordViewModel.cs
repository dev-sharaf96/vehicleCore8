using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Dtos;

namespace Tameenk.Services.Profile.Component
{
    public class ChangePasswordViewModel : BaseViewModel
    {
        public string UserId { get; set; }
        public string Channel { get; set; }
        public string Lang { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmNewPassword { get; set; }

        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public int? OTP { get; set; }

        [JsonIgnore]
        public string LogDescription { get; set; }
    }
}
