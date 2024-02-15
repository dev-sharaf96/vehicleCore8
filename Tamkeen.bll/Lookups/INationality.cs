using Tamkeen.bll.Model;

namespace Tamkeen.bll.Lookups
{
    public interface INationality
    {
        NationalityResponseMessage GetNationalityList(NationalityRequestMessage requestMessage);

    }
}