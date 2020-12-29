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
        public static string loaders = ApkConfig.Loaders;

        public FormChooseDataLoaders()
        {
            InitializeComponent();

            ChechUncheckLoaders();
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

            ApkConfig.Loaders = loaders;
        }

        private void ChechUncheckLoaders()
        {
            if (loaders.Contains("b1"))
                checkBox1.Checked = true;
            else
                checkBox1.Checked = false;

            if (loaders.Contains("x"))
                checkBox2.Checked = true;
            else
                checkBox2.Checked = false;

            if (loaders.Contains("j"))
                checkBox3.Checked = true;
            else
                checkBox3.Checked = false;

            if (loaders.Contains("b2"))
                checkBox4.Checked = true;
            else
                checkBox4.Checked = false;
        }
    }
}
