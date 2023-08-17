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
                szReturn = string.Format("{0:###,###,###,###,###,###}", data);

            }
            return szReturn;
        }
        #endregion

        #region SetListViewRowColors(ListView lst, Color color1, Color color2) : 행 배경색 설정하기

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

        #region SetDataGridViewRowColors(DataGridView lst, Color color1, Color color2) : 행 배경색 설정하기

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

        public void SetInitDataGridView(DataGridView dgv, Color bc, Color fc)
        {
            dgv.ColumnHeadersDefaultCellStyle.BackColor = bc;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = fc;
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dgv.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dgv.EnableHeadersVisualStyles = false;
        }

        public void SetGridHeader(DataGridView dgv, bool sort, Color bc, Color fc)
        {
            SetInitDataGridView(dgv, bc, fc);                                                                       // Grid 초기화 및 해더 색상 변경
            dgv.Columns.Cast<DataGridViewColumn>().ToList().ForEach(f =>
            {
                if (!sort)
                    f.SortMode = DataGridViewColumnSortMode.NotSortable;                                // sort 막기

                f.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;           //헤더텍스트 센터 정렬
                f.HeaderCell.Style.Font = new Font("굴림", 12F, FontStyle.Bold, GraphicsUnit.Pixel);      // 폰트 사이트 및 크기 조정
            });
        }
    }
}
