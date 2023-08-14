using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;
namespace SalesManagement
{
    public partial class frmMain : Form
    {
        DataManager dm = new DataManager();
        ComModules cm = new ComModules();

        public frmMain()
        {
            InitializeComponent();

            if (!DataManager.initDB())
                Environment.Exit(0);    // 프로그램 시작도 하지 말고 종료             

            dm.CbItem(cbItem);
            dm.SalesList(lvSale);
            statusStrip.Items[0].Text = "";
        }


        // 매출 등록
        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (txtCustomer .Text == "")
            {
                MessageBox.Show("고객명이 입력되지 않았습니다..", "알림", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                txtCustomer.Focus();
                return;
            }
            string sDateTime = dtDate.Value.ToString("yyyy-MM-dd HH:mm:ss");
            string sItem = cbItem.Text.Substring(0, 8);
            int sPrice = int.Parse(txtPrice.Text.ToString().Replace(",", ""));
            string sSql = String.Format("insert into T_SALES(CUSTOMER, ITEM_CODE, S_DATE, QTY, PRICE) values ('{0}','{1}',to_date('{2}','yyyy-mm-dd hh24:mi:ss'),{3},{4})",
                                        txtCustomer.Text, sItem, sDateTime, nupQty.Value, sPrice);

            if (dm.DataProcess(sSql))
            {
                dm.SalesList(lvSale);
                statusStrip.Items[0].Text = "매출 등록이 정상 처리 되었습니다.";
                timer.Start();
                ScreenClear();
            }
        }

        // 매출 삭제
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (lvSale.SelectedItems.Count == 0)
                MessageBox.Show("삭제 할 데이타가 선택되지 않았습니다..", "알림", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            else
            {
                string sSaleCode = lvSale.SelectedItems[0].Text;
                DialogResult re = MessageBox.Show("삭제 하시겠습니까?", "알림", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (re == DialogResult.Yes)
                {
                    string sSql = String.Format("delete from T_SALES where SALECODE = '{0}'", sSaleCode);
                    if (dm.DataProcess(sSql))
                    {
                        dm.SalesList(lvSale);
                        statusStrip.Items[0].Text = "매출 삭제가 정상 처리 되었습니다.";
                        timer.Start();

                        ScreenClear();
                    }
                }
            }
            
        }

        // 고객별 판매금액 분석
        private void btnCustomerSales_Click(object sender, EventArgs e)
        {
            txtAnalysis.Text = dm.CustomerSales("C");
        }

        // 제품별 판매금액 분석
        private void btnItemSales_Click(object sender, EventArgs e)
        {
            txtAnalysis.Text = dm.CustomerSales("I");
        }

        private void txtCustomer_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
                cbItem.Focus();
        }

        private void cbItem_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
                dtDate.Focus();
        }

        private void dtDate_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
                nupQty.Focus();
        }
        private void nupQty_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
                txtPrice.Focus();
        }

        private void txtPrice_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
                btnAdd.Focus();
        }

        private void txtPrice_TextChanged(object sender, EventArgs e)
        {
            txtPrice.Text = cm.ToComma(txtPrice.Text);           // 금액 단위로 받아오기
        }

        private void nupQty_ValueChanged(object sender, EventArgs e)
        {
            if (cbItem.Text != "")
            { 
                int nPrice = dm.ItemPrice(cbItem.Text.Substring(0, 8)) * int.Parse(nupQty.Value.ToString());
                txtPrice.Text = nPrice.ToString();
            }                      
        }

        private void nupQty_Click(object sender, EventArgs e)
        {
            if (cbItem.Text == "")
            {
                MessageBox.Show("제품을 먼저 선택하세요!", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
                nupQty.Value = 0;
            }
        }

        private void ScreenClear()          // 화면 클리어
        {
            txtCustomer.Text = "";
            cbItem.SelectedIndex = -1;
            dtDate.Value = DateTime.Now;
            nupQty.Value = 0;
            txtPrice.Text = "0";
        }

        // status message는 3초 후 삭제
        private void timer_Tick(object sender, EventArgs e)
        {
            statusStrip.Items[0].Text = "";
            timer.Stop();
        }

        private void cbItem_SelectedIndexChanged(object sender, EventArgs e)
        {
            nupQty.Value = 0;
            txtPrice.Text = "0";
        }
    }
}
