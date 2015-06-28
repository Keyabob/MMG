using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TradeProxy;

namespace FormTest
{
    public partial class Form1 : Form
    {
        AuTest au = new AuTest();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            au.Test(this.Handle.ToInt32());
            au.GetControls();
            /*
            TestFindForm f = new TestFindForm();
            var hwd = f.Test(this.Handle);
            var controlId = f.FindBuy(new IntPtr(hwd));
            f.EnumChildWindowsCallback(new IntPtr(hwd));
            */
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //au.Test2();
            au.TestBuy();
        }
    }
}
