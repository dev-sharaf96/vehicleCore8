using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tameenk.Services.QuotationDependancy.Component
{
    public interface ICompaniesGradesService
    {
        Task<List<AllComapanyWithGrade>> GetCompaniesWithGrades();
    }
}
