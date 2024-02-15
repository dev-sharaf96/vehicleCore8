using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestQuotationReports
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("start");
            QuotationContext.GenerateQuotationRequestReport();

            Console.WriteLine("end");
        }
    }
}
