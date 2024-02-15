using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Tameenk.Integration.Dto.Najm
{
    public class NajmDriverCaseResponse
    {
        public ResponseData ResponseData { get; set; }
    }

    public class ResponseData
    {
        public CaseDetails CaseDetails { get; set; }
        public string MessageID { get; set; }
        public string Message { get; set; }
        public string ReferenceNo { get; set; }
    }
    public class CaseDetails
    {
        public int Noofaccident { get; set; }
        public string Name { get; set; }
        public string ReferenceNo { get; set; }
        [XmlElement("CaseDetail")]
        public List<CaseDetail> CaseDetail { get; set; }

    }
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
        public string SequenceNumber { get; set; }
        public string OwnerID { get; set; }
        public List<object> EstimatedAmount { get; set; }
        public string DamageParts { get; set; }
        public List<object> CauseOfAccident { get; set; }
    }
}
