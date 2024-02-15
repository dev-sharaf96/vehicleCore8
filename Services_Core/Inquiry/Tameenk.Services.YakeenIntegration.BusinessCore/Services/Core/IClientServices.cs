using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities;
using Tameenk.Integration.Dto.Yakeen;
using Tameenk.Loggin.DAL;
using Tameenk.Services.YakeenIntegration.Business.Dto;

namespace Tameenk.Services.YakeenIntegration.Business.Services
{
   public interface IClientServices
    {
        CustomerIdYakeenInfoDto GetClientInfo(ClientRequestModel clientRequestModel);
    }
}
