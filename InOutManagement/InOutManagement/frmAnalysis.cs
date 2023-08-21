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
    
    public partial class frmAnalysis : Form
    {
        ComModules cm;
        DataManager dm;
        private frmMain m_frmParent;
        private string m_item, m_rack;

        public frmAnalysis()
        {
            InitializeComponent();            
        }

        private void frmAnalysis_Load(object sender, EventArgs e)
        {
            

            dm.AnalysisList(dgvList, m_item, m_rack);
            SetGridHeader();
        }

        public void SetParent(frmMain m_parent, string item, string rack)
        {
            m_frmParent = m_parent;
            dm = m_frmParent.GetDm();
            cm = m_frmParent.GetCm();
            m_item = item;
            m_rack = rack;
        }

        private void SetGridHeader()
        {
            cm.SetGridHeader(dgvList, false, Color.Aqua, Color.Black, Color.LightSkyBlue, Color.Black);

            dgvList.Columns[0].HeaderText = "총입고량";
            dgvList.Columns[0].Width = 130;
            dgvList.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dgvList.Columns[1].HeaderText = "총출고량";
            dgvList.Columns[1].Width = 130;
            dgvList.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dgvList.Columns[2].HeaderText = "현재고량";
            dgvList.Columns[2].Width = 140;
            dgvList.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        }
    }
}
