using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesManagement
{
    class ComModules
    {
        public string ToComma(string szData)
        {
            string szReturn = string.Empty;
            if (szData != "")
            {
                string szValue = szData.Replace(",", "");
                Double data = Double.Parse(szValue);
                szReturn = string.Format("{0:###,###,###,###,###,##0}", data);
            }
            return szReturn;
        }
    }
}
