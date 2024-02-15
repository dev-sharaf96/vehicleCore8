using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.InquiryGateway.Models.YakeenMissingFields
{
    public class DatePickerField : YakeenMissingFieldBase
    {
        public DatePickerField()
        {
            ControlType = "datePicker";
        }
    }
}