using System.Threading.Tasks;

namespace Tameenk.Services.InquiryGateway.Services.Core.SaudiPost
{
    public interface ISaudiPostService
    {
        Task<SaudiPostApiResult> GetAddresses(string id);
    }
}