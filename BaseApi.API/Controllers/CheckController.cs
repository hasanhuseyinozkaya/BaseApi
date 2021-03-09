using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BaseApi.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckController : ControllerBase
    {
        // GET: api/Check
        [HttpGet]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(typeof(Guid), 500)]
        public ActionResult Get()
        {
            return Ok("On Run");
        }

        [Authorize]
        [HttpPost("AuthCheck")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(typeof(Guid), 500)]
        public ActionResult AuthCheck()
        {
            return Ok("On Run");
        }
    }
}
