using System;
using System.Windows.Forms;
using System.Drawing;
using System.Linq;

namespace InOutManagement
{
    public class ComModules
    {
        #region ToComma(string szData) : 숫자를 문자로 받아 셋째 자리마다 숫자를 찍어 문자로 리턴

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
                szReturn = string.Format("{0:###,###,###,###,###,##0}", data);

            }
            return szReturn;
        }
        #endregion

        #region SetListViewRowColors(ListView lst, Color color1, Color color2) : ListView 행 배경색 설정하기

        /// <summary>
        /// 행 배경색 설정하기
        /// </summary>
        /// <param name="ListView">ListView 객체</param>
        /// <param name="color1">홀수 행 색상</param>
        /// <param name="color2">짝수 행 색상</param>
        public void SetListViewRowColors(ListView lst, Color color1, Color color2)
        {

            foreach (ListViewItem item in lst.Items)
            {
                if ((item.Index % 2) == 0)
                    item.BackColor = color1;
                else
                    item.BackColor = color2;
            }
        }
        #endregion 

        #region SetDataGridViewRowColors(DataGridView lst, Color color1, Color color2) : DataGridView 행 배경색 설정하기

        /// <summary>
        /// 행 배경색 설정하기
        /// </summary>
        /// <param name="DataGridView">DataGridView 객체</param>
        /// <param name="color1">홀수 행 색상</param>
        /// <param name="color2">짝수 행 색상</param>
        public void SetDataGridViewRowColors(DataGridView lst, Color color1, Color color2)
        {

            foreach (DataGridViewRow item in lst.Rows)
            {
                if ((item.Index % 2) == 0)
                    item.DefaultCellStyle.BackColor = color1;
                else
                    item.DefaultCellStyle.BackColor = color2;
            }
        }
        #endregion 

        public void SetGridHeader(DataGridView dgv, bool sort, Color bc, Color fc, Color sbc, Color sfc)
        {
            SetInitDataGridView(dgv, bc, fc, sbc, sfc);                                                               // Grid 초기화 및 해더 색상 변경
            dgv.Columns.Cast<DataGridViewColumn>().ToList().ForEach(f =>
            {
                if (!sort)
                    f.SortMode = DataGridViewColumnSortMode.NotSortable;                                // sort 막기
                else
                    f.SortMode = DataGridViewColumnSortMode.Automatic;                                  // sort 하기
                f.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;           //헤더텍스트 센터 정렬
                f.HeaderCell.Style.Font = new Font("굴림", 12F, FontStyle.Bold, GraphicsUnit.Pixel);      // 폰트 사이트 및 크기 조정
            });
        }

        #region SetInitDataGridView(DataGridView dgv, Color bc, Color fc) : DataGridView Header의 BackColor, ForeColor, BorderStyle 변경
        /// <summary>
        /// DataGridView Header의 BackColor, ForeColor, BorderStyle 변경
        /// </summary>
        /// <param name="dgv">DataGridView 객체</param>
        /// <param name="bc">  BackColor           </param>
        /// <param name="fc">  ForeColor             </param>
        /// <param name="fbc"> SelectedBackColor </param>
        /// <param name="ffc"> SelectedForeColor  </param>
        public void SetInitDataGridView(DataGridView dgv, Color bc, Color fc, Color sbc, Color sfc)
        {
            dgv.ColumnHeadersDefaultCellStyle.BackColor = bc;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = fc;
            dgv.ColumnHeadersDefaultCellStyle.SelectionBackColor = sbc;
            dgv.ColumnHeadersDefaultCellStyle.SelectionForeColor = sfc;
            dgv.RowHeadersDefaultCellStyle.SelectionBackColor = sbc;
            dgv.RowHeadersDefaultCellStyle.SelectionForeColor = sfc;
            dgv.RowsDefaultCellStyle.SelectionBackColor = sbc;
            dgv.RowsDefaultCellStyle.SelectionForeColor = sfc;
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dgv.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dgv.EnableHeadersVisualStyles = false;
        }
        #endregion 

        #region SetGridAlternatingRowColor(DataGridView dgv, Color rc) : DataGridView Row색상 번갈아 변경
        /// <summary>
        /// DataGridView Row색상 번갈아 변경
        /// </summary>
        /// <param name="dgv">DataGridView 객체</param>
        /// <param name="rc">변경 색상</param>
        public void SetGridAlternatingRowColor(DataGridView dgv, Color rc) => dgv.AlternatingRowsDefaultCellStyle.BackColor = rc;
        #endregion 

        #region Key_Press(object sender, KeyPressEventArgs e) : 텍스트박스에서 숫자와 백스페이스 키만 처리 되게..
        /// <summary>
        /// 텍스트박스에서 숫자와 백스페이스 키만 처리 되게..
        public void Key_Press(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsDigit(e.KeyChar) || e.KeyChar == Convert.ToChar(Keys.Back)))    //숫자와 백스페이스를 제외한 나머지를 바로 처리             
            {
                e.Handled = true;
            }
        }
        #endregion 
    }
}
