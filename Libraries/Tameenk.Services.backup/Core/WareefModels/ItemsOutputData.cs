using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Services
{
  public  class ItemsOutputData
    {
        public int Id { set; get; }
        public string NameAr { set; get; }
        public string NameEn { get; set; }
        public string ImageBytes { get; set; }
        public int WareefCategoryId { get; set; }
        public string ItemURl { get; set; }

        public List<DiscountItem> ItemDiscounts { get; set; }
    }
}
