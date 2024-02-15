using System;
using Tameenk.Services.Core.InsuranceCompanies;
using Tameenk.Services.Core.Policies;
namespace Tameenk.Services.Policy.Components
{
    public class CompanyGradeTask : ITask
    {
        public IInsuranceCompanyService _iInsuranceCompanyService { get; }
        public CompanyGradeTask(IInsuranceCompanyService iInsuranceCompanyService)
        {
            _iInsuranceCompanyService = iInsuranceCompanyService;
        }

        #region Methods
        public async void Execute(int maxTrials, int? sendingThreshold, string commonPolicyFailureRecipient)
        {
            try
            {
                if (DateTime.Now.Hour == 1)
                    _iInsuranceCompanyService.UpdateCompanyGrade();
            }
            catch (Exception ex)
            {
                System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\ScheduledTask\logs\UpdateCompanyGrade" + DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss_mms") + ".txt", " Exception is:" + ex.ToString());
            }
        }
        #endregion
    }

}
