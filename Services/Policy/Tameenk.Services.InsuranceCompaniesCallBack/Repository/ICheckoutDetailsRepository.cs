using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Integration.Dto.Providers;
using Tameenk.Services.InsuranceCompaniesCallBack.Models;

namespace Tameenk.Services.InsuranceCompaniesCallBack.Repository
{
    public interface ICheckoutDetailsRepository
    {
        List<AttachmentModel> GetCarImages(string referenceId);
        AdditionalInfoDetails GetPolicyRequestAdditionalInfo(string referenceId, string policyNo);
        List<AttachmentModel> GetCarImagesWithURL(string referenceId);
    }
}
