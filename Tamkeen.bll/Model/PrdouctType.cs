using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Tamkeen.bll.Model
{
    public class PrdouctType
    {
        public int productTypeCode { get; set; }
        public string productType { get; set; }
    }

    public class ProductsTypeResponseMessage
    {
        public int status { get; set; }
        public List<PrdouctType> productList { get; set; }
        public string errorMsg { get; set; }
    }

    public class ProductsTypeRequestMessage
    {
        [Required]
        public String language { get; set; }
    }
}
