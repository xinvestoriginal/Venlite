using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VenLight
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("ВСЕ ИЗМЕНЕНИЯ ВСТУПЯТ В СИЛУ ПРИ СЛЕДУЮЩЕМ ЗАПУСКЕ ПРОГРАММЫ");
            Close();
        }

      

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
