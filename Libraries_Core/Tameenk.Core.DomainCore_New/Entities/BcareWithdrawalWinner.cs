using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Domain.Entities
{
    public class BcareWithdrawalWinner : BaseEntity
    {
        public int Id { get; set; }
        public string NationalId { get; set; }
        public string SequenceNumber { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        public int WeekNumber { get; set; }
        public int ProductType { get; set; }
        public bool IsDeleted { get; set; }
        public string ArabicName { get; set; }
        public string EnglishName { get; set; }
        public string MobileNumber { get; set; }
        public int PrizeNumber { get; set; }
    }
}