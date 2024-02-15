using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Domain.Entities.Policies
{
    public class Endorsment : BaseEntity
    {
     
        public int Id { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string ServerIP { get; set; }
        public string UserIP { get; set; }
        public string UserAgent { get; set; }
        public string Channel { get; set; }
        public string FilePath { set; get; }
        public int? InsurranceCompanyId { set; get; }
        public string ReferenceId { set; get; }
        public int? PolicyModificationRequestId { set; get; }
        public string QuotationReferenceId { get; set; }

    }
}
