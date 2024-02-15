using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Domain.Entities
{
    public class Region
    {
        public Region() {
            Cities = new HashSet<City>();
        }
        public int Id { get; set; }

        public string Name { get; set; }

        public ICollection<City> Cities { get; set; }
    }
}
