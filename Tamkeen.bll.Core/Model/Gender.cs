using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tamkeen.bll.Model
{

    public class Gender
    {
        public String code { get; set; }
        public String gender { get; set; }
    }


    public class GenderRequestMessage
    {
        [Required]
        public String language { get; set; }
    }
    public class GenderResponseMessage
    {
        public int status { get; set; }
        public List<Gender> genderList { get; set; }
        public string errorMsg { get; set; }
    }
}
