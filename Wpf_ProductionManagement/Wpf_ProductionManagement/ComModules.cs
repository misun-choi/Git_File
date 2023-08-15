using System;
using System.Drawing;
using System.Windows.Controls;
using System.Windows.Media;

namespace Wpf_ProductionManagement
{
    public class ComModules
    {
        #region 숫자를 문자로 받아 셋째 자리마다 숫자를 찍어 문자로 리턴 - ToComma(string szData)

        /// <summary>
        /// 숫자를 문자로 받아 셋째 자리마다 숫자를 찍어 문자로 리턴
        /// </summary>
        /// <param name="szData">숫자</param>
        public string ToComma(string szData)
        {
            string szReturn = string.Empty;
            if (szData != "")
            {
                string szValue = szData.Replace(",", "");
                Double data = Double.Parse(szValue);
                szReturn = string.Format("{0:###,###,###,###,###,###}", data);

            }
            return szReturn;
        }
        #endregion

       
    }
}
