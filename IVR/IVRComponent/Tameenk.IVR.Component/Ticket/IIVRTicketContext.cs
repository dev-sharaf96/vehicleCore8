using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Loggin.DAL;
using Tameenk.Services.Core;
using Tameenk.Services.Core.IVR;

namespace Tameenk.IVR.Component
{
    public interface IIVRTicketContext
    {
        IVRTicketOutput<TicketDetailsModel> GetTicketDetails(int ticketNo, string methodName);
        IVRTicketOutput<NewTicketResponseModel> CreateNewTicket(NewTicketModel ticketModel, string methodName);
        IVRTicketOutput<bool> UpdateNationalAddress(IVRUpdateNationalAddressModel model, string methodName);

        void AddBasicLog(IVRServicesLog log, string methodName, IVRModuleEnum module);
    }
}
