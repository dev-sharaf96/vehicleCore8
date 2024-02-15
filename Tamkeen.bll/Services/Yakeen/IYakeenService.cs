using Tamkeen.bll.Services.Yakeen.Models;

namespace Tamkeen.bll.Services.Yakeen
{
    public interface IYakeenService
    {
        /// <summary>
        /// data needed in req : (IsCitizen{citizen:1,alien:2},UserName,Password,ChargeCode,ReferenceNumber,NIN,DateOfBirth)
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        CustomerIdInfoResult GetCustomerIdInfo(YakeenRequest req);

        /// <summary>
        /// data needed in req : (IsCitizen{citizen:1,alien:2},UserName,Password,ChargeCode,ReferenceNumber,NIN,DateOfBirth)
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        CustomerNameInfoResult GetCustomerNameInfo(YakeenRequest req);

        /// <summary>
        /// data needed in req : (IsCitizen{citizen:1,alien:2},UserName,Password,ChargeCode,ReferenceNumber,NIN,licExpiryDate)
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        DriverInfoResult GetDriverInfo(YakeenRequest req);

        /// <summary>
        /// data needed in req : registered : (IsCarRegistered=true,UserName,Password,ChargeCode,ReferenceNumber,CarOwnerId,CarSequenceNumber)
        /// not registered : (IsCarRegistered=false,UserName,Password,ChargeCode,ReferenceNumber,CarModelYear,CustomCarCardNumber)
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        CarInfoResult GetCarInfo(YakeenRequest req);

        /// <summary>
        /// data needed in req : (IsCarRegistered=true,UserName,Password,ChargeCode,ReferenceNumber,CarOwnerId,CarSequenceNumber)
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        CarPlateResult GetCarPlateInfo(YakeenRequest req);

        string GetChassisNumberByCustom(YakeenRequest req);
    }
}
