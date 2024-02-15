using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Integration.Dto.Najm
{
    public class NajmVehicleCaseResponse
    {
        public class CaseDetail
        {
            public string CaseNumber { get; set; }
            public DateTime AccidentDate { get; set; }
            public string Liability { get; set; }
            public string CityName { get; set; }
            public string DriverAge { get; set; }
            public List<object> CarModel { get; set; }
            public string CarType { get; set; }
            public string DriverID { get; set; }
            public List<object> SequenceNumber { get; set; }
            public List<object> OwnerID { get; set; }
            public List<object> EstimatedAmount { get; set; }
            public string DamageParts { get; set; }
            public object CauseOfAccident { get; set; }
        }

        public class CaseDetails
        {
            public string Noofaccident { get; set; }
            public List<object> Name { get; set; }
            public string ReferenceNo { get; set; }
            public List<CaseDetail> CaseDetail { get; set; }
        }

        public class RootObject
        {
            public CaseDetails CaseDetails { get; set; }
        }
    }
}
