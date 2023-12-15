using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Net;
using System.Windows.Forms;
using System.IO;

namespace DNT
{
    class DNTable
    {
        public DataTable DataSource;
        public int RowsCount;
        public int ColumnsCount;
        public DNTable(string FileNames)
        {
            DataSource = new DataTable();

            BinaryReader Binary = new BinaryReader(File.Open(FileNames, FileMode.Open));
            Binary.ReadBytes(4);

            ColumnsCount = Binary.ReadInt16();
            RowsCount = Binary.ReadInt32();

            // Read Columns
            DataSource.Columns.Add(new DataColumn { ColumnName="ID",DataType = Type.GetType("System.Int32")});
            for (int i=0;i< ColumnsCount; i++)
            {
                
                DataColumn Column = new DataColumn();
                Column.ColumnName = GetStr(Binary.ReadBytes(Binary.ReadInt16()));
                switch(Binary.ReadByte())
                {
                    case 1:
                        Column.DataType = Type.GetType("System.String");
                        break;
                    case 2:
                        Column.DataType = Type.GetType("System.Boolean");
                        break;
                    case 3:
                        Column.DataType = Type.GetType("System.Int32");
                        break;
                    case 4:
                        Column.DataType = Type.GetType("System.Decimal");
                        break;
                    case 5:
                        Column.DataType = Type.GetType("System.Single");
                        break;

                }

                DataSource.Columns.Add(Column);
            }

            // Read Rows
            for (int a=0;a<RowsCount;a++)
            {
                DataRow Row = DataSource.NewRow();
                Row[0] = Binary.ReadInt32();
                for (int i = 1; i <= ColumnsCount; i++)
                { 
                    switch (DataSource.Columns[i].DataType.ToString())
                    {
                        case "System.String":
                            Row[i] = GetStr(Binary.ReadBytes(Binary.ReadInt16()));
                            break;
                        case "System.Boolean":
                            Row[i] = Binary.ReadInt32();
                            break;
                        case "System.Int32":
                            Row[i] = Binary.ReadInt32();
                            break;
                        case "System.Decimal":
                            Row[i] = Binary.ReadSingle();
                            break;
                        case "System.Single":
                            Row[i] = Binary.ReadSingle();
                            break;
                    }
                }
                DataSource.Rows.Add(Row);

                //if (DataSource.Rows.Count == 2005) break;
            }

            Binary.Close();
        }

        public static string GetStr(byte[] bytes)
        {
            return Encoding.ASCII.GetString(bytes);
        }

        public static void Export(DataTable Data,string FileNames)
        {

            var colstype = new string[] { "System.String", "System.Boolean", "System.Int32", "System.Decimal", "System.Single", "System.Double" };
            

            

            int RowsCount = Data.Rows.Count;
            //MessageBox.Show("Rows Count :" + Data.Rows.Count + " Col : " + Data.Columns.Count);

            var a = File.Create(FileNames);
            a.Write(new byte[4] { 0,0,0,0},0,4);

            a.Write(BitConverter.GetBytes(Data.Columns.Count - 1 ),0,2);
            a.Write(BitConverter.GetBytes(RowsCount),0,4);
            //MessageBox.Show(Data.Rows.Count +" - "+ BitConverter.ToString(BitConverter.GetBytes(Data.Rows.Count)));

            // Write Colname
            for (int i = 1; i < Data.Columns.Count; i++)
            {
                int len = Data.Columns[i].ColumnName.Length;
                a.Write(BitConverter.GetBytes(len), 0, 2);
                a.Write(Encoding.ASCII.GetBytes(Data.Columns[i].ColumnName), 0, len);
                switch (Data.Columns[i].DataType.ToString())
                {
                    case "System.String":
                        a.WriteByte(0x1);
                        break;
                    case "System.Boolean":
                        a.WriteByte(0x2);
                        break;
                    case "System.Int32":
                        a.WriteByte(0x3);
                        break;
                    case "System.Decimal":
                        a.WriteByte(0x4);
                        break;
                    case "System.Single":
                        a.WriteByte(0x5);
                        break;
                    case "System.Double":
                        a.WriteByte(0x5);
                        break;
                }
                if(!colstype.ToList().Contains(Data.Columns[i].DataType.ToString()))
                {
                    MessageBox.Show(FileNames + " :("+ Data.Columns[i].ColumnName + ") ("+ Data.Columns[i].DataType.ToString() + ")");
                }
            }
            // Write Rows
            for (int b = 0; b < RowsCount ; b++)
            {
                var r = Data.Rows[b];
                a.Write(BitConverter.GetBytes(Convert.ToInt32(r[0])), 0, 4);
                for (int i = 1; i < Data.Columns.Count; i++)
                {
                    switch (Data.Columns[i].DataType.ToString())
                    {
                        case "System.String":
                            int len = r[i].ToString().Length;
                            a.Write(BitConverter.GetBytes(len), 0, 2);
                            if (len > 0)
                            {
                                a.Write(Encoding.ASCII.GetBytes(r[i].ToString()), 0, len);
                            }
                            break;
                        case "System.Boolean":
                            a.Write(BitConverter.GetBytes(Convert.ToInt32(r[i])), 0, 4);
                            break;
                        case "System.Int32":
                            a.Write(BitConverter.GetBytes(Convert.ToInt32(r[i])), 0, 4);
                            break;
                        case "System.Decimal":
                            a.Write(BitConverter.GetBytes(Convert.ToSingle(r[i])), 0, 4);
                            break;
                        case "System.Single":
                            a.Write(BitConverter.GetBytes(Convert.ToSingle(r[i])), 0, 4);
                            break;
                        case "System.Double":
                            a.Write(BitConverter.GetBytes(Convert.ToSingle(r[i])), 0, 4);
                            break;
                    }
                }
                //break;
                //if (b == RowsCount) break;
            }
            a.WriteByte(0x05);
            a.Write(Encoding.ASCII.GetBytes("THEND"), 0, 5);
            a.Dispose();
            a.Close();
        }
    }
}
