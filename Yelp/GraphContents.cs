using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yelp
{
    public class GraphContents
    {
        // Temp BID = EQq1K0q9VVJm4zYw0NGwfA, City = Scottsdale, Zip = 85260, Name = ADEQ

        public Dictionary<int, string> monthList = new Dictionary<int, string>();
        
        public GraphContents()
        {
            monthList.Add(1,"January");
            monthList.Add(2,"February");
            monthList.Add(3,"March");
            monthList.Add(4,"April");
            monthList.Add(5,"May");
            monthList.Add(6,"June");
            monthList.Add(7,"July");
            monthList.Add(8,"August");
            monthList.Add(9,"September");
            monthList.Add(10,"October");
            monthList.Add(11,"November");
            monthList.Add(12,"December");
        }

        public string GetMonth(int month)
        {
            return monthList[month];
        }

    }
}
