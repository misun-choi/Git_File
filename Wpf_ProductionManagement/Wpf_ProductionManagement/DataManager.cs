using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;

namespace Wpf_ProductionManagement
{
    public class DataManager
    {
        public static OracleConnection m_oracleConn;
        ComModules cm = new ComModules();
        private BindingList<object> itemList = new BindingList<object>();
        private BindingList<object> centerList = new BindingList<object>();

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
                System.Windows.MessageBox.Show(e.Message + "\n프로그램을 종료합니다.", "Error");
                return false;
            }
            return true;
        }

        // 메인 화면의 품목 또는 작업장 콤보 Display
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
                    cb.ItemsSource  = itemList;
                }
                else                        // 작업장 쿼리
                {
                    OracleCommand oracleCmd = new OracleCommand("select CENTER_CODE, CENTER_NAME  from T_WORKCENTER", m_oracleConn);
                    oracleDR = oracleCmd.ExecuteReader();

                    while (oracleDR != null && oracleDR.Read())
                        centerList.Add(new { Text = oracleDR["CENTER_NAME"].ToString(), Value = oracleDR["CENTER_CODE"].ToString() });
                    cb.ItemsSource = centerList;
                }


                cb.DisplayMemberPath  = "Text";
                cb.SelectedValuePath = "Value";

                oracleDR.Close();
            }
            catch (OracleException e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
                MessageBox.Show(err.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return false;
        }

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
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return cnt;
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
                MessageBox.Show(err.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        // 생산계획 리스트 쿼리
        public void ProductionList(ListView lv, string sYM)
        {
            lv.Items.Clear();
            List<ListItems> items = new List<ListItems>();

            try
            {
                string sSql = "select a.pro_code, " +
                              "       (select b.item_name from T_ITEMS b where a.item_code = b.item_code) item, " +
                              "       (select c.center_name from T_WORKCENTER c where a.workcenter_code = c.center_code) center, " +
                              "       a.qty, to_char(a.sdate,'yyyy-mm-dd') sdate, to_char(a.edate,'yyyy-mm-dd') edate " +
                              "  from T_PRODUCTIONS a  " +
                              "where to_char(a.sdate,'yyyymm') = '" + sYM + "'" +
                              " order by a.pro_code";
                OracleDataReader oracleDR;
                OracleCommand oracleCmd = new OracleCommand(sSql, m_oracleConn);
                oracleDR = oracleCmd.ExecuteReader();
                ListItems row;

                while (oracleDR != null && oracleDR.Read())
                {
                    row = new ListItems();
                    row.pro_code = oracleDR["pro_code"].ToString();
                    row.item = oracleDR["item"].ToString();
                    row.center = oracleDR["center"].ToString();
                    row.qty = int.Parse(oracleDR["qty"].ToString());
                    row.sdate = oracleDR["sdate"].ToString();
                    row.edate = oracleDR["edate"].ToString();

                    items.Add(row);
                }
                lv.ItemsSource = null;
                lv.ItemsSource = items;
                oracleDR.Close();
            }
            catch (OracleException e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
