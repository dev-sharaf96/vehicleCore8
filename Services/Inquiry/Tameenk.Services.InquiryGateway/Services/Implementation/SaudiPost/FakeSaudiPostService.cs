using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Tameenk.Core.Infrastructure;
using Tameenk.Services.Logging;
using Tameenk.Core.Domain.Entities;
using Tameenk.Services.InquiryGateway.Services.Core.SaudiPost;
using Tameenk.Core.Exceptions;
using Tameenk.Services.Core.Http;
using System.Threading.Tasks;
using Tameenk.Core.Configuration;
using System.Reflection;
using System.IO;

namespace Tameenk.Services.InquiryGateway.Services.Implementation.SaudiPost
{
    public class FakeSaudiPostService : ISaudiPostService
    {

        public FakeSaudiPostService()
        {
        }

        public async Task<SaudiPostApiResult> GetAddresses(string id)
        {
            string responseData = ReadResource("Tameenk.Services.InquiryGateway.FakeResponses.SaudiPost.json");

            return JsonConvert.DeserializeObject<SaudiPostApiResult>(responseData);
        }

        private string ReadResource(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            //var resourceName = "MyCompany.MyProduct.MyFile.txt";
            string result = "";
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                result = reader.ReadToEnd();
            }
            return result;

        }

        //private SaudiPostApiResult getMockedData()
        //{
        //    Address resultAddress = new Address()
        //    {
        //        Title = "شركة عناية الوسيط لوساطة التأمين",
        //        Address1 = "التعاون – طريق الدائري الشمالي الفرعي – 2355",
        //        Address2 = "الرياض 1277 – 7889",
        //        BuildingNumber = "2335",
        //        Street = "التعاون",
        //        District = "طريق الدائري الشمالي الفرعي",
        //        City = "الرياض",
        //        PostCode = "7889",
        //        AdditionalNumber = "1277",
        //        UnitNumber = "2",
        //    };
        //    var saudiPostApiResult = new SaudiPostApiResult()
        //    {
        //        totalSearchResults = "1",
        //        Addresses = new List<Address>() { resultAddress },
        //        PostCode = "7880",
        //        success = true,
        //        statusdescription = "SUCCESS",
        //        fullexception = string.Empty
        //    };
        //    return saudiPostApiResult;
        //}
    }
}
