using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Integration.Dto.Providers
{
    public class CancelPolicyResponse
    {
        public string ReferenceId { get; set; }
        public ErrorCode StatusCode { get; set; }
        public enum ErrorCode
        {
            Success = 1,
            Failure = 2,
            InProcess = 3
        }
        public List<Error> Errors { get; set; }
        public decimal RefundAmount { get; set; }
        public byte[] CreditNoteFile { get; set; }
        public string CreditNoteFileUrl { get; set; }
    }
}
