using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CrudBuroApi.Controllers
{
    [RoutePrefix("api/health")]
    public class HealthController : ApiController
    {
        [HttpGet]
        [Route("")]
        public IHttpActionResult Get()
        {
            return Ok(new
            {
                Status = "Healthy",
                Timestamp = DateTime.UtcNow,
                Environment = Environment.GetEnvironmentVariable("ASPNET_ENVIRONMENT") ?? "Production"
            });
        }
    }
}
