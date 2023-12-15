using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
namespace DNT
{
    public partial class Export : Form
    {
        public Export()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var d = new FolderBrowserDialog();
            d.ShowDialog();
            if(d.SelectedPath!="")
            {
                textBox1.Text = d.SelectedPath;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void Export_Load(object sender, EventArgs e)
        {
            var lst = Db.Rs("Select Name from Sys.databases");
            comboBox1.Items.Clear();
            foreach(DataRow r in lst.Rows)
            {
                comboBox1.Items.Add(r[0]);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
           if(comboBox1.SelectedItem !="")
            {
                var lst = Db.Rs("Use [" + comboBox1.SelectedItem + "];Select Name From sys.Objects where Type='U' Order by Name asc");
                listBox1.Items.Clear();
                foreach (DataRow r in lst.Rows)
                {
                    listBox1.Items.Add(r[0]);
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if(textBox1.Text != "" && comboBox1.SelectedItem != "")
            {
                button3.Enabled = button1.Enabled = comboBox1.Enabled = textBox1.Enabled = false;

                var lst = Db.Rs("Use [" + comboBox1.SelectedItem + "];Select Name From sys.Objects where Type='U' Order by Name asc");
                foreach (DataRow r in lst.Rows)
                {
                    var da = Db.Rs("Use [" + comboBox1.SelectedItem + "];Select * from " + r[0]);
                    DNTable.Export(da, Path.Combine(textBox1.Text,r[0].ToString() + ".dnt"));
                }
                MessageBox.Show("Export Complete!");
               button3.Enabled = button1.Enabled = comboBox1.Enabled = textBox1.Enabled = true;
            }
        }
    }
}
