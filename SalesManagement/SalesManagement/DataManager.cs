using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;

namespace SalesManagement
{

    class DataManager
    {
        public static OracleConnection m_oracleConn;
        ComModules cm = new ComModules();

        static DataManager()
        {

        }

        // 오라클 DB 연결
        public static bool initDB()
        {
            string strConn = "Data Source=DBSVR19C;User Id=mschoi;Password=mschoi;";
            m_oracleConn = new OracleConnection(strConn);
            try
            {
                m_oracleConn.Open();
            }
            catch (OracleException e)
            {
                MessageBox.Show(e.Message + "\n프로그램을 종료합니다.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        // 제품명 콤보박스 쿼리
        public void CbItem(ComboBox cbItem)
        {
            cbItem.Items.Clear();
            try
            {
                OracleDataReader oracleDR;
                OracleCommand oracleCmd = new OracleCommand("select ITEM_CODE || ' : ' || ITEM_NAME ITEM  from T_ITEMS", m_oracleConn);
                oracleDR = oracleCmd.ExecuteReader();

                while (oracleDR != null && oracleDR.Read())
                {
                    cbItem.Items.Add(oracleDR["ITEM"].ToString());
                }
                oracleDR.Close();
            }
            catch (OracleException e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 판매리스트 쿼리
        public void SalesList(ListView lvSale)
        {
            lvSale.Items.Clear();
            try
            {
                string sSql = "select a.salecode salecode, " +
                              "       a.customer customer, " +
                              "       (select b.item_name " +
                              "          from T_ITEMS b where b.item_code = a.item_code) as item , " +
                              "       a.s_date s_date, a.qty qty, a.price price " +
                              "  from T_SALES a order by a.salecode";
                OracleDataReader oracleDR;
                ListViewItem row;
                OracleCommand oracleCmd = new OracleCommand(sSql, m_oracleConn);
                oracleDR = oracleCmd.ExecuteReader();

                while (oracleDR != null && oracleDR.Read())
                {
                    row = new ListViewItem(oracleDR["SALECODE"].ToString());
                    row.SubItems.Add(oracleDR["CUSTOMER"].ToString());
                    row.SubItems.Add(oracleDR["ITEM"].ToString());
                    row.SubItems.Add(oracleDR["S_DATE"].ToString());
                    row.SubItems.Add(oracleDR["QTY"].ToString());
                    row.SubItems.Add(cm.ToComma(oracleDR["PRICE"].ToString()));

                    lvSale.Items.Add(row);

                }
                oracleDR.Close();
            }
            catch (OracleException e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Insert, Update 쿼리 처리..
        public bool DataProcess(string sSql)
        {
            try
            {
                ExecuteNonQuery(sSql);
                return true;
            }
            catch (OracleException err)
            {
                MessageBox.Show(err.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return false;
        }

        private bool ExecuteNonQuery(string sSql)
        {
            OracleCommand oracleCmd;
            OracleTransaction oracleTran;

            try
            {
                oracleCmd = new OracleCommand(sSql, m_oracleConn);
                oracleTran = m_oracleConn.BeginTransaction();
                oracleCmd.Transaction = oracleTran;
                oracleCmd.ExecuteNonQuery();
                oracleTran.Commit();
            }
            catch (OracleException err)
            {
                MessageBox.Show(err.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        public int ItemPrice(string sItemCode)
        {
            try
            {
                OracleDataReader oracleDR;
                OracleCommand oracleCmd = new OracleCommand("select ITEM_PRICE from T_ITEMS WHERE ITEM_CODE = '" + sItemCode + "'", m_oracleConn);
                oracleDR = oracleCmd.ExecuteReader();

                if (oracleDR != null)
                {
                    oracleDR.Read();
                    int nPrice = int.Parse(oracleDR["ITEM_PRICE"].ToString());
                    oracleDR.Close();
                    return nPrice;
                }

            }
            catch (OracleException e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return -1;
        }

        public string CustomerSales(string chk)
        {
            try
            {
                OracleDataReader oracleDR;
                string sAnalysis = string.Empty;
                if (chk == "C")
                {
                    OracleCommand oracleCmd = new OracleCommand("select customer, sum(price) sum_price from T_SALES group by customer", m_oracleConn);
                    oracleDR = oracleCmd.ExecuteReader();
                    sAnalysis = "고객명 \t 누적판매금액 \r\n";
                    while (oracleDR != null && oracleDR.Read())
                    {
                        string row = string.Format("{0} \t {1} \r\n", oracleDR["customer"].ToString(), oracleDR["sum_price"].ToString());
                        sAnalysis += row;
                    }
                }
                else
                {
                    OracleCommand oracleCmd = new OracleCommand("select (select b.item_name from t_items b where a.item_code = b.item_code) as item_name, sum(a.price) sum_price from T_SALES a group by a.item_code", m_oracleConn);
                    oracleDR = oracleCmd.ExecuteReader();
                    sAnalysis = "제품명 \t 누적판매금액 \r\n";
                    while (oracleDR != null && oracleDR.Read())
                    {
                        string row = string.Format("{0} \t {1} \r\n", oracleDR["item_name"].ToString(), oracleDR["sum_price"].ToString());
                        sAnalysis += row;
                    }
                }
                oracleDR.Close();
                return sAnalysis;
            }
            catch (OracleException e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return "";
        }
    }
}