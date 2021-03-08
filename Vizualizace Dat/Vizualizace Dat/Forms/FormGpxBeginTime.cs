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
    public partial class FormGpxBeginTime : Form
    {
        public static DateTime BeginTime { get; set; }

        public FormGpxBeginTime(string labelText, string buttonText)
        {
            InitializeComponent();

            lText.Text = labelText;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;

            dateTimePicker1.Format = DateTimePickerFormat.Custom;
            dateTimePicker1.CustomFormat = "dd.MM.yyyy HH:mm:ss";

            if(buttonText != null && buttonText.Length > 0)
            {
                button1.Text = buttonText;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(dateTimePicker1.Value < DateTime.Now.AddMinutes(-5) || dateTimePicker1.Value > DateTime.Now.AddDays(7))
            {
                throw new Exception("Zvolené čas musí být větší než je aktuální čas a zároveň nesmí přesahovat hranici týdne dopředu.");
                this.Close();
            }

            BeginTime = dateTimePicker1.Value;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void FormGpxBeginTime_Load(object sender, EventArgs e)
        {

        }
    }
}
