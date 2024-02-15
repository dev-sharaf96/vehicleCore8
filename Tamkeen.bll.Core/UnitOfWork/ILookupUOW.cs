using Tamkeen.bll.Lookups;

namespace Tamkeen.bll.UnitOfWork
{
    public interface ILookupUOW
    {
        IYearLookup YearLookup { get; }
        ICityLookup CityLookup { get; }
        IBankLookup BankLookup { get; }
        IColorLookup ColorLookup { get; }

    }
}