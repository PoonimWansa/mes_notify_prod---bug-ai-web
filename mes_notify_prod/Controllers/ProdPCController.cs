using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace mes_notify_prod.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ProdPCController : ApiController
    {
        [HttpGet]
        [Route("api/prod_pc")]
        public IHttpActionResult Help()
        {
            List<Models.PROD_PC_API> result = new List<Models.PROD_PC_API>();

            result.Add(new Models.PROD_PC_API
            {
                API_NAME = "Check power on status of production PC"
                ,
                API_FORMAT = "api/prod_pc/power_on"
                ,
                API_PARAMETER = ""
                ,
                API_SAMPLE = "api/prod_pc/power_on"
            });

            result.Add(new Models.PROD_PC_API
            {
                API_NAME = "Check power on status of production PC group by line"
                ,
                API_FORMAT = "api/prod_pc/power_on/groupby/{type}"
                ,
                API_PARAMETER = "type : line,.."
                ,
                API_SAMPLE = "api/prod_pc/power_on/groupby/line"
            });

            result.Add(new Models.PROD_PC_API
            {
                API_NAME = "Check power on status timeline of production PC"
                ,
                API_FORMAT = "api/prod_pc/power_on/timeline/{pc_name}"
                ,
                API_PARAMETER = "pc_name : TE-M6L-ATS1-01"
                ,
                API_SAMPLE = "api/prod_pc/power_on/timeline/TE-M6L-ATS1-01"
            });

            return Content(HttpStatusCode.OK, result);
        }

        [HttpGet]
        [Route("api/prod_pc/power_on")]
        public IHttpActionResult PowerOn([FromUri] Models.POWER_ON_PC data)
        {
            List<Models.POWER_ON_PC> result = Models.ProdPCAction.GetPowerOn(data);

            if (result.Count > 0)
            {
                return Content(HttpStatusCode.OK, result);
            }
            else
            {
                return Content(HttpStatusCode.NotFound, result);
            }
        }

        [HttpGet]
        [Route("api/prod_pc/power_on/groupby/{type}")]
        public IHttpActionResult PowerOnGroupBy(string type, [FromUri] Models.POWER_ON_PC_GROUP_BY_LINE data)
        {
            List<Models.POWER_ON_PC_GROUP_BY_LINE> result = new List<Models.POWER_ON_PC_GROUP_BY_LINE>();
            if (type == "line")
            {
                result = Models.ProdPCAction.GetPowerOnGroupByLine(data);
            }
            else
            {
                return Content(HttpStatusCode.BadRequest, $"Format is not correct..,{DateTime.Now}");
            }

            if (result.Count > 0)
            {
                return Content(HttpStatusCode.OK, result);
            }
            else
            {
                return Content(HttpStatusCode.NotFound, result);
            }
        }

        [HttpGet]
        [Route("api/prod_pc/power_on/timeline/{pc_name}")]
        public IHttpActionResult PowerOnTimeLine(string pc_name)
        {
            List<Models.POWER_ON_PC_TIMELINE> result = new List<Models.POWER_ON_PC_TIMELINE>();
            result = Models.ProdPCAction.GetPowerOnTimeline(pc_name);
            if (result.Count > 0)
            {
                return Content(HttpStatusCode.OK, result);
            }
            else
            {
                return Content(HttpStatusCode.NotFound, result);
            }
        }

    }
}