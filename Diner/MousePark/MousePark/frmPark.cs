using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MousePark
{
    public partial class frmPark : Form
    {
        private bool isParked = false;
        public frmPark()
        {
            InitializeComponent();
        }

        private void frmPark_MouseEnter(object sender, EventArgs e)
        {
           
        }

        private void frmPark_MouseLeave(object sender, EventArgs e)
        {
            if (isParked)
                Application.Exit();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            SendKeys.Send("{F15}");
        }

        private void frmPark_Load(object sender, EventArgs e)
        {
            timer1.Enabled = false;

        }

        private void frmPark_Click(object sender, EventArgs e)
        {
            isParked = true;
            this.BackColor = Color.Yellow;
            if (!timer1.Enabled)
            {
                timer1.Interval = 60000;
                timer1.Enabled = true;
            }
        }
    }
}
