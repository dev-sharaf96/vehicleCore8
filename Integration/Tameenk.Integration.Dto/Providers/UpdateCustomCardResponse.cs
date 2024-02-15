using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Integration.Dto.Providers;

namespace Tameenk.Integration.Dto.Providers
{
    public class UpdateCustomCardResponse
    {
        public string ReferenceId { get; set; }
        public int? StatusCode { get; set; }
        public List<Error> Errors { get; set; }
        public Byte[] PolicyFile { get; set; }
        public string PolicyFileUrl { get; set; }
    }
}
