using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tameenk.Core.Domain.Entities.PromotionPrograms;

namespace Tameenk.Services.AdministrationApi.Models
{
    public class PromotionProgramModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        /// <summary>
        /// Start date of promotion program
        /// </summary>
        public DateTime? EffectiveDate { get; set; }
        /// <summary>
        /// End date of promotion program
        /// </summary>
        public DateTime? DeactivatedDate { get; set; }
        public int ValidationMethodId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreationDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModificationDate { get; set; }
        [JsonIgnore]
        public ICollection<PromotionProgramDomainModel> PromotionProgramDomains { get; set; }
        [JsonIgnore]
        public ICollection<PromotionProgramUser> PromotionProgramUsers { get; set; }
        [JsonIgnore]
        public ICollection<PromotionProgramCodeModel> PromotionProgramCodes { get; set; }
    }
}