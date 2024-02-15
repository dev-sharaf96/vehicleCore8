using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tamkeen.bll.Model
{
    public class CardType
    {
        public int code { get; set; }
        public String description { get; set; }
    }


    public class CardTypeRequestMessage
    {
        [Required]
        public String language { get; set; }
    }
    public class CardTypeResponseMessage
    {
        public int status { get; set; }
        public List<CardType> cardList { get; set; }
        public string errorMsg { get; set; }
    }

}
