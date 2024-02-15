using System.Collections.Generic;

namespace Tameenk.Services
{
    public class Category
    {
        public int Id { set; get; }
        public  string? NameAr { set; get; }
        public  string? NameEn { get; set; }
        public  string? Icon { get; set; }
        public List<ItemsOutputData>? ItemsOutputData { get; set; }
    }
}
