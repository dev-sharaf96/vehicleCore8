using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Services.Implementation;
using Tameenk.Services.Policy.Components.Morni;

namespace Tameenk.Services.Policy.Components
{
    public interface IMorniContext
    {
        MorniMembershipOutput CreateMembership(string refrenceId, string channel);
        bool CreateMorniMembership(string refrenceId, string channel);
    }
}
