using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DNT
{
    public partial class Form2 : Form
    {
        protected DataTable Source { get; set; }
        public Form2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var a = new OpenFileDialog();
            a.ShowDialog();
            if(a.CheckFileExists)
            {
                DataSet ds = new DataSet();
                ds.ReadXml(a.FileName);

                try
                {
                    ds.Tables[1].Columns["mid"].ColumnName = "MID";
                    ds.Tables[1].Columns["message_Text"].ColumnName = "Message";
                }
                catch { }
                dataGridView1.DataSource = Source = ds.Tables[1];
            }
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            var lst = Db.Rs("Select Name from Sys.databases");
            comboBox1.Items.Clear();
            foreach (DataRow r in lst.Rows)
            {
                comboBox1.Items.Add(r[0]);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(Source == null)
            {
                MessageBox.Show("Please select xml file!");
                return;
            }
            if(string.IsNullOrWhiteSpace(comboBox1.SelectedItem.ToString()))
            {
                MessageBox.Show("Please select database!");
                return;
            }
            Db.Execute($"use {comboBox1.SelectedItem.ToString()}; IF OBJECT_ID('dbo.UIString', 'U') IS NOT NULL DROP TABLE dbo.UIString;");
            Db.Execute($"use {comboBox1.SelectedItem.ToString()}; create table UIString (MID bigint not null, [Message] nvarchar(max), messages_Id int)");

            Db.Import(Source, "UIString", comboBox1.SelectedItem.ToString());

            MessageBox.Show("Import UI String complete!");
        }
    }
}
