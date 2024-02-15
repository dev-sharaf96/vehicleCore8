﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Domain.Entities.PromotionPrograms
{
    public class PromotionProgramNins : BaseEntity
    {
        public int Id { get; set; }
        public int PromotionProgramId { get; set; }
        public string NationalId { get; set; }
        //public string Email { get; set; }
        //public bool EmailVerified { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreationDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModificationDate { get; set; }
        public bool isDeleted { get; set; }

        public PromotionProgram PromotionProgram { get; set; }
    }
}
