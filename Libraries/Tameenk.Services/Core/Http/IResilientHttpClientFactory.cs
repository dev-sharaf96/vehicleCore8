using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Core.Http
{
    public interface IResilientHttpClientFactory
    {
        IHttpClient CreateResilientHttpClient();
    }
}
