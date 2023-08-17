using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InOutManagement
{
    public class Inouts
    {
        public int inout_code { get; set; }
        public string rack_name { get; set; }
        public string item_name { get; set; }
        public string in_date { get; set; }
        public string in_qty { get; set; }
        public string out_date { get; set; }
        public string out_qty { get; set; }
        public string remain { get; set; }
    }
}
