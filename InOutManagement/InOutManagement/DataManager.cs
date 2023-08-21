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

namespace InOutManagement
{
    public class DataManager
    {
        public static OracleConnection m_oracleConn;
        ComModules cm = new ComModules();
        private BindingList<object> itemList = new BindingList<object>();
        private BindingList<object> rackrList = new BindingList<object>();
        public List<Inouts> Inouts = new List<Inouts>();

        #region  initDB() : DB Connection
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
        #endregion 

        #region SelComboData(string s, ComboBox cb) : 메인 화면의 품목 또는 작업장 콤보 Display
        public void SelComboData(string s, ComboBox cb)
        {
            cb.Items.Clear();
            try
            {
                OracleDataReader oracleDR;
                if (s == "Item")            // 품목 쿼리
                {
                    OracleCommand oracleCmd = new OracleCommand("select ITEM_CODE, ITEM_NAME  from T_ITEMS", m_oracleConn);
                    oracleDR = oracleCmd.ExecuteReader();

                    while (oracleDR != null && oracleDR.Read())
                        itemList.Add(new { Text = oracleDR["ITEM_NAME"].ToString(), Value = oracleDR["ITEM_CODE"].ToString() });
                    itemList.Insert(0, new { Text = "전체", Value = "All" });
                    cb.DataSource = itemList;
                }
                else                        // 작업장 쿼리
                {
                    OracleCommand oracleCmd = new OracleCommand("select RACK_CODE, RACK_NAME  from T_RACKS", m_oracleConn);
                    oracleDR = oracleCmd.ExecuteReader();

                    while (oracleDR != null && oracleDR.Read())
                        rackrList.Add(new { Text = oracleDR["RACK_NAME"].ToString(), Value = oracleDR["RACK_CODE"].ToString() });
                    rackrList.Insert(0, new { Text = "전체", Value = "All" });
                    cb.DataSource = rackrList;
                }
                
                
                cb.DisplayMember = "Text";
                cb.ValueMember = "Value";
                //string typeValue = cb.SelectValue.ToString();
                oracleDR.Close();
            }
            catch (OracleException e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion 

        #region DataProcess(string sSql) : Insert, Update, Delete시  처리..
        // 
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
        #endregion 

        #region InOutList(DataGridView lv) : 테이블 T_INOUTS(창고 입출고)에 있는 정보를 DataGridView에 Listing...
        public void InOutList(DataGridView lv)
        {
            Inouts.Clear();
            Inouts inout;
            try
            {
                string sSql = "select a.inout_code, " +
                                        " (select b.rack_name from t_racks b where a.rack_code = b.rack_code) rack_name, " +
                                        " (select b.item_name from t_items b where a.item_code = b.item_code) item_name," +
                                        " to_char(a.in_date, 'yyyy-mm-dd') in_date, a.in_qty, to_char(a.out_date, 'yyyy-mm-dd') out_date, decode(a.out_qty,null,0,a.out_qty) out_qty, " +
                                        " decode(a.in_qty,null,0,a.in_qty) - decode(a.out_qty,null,0,a.out_qty) remain" +
                                "  from T_INOUTS a " +
                                "order by a.inout_code";

                OracleDataReader oracleDR;
                OracleCommand oracleCmd = new OracleCommand(sSql, m_oracleConn);
                oracleDR = oracleCmd.ExecuteReader();
                
                while (oracleDR != null && oracleDR.Read())
                {
                    inout = new Inouts();
                    inout.inout_code = int.Parse(oracleDR["inout_code"].ToString());
                    inout.rack_name = oracleDR["rack_name"].ToString();
                    inout.item_name = oracleDR["item_name"].ToString();
                    inout.in_date = oracleDR["in_date"].ToString();
                    inout.in_qty = cm.ToComma(oracleDR["in_qty"].ToString());
                    inout.out_date = oracleDR["out_date"].ToString();
                    inout.out_qty = cm.ToComma(oracleDR["out_qty"].ToString());
                    inout.remain = cm.ToComma(oracleDR["remain"].ToString());

                    Inouts.Add(inout);
                }
                lv.DataSource = Inouts;
                oracleDR.Close();                
            }
            catch (OracleException e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion 

        #region AnalysisList(DataGridView lv) : 테이블 T_INOUTS(창고 입출고)에 있는 정보를 DataGridView에 summary하여 Listing...
        public void AnalysisList(DataGridView lv, string item, string rack)
        {
            try
            {
                string sSql = string.Format(" select nvl(sum(a.in_qty),0) in_qty, nvl(sum(a.out_qty),0) out_qty " +
                                                    "  from t_inouts a " +
                                                    " where a.item_code like '%{0}' " +
                                                    "    and a.rack_code like '%{1}'", item, rack);

                OracleDataReader oracleDR;
                OracleCommand oracleCmd = new OracleCommand(sSql, m_oracleConn);
                oracleDR = oracleCmd.ExecuteReader();                
                if  (oracleDR != null)
                {
                    oracleDR.Read();
                    int remain = int.Parse(oracleDR["in_qty"].ToString()) - int.Parse(oracleDR["out_qty"].ToString());
                    lv.Rows.Add(cm.ToComma(oracleDR["in_qty"].ToString()),
                                    cm.ToComma(oracleDR["out_qty"].ToString()),
                                    cm.ToComma(remain.ToString()));                   
                }
               
                oracleDR.Close();
            }
            catch (OracleException e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion 

        public int ExistCheck(string sSql)
        {
            int cnt = 0;
            try
            {
                OracleDataReader oracleDR;
                OracleCommand oracleCmd = new OracleCommand(sSql, m_oracleConn);
                oracleDR = oracleCmd.ExecuteReader();

                if (oracleDR != null)
                {
                    oracleDR.Read();
                    cnt = int.Parse(oracleDR["cnt"].ToString());
                }
                oracleDR.Close();
                return cnt;
            }
            catch (OracleException e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return cnt;
        }

        public void DisplaySumList(ListView lv)
        {
            lv.Items.Clear();
            try
            {
                string sSql = "select (select c.item_name from t_items c where a.item_code = c.item_code) item, " + "        sum(b.qty) planQty, " + "        sum(decode(b.edate,null,0,b.qty)) completeQty, " + "        round(100 * (sum(decode(b.edate,null,0,b.qty)) / sum(b.qty)),3) progress " + "  from (select distinct(t.item_code)  from T_PRODUCTIONS t) a, " + "         T_PRODUCTIONS b " + " where a.item_code = b.item_code " + "group by a.item_code";
                OracleDataReader oracleDR;
                ListViewItem row;
                OracleCommand oracleCmd = new OracleCommand(sSql, m_oracleConn);
                oracleDR = oracleCmd.ExecuteReader();
                                
                while (oracleDR != null && oracleDR.Read())
                {
                    row = new ListViewItem(oracleDR["item"].ToString());
                    row.SubItems.Add(oracleDR["planQty"].ToString());
                    row.SubItems.Add(oracleDR["completeQty"].ToString());
                    string rate = string.Format("{0:0.000}", double.Parse(oracleDR["progress"].ToString()));
                    row.SubItems.Add(rate);
                   if (double.Parse(oracleDR["progress"].ToString()) < 50)     // 진척율이 50% 이하는 글씨색을 빨간색으로..
                    {
                        row.UseItemStyleForSubItems = false;
                        row.SubItems[0].ForeColor = Color.Red;
                        row.UseItemStyleForSubItems = true;
                    }
                    lv.Items.Add(row);                 
                }
                oracleDR.Close();
            }
            catch (OracleException e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #region ExecuteNonQuery(string sSql) : 오라클 명령어 실행. insert, update시 사용..
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
        #endregion 
    }
}
