using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Payment.Esal.Component
{
    public class EsalCancelResponseDto
    {
        public string Result { get; set; }
        public List<EsalErrorDto> Errors { get; set; }
    }
}
