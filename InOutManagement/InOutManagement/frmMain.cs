using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InOutManagement
{
    public partial class frmMain : Form
    {
        DataManager dm = new DataManager();
        ComModules cm = new ComModules();

        Inouts ls = new Inouts();
        public string g_sIn_qty, g_Inout_code;

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

        #region InitGetData() : 콤보 데이타 쿼리해 옴 (품목, 작업장콤보) ... 리스트의 해더 Setting...
        private void InitGetData()
        {
            dm.SelComboData("Item", cbItem);                                      // 품목 콤보
            dm.SelComboData("Rack", cbRack);                                      // 작업장 콤보            

            InitForm();
            dgvList.ClearSelection();
        }
        #endregion

        #region InitForm() => Form 초기화 후 창고입출고정보 Listing...
        private void InitForm()
        {
            g_sIn_qty = string.Empty;
            g_Inout_code = string.Empty;

            cbItem.SelectedIndex = 0;
            cbRack.SelectedIndex = 0;
            txtQty.Text = "";
            dtDate.Value = DateTime.Now;

            dgvList.CurrentCell = null;
            dgvList.ClearSelection();
            dgvList.DataSource = null;

            dm.InOutList(dgvList);                                                                     // 창고입출고정보 Listing...

            SetGridHeader();                                                                           // DataGridView 해더 초기화...
            cm.SetGridAlternatingRowColor(dgvList,  Color.LightPink);                      // Row색상 번갈아 변경
            //SetImageInCell();
        }
        #endregion

        #region SetGridHeader() : DataGridView 해더 초기화...
        private void SetGridHeader()
        {
            cm.SetGridHeader(dgvList, true, Color.DarkGray, Color.Black, Color.LightSkyBlue, Color.Black);

            dgvList.Columns[0].HeaderText = "입출고번호";
            dgvList.Columns[0].Width = 100;
            dgvList.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
 
            dgvList.Columns[1].HeaderText = "창고명";
            dgvList.Columns[1].Width = 200;
            dgvList.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dgvList.Columns[2].HeaderText = "품목";
            dgvList.Columns[2].Width = 200;
            dgvList.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
 
            dgvList.Columns[3].HeaderText = "입고일자";
            dgvList.Columns[3].Width = 115;
            dgvList.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
 
            dgvList.Columns[4].HeaderText = "입고수량";
            dgvList.Columns[4].Width = 90;
            dgvList.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dgvList.Columns[5].HeaderText = "출고일자";
            dgvList.Columns[5].Width = 115;
            dgvList.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
 
            dgvList.Columns[6].HeaderText = "출고수량";
            dgvList.Columns[6].Width = 90;
            dgvList.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dgvList.Columns[7].HeaderText = "남은수량";
            dgvList.Columns[7].Width = 100;
            dgvList.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        }
        #endregion 

        #region ButtonEvent() : 버튼처리...
        private void ButtonEvent()
        {
            // 입고등록 버튼 클릭시 처리,,,,
            btnInput.Click += (sender, e) =>
            {
                string sDateTime = dtDate.Value.ToString("yyyy-MM-dd HH:mm:ss");
                string sSql = "select count(*) cnt from T_INOUTS a where a.item_code = '" +
                                  cbItem.SelectedValue.ToString() + "'  and a.rack_code = '" + cbRack.SelectedValue.ToString() + "'" +
                                  " and to_char(a.in_date, 'yyyy-mm-dd') = '" + sDateTime.Substring(0, 10) + "'";
                if (dm.ExistCheck(sSql) > 0)
                {
                    if (MessageBox.Show("입고 된 것이 있습니다..계속하시겠습니까?", "알림", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                        return;
                }
                
                if (!InputChk())
                    return;

                sSql = String.Format("insert into T_INOUTS(item_code, rack_code, in_qty, in_date) values ('{0}','{1}',{2}, to_date('{3}','yyyy-mm-dd hh24:mi:ss'))",
                                    cbItem.SelectedValue.ToString(), cbRack.SelectedValue.ToString(), int.Parse(txtQty.Text.Replace(",", "")), sDateTime);

                if (dm.DataProcess(sSql))
                {
                    InitForm();
                    statusStrip.Items[0].Text = "생산계획이 정상 등록 되었습니다.";
                    timer.Start();
                }
            };

            // 출고등록 버튼 클릭시 처리,,,,
            btnOutput.Click += (sender, e) =>
            {
                string sDateTime = dtDate.Value.ToString("yyyy-MM-dd HH:mm:ss");
                string sSql;
                if (int.Parse(g_sIn_qty.Replace(",", "")) < int.Parse(txtQty.Text.Replace(",", "")))
                {
                    MessageBox.Show("입고수량보다 출고수량이 많습니다. 다시 확인하세요", "알림", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtQty.Focus();
                    return;
                }

                if (MessageBox.Show(cbItem.Text + " " + txtQty .Text + "개를 출고 등록 하시겠습니까?", "알림", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (!InputChk())
                        return;

                    sSql = String.Format("update T_INOUTS set out_qty = {0}, out_date = to_date('{1}','yyyy-mm-dd hh24:mi:ss')  where inout_code = {2} ",
                                    txtQty.Text.Replace(",", ""), sDateTime, g_Inout_code);

                    if (dm.DataProcess(sSql))
                    {
                        InitForm();
                        statusStrip.Items[0].Text = "출고수량이 정상 등록 되었습니다.";
                        timer.Start();
                    }
                }
                //dm.ProductionList(lvList, cbYear.Text + string.Format("{0:D2}", int.Parse(cbMonth.Text)));
            };

            // 삭제 버튼 클릭시 처리....
            btnDelete.Click += (sender, e) =>
            {
                string sSql;
                if (MessageBox.Show(cbItem.Text + "을/를 삭제 하시겠습니까?", "알림", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    sSql = string.Format("delete from T_INOUTS where inout_code ={0}", g_Inout_code);
                    if (dm.DataProcess(sSql))
                    {
                        InitForm();
                        statusStrip.Items[0].Text = "삭제가 정상 처리 되었습니다.";
                        timer.Start();
                    }
                }

            };

            // 생산계획 등록 버튼 클릭시 처리....
            btnAnalysis.Click += (sender, e) =>
            {
                string item, rack;
                frmAnalysis frmSub = new frmAnalysis();
                frmSub.Location = new Point(this.Location.X +btnAnalysis.Left, btnAnalysis.Height + this.Location.Y +dgvList .Top);
                frmSub.StartPosition = FormStartPosition.Manual;
                if (cbItem.SelectedValue.ToString() == "All")   item = "";
                else item = cbItem.SelectedValue.ToString();
                if (cbRack.SelectedValue.ToString() == "All")   rack = "";
                else rack = cbRack.SelectedValue.ToString();
                frmSub.SetParent(this, item, rack);
                frmSub.ShowDialog();
            };
            #endregion 
        }
        #region InputChk() : 필요한 항목의 입력 여부 체크
        private bool InputChk()
        {
            if (cbItem.SelectedIndex < 0)
            {
                MessageBox.Show("품목이 선택되지 않았습니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                cbItem.Focus();
                return false;
            }

            if (cbRack.SelectedIndex < 0)
            {
                MessageBox.Show("창고가 선택되지 않았습니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                cbRack.Focus();
                return false;
            }

            if (txtQty.Text  == "" ||  txtQty.Text == "0")
            {
                MessageBox.Show("수량이 입력되지 않았습니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                txtQty.Focus();
                return false;
            }

            return true;
        }
        #endregion 

        private void timer_Tick(object sender, EventArgs e)
        {
            statusStrip.Items[0].Text = "";
            timer.Stop();
        }

        private void dgvList_CurrentCellChanged(object sender, EventArgs e)
        {
            if (dgvList.CurrentRow != null)
            {
                // 그리드의 셀이 선택되면 텍스트박스에 글자 지정
                Inouts inout = dgvList.CurrentRow.DataBoundItem as Inouts;

                g_Inout_code = inout.inout_code.ToString();
                cbItem.Text = inout.item_name;
                cbRack.Text = inout.rack_name;
                g_sIn_qty = inout.in_qty;
            }
        }

        private void txtQty_TextChanged(object sender, EventArgs e)
        {
            txtQty.Text = cm.ToComma(txtQty.Text);
            txtQty.SelectionStart = txtQty.TextLength;
            txtQty.SelectionLength = 0;
        }

        #region dgvList_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e) : 남은수량이 0이면 row 색상을 빨간색으로 빠꾼다.
        private void dgvList_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvList.Rows[e.RowIndex].Cells["remain"].Value.ToString() == "0")
            {
                e.CellStyle.BackColor = Color.Red;
                e.CellStyle.ForeColor = Color.White;
            }
        }
        #endregion

        //텍스트박스에서 숫자와 백스페이스 키만 처리 되게..
        private void txtQty_KeyPress(object sender, KeyPressEventArgs e)
        {
            cm.Key_Press(sender, e);
        }

        // Row Header에 Numbering...
        private void dgvList_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            Image _image = Properties.Resources.fileImg2;
            
            using (SolidBrush b = new SolidBrush(dgvList.RowHeadersDefaultCellStyle.ForeColor))
            {
                StringFormat sf = new StringFormat();
                sf.Alignment = StringAlignment.Center;
                e.Graphics.DrawString((e.RowIndex + 1).ToString(), e.InheritedRowStyle.Font, b, e.RowBounds.Location.X + 11, e.RowBounds.Location.Y + 4, sf);
             // PointF ulCorner = new PointF(100.0F, 100.0F);
             // e.Graphics.DrawImage(_image, ulCorner);
            }
        }

        // Cell에 이미지 넣기
        private void SetImageInCell()
        {
            Image _image = Properties.Resources.fileImg2;
            //Icon treeIcon = new Icon(this.GetType(), "tree.ico");
            DataGridViewImageColumn iconColumn = new DataGridViewImageColumn();
            iconColumn.Image = _image;
            iconColumn.Name = "I";
            iconColumn.HeaderText = "";
            iconColumn.Width = 20;
            dgvList.Columns.Insert(0, iconColumn);
        }
        // DataGridView Header Sort 처리...
        private void dgvList_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            DataGridViewColumn column = dgvList.Columns[e.ColumnIndex];
            string columnName = column.Name;
            SortOrder sortOrder = column.HeaderCell.SortGlyphDirection == SortOrder.Ascending
                                      ? SortOrder.Descending
                                      : SortOrder.Ascending;
            dm.Inouts.Sort(new SortingProcess(columnName, sortOrder));
            dgvList.Refresh();
            column.HeaderCell.SortGlyphDirection = sortOrder;
        }
    }
}