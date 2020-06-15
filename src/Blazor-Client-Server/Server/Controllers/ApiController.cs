using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebDav;
using WebDav.AudioPlayer.Client;
using WebDav.AudioPlayer.Models;

namespace Blazor_Client_Server.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ApiController : ControllerBase
    {
        private readonly ILogger<ApiController> _logger;
        private readonly IConnectionSettings _connectionSettings;
        private readonly WebDavClient _client;

        public ApiController(ILogger<ApiController> logger, IConnectionSettings connectionSettings)
        {
            _logger = logger;
            _connectionSettings = connectionSettings;
            _client = new WebDavClient(new WebDavClientParams
            {
                Credentials = new NetworkCredential(connectionSettings.UserName, connectionSettings.GetPassword()),
                UseDefaultCredentials = false,
                Timeout = TimeSpan.FromMinutes(5)
            });
        }

        [HttpGet("GetRoot")]
        public ResourceItem GetRoot()
        {
            return new ResourceItem
            {
                DisplayName = _connectionSettings.RootFolder,
                FullPath = OnlinePathBuilder.Combine(_connectionSettings.StorageUri, _connectionSettings.RootFolder)
            };
        }

    }
}
