using Tameenk.bll.Najm.model;

namespace Tamkeen.bll.Services.Najm
{
    public interface INajmService
    {
        NajmResponseMessage GetNajm(NajmRequestMessage request);
    }
}