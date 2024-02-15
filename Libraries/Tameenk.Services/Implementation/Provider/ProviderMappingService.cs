using Tameenk.Core.Domain.Enums;
using Tameenk.Services.Core.Provider;

namespace Tameenk.Services.Implementation.Provider
{
    public class ProviderMappingService: IProviderMappingService
    {
        public string GetProviderCompanyName(Company company)
        {
            switch (company)
            {
                case Company.ACIG:
                    return "ACIG - المجموعة المتحدة للتأمين التعاوني";
                case Company.AICC:
                    return "AICC - شركة العربية للتأمين التعاونية";
                case Company.Saqr:
                    return "Sagr - الصقر للتأمين";
                case Company.Solidarity:
                    return "Solidarity - سوليدرتي للتأمين";
                case Company.TUIC:
                    return "TUIC - الاتحاد للتأمين";
                case Company.Wafa:
                    return "Wafa - وفا للتأمين";
                case Company.Wala:
                    return "Wala - ولاء للتأمين التعاوني";
                case Company.MedGulf:
                    return "MedGulf - ميدجلف";
                case Company.Ahlia:
                    return "Ahlia - الأهلية";
                case Company.ArabianShield:
                    return "Arabian Shield - الدرع العربي";
                case Company.Tawuniya:
                    return "Tawuniya - التعاونية";
                default:
                    return "";
            }
        }

        public string GetProviderCompanyNameAr(Company company)
        {
            switch (company)
            {
                case Company.ACIG:
                    return "المجموعة المتحدة للتأمين التعاوني";
                case Company.AICC:
                    return "شركة العربية للتأمين التعاونية";
                case Company.Saqr:
                    return "الصقر للتأمين";
                case Company.Solidarity:
                    return "سوليدرتي للتأمين";
                case Company.TUIC:
                    return "الاتحاد للتأمين";
                case Company.Wafa:
                    return "وفا للتأمين";
                case Company.Wala:
                    return "ولاء للتأمين التعاوني";
                case Company.MedGulf:
                    return "ميدجلف";
                case Company.ArabianShield:
                    return "الدرع العربي";
                case Company.Ahlia:
                    return "الاهلية";
                case Company.Tawuniya:
                    return "التعاونية";

                case Company.GGI:
                    return "الخليجية العامة للتأمين التعاوني";
                default:
                    return "";
            }
        }

        public string GetProviderCompanyNameEn(Company company)
        {
            switch (company)
            {
                case Company.ACIG:
                    return "ACIG";
                case Company.AICC:
                    return "AICC";
                case Company.Saqr:
                    return "Sagr";
                case Company.Solidarity:
                    return "Solidarity"; 
                case Company.TUIC:
                    return "TUIC";
                case Company.Wafa:
                    return "Wafa";
                case Company.Wala:
                    return "Wala";
                case Company.MedGulf:
                    return "MedGulf";
                case Company.Ahlia:
                    return "Ahlia";
                case Company.ArabianShield:
                    return "Arabian Shield";
                case Company.GGI:
                    return "Gulf General";
                case Company.Tawuniya:
                    return "Tawuniya";
                default:
                    return "";
            }
        }
    }

}
