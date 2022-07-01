using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DELTA_MES.DB;
using System.Data;

namespace mes_notify_prod.Models
{
    public class PROD_PC_API
    {
        public string API_NAME { get; set; }
        public string API_FORMAT { get; set; }
        public string API_PARAMETER { get; set; }
        public string API_SAMPLE { get; set; }
    }

    public class POWER_ON_PC
    {
        public string PC_HOST_NAME { get; set; }

        public string PC_IP { get; set; }

        public string LINE { get; set; }

        public string FACTORY { get; set; }

        public string STATION { get; set; }

        public string CDATE { get; set; }

        public string CREATE_BY { get; set; }

        public string TEST_RESULT { get; set; }

        public string PING_STATUS { get; set; }

        public string PING_TIME { get; set; }

    }

    public class POWER_ON_PC_GROUP_BY_LINE
    {
        public string TOTAL_PC { get; set; }
        public string TURN_ON_PC { get; set; }
        public string TURN_OFF_PC { get; set; }
        public string LINE { get; set; }
        public string FACTORY { get; set; }
    }

    public class POWER_ON_PC_TIMELINE
    {
        public string ID { get; set; }

        public string PC_HOST_NAME { get; set; }

        public string PC_IP { get; set; }

        public string LINE { get; set; }

        public string FACTORY { get; set; }

        public string STATION { get; set; }

        public string CDATE { get; set; }

        public string UNIX_CDATE { get; set; }

        public string CREATE_BY { get; set; }

        public string TEST_RESULT { get; set; }

        public string PING_STATUS { get; set; }

        public string PING_TIME { get; set; }
    }

    public static class ProdPCAction
    {
        private static string dbcon = System
       .Configuration
       .ConfigurationManager.ConnectionStrings["mesdb"].ConnectionString;

        public static List<POWER_ON_PC> GetPowerOn(POWER_ON_PC data)
        {
            List<POWER_ON_PC> result = new List<POWER_ON_PC>();
            string FACTORY = "", PC_HOST_NAME = "", PC_IP = "", CREATE_BY = "", TEST_RESULT = "";
            string LINE = "", STATION = "", CDATE = "", PING_STATUS = "", PING_TIME = "";
            if (data != null)
            {
                if (data.PC_HOST_NAME != null)
                {
                    PC_HOST_NAME = $"AND PC_HOST_NAME = '{data.PC_HOST_NAME}'";
                }

                if (data.STATION != null)
                {
                    STATION = $"AND STATION = '{data.STATION}'";
                }

                if (data.LINE != null)
                {
                    LINE = $"AND LINE = '{data.LINE}'";
                }

                if (data.FACTORY != null)
                {
                    FACTORY = $"AND FACTORY = '{data.FACTORY}'";
                }

                if (data.CDATE != null)
                {
                    CDATE = $"AND CDATE = '{data.CDATE}'";
                }

                if (data.PC_IP != null)
                {
                    PC_IP = $"AND PC_IP = '{data.PC_IP}'";
                }

                if (data.PING_STATUS != null)
                {
                    PING_STATUS = $"AND PING_STATUS = '{data.PING_STATUS}'";
                }

                if (data.PING_TIME != null)
                {
                    PING_TIME = $"AND PING_TIME = '{data.PING_TIME}'";
                }

                if (data.TEST_RESULT != null)
                {
                    TEST_RESULT = $"AND TEST_RESULT = '{data.TEST_RESULT}'";
                }
            }

            using (MyOleDb DB = new MyOleDb(dbcon))
            {
                string SQL = $@"SELECT * FROM
		                    MESPRDDB.dbo.MES_LOG_PING_MT
                            WHERE 1=1 {FACTORY} {STATION} {LINE} {CDATE} {CREATE_BY} {PC_HOST_NAME} {PC_IP} {PC_HOST_NAME} {PING_STATUS} {PING_TIME} {TEST_RESULT}";
                DB.ExecQuery(SQL, out DataTable DT, out string message);

                for (int idx = 0; idx < DT.Rows.Count; idx++)
                {
                    result.Add(new POWER_ON_PC
                    {
                        FACTORY = MyDataTable.GetCell(DT, "FACTORY", "", idx)
                        ,
                        LINE = MyDataTable.GetCell(DT, "LINE", "", idx)
                        ,
                        CDATE = MyDataTable.GetCell(DT, "CDATE", "", idx)
                        ,
                        CREATE_BY = MyDataTable.GetCell(DT, "CREATE_BY", "", idx)
                        ,
                        PC_HOST_NAME = MyDataTable.GetCell(DT, "PC_HOST_NAME", "", idx)
                        ,
                        PC_IP = MyDataTable.GetCell(DT, "PC_IP", "", idx)
                        ,
                        PING_STATUS = MyDataTable.GetCell(DT, "PING_STATUS", "", idx)
                        ,
                        PING_TIME = MyDataTable.GetCell(DT, "PING_TIME", "", idx)
                        ,
                        STATION = MyDataTable.GetCell(DT, "STATION", "", idx)
                        ,
                        TEST_RESULT = MyDataTable.GetCell(DT, "TEST_RESULT", "", idx)
                    });
                }
            }

            return result;
        }

        public static List<POWER_ON_PC_GROUP_BY_LINE> GetPowerOnGroupByLine(POWER_ON_PC_GROUP_BY_LINE data)
        {
            List<POWER_ON_PC_GROUP_BY_LINE> result = new List<POWER_ON_PC_GROUP_BY_LINE>();
            string FACTORY = "";
            string LINE = "";
            if (data != null)
            {
                if (data.FACTORY != null)
                {
                    FACTORY = $"AND FACTORY = '{data.FACTORY}'";
                }

                if (data.LINE != null)
                {
                    LINE = $"AND LINE = '{data.LINE}'";
                }
            }


            using (MyOleDb DB = new MyOleDb(dbcon))
            {
                string SQL = $@"SELECT
	                    COUNT(*) TOTAL_PC, 
	                    SUM(TEST_RESULT_OK) TURN_ON_PC,
                        SUM(TEST_RESULT_NG) TURN_OFF_PC,
	                    LINE,FACTORY
                    FROM
	                    (
	                    SELECT
		                    CASE
			                    WHEN TEST_RESULT = 'Normal' THEN 1
			                    WHEN TEST_RESULT = 'Abnormal' THEN 0
			                    ELSE null
		                    END TEST_RESULT_OK,
		                    CASE
			                    WHEN TEST_RESULT = 'Normal' THEN 0
			                    WHEN TEST_RESULT = 'Abnormal' THEN 1
			                    ELSE null
		                    END TEST_RESULT_NG
                    ,   LINE
                    ,   FACTORY
	                    FROM
		                    MESPRDDB.dbo.MES_LOG_PING_MT
                    ) MES_LOG_PING_MT WHERE 1=1 {FACTORY} {LINE}
                    GROUP BY LINE,FACTORY";
                DB.ExecQuery(SQL, out DataTable DT, out string message);

                for (int idx = 0; idx < DT.Rows.Count; idx++)
                {
                    result.Add(new POWER_ON_PC_GROUP_BY_LINE
                    {
                        FACTORY = MyDataTable.GetCell(DT, "FACTORY", "", idx)
                        ,
                        LINE = MyDataTable.GetCell(DT, "LINE", "", idx)
                        ,
                        TOTAL_PC = MyDataTable.GetCell(DT, "TOTAL_PC", "", idx)
                        ,
                        TURN_OFF_PC = MyDataTable.GetCell(DT, "TURN_OFF_PC", "", idx)
                        ,
                        TURN_ON_PC = MyDataTable.GetCell(DT, "TURN_ON_PC", "", idx)
                    });
                }
            }

            return result;
        }

        public static List<POWER_ON_PC_TIMELINE> GetPowerOnTimeline(string pc_name)
        {
            List<POWER_ON_PC_TIMELINE> result = new List<POWER_ON_PC_TIMELINE>();
            string PC_HOST_NAME = "";
            if (pc_name != null)
            {
                PC_HOST_NAME = $"AND PC_HOST_NAME = '{pc_name}'";
            }

            using (MyOleDb DB = new MyOleDb(dbcon))
            {
                string SQL = $@"SELECT
	                                MESPRDDB.dbo.MES_LOG_PING_TXN.*,
	                                dbo.UNIX_TIMESTAMP(CDATE) UNIX_CDATE
                                FROM
	                                MESPRDDB.dbo.MES_LOG_PING_TXN
                                WHERE
	                                1 = 1 {PC_HOST_NAME} ORDER BY CDATE DESC";
                DB.ExecQuery(SQL, out DataTable DT, out string message);

                for (int idx = 0; idx < DT.Rows.Count; idx++)
                {
                    result.Add(new POWER_ON_PC_TIMELINE
                    {
                        FACTORY = MyDataTable.GetCell(DT, "FACTORY", "", idx)
                        ,
                        LINE = MyDataTable.GetCell(DT, "LINE", "", idx)
                        ,
                        CDATE = MyDataTable.GetCell(DT, "CDATE", "", idx)
                        ,
                        UNIX_CDATE = MyDataTable.GetCell(DT, "UNIX_CDATE", "", idx)
                        ,
                        CREATE_BY = MyDataTable.GetCell(DT, "CREATE_BY", "", idx)
                        ,
                        PC_HOST_NAME = MyDataTable.GetCell(DT, "PC_HOST_NAME", "", idx)
                        ,
                        PC_IP = MyDataTable.GetCell(DT, "PC_IP", "", idx)
                        ,
                        PING_STATUS = MyDataTable.GetCell(DT, "PING_STATUS", "", idx)
                        ,
                        PING_TIME = MyDataTable.GetCell(DT, "PING_TIME", "", idx)
                        ,
                        STATION = MyDataTable.GetCell(DT, "STATION", "", idx)
                        ,
                        TEST_RESULT = MyDataTable.GetCell(DT, "TEST_RESULT", "", idx)
                    });
                }
            }

            return result;
        }
    }
}