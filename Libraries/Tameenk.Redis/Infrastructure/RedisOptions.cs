using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Redis
{
    public class RedisOptions
    {
        public bool Enabled { get; set; }
        public string ConnectionString { get; set; }
        public string Instance { get; set; }
    }
}
