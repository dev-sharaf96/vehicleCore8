using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Domain.Entities
{
    public class MorniRequest:BaseEntity
    {
        public int Id { get; set; }
        public DateTime? CreatedDate { get; set; }
        //User Object
        public string PhoneNumber { get; set; }
        public string NationalId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        //Vehicle Object 
        public string Make { get; set; }
        public string Model { get; set; }
        public string Year { get; set; }
        public string Color { get; set; }
        public string PlateNumber { get; set; }
        public string PlateFirstLetterId { get; set; }
        public string PlateSecondLetterId { get; set; }
        public string PlateThirdLetterId { get; set; }
        public string VIN { get; set; }
        public string SequenceNumber { get; set; }
        public string CustomsNumber { get; set; }

        // Membership Object
        public string PlanReferenceNumber { get; set; }
        public DateTime? PolicyEffectiveDate { get; set; }
        public DateTime? PolicyExpiryDate { get; set; }
        public string PolicyNumber { get; set; }
        public string PolicyType { get; set; }
        public string RefrenceId { get; set; }
    }
}
