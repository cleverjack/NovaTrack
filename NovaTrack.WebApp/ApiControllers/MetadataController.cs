using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NovaTrack.WebApp;

namespace NovaTrack.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MetadataController : ControllerBase
    {
        [HttpGet]
        [Route("")]
        public ActionResult Index()
        {
            Dictionary<string, string> premises = Lists.Premises();

            Dictionary<string, string> assetTypes = Lists.AssetTypes();

            var res = new { premises, assetTypes };

            return Ok(res);
        }

       
    }
}