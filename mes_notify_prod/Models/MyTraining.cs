using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DELTA_MES.DB;
using System.Data;

namespace mes_notify_prod.Models
{
    public class Table1
    {
        public string id { get; set; }

        public string name { get; set; }

        public string cdate { get; set; }
    }

    public static class MyTable1Action
    {
        public static string url = "https://localhost:44357/";
        public static string token = "seaitmes";
        private static string dbcon = $@"Provider=sqloledb;Data Source=localhost\SQLEXPRESS;Initial Catalog=master;User Id=root;Password=root;";

        private static List<Table1> GetList(DataTable DT)
        {
            List<Table1> data = new List<Table1>();
            DT = MyDataTable.GetTableBySort(DT, "cdate desc");
            for (int idx = 0; idx < DT.Rows.Count; idx++)
            {
                data.Add(new Table1
                {
                    id = MyDataTable.GetCell(DT, "id", "", idx)
                    ,
                    name = MyDataTable.GetCell(DT, "name", "", idx)
                    ,
                    cdate = MyDataTable.GetCell(DT, "cdate", "", idx)
                });
            }
            return data;
        }

        public static List<Table1> Get()
        {
            List<Table1> data = new List<Table1>();
            using (MyOleDb DB = new MyOleDb(dbcon))
            {
                string SQL = $"SELECT id,name,format(cdate,'yyyy-MM-dd') cdate FROM MYDATABASE.dbo.TABLE1";
                DB.ExecQuery(SQL, out DataTable DT, out string Status);
                data = GetList(DT);
                return data;
            }
        }

        public static List<Table1> Get(string id)
        {
            List<Table1> data = new List<Table1>();
            using (MyOleDb DB = new MyOleDb(dbcon))
            {
                string SQL = $"SELECT id,name,format(cdate,'yyyy-MM-dd') cdate FROM MYDATABASE.dbo.TABLE1 where id = '{id}'";
                DB.ExecQuery(SQL, out DataTable DT, out string Status);
                data = GetList(DT);
                return data;
            }
        }

        public static bool Insert(Table1 data, out string Status)
        {
            using (MyOleDb DB = new MyOleDb(dbcon))
            {
                string id = "null";
                if (data.id != "")
                {
                    id = $"'{data.id}'";
                }

                string name = "null";
                if (data.name != "")
                {
                    name = $"'{data.name}'";
                }

                string cdate = "null";
                if (data.cdate != "")
                {
                    cdate = $"convert(datetime, '{data.cdate}', 120)";
                }

                string sql = $"insert into MYDATABASE.dbo.TABLE1(id,name,cdate)" +
                    $"values({id},{name},{cdate})";
                return DB.ExecNonQuery(sql, out Status);
            }
        }

        public static bool Update(Table1 data, out string Status)
        {
            using (MyOleDb DB = new MyOleDb(dbcon))
            {
                string id = "null";
                if (data.id != "")
                {
                    id = $"'{data.id}'";
                }

                string name = "null";
                if (data.name != "")
                {
                    name = $"'{data.name}'";
                }

                string cdate = "null";
                if (data.cdate != "")
                {
                    cdate = $"convert(datetime, '{data.cdate}', 120)";
                }

                string sql = $"update MYDATABASE.dbo.TABLE1 set " +
                    $"name = {name}" +
                    $",cdate = {cdate} " +
                    $"where id = {id}";
                return DB.ExecNonQuery(sql, out Status);
            }
        }

        public static bool Delete(string id, out string Status)
        {
            using (MyOleDb DB = new MyOleDb(dbcon))
            {
                if (id != "")
                {
                    id = $"'{id}'";
                }
                else
                {
                    id = "null";
                }

                string sql = $"delete from MYDATABASE.dbo.TABLE1 where " +
                    $"id = {id}";
                return DB.ExecNonQuery(sql, out Status);
            }
        }
    }
}