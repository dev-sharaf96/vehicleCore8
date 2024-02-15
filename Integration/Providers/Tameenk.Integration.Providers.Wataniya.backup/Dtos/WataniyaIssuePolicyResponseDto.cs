using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Integration.Providers.Wataniya.Dtos
{
    public class WataniyaIssuePolicyResponseDto
    {
        public bool Status { get; set; }
        public string PolicyNO { get; set; }
        public List<Errors> Errors { get; set; }
    }
}
