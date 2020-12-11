using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Vizualizace_Dat
{
    public partial class FormChooseDataLoaders : Form
    {
        public static string loaders = "";

        public FormChooseDataLoaders()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SetLoaders();

            this.Close();
        }

        private void FormChooseDataLoaders_Load(object sender, EventArgs e)
        {

        }

        private void SetLoaders()
        {
            loaders = "";

            if (checkBox1.Checked)
                loaders += "b1";

            if (checkBox2.Checked)
                loaders += "x";

            if (checkBox3.Checked)
                loaders += "j";

            if (checkBox4.Checked)
                loaders += "b2";
        }
    }
}
