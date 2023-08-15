using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProductionManagement
{
    public partial class frmSum : Form
    {
        private frmMain m_frmParent;
        DataManager dm;
        ComModules cm;

        public frmSum()
        {
            InitializeComponent();            
        }
        
        public void SetParent(frmMain m_parent)
        {
            m_frmParent = m_parent;
            dm = m_frmParent.GetDm();
            cm = m_frmParent.GetCm();
        }

        private void frmSum_Load(object sender, EventArgs e)
        {
            dm.DisplaySumList(lvList);
        }

        private void lvList_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            e.DrawDefault = true;
        }

        private void lvList_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            cm.SetAlternatingRowColors(lvList, Color.White, Color.Beige);
            e.Graphics.FillRectangle(Brushes.LightSkyBlue, e.Bounds);
            e.DrawText();

            
        }
    }
}
