using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Core
{
    public enum IVRTicketSubTypeEnum
    {
        [EnumSubType(IVRTicketTypeEnum.ChangePolicyData)]
        UpdateCustomToSequesne = 1,
        [EnumSubType(IVRTicketTypeEnum.ChangePolicyData)]
        UpdateCarPlate,
        [EnumSubType(IVRTicketTypeEnum.ChangePolicyData)]
        UpdatePersonalInformation,
        [EnumSubType(IVRTicketTypeEnum.ChangePolicyData)]
        UpdateMoreInformationDetails,

        //[EnumSubType(IVRTicketTypeEnum.CouldNotPrintThePolicy)]
        //PolicyExist,
        //[EnumSubType(IVRTicketTypeEnum.CouldNotPrintThePolicy)]
        //PolicyNotExistWithSada,
        //[EnumSubType(IVRTicketTypeEnum.CouldNotPrintThePolicy)]
        //PolicyNotExistWithMadaOrVisaOrMasterCard
    }
}
