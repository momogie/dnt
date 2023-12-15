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
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public static string CurrentPath = @"C:\";
        public static string CurrentFileName ;
        public static string CurrentFileNames;
        public static DataTable CurrentDataSource;
        public static List<FileChanged> FilesChanged;
        private void Form1_Load(object sender, EventArgs e)
        {
           
            toolStripStatusLabel1.Text = "Rows Count : " + 0;
            toolStripStatusLabel2.Text = "Column Count : " + 0;
            toolStripStatusLabel3.Text = CurrentPath;

            FilesChanged = new List<FileChanged>();

        }

        private void openFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();
            folderBrowserDialog1.ShowDialog();
            if(folderBrowserDialog1.SelectedPath != "")
            {
                CurrentPath = folderBrowserDialog1.SelectedPath;

                toolStripStatusLabel3.Text = CurrentPath;

                foreach (FileSystemInfo file in new DirectoryInfo(CurrentPath).GetFiles("*.dnt"))
                {
                    var k = listView1.Items.Add(file.Name);
                }
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (listView1.SelectedItems.Count > 0)
                {
                    var b = listView1.SelectedItems[0];
                    CurrentFileName = b.Text;
                    CurrentFileNames = toolStripStatusLabel3.Text = Path.Combine(CurrentPath, CurrentFileName);

                    var d = FilesChanged.SingleOrDefault(p => p.FileNames == CurrentFileNames);
                    if(d == null)
                    {
                        var a = new DNTable(CurrentFileNames);
                        dataGridView1.DataSource = a.DataSource;
                        CurrentDataSource = a.DataSource;
                    }
                    else
                    {
                        dataGridView1.DataSource = d.DataSource;
                        CurrentDataSource = d.DataSource;
                    }

                    toolStripStatusLabel1.Text = "Rows Count : " + dataGridView1.Rows.Count;
                    toolStripStatusLabel2.Text = "Column Count : " + dataGridView1.ColumnCount;
                }
               
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public static bool isNumeric(object x)
        {
            if (x == null || x.ToString() == "") return false;
            decimal z;

            return Decimal.TryParse(x.ToString(), out z);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                var a = CurrentDataSource;
                
                if(!string.IsNullOrEmpty(textBox1.Text) && !string.IsNullOrWhiteSpace(textBox1.Text))
                {
                    DataView dv = new DataView(a);
                    dv.RowFilter = textBox1.Text;
                    if(string.IsNullOrEmpty(textBox2.Text) || string.IsNullOrWhiteSpace(textBox2.Text))
                    {
                        dataGridView1.DataSource = dv;
                    }
                    else
                    {
                        var sets = textBox2.Text.Split(',');
                        int i = 0;
                        foreach (var r in sets)
                        {
                            i = 0;
                            var set = r.Split('=');
                            if(set.Length == 2)
                            {
                                var da = a.Select(textBox1.Text);
                                foreach(DataRow d in da)
                                {
                                    var idx = a.Rows.IndexOf(d);
                                    if(isNumeric(set[1]))
                                    {
                                        var type = a.Columns[set[0]].DataType;
                                        if (type == Type.GetType("System.Int32"))
                                        {
                                            a.Rows[idx][set[0]] = Convert.ToInt32(set[1]);
                                        }
                                        if (type == Type.GetType("System.Decimal"))
                                        {
                                            a.Rows[idx][set[0]] = Convert.ToDecimal(set[1]);
                                        }
                                        if (type == Type.GetType("System.Single"))
                                        {
                                            a.Rows[idx][set[0]] = Convert.ToSingle(set[1]);
                                        }
                                        if (type == Type.GetType("System.Boolean"))
                                        {
                                            a.Rows[idx][set[0]] = Convert.ToBoolean(set[1]);
                                        }
                                    }
                                    i++;
                                }
                            }
                            MessageBox.Show("Updated " + i + " Row(s)");
                        }
                        CurrentDataSource = a;

                    }

                }

                dataGridView1.DataSource = a;
                toolStripStatusLabel1.Text = "Rows Count : " + dataGridView1.Rows.Count;
                toolStripStatusLabel2.Text = "Column Count : " + dataGridView1.ColumnCount;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        public class FileChanged
        {
            public string FileNames;
            public string FileName;
            public DataTable DataSource;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                //var a = new DNTable(CurrentFileNames);
                //MessageBox.Show("Rows Count :" + a.RowsCount + " Col : " + a.ColumnsCount);

                //DNT.DNTable.Export(a.DataSource, textBox3.Text + CurrentFileName);
                //MessageBox.Show("Saved!");
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            if(string.IsNullOrEmpty(textBox3.Text) || string.IsNullOrWhiteSpace(textBox3.Text))
            {
                MessageBox.Show("Invalid Save Directory!");
            }
            try
            {
                if(!Directory.Exists(textBox3.Text))
                {
                    Directory.CreateDirectory(textBox3.Text);
                }
                foreach(var r in FilesChanged)
                {
                    DNTable.Export(r.DataSource, Path.Combine(textBox3.Text,r.FileName));
                }
                MessageBox.Show("Save Complete!");
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            folderBrowserDialog1.ShowDialog();
            textBox3.Text = folderBrowserDialog1.SelectedPath;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            updateCurrentDataSource();
        }
        void updateCurrentDataSource()
        {
            var d = FilesChanged.SingleOrDefault(p => p.FileNames == CurrentFileNames);
            if (d == null)
            {
                FilesChanged.Add(new FileChanged
                {
                    FileName = CurrentFileName,
                    FileNames = CurrentFileNames,
                    DataSource = CurrentDataSource
                });
            }
            else
            {
                d.DataSource = CurrentDataSource;
            }
        }

        private void importToSqlServerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var d = new Import();
            d.ShowDialog();
           
        }

        private void exportFromSqlServerToolStripMenuItem_Click(object sender, EventArgs e)
        {

            var d = new Export();
            d.ShowDialog();

        }

        private void xMLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var d = new Form2();
            d.ShowDialog();
        }
    }
}
