using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Loggin.DAL.Dtos
{
    public class VehicleMakerModelsModel
    {
        public int Code { get; set; }
        public int MakerCode { get; set; }
        public string EnglishDescription { get; set; }
        public string ArabicDescription { get; set; }
    }
}
