using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DELTA_MES.DB;
using System.Data;

namespace mes_notify_prod.Models
{
    public class MesLogTdcOnline
    {
        public string id { get; set; }
        public string function_name { get; set; }
        public string total_time { get; set; }
        public string start_date { get; set; }
        public string error_detail { get; set; }
        public string line { get; set; }
        public string factory { get; set; }
        public string sender { get; set; }
        public string ping_time { get; set; }
        public string ping_ttl { get; set; }
        public string ping_bytes { get; set; }
        public string ping_status { get; set; }
        public string description { get; set; }
        public string test_result { get; set; }
    }

    public class HighchartCycle
    {
        public double y { get; set; }
        public string name { get; set; }
    }

    public class HighchartCycleByProbAndFac
    {
        public string[] categories { get; set; }

        public List<highchartsSeries> highchartsSeries { get; set; }
    }

    public class highchartsSeries
    {
        public string name { get; set; }

        public double[] data { get; set; }
    }

    public class TestResult
    {
        public string test_result { get; set; }
        public string avg_response { get; set; }
        public string max_response { get; set; }
        public string min_response { get; set; }
        public string times { get; set; }
    }

    public class PingResult
    {
        public string test_result { get; set; }
        public string ping_status { get; set; }
        public string times { get; set; }
    }

    public static class MesLogTdcOnlineAction
    {
        public static string token = "seaitmes";
        private static string dbcon = System
            .Configuration
            .ConfigurationManager.ConnectionStrings["mesdb"].ConnectionString;

        private static List<MesLogTdcOnline> GetList(DataTable DT)
        {
            List<MesLogTdcOnline> data = new List<MesLogTdcOnline>();
            DT = MyDataTable.GetTableBySort(DT, "start_date desc");
            for (int idx = 0; idx < DT.Rows.Count; idx++)
            {
                data.Add(new MesLogTdcOnline
                {
                    id = MyDataTable.GetCell(DT, "id", "", idx)
                    ,
                    function_name = MyDataTable.GetCell(DT, "function_name", "", idx)
                    ,
                    error_detail = MyDataTable.GetCell(DT, "error_detail", "", idx)
                    ,
                    start_date = MyDataTable.GetCell(DT, "start_date", "", idx)
                    ,
                    total_time = MyDataTable.GetCell(DT, "total_time", "", idx)
                    ,
                    factory = MyDataTable.GetCell(DT, "factory", "", idx)
                    ,
                    line = MyDataTable.GetCell(DT, "line", "", idx)
                    ,
                    sender = MyDataTable.GetCell(DT, "sender", "", idx)
                    ,
                    ping_bytes = MyDataTable.GetCell(DT, "ping_bytes", "", idx)
                    ,
                    ping_time = MyDataTable.GetCell(DT, "ping_time", "", idx)
                    ,
                    ping_ttl = MyDataTable.GetCell(DT, "ping_ttl", "", idx)
                    ,
                    ping_status = MyDataTable.GetCell(DT, "ping_status", "", idx)
                    ,
                    test_result = MyDataTable.GetCell(DT, "test_result", "", idx)
                    ,
                    description = MyDataTable.GetCell(DT, "description", "", idx)
                });
            }
            return data;
        }

        private static string CommonSelect()
        {
            string SQL = "SELECT " +
                " MESPRDDB.dbo.MES_LOG_TDC_ONLINE.*," +
                " CONCAT( DATEDIFF(second, { d '1970-01-01' }, start_date), '000') unix_start_date" +
                " FROM" +
                " MESPRDDB.dbo.MES_LOG_TDC_ONLINE";
            return SQL;
        }

        public static List<MesLogTdcOnline> Get()
        {
            List<MesLogTdcOnline> data = new List<MesLogTdcOnline>();
            using (MyOleDb DB = new MyOleDb(dbcon))
            {
                string SQL = CommonSelect();
                DB.ExecQuery(SQL, out DataTable DT, out string Status);
                data = GetList(DT);
                return data;
            }
        }

        public static List<MesLogTdcOnline> Get(out DataTable DT, string dateFrom, string dateTo, string factory)
        {
            List<MesLogTdcOnline> data = Get(out DT, dateFrom, dateTo, factory, null);
            return data;
        }

        public static List<MesLogTdcOnline> Get(out DataTable DT, string dateFrom, string dateTo, string factory, string testResult)
        {
            List<MesLogTdcOnline> data = new List<MesLogTdcOnline>();

            if (factory != null)
            {
                factory = $"and factory = '{factory}'";
            }

            if (testResult != null)
            {
                testResult = $"and test_result = '{testResult}'";
            }

            using (MyOleDb DB = new MyOleDb(dbcon))
            {
                string SQL = $"{CommonSelect()} " +
                    $"WHERE FORMAT(start_date, 'yyyy-MM-dd') >= '{dateFrom}' " +
                    $"and FORMAT(start_date, 'yyyy-MM-dd') <= '{dateTo}'" +
                    $"{factory}" +
                    $"{testResult}";
                DB.ExecQuery(SQL, out DT, out string Status);
                data = GetList(DT);
                return data;
            }
        }

        public static List<HighchartCycle> GetHichchartCycle(string dateFrom, string dateTo, string factory)
        {
            List<HighchartCycle> data = new List<HighchartCycle>();

            if (factory != "")
            {
                factory = $"and factory = '{factory}'";
            }

            using (MyOleDb DB = new MyOleDb(dbcon))
            {
                string SQL = $@"select COUNT(*) times, error_detail 
                from MESPRDDB.dbo.MES_LOG_TDC_ONLINE 
                WHERE test_result = 'Abnormal'
                and FORMAT(start_date, 'yyyy-MM-dd') >= '{dateFrom}' 
                and FORMAT(start_date, 'yyyy-MM-dd') <= '{dateTo}'
                {factory}
                group by error_detail";
                DB.ExecQuery(SQL, out DataTable DT, out string Status);
                for (int idx = 0; idx < DT.Rows.Count; idx++)
                {
                    data.Add(new HighchartCycle
                    {
                        name = MyDataTable.GetCell(DT, "error_detail", "", idx),
                        y = double.Parse(MyDataTable.GetCell(DT, "times", "0", idx))
                    });
                }
                return data;
            }
        }

        public static List<TestResult> GetTestResultTDCOnline(string dateFrom, string dateTo, string factory, out DataTable dtLastResult)
        {
            List<TestResult> data = new List<TestResult>();

            if (factory != "")
            {
                factory = $"and factory = '{factory}'";
            }

            using (MyOleDb DB = new MyOleDb(dbcon))
            {
                string SQL = $@"select ROUND(AVG(total_time),3) avg_response
                            ,MAX(total_time) max_response  
                            ,MIN(total_time) min_response
                            ,count(*) times
                            ,factory
                            ,test_result 
                            from MESPRDDB.dbo.MES_LOG_TDC_ONLINE 
                            WHERE function_name = 'tdconline_get_molist'
                            and FORMAT(start_date, 'yyyy-MM-dd') >= '{dateFrom}' 
                            and FORMAT(start_date, 'yyyy-MM-dd') <= '{dateTo}'
                            {factory}
                            group by factory,test_result order by test_result desc  ";
                DB.ExecQuery(SQL, out DataTable dtResult, out string statusResult);

                for (int idx = 0; idx < dtResult.Rows.Count; idx++)
                {
                    data.Add(new TestResult
                    {
                        avg_response = MyDataTable.GetCell(dtResult, "avg_response", "", idx),
                        max_response = MyDataTable.GetCell(dtResult, "max_response", "", idx),
                        min_response = MyDataTable.GetCell(dtResult, "min_response", "", idx),
                        test_result = MyDataTable.GetCell(dtResult, "test_result", "", idx),
                        times = MyDataTable.GetCell(dtResult, "times", "", idx),
                    });
                }

                SQL = $@"select test_result,total_time,start_date FROM 
                        (
                        select * 
                        from MESPRDDB.dbo.MES_LOG_TDC_ONLINE 
                        WHERE function_name = 'tdconline_get_molist'
                        and FORMAT(start_date, 'yyyy-MM-dd') >= '{dateFrom}' 
                        and FORMAT(start_date, 'yyyy-MM-dd') <= '{dateTo}'
                        {factory}
                        ) a ORDER by start_date desc";
                DB.ExecQuery(SQL, out dtLastResult, out string statusLastResult);


                return data;
            }
        }

        public static List<TestResult> GetTestResultPing(string dateFrom, string dateTo, string factory, out DataTable dtLastResult, out List<PingResult> listPingResult)
        {
            List<TestResult> data = new List<TestResult>();
            listPingResult = new List<PingResult>();
            if (factory != "")
            {
                factory = $"and factory = '{factory}'";
            }
            using (MyOleDb DB = new MyOleDb(dbcon))
            {
                string SQL = $@"select ROUND(AVG(ping_time),3) avg_response
                            ,MAX(ping_time) max_response  
                            ,MIN(ping_time) min_response
                            ,count(*) times
                            from MESPRDDB.dbo.MES_LOG_TDC_ONLINE 
                            WHERE function_name = 'ping' and test_result = 'Normal'
                            and FORMAT(start_date, 'yyyy-MM-dd') >= '{dateFrom}' 
                            and FORMAT(start_date, 'yyyy-MM-dd') <= '{dateTo}'
                            {factory}
                            group by test_result  ";
                DB.ExecQuery(SQL, out DataTable DT, out string Status);

                for (int idx = 0; idx < DT.Rows.Count; idx++)
                {
                    data.Add(new TestResult
                    {
                        avg_response = MyDataTable.GetCell(DT, "avg_response", "", idx),
                        max_response = MyDataTable.GetCell(DT, "max_response", "", idx),
                        min_response = MyDataTable.GetCell(DT, "min_response", "", idx),
                        test_result = MyDataTable.GetCell(DT, "test_result", "", idx),
                        times = MyDataTable.GetCell(DT, "times", "", idx),
                    });
                }

                SQL = $@"select test_result,ping_time,ping_status,start_date FROM 
                        (
                        select * 
                        from MESPRDDB.dbo.MES_LOG_TDC_ONLINE 
                        WHERE function_name = 'ping'
                            and FORMAT(start_date, 'yyyy-MM-dd') >= '{dateFrom}' 
                            and FORMAT(start_date, 'yyyy-MM-dd') <= '{dateTo}'
                            {factory}
                        ) a ORDER by start_date desc";
                DB.ExecQuery(SQL, out dtLastResult, out string statusLastResult);

                SQL = $@"select COUNT(*) times,test_result, ping_status 
                        from MESPRDDB.dbo.MES_LOG_TDC_ONLINE 
                        WHERE function_name = 'ping'
                        and FORMAT(start_date, 'yyyy-MM-dd') >= '{dateFrom}' 
                        and FORMAT(start_date, 'yyyy-MM-dd') <= '{dateTo}'
                        {factory}
                        group by error_detail,test_result,ping_status order by test_result desc, ping_status desc   ";
                DB.ExecQuery(SQL, out DataTable dtPing, out string statusPingResult);

                for (int idx = 0; idx < dtPing.Rows.Count; idx++)
                {
                    listPingResult.Add(new PingResult
                    {
                        ping_status = MyDataTable.GetCell(dtPing, "ping_status", "", idx),
                        times = MyDataTable.GetCell(dtPing, "times", "", idx),
                        test_result = MyDataTable.GetCell(dtPing, "test_result", "", idx)
                    });
                }

                return data;
            }
        }

        public static List<HighchartCycleByProbAndFac> GetHighchartByProbAndFac(string dateFrom, string dateTo)
        {
            using (MyOleDb DB = new MyOleDb(dbcon))
            {
                string SQL = $"select round(sum(total_time),0) total_time, error_detail,factory from MESPRDDB.dbo.MES_LOG_TDC_ONLINE  " +
                    $" WHERE FORMAT(start_date, 'yyyy-MM-dd') >= '{dateFrom}' " +
                    $" and FORMAT(start_date, 'yyyy-MM-dd') <= '{dateTo}'" +
                    $" and test_result = 'Abnormal'" +
                    $" group by error_detail,factory";
                DB.ExecQuery(SQL, out DataTable dtProblemAllFactory, out string Status);

                SQL = $"select DISTINCT error_detail from MESPRDDB.dbo.MES_LOG_TDC_ONLINE where test_result = 'Abnormal'";
                DB.ExecQuery(SQL, out DataTable dtErrorDetail, out Status);

                SQL = $"select DISTINCT factory from MESPRDDB.dbo.MES_LOG_TDC_ONLINE where test_result = 'Abnormal'";
                DB.ExecQuery(SQL, out DataTable dtFactory, out Status);

                string[] categories = new string[dtFactory.Rows.Count];
                for (int idx = 0; idx < dtFactory.Rows.Count; idx++)
                {
                    categories[idx] = MyDataTable.GetCell(dtFactory, "factory", "blank", idx);
                }

                string[] series = new string[dtErrorDetail.Rows.Count];
                for (int idx = 0; idx < dtErrorDetail.Rows.Count; idx++)
                {
                    series[idx] = MyDataTable.GetCell(dtErrorDetail, "error_detail", "blank", idx);
                }

                List<HighchartCycleByProbAndFac> retData = new List<HighchartCycleByProbAndFac>();
                List<highchartsSeries> highchartsSeries = new List<highchartsSeries>();

                for (int idxS = 0; idxS < series.Length; idxS++)
                {
                    string seriesName = series[idxS];
                    double[] arrData = new double[categories.Length];

                    DataTable dtData = MyDataTable.GetTableBySelect(dtProblemAllFactory, $"error_detail = '{seriesName}'");

                    for (int idx = 0; idx < categories.Length; idx++)
                    {
                        DataTable dtCategories = MyDataTable.GetTableBySelect(dtData, $"factory = '{categories[idx]}'");
                        arrData[idx] = double.Parse(MyDataTable.GetCell(dtCategories, "total_time", "0"));
                    }

                    highchartsSeries.Add(new highchartsSeries { name = seriesName, data = arrData });
                }

                retData.Add(new HighchartCycleByProbAndFac { categories = categories, highchartsSeries = highchartsSeries });

                return retData;
            }
        }


        /// <summary>
        /// Get result of current month
        /// </summary>
        /// <param name="factory"></param>
        /// <returns>true:data available, false:no data available</returns>
        public static bool GetNormalResultOfCurrentMonth(MesLogTdcOnline data)
        {
            string factory = "";
            if (data.factory != "")
            {
                factory = $"and factory = '{data.factory}'";
            }

            string function_name = "";
            if (data.function_name != "")
            {
                function_name = $"and function_name = '{data.function_name}'";
            }

            using (MyOleDb DB = new MyOleDb(dbcon))
            {
                string SQL = $@"select COUNT(*) times,test_result
                            from MESPRDDB.dbo.MES_LOG_TDC_ONLINE 
                            WHERE test_result = 'Normal'
                            and FORMAT(start_date, 'yyyy-MM-dd-HH') = FORMAT(GETDATE(),'yyyy-MM-dd-HH')
                            {factory}
                            {function_name}
                            group by test_result
                            HAVING COUNT(*) > 0";
                DB.ExecQuery(SQL, out DataTable DT, out string Status);
                if (DT.Rows.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
        }

        public static bool Insert(MesLogTdcOnline data, out string status)
        {
            using (MyOleDb DB = new MyOleDb(dbcon))
            {
                string id = "null";
                if (data.id != "")
                {
                    id = $"'{data.id}'";
                }

                string function_name = "null";
                if (data.function_name != "")
                {
                    function_name = $"'{data.function_name}'";
                }

                string start_date = "null";
                if (data.start_date != "")
                {
                    start_date = $"convert(datetime, '{data.start_date}', 120)";
                }

                string total_time = "null";
                if (data.total_time != "")
                {
                    total_time = $"'{data.total_time}'";
                }

                string error_detail = "null";
                if (data.error_detail != "")
                {
                    error_detail = $"'{data.error_detail}'";
                }

                string line = "null";
                if (data.line != "")
                {
                    line = $"'{data.line}'";
                }

                string factory = "null";
                if (data.factory != "")
                {
                    factory = $"'{data.factory}'";
                }

                string sender = "null";
                if (data.factory != "")
                {
                    sender = $"'{data.sender}'";
                }

                string ping_bytes = "null";
                if (data.ping_bytes != "")
                {
                    ping_bytes = $"'{data.ping_bytes}'";
                }

                string ping_time = "null";
                if (data.ping_time != "")
                {
                    ping_time = $"'{data.ping_time}'";
                }

                string ping_ttl = "null";
                if (data.ping_ttl != "")
                {
                    ping_ttl = $"'{data.ping_ttl}'";
                }

                string description = "null";
                if (data.description != "")
                {
                    description = $"'{data.description}'";
                }

                string ping_status = "null";
                if (data.ping_status != "")
                {
                    ping_status = $"'{data.ping_status}'";
                }

                string test_result = "null";
                if (data.test_result != "")
                {
                    test_result = $"'{data.test_result}'";
                }

                string sql = $@"insert into MESPRDDB.dbo.MES_LOG_TDC_ONLINE
                        (id,function_name,total_time,start_date,error_detail,factory,line,sender
                        ,ping_bytes,ping_time,ping_ttl,ping_status,description,test_result)values
                        ({id},{function_name},{total_time},{start_date},{error_detail},{factory},{line},{sender}
                        ,{ping_bytes},{ping_time},{ping_ttl},{ping_status},{description},{test_result})";

                return DB.ExecNonQuery(sql, out status);
            }
        }
    }
}
