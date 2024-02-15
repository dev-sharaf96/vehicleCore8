using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Domain.Enums
{
    public enum EUserTicketTypes
    {
        LinkWithNajm = 1,
        ChangePolicyData = 2,
        CouldnotPrintPolicy = 3,
        PolicyGeneration = 4,
        Percentage = 5,
        NationalAddress = 6,
        AddDrivers = 7,
        CancelPolicy = 8,
        Others = 9,
        UpdateCustomToSequence = 10,
        ProofIDAndIBANAndEmail = 11,
        ReachMaximumPolicyIssuance = 12,
    }
}
