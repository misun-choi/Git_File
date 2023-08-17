using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Wpf_ProductionManagement
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        DataManager dm = new DataManager();
        ComModules cm = new ComModules();

        public MainWindow()
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
            lblProCode.Content = "";
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
                if (lvList.SelectedItems.Count > 0)
                {
                    if (MessageBox.Show("생산계획을 삭제하시겠습니까?.", "알림", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        string sProCode = lblProCode.Content.ToString();
                        string sSql = String.Format("delete from T_PRODUCTIONS where pro_code = '{0}'", sProCode);
                        dm.DataProcess(sSql);
                        dm.ProductionList(lvList, cbYear.Text + string.Format("{0:D2}", int.Parse(cbMonth.Text)));
                        //statusStrip.value = "생산계획 삭제가 정상 처리 되었습니다.";
                        //timer.Start();
                    }
                }
                else
                {
                    MessageBox.Show("삭제 할 생산계획이 선택되지 않았습니다.", "알림", MessageBoxButton.OK, MessageBoxImage.Stop);
                    return;
                }
            };

            // 생산계획 등록 버튼 클릭시 처리....
            btnAdd.Click += (sender, e) =>
            {
                string sDateTime = dtSdate.Text("yyyy-MM-dd HH:mm:ss");
                string sSql = "select count(*) cnt from T_PRODUCTIONS a where a.item_code = '" +
                                  cbItem.SelectedValue.ToString() + "'  and a.workcenter_code = '" + cbWorkCenter.SelectedValue.ToString() + "'" +
                                  " and to_char(a.sdate, 'yyyy-mm-dd') = '" + sDateTime.Substring(0, 10) + "'";
                if (dm.ExistCheck(sSql) > 0)
                {
                    if (MessageBox.Show("생산계획이 존재합니다..계속하시겠습니까?", "알림", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
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
                        //statusStrip.Items[0].Text = "생산계획이 정상 등록 되었습니다.";
                        //timer.Start();
                    }
                }
            };
        }

        private bool InputChk()
        {
            if (cbItem.SelectedIndex < 0)
            {
                MessageBox.Show("품목이 선택되지 않았습니다.", "알림", MessageBoxButton.OK, MessageBoxImage.Stop);
                cbItem.Focus();
                return false;
            }

            if (cbWorkCenter.SelectedIndex < 0)
            {
                MessageBox.Show("작업장이 선택되지 않았습니다.", "알림", MessageBoxButton.OK, MessageBoxImage.Stop);
                cbWorkCenter.Focus();
                return false;
            }

            return true;
        }
        private void DateTimePickerFormatChange(DatePicker dt, string s)
        {
            /*
            if (s == "")
            {
                dt.Text = DateTime.Now;
                dt..CustomFormat = " ";
                dt.Format = DatePickerFormat.Custom;
            }
            else
            {
                dt.CustomFormat = "yyyy년 MM월 dd일 dddd";
                dt.Text = DateTime.Parse(s);
            }
            */
        }
        private void btnPlanQuery_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
