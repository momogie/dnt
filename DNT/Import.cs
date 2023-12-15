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
    public partial class Import : Form
    {
        public Import()
        {
            InitializeComponent();
            textBox2.Text = "DNT_" + DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var a = new FolderBrowserDialog();
            a.ShowDialog();
            textBox1.Enabled = true;

            if (a.SelectedPath != "")
            {
                textBox1.Text = a.SelectedPath;
                textBox1.Enabled = false;
                foreach (FileSystemInfo file in new DirectoryInfo(textBox1.Text).GetFiles("*.dnt"))
                {
                    var k = listBox1.Items.Add(file.Name);
                }
            }
        }
        public static string CreateTable(DataTable Data, string TableName, string DBName)
        {
            string str1 = "Use " + DBName + "; Create Table " + TableName + " ( ID int ,";
            for (int index = 1; index < Data.Columns.Count; ++index)
            {
                string str2 = "";
                switch (Data.Columns[index].DataType.ToString())
                {
                    case "System.String":
                        str2 = "nvarchar(max)";
                        break;
                    case "System.Boolean":
                        str2 = "bit";
                        break;
                    case "System.Int32":
                        str2 = "int";
                        break;
                    case "System.Decimal":
                        str2 = "money";
                        break;
                    case "System.Single":
                        str2 = "float";
                        break;
                }
                str1 = str1 + " " + Data.Columns[index].ColumnName + " " + str2;
                if (index < Data.Columns.Count - 1)
                    str1 += ", ";
            }
            return str1 + " )";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(textBox2.Text != "")
            {
                string DBName = textBox2.Text;
                Db.Execute("If(db_id(N'" + DBName + "') IS NOT NULL) DROP DATABASE " + DBName + " ");
                Db.Execute("If(db_id(N'" + DBName + "') IS NULL) CREATE DATABASE " + DBName + " ");
                foreach (FileSystemInfo file in new DirectoryInfo(textBox1.Text).GetFiles("*.dnt"))
                {
                    var d = new DNTable(Path.Combine(textBox1.Text, file.Name));
                    string TableName = file.Name.Replace(".dnt", "");

                    string query = CreateTable(d.DataSource, TableName, DBName);
                    Db.Execute(query);
                    Db.Import(d.DataSource, TableName, DBName);

                }
                MessageBox.Show("Import Complete!");
            }
            
        }
    }
}
