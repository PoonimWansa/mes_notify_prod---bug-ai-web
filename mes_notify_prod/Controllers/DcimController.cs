using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace mes_notify_prod.Controllers
{
    public class DcimController : ApiController
    {
        // GET api/<controller>

        [HttpGet]
        [Route("api/dcim")]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }


        [HttpGet]
        [Route("api/dcim")]
        public IHttpActionResult SendLineNotify(string message, string sender)
        {
            string MsgMQTT = "";
            bool StatusMQTT = false;

            string MessageMQTT = $"" +
                $"\nMessage: {message}" +
                $"\nSender: {sender}" +
                $"\nTime: {DateTime.Now}" +
                $"";

            StatusMQTT = Models.MyMQTT.SentMessage(Models.MQTTDCIM.MQServer
                , Models.MQTTDCIM.Topic, MessageMQTT, out MsgMQTT);

            return Content(HttpStatusCode.OK, new
            {
                StatusMQTT = StatusMQTT,
                MsgMQTT = MsgMQTT
            });
        }



    }
}