using DeviceManager.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DeviceManager.Api.Controllers
{
    public class SettingsController : ApiController
    {
        private readonly ISettingsService _settingsService;

        public SettingsController(
            ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        // GET api/settings
        [HttpGet]
        public HttpResponseMessage Get()
        {
            return Request.CreateResponse(HttpStatusCode.OK, _settingsService.Get());
        }
    }
}
