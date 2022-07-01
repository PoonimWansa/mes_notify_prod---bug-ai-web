using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace mes_notify_prod.Controllers
{
    public class TrainingController : ApiController
    {
        [HttpGet]
        public IHttpActionResult Get()
        {
            if (!Request.Headers.Contains("apikey"))
            {
                return Content(HttpStatusCode.BadRequest, "Not found the Token Key.");
            }
            else
            {
                if (Request.Headers.GetValues("apikey").First() != System.Configuration.ConfigurationManager.AppSettings["token"])
                {
                    return Content(HttpStatusCode.BadRequest, "Request authorization");
                }
            }

            List<Models.Table1> retData = new List<Models.Table1>();
            retData = Models.MyTable1Action.Get();
            return Content(HttpStatusCode.OK, retData);
        }

        [HttpPost]
        public IHttpActionResult Insert(Models.Table1 data)
        {
            bool result = false;
            string Message = "";
            if (!Request.Headers.Contains("apikey"))
            {
                return Content(HttpStatusCode.BadRequest, "Not found the Token Key.");
            }
            else
            {
                if (Request.Headers.GetValues("apikey").First() != System.Configuration.ConfigurationManager.AppSettings["token"])
                {
                    return Content(HttpStatusCode.BadRequest, "Request authorization");
                }
            }

            data.id = Models.KeyGenerator.GetUniqueKey(10);

            result = Models.MyTable1Action.Insert(data, out Message);
            if (!result)
            {
                return Content(HttpStatusCode.BadRequest, Message);
            }
            else
            {
                return Content(HttpStatusCode.OK, data);
            }
        }

        [HttpPut]
        public IHttpActionResult Update(Models.Table1 data)
        {
            bool result = false;

            if (!Request.Headers.Contains("apikey"))
            {
                return Content(HttpStatusCode.BadRequest, "Not found the Token Key.");
            }
            else
            {
                if (Request.Headers.GetValues("apikey").First() != System.Configuration.ConfigurationManager.AppSettings["token"])
                {
                    return Content(HttpStatusCode.BadRequest, "Request authorization");
                }
            }

            result = Models.MyTable1Action.Update(data, out string Message);
            if (!result)
            {
                return Content(HttpStatusCode.BadRequest, Message);
            }
            else
            {
                return Content(HttpStatusCode.OK, data);
            }
        }

        public IHttpActionResult Delete(string id)
        {
            bool result = false;

            if (!Request.Headers.Contains("apikey"))
            {
                return Content(HttpStatusCode.BadRequest, "Not found the Token Key.");
            }
            else
            {
                if (Request.Headers.GetValues("apikey").First() != System.Configuration.ConfigurationManager.AppSettings["token"])
                {
                    return Content(HttpStatusCode.BadRequest, "Request authorization");
                }
            }

            result = Models.MyTable1Action.Delete(id, out string Message);
            if (!result)
            {
                return Content(HttpStatusCode.BadRequest, Message);
            }
            else
            {
                return Content(HttpStatusCode.OK, $"Delete {id}");
            }
        }
    }
}