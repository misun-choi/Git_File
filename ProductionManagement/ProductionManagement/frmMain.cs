using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Windows.Forms;

namespace ProductionManagement
{
    public partial class frmMain : Form
    {
        DataManager dm = new DataManager();
        ComModules cm = new ComModules();
        //Sorter sorter = new Sorter();

        public frmMain()
        {
            InitializeComponent();

            if (!DataManager.initDB())
                Environment.Exit(0);    // 프로그램 시작도 하지 말고 종료     

            InitGetData();

            ButtonEvent();
        }
        public DataManager GetDm() => dm;
        public ComModules GetCm() => cm;

        private void InitGetData()
        {
            for (int i = DateTime.Now.Year; i >= DateTime.Now.Year - 20; i--)         // 생산년도
                cbYear.Items.Add(i);
            for (int i = 1; i <= 12; i++)                                                           // 생산월
                cbMonth.Items.Add(i);
            dm.SelComboData("Item", cbItem);                                                // 품목 콤보
            dm.SelComboData("Center", cbWorkCenter);                                    // 작업장 콤보

            cbYear.Text = DateTime.Now.Year.ToString();
            cbMonth.Text = DateTime.Now.Month.ToString();

            InitForm();
        }

        private void InitForm()
        {
            lblProCode.Text = "";
            cbItem.SelectedIndex = -1;
            cbWorkCenter.SelectedIndex = -1;
            txtQty.Text = "";
            DateTimePickerFormatChange(dtEdate, "");
            dm.ProductionList(lvList, cbYear.Text + string.Format("{0:D2}", int.Parse(cbMonth.Text)));
        }

        #region 버튼처리...
        private void ButtonEvent()
        {
            // 생산계획 조회 버튼 클릭시 처리,,,,
            btnPlanQuery.Click += (sender, e) =>
            {
                dm.ProductionList(lvList, cbYear.Text + string.Format("{0:D2}", int.Parse(cbMonth.Text)));
            };

            // 선택된 생산계획 삭제 버튼 클릭시 처리....
            btnDelete.Click += (sender, e) =>
            {
                if (lvList .SelectedItems .Count > 0)
                {
                    if (MessageBox.Show("생산계획을 삭제하시겠습니까?.", "알림", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        string sProCode = lblProCode.Text;
                        string sSql = String.Format("delete from T_PRODUCTIONS where pro_code = '{0}'", sProCode);
                        dm.DataProcess(sSql);
                        dm.ProductionList(lvList, cbYear.Text + string.Format("{0:D2}", int.Parse(cbMonth.Text)));
                        statusStrip.Items[0].Text = "생산계획 삭제가 정상 처리 되었습니다.";
                        timer.Start();                        
                    }
                }
                else
                {
                    MessageBox.Show("삭제 할 생산계획이 선택되지 않았습니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }
             };

            // 생산계획 등록 버튼 클릭시 처리....
            btnAdd.Click += (sender, e) =>
            {
                string sDateTime = dtSdate.Value.ToString("yyyy-MM-dd HH:mm:ss");
                string sSql = "select count(*) cnt from T_PRODUCTIONS a where a.item_code = '" +
                                  cbItem.SelectedValue.ToString() + "'  and a.workcenter_code = '" + cbWorkCenter.SelectedValue.ToString() + "'" +
                                  " and to_char(a.sdate, 'yyyy-mm-dd') = '" + sDateTime.Substring (0,10) + "'";
                if (dm.ExistCheck(sSql) > 0)
                {
                    if (MessageBox.Show("생산계획이 존재합니다..계속하시겠습니까?", "알림", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                        return;
                }
                else
                {
                    if (!InputChk())
                        return;

                    sSql = String.Format("insert into T_PRODUCTIONS(item_code, Workcenter_Code, Qty, Sdate) values ('{0}','{1}',{2}, to_date('{3}','yyyy-mm-dd hh24:mi:ss'))",
                                        cbItem.SelectedValue.ToString(), cbWorkCenter.SelectedValue.ToString(), int.Parse(txtQty.Text.Replace(",", "")), sDateTime);

                    if (dm.DataProcess(sSql))
                    {
                        InitForm();
                        statusStrip.Items[0].Text = "생산계획이 정상 등록 되었습니다.";
                        timer.Start();
                    }
                }             
            };

             // 생산완료 등록 버튼 클릭시 처리....
            btnComplete.Click += (sender, e) =>
            {
                if (!InputChk())
                    return;

                string eDateTime = dtEdate.Value.ToString("yyyy-MM-dd HH:mm:ss");
                string sSql = String.Format("update T_PRODUCTIONS set edate = to_date('{0}','yyyy-mm-dd hh24:mi:ss') " +
                                            " where pro_code = '{1}' ", eDateTime, lblProCode.Text);

                if (dm.DataProcess(sSql))
                {
                    dm.ProductionList(lvList, cbYear.Text + string.Format("{0:D2}", int.Parse(cbMonth.Text)));
                    statusStrip.Items[0].Text = "생산완료일이 정상 등록 되었습니다.";
                    timer.Start();
                    
                }
            };

            // 생산실적 조회 버튼 클릭시 처리....
            btnSummary.Click += (sender, e) =>
            {
                frmSum frmsum = new frmSum();
                frmsum.SetParent(this);
                frmsum.ShowDialog();
            };
        }
        #endregion 

        private bool InputChk()
        {
            if (cbItem.SelectedIndex < 0)
            {
                MessageBox.Show("품목이 선택되지 않았습니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                cbItem.Focus();
                return false;
            }
 
            if (cbWorkCenter.SelectedIndex < 0)
            {
                MessageBox.Show("작업장이 선택되지 않았습니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                cbWorkCenter.Focus();
                return false;
            }

            return true;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            statusStrip.Items[0].Text = "";
            timer.Stop();
        }

        private void dtEdate_ValueChanged(object sender, EventArgs e)
        {
            dtEdate.CustomFormat = "yyyy년 MM월 dd일 dddd";
        }

        private void lvList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvList.SelectedItems.Count == 0)
                return;

            dtEdate.Value = DateTime .Now.AddDays (-1);             // 어제날짜를 넣은 이유 -> 선택한 날짜가 오늘이면 dtEdate_ValueChanged가 실행이 안 됨.. 따라서 어제 날짜를 일부러 주어서 오늘 오늘날짜를 선택해서 dtEdate_ValueChanged가 실행 될수 있게 하기 위함.
            lblProCode.Text = lvList.SelectedItems[0].SubItems[0].Text;
            cbItem.Text = lvList.SelectedItems[0].SubItems[1].Text;
            txtQty.Text = lvList.SelectedItems[0].SubItems[3].Text;
            cbWorkCenter.Text = lvList.SelectedItems[0].SubItems[2].Text;
            dtSdate.Value = DateTime.Parse(lvList.SelectedItems[0].SubItems[4].Text);
            if (lvList.SelectedItems[0].SubItems[5].Text != "")
                DateTimePickerFormatChange(dtEdate, lvList.SelectedItems[0].SubItems[5].Text);
            else
                DateTimePickerFormatChange(dtEdate, "");
        }

        private void DateTimePickerFormatChange(DateTimePicker dt, string s)
        {
            if (s == "")
            {
                dt.Value = DateTime.Now;
                dt.CustomFormat = " ";
                dt.Format = DateTimePickerFormat.Custom;
            }
            else
            {
                dt.CustomFormat = "yyyy년 MM월 dd일 dddd";
                dt.Value = DateTime.Parse(s);
            }                
        }

        private void txtQty_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsDigit(e.KeyChar) || e.KeyChar == Convert.ToChar(Keys.Back)))    //숫자와 백스페이스를 제외한 나머지를 바로 처리             
            {
                e.Handled = true;
            }
        }

        private void txtQty_TextChanged(object sender, EventArgs e)
        {
            txtQty.Text = cm.ToComma(txtQty.Text);
            txtQty.SelectionStart = txtQty.TextLength;
            txtQty.SelectionLength = 0;
         }

        private void lvList_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (lvList.Sorting == SortOrder.Ascending)   lvList.Sorting = SortOrder.Descending;
            else                                                   lvList.Sorting = SortOrder.Ascending;

            lvList.ListViewItemSorter = new Sorter();      // * 1
            Sorter s = (Sorter)lvList.ListViewItemSorter;
            s.Order = lvList.Sorting;
            s.Column = e.Column;
            lvList.Sort();
        }

        private void lvList_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            cm.SetAlternatingRowColors(lvList, Color.White, Color.Beige);
            e.Graphics.FillRectangle(Brushes.Bisque, e.Bounds);
            e.DrawText();
        }

        private void lvList_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            /*
            if ((e.ItemState & ListViewItemStates.Focused) > 0)
            {
                e.Graphics.FillRectangle(SystemBrushes.ControlDarkDark,
                e.Bounds);
                e.Graphics.DrawString(e.Item.Text, lvList.Font,
                SystemBrushes.HighlightText, e.Bounds);
            }
            else
            {
                e.DrawBackground();
                e.DrawText();
            }
            */
        }

        private void lvList_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            e.DrawDefault = true;           // lvList.OwnerDraw를 True를 했을때는 DrawSubItem에서 Text를 정의하지 않아도 됨. =>  기본 그리기 기능을 사용
        }
    }
}
