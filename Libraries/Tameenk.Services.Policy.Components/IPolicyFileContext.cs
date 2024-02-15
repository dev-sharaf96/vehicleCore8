using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Enums;
using Tameenk.Integration.Dto.Providers;

namespace Tameenk.Services.Policy.Components
{
    public interface IPolicyFileContext
    {
        PolicyGenerationOutput GetPolicyTemplateGenerationModel(string referenceId);
        PolicyGenerationOutput GeneratePolicyPdfFile(PolicyResponse policy, int companyId, string channel, LanguageTwoLetterIsoCode selectedLanguage, bool generateTemplateFromOurSide);
    }
}
