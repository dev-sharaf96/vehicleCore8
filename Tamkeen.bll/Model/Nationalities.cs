using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tamkeen.bll.Model
{
    public class Nationality
    {
        public int code { get; set; }
        public String nationality { get; set; }
    }


    public class NationalityRequestMessage
    {
        [Required]
        public String language { get; set; }
    }
    public class NationalityResponseMessage
    {
        public int status { get; set; }
        public List<Nationality> nationalityList { get; set; }
        public string errorMsg { get; set; }
    }
}
