using Tamkeen.bll.Model;

namespace Tamkeen.bll.Lookups
{
    public interface IGender
    {
        GenderResponseMessage GetGender(GenderRequestMessage genderRequestMessage);
    }
}