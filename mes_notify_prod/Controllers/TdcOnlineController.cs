using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data;
using DELTA_MES.DB;


namespace mes_notify_prod.Controllers
{

    public class TdcOnlineController : ApiController
    {
        [HttpGet]
        [Route("api/tdconline")]
        public IHttpActionResult Get()
        {
            List<Models.MesLogTdcOnline> retData = Models.MesLogTdcOnlineAction.Get();
            return Content(HttpStatusCode.OK, retData);
        }

        [HttpGet]
        [Route("api/tdconline")]
        public IHttpActionResult Get(string factory, string datefrom, string dateto)
        {
            List<Models.MesLogTdcOnline> retData = Models.MesLogTdcOnlineAction.Get(out DataTable retDT, datefrom, dateto, factory);

            if (retData.Count > 0)
            {
                return Content(HttpStatusCode.OK, retData);
            }
            else
            {
                return Content(HttpStatusCode.NotFound, $"Not found data from {datefrom} to {dateto}");
            }
        }

        [HttpGet]
        [Route("api/tdconline")]
        public IHttpActionResult Get(string factory, string datefrom, string dateto, string test_result)
        {
            List<Models.MesLogTdcOnline> retData = Models.MesLogTdcOnlineAction.Get(out DataTable retDT, datefrom, dateto, factory, test_result);

            if (retData.Count > 0)
            {
                return Content(HttpStatusCode.OK, retData);
            }
            else
            {
                return Content(HttpStatusCode.NotFound, $"Not found data from {datefrom} to {dateto}");
            }
        }

        [HttpGet]
        [Route("api/tdconline")]
        public IHttpActionResult GetSummary(string summary, string datefrom, string dateto)
        {
            List<Models.HighchartCycle> retSummary = Models.MesLogTdcOnlineAction.GetHichchartCycle(datefrom, dateto, summary);
            List<Models.TestResult> retTDCOnline = Models.MesLogTdcOnlineAction.GetTestResultTDCOnline(datefrom, dateto, summary, out DataTable dtLastTDCOnline);
            List<Models.TestResult> retPing = Models.MesLogTdcOnlineAction.GetTestResultPing(datefrom, dateto, summary, out DataTable dtLastPing, out List<Models.PingResult> retPingResult);

            var lastResultTDCOnline = MyDataTable.GetCell(dtLastTDCOnline, "test_result", "");

            if (lastResultTDCOnline == "Normal")
            {
                lastResultTDCOnline = "<span class='badge badge-pill badge-success'>Normal</span>";
            }
            else
            {
                lastResultTDCOnline = "<span class='badge badge-pill badge-danger'>Abnormal</span>";
            }

            var lastResultPing = MyDataTable.GetCell(dtLastTDCOnline, "test_result", "");

            if (lastResultPing == "Normal")
            {
                lastResultPing = "<span class='badge badge-pill badge-success'>Normal</span>";
            }
            else
            {
                lastResultPing = "<span class='badge badge-pill badge-danger'>Abnormal</span>";
            }

            var lastTDCOnline = $"Last result is {lastResultTDCOnline}" +
                $", {MyDataTable.GetCell(dtLastTDCOnline, "total_time", "")}s, {MyDataTable.GetCell(dtLastTDCOnline, "start_date", "")}";

            var lastPing = $"Last result is {lastResultPing}" +
                $", {MyDataTable.GetCell(dtLastPing, "ping_status", "")}" +
                $", {MyDataTable.GetCell(dtLastPing, "ping_time", "")}ms" +
                $", {MyDataTable.GetCell(dtLastPing, "start_date", "")}";
            if (retTDCOnline.Count > 0 || retPing.Count > 0 || retPingResult.Count > 0)
            {
                return Content(HttpStatusCode.OK, new
                {
                    retSummary = retSummary,
                    retTDCOnline = retTDCOnline,
                    retPing = retPing,
                    lastTDCOnline = lastTDCOnline,
                    lastPing = lastPing,
                    retPingResult = retPingResult
                });
            }
            else
            {
                return Content(HttpStatusCode.NotFound, $"Not found data from {datefrom} to {dateto}");
            }
        }


        [HttpPost]
        [Route("api/tdconline")]
        public IHttpActionResult Post([FromBody] Models.MesLogTdcOnline mesLogTdcOnline)
        {
            string MsgMQTT = "", MsgSQL = "";
            bool StatusMQTT = false, StatusSQL = false;

            bool StatusNormalResult = Models.MesLogTdcOnlineAction.GetNormalResultOfCurrentMonth(mesLogTdcOnline);

            //Record in case of Normal result of current hour
            if (StatusNormalResult == false && mesLogTdcOnline.test_result == "Normal")
            {
                StatusSQL = Models.MesLogTdcOnlineAction.Insert(mesLogTdcOnline, out MsgSQL);
            }

            //Record in case of Abnormal result
            if (mesLogTdcOnline.test_result == "Abnormal")
            {
                StatusSQL = Models.MesLogTdcOnlineAction.Insert(mesLogTdcOnline, out MsgSQL);
                string MessageMQTT = $"" +
                $"\nFactory: {mesLogTdcOnline.factory}" +
                $"\nLine: {mesLogTdcOnline.line}" +
                $"\nTest Function: {mesLogTdcOnline.function_name}" +
                $"\nTest Result: {mesLogTdcOnline.test_result}" +
                $"\nTest Time: {mesLogTdcOnline.total_time}s" +
                $"\n\nProblem: {mesLogTdcOnline.error_detail}" +
                $"\nWhen: {mesLogTdcOnline.start_date}" +
                $"\nDescription: {mesLogTdcOnline.description}" +
                $"\n\nLocation: {mesLogTdcOnline.sender}" +
                $"";

                StatusMQTT = Models.MyMQTT.SentMessage(Models.MQTTMES.MQServer
                , Models.MQTTMES.Topic, MessageMQTT, out MsgMQTT);
            }

            return Content(HttpStatusCode.OK, new
            {
                StatusSQL = StatusSQL,
                MsgSQL = MsgSQL,
                StatusMQTT = StatusMQTT,
                MsgMQTT = MsgMQTT
            });
        }
    }
}
