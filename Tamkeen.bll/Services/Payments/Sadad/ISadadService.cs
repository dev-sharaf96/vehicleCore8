using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tamkeen.bll.Model;
using Tamkeen.bll.Services.Sadad.Models;

namespace Tamkeen.bll.Services.Sadad
{
    public interface ISadadService
    {
        SadadPaymentResponse PayUsingSadad(PaymentRequestModel paymentRequestModel);
    }
}
