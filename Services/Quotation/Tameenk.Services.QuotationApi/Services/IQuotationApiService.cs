using System;
using Tameenk.Core;
using Tameenk.Core.Domain.Entities.Quotations;
using Tameenk.Loggin.DAL;

namespace Tameenk.Services.QuotationApi.Services
{
    public interface IQuotationApiService
    {

        


        QuotationResult GetQuotation(int insuranceCompanyId, string qtRqstExtrnlId, ServiceRequestLog predefinedLogInfo, int insuranceTypeCode = 1, bool vehicleAgencyRepair = false, short? deductibleValue = null, bool automatedTest = false);

        /// <summary>
        /// Get Quotation Request by user id
        /// </summary>
        /// <param name="userId">user id</param>
        /// <param name="pageIndx">page index</param>
        /// <param name="pageSize">page size</param>
        /// <returns></returns>
        IPagedList<QuotationRequest> GetQuotationRequestsByUserId(string userId, int pageIndx = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Save byte data in dll file ( Bin folder ) 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        void SaveFileInBin(string fileName, Byte[] file);

        /// <summary>
        /// get number of offers fro specific user
        /// </summary>
        /// <param name="id">user id</param>
        /// <returns></returns>
        int GetUserOffersCount(string id);

        /// <summary>
        /// Export Quotation Automated Test Excel Sheet
        /// </summary>
        /// <returns>Excel Sheet File Name</returns>
        string ExportAutomatedTestResultToExcel(bool Quotation);
    }
}
