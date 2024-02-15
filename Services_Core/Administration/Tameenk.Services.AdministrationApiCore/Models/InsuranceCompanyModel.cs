using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Tameenk.Services.AdministrationApi.Models
{
    [JsonObject("insuranceCompany")]
    public class InsuranceCompanyModel
    {
        /// <summary>
        /// insurance company id
        /// </summary>
        [JsonProperty("id")]
        public int InsuranceCompanyID { get; set; }

        /// <summary>
        /// insurance company name in arabic
        /// </summary>
        [JsonProperty("nameAR")]
        public string NameAR { get; set; }

        /// <summary>
        /// insurance company key use in image 
        /// </summary>
        [JsonProperty("key")]
        public string Key { get; set; }

        /// <summary>
        /// insurance company name in English
        /// </summary>
        [JsonProperty("nameEN")]
        public string NameEN { get; set; }

        /// <summary>
        /// Description in Arabic
        /// </summary>
        [JsonProperty("descAR")]
        public string DescAR { get; set; }

        /// <summary>
        /// Description in English
        /// </summary>
        [JsonProperty("descEN")]
        public string DescEN { get; set; }

        /// <summary>
        /// Name space Type Name
        /// </summary>
        [JsonProperty("namespaceTypeName")]
        public string NamespaceTypeName { get; set; }

        /// <summary>
        /// ClassTypeName
        /// </summary>
        [JsonProperty("classTypeName")]
        public string ClassTypeName { get; set; }

        /// <summary>
        /// ReportTemplateName
        /// </summary>
        [JsonProperty("reportTemplateName")]
        public string ReportTemplateName { get; set; }

        /// <summary>
        /// Created date
        /// </summary>
        [JsonProperty("createdDate")]
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Created by
        /// </summary>
        [JsonProperty("createdBy")]
        public Guid? CreatedBy { get; set; }

        /// <summary>
        /// Last modeify data in date
        /// </summary>
        [JsonProperty("lastModifiedDate")]
        public DateTime? LastModifiedDate { get; set; }

        /// <summary>
        /// Modified By
        /// </summary>
        [JsonProperty("modifiedBy")]
        public Guid? ModifiedBy { get; set; }

        /// <summary>
        /// Address Object
        /// </summary>
        [JsonProperty("address")]
        public AddressModel Address { get; set; }

        /// <summary>
        /// Contact Object
        /// </summary>
        [JsonProperty("contact")]
        public ContactModel Contact { get; set; }     

        /// <summary>
        /// active company or not
        /// </summary>
        [JsonProperty("isActive")]
        public Boolean IsActive { get; set; }

        /// <summary>
        /// active TPL company or not
        /// </summary>
        [JsonProperty("isActiveTPL")]
        public Boolean IsActiveTPL { get; set; }

        /// <summary>
        /// active Comprehensive company or not
        /// </summary>
        [JsonProperty("isActiveComprehensive")]
        public Boolean IsActiveComprehensive { get; set; }

        /// <summary>
        /// DLL File 
        /// </summary>
        [JsonProperty("fileToUpload")]
        public Byte[] FileToUpload { get; set; }

        [JsonProperty("companyLogo")]
        public string CompanyLogo { get; set; }

        [JsonProperty("hasDiscount")]
        public bool? HasDiscount { get; set; }

        [JsonProperty("discountText")]
        public string DiscountText { get; set; }

        [JsonProperty("discountStartDate")]
        public DateTime? DiscountStartDate { get; set; }

        [JsonProperty("discountEndDate")]
        public DateTime? DiscountEndDate { get; set; }
        /// <summary>        /// active address validation company or not        /// </summary>        [JsonProperty("isAddressValidationEnabled")]        public bool IsAddressValidationEnabled { get; set; }
        [JsonProperty("usePhoneCamera")]
        public bool? UsePhoneCamera { get; set; }

        [JsonProperty("policyFailureRecipient")]
        public string PolicyFailureRecipient { get; set; }

        [JsonProperty("isUseNumberOfAccident")]
        public bool IsUseNumberOfAccident { get; set; }

        [JsonProperty("najmNcdFreeYearsToUseNumberOfAccident")]
        public string NajmNcdFreeYearsToUseNumberOfAccident { get; set; }

        [JsonProperty("allowAnonymousRequest")]
        public bool AllowAnonymousRequest { get; set; }

        [JsonProperty("showQuotationToUser")]
        public bool ShowQuotationToUser { get; set; }

        [JsonProperty("vAT")]
        public string VAT { get; set; }

        [JsonProperty("termsAndConditionsFile")]
        public Byte[] TermsAndConditionsFile { get; set; }

        [JsonProperty("validateSaudiPostAddressInQuotation")]
        public bool ValidateSaudiPostAddressInQuotation { get; set; }

        [JsonProperty("order")]
        public int Order { get; set; }

        [JsonProperty("activeTabbyTPL")]
        public bool ActiveTabbyTPL { get; set; }
        [JsonProperty("activeTabbyComp")]        public bool ActiveTabbyComp { get; set; }
    }
}