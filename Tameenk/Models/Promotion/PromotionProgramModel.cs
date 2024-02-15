using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Models.Promotion
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
        public bool IsPromoByEmail { get; set; }

    }
}