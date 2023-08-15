using System;
using System.Windows.Forms;
using System.Drawing;

namespace ProductionManagement
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

        #region 행 배경색 설정하기 - SetRowBackgroundColor(listView, oddRowColor, evenRowColor)

        /// <summary>
        /// 행 배경색 설정하기
        /// </summary>
        /// <param name="listView">ListView 객체</param>
        /// <param name="oddRowColor">홀수 행 색상</param>
        /// <param name="evenRowColor">짝수 행 색상</param>
        public void SetAlternatingRowColors(ListView lst, Color color1, Color color2)
        {

            //loop through each ListViewItem in the ListView control
            foreach (ListViewItem item in lst.Items)
            {
                if ((item.Index % 2) == 0)
                    item.BackColor = color1;
                else
                    item.BackColor = color2;
             }
        }
        #endregion 
    }
}
