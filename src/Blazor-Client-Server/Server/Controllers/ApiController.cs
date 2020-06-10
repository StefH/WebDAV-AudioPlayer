using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Blazor_Client_Server.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ApiController : ControllerBase
    {
        private readonly ILogger<ApiController> logger;

        public ApiController(ILogger<ApiController> logger, IConnectionSettings connectionSettings)
        {
            this.logger = logger;
        }

       // [HttpGet]
        
    }
}
