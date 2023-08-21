using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InOutManagement
{
    class SortingProcess : IComparer<Inouts>
    {
        private readonly string _memberName = string.Empty;         // the member name to be sorted
        private readonly SortOrder _sortOrder = SortOrder.None;

        public SortingProcess(string memberName, SortOrder sortingOrder)
        {
            _memberName = memberName;
            _sortOrder = sortingOrder;
        }

        public int Compare(Inouts inout1, Inouts inout2)
        {
            if (_sortOrder != SortOrder.Ascending)
            {
                var tmp = inout1;
                inout1 = inout2;
                inout2 = tmp;
            }

            switch (_memberName)
            {
                case "inout_code":
                    return inout1.inout_code.CompareTo(inout2.inout_code);
                case "item_name":
                    return inout1.item_name.CompareTo(inout2.item_name);
                case "rack_name":
                    return inout1.rack_name.CompareTo(inout2.rack_name);
                case "in_date":
                    return inout1.in_date.CompareTo(inout2.in_date);
                case "in_qty":
                    return inout1.in_qty.CompareTo(inout2.in_qty);
                case "out_date":
                    return inout1.out_date.CompareTo(inout2.out_date);
                case "out_qty":
                    return inout1.out_date.CompareTo(inout2.out_date);
                case "remain":
                    return inout1.remain.CompareTo(inout2.remain);
                default:
                    return -1;
            }
        }
    }
}
