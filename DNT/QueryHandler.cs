using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Text.RegularExpressions;
namespace DNT
{
    class QueryHandler
    {
        public static void Execute(ref DataTable Data, string Query)
        {
            Query = Query.Replace("   ", " ");
            Query = Query.Replace("  ", " ");
            var data_filtered = new DataView(Data);
            if (Query.ToLower().Contains("select"))
            {
                if (Query.ToLower().Contains("where"))
                {
                    data_filtered.RowFilter = Query.Substring(Query.IndexOf("where") + 5, Query.Length - (Query.IndexOf("where") + 5));
                }
                string cols = Query.Substring(
                    Query.IndexOf("select") + 6,
                    (
                        Query.ToLower().Contains("where") ?
                            Query.Length - (Query.IndexOf("where") + 5) :
                                (Query.IndexOf("select") + 6)
                    )
                );

                Data = data_filtered.Table;

                cols = cols.Replace(" ", "");
                var colsarr = cols.Split(',');

                foreach (DataColumn r in Data.Columns)
                {
                    if (!colsarr.ToList().Contains(r.ColumnName))
                    {
                        Data.Columns.Remove(r.ColumnName);
                    }
                }


            }
            if (Query.ToLower().Contains("update"))
            {
                if (!Query.ToLower().Contains("set"))
                {
                    throw new QueryExeption();
                }
                else
                {
                    if (Query.ToLower().Contains("where"))
                    {
                        data_filtered.RowFilter = Query.Substring(Query.IndexOf("where") + 5, Query.Length - (Query.IndexOf("where") + 5));
                    }


                }
            }
        }

        public static void Select(ref DataTable Data,List<string> Cols)
        {

        }

    }
    class QueryExeption : Exception
    {
        public QueryExeption() : base("Query Error!")
        {

        }
    }
}
