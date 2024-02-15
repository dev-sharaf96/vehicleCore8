using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tamkeen.bll.Model
{
    public class City
    {
        public int code { get; set; }
        public String description { get; set; }
    }


    public class CitesRequestMessage
    {
        [Required]
        public String language { get; set; }
    }
    public class CitesResponseMessage
    {
        public int status { get; set; }
        public List<City> citiesList { get; set; }
        public string errorMsg { get; set; }
    }

}
