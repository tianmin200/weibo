using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WeiboManager
{
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();
            FrmColTask frmcoltask = new FrmColTask();
            frmcoltask.Show(this.dockPanel_Main);
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            
        }
    }
}
