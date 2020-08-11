using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace piform
{
    public partial class Form2 : Form
    {
        private Form1 frm = null;
        public Form2(Form1 frm)
        {
            InitializeComponent();
            this.frm = frm;

            if (frm.carList[0].carNumber.Contains(frm.search))
                pictureBox1.Visible = true;
            else
                pictureBox1.Visible = false;

            if (frm.carList[1].carNumber.Contains(frm.search))
                pictureBox2.Visible = true;
            else
                pictureBox2.Visible = false;

            if (frm.carList[2].carNumber.Contains(frm.search))
                pictureBox3.Visible = true;
            else
                pictureBox3.Visible = false;

        }
    }
}
