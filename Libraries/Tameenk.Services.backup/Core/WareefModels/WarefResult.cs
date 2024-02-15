using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services
{
    public class WarefResult
    {
        public int Id { set; get; }
        public string NameAr { set; get; }
        public string NameEn { get; set; }
        public string ImageBytes { get; set; }
        public int WareefCategoryId { get; set; }
        public string ItemURl { get; set; }
    }
}
