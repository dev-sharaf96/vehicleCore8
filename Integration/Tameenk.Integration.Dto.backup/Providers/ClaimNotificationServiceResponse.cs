using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Integration.Dto.Providers
{
    public class ClaimNotificationServiceResponse
    {
        public string ReferenceId { get; set; }
        public int StatusCode { get; set; }
        public List<Error> Errors { get; set; }
    }
}
