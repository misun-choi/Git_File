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
    }
}
