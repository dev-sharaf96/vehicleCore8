using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Common.Utilities;

namespace Tameenk.Services.Core.IVR
{
    public interface IIVRService
    {
        IVRTicketPolicyDetails GetLastPolicyBySequenceOrCustomCardNumber(string sequenceOrCustomCardNumber, out string exception);
        RenewalPolicyDriversDataModel GetLastPolicyDriversCheckoutuserIdAndPolicyNo(string checkoutuserId, string policyNo, out string exception);
        bool SendSMS(string phone, string body, SMSMethod smsMethod, out string exception);
    }
}
