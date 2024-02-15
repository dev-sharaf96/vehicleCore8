using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Domain.Entities.Payments.Esal
{
    public class ProfitMarginRequest: BaseEntity
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ReferenceId { get; set; }
        public string Description { get; set; }
        public Boolean Applied { get; set; }
        public string ProcedureArabic { get; set; }
        public string procedureEnglish { get; set; }
        
    }
}
