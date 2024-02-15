using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Integration.Dto.Providers
{
    public class PurchaseDriverResponse
    {
        public string ReferenceId { set; get; }
        public int StatusCode { set; get; }         
        public List<Error> Errors { get; set; }
        public byte[] EndorsementFile { set; get; }
        public string EndorsementFileUrl { set; get; }
    }
}
