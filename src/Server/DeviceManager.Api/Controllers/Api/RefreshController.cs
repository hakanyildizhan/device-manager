using DeviceManager.Api.Model;
using DeviceManager.Service;
using DeviceManager.Service.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DeviceManager.Api.Controllers
{
    public class RefreshController : ApiController
    {
        private readonly ISettingsService _settingsService;
        private readonly ISessionService _sessionService;

        public RefreshController(
            ISettingsService settingsService,
            ISessionService sessionService)
        {
            _settingsService = settingsService;
            _sessionService = sessionService;
        }

        // POST api/refresh
        [HttpPost]
        public HttpResponseMessage Post(RefreshRequest request)
        {
            // determine if device list was updated since last refresh request
            DateTime lastDeviceListUpdate = DateTimeHelpers.Parse(_settingsService.Get(ServiceConstants.Settings.LAST_DEVICE_LIST_UPDATE));
            DateTime lastSuccessfulRefreshOfClient = DateTimeHelpers.Parse(request.LastSuccessfulRefresh);

            RefreshResponse response = new RefreshResponse();
            response.FullUpdateRequired = lastSuccessfulRefreshOfClient < lastDeviceListUpdate;

            if (!response.FullUpdateRequired)
            {
                response.DeviceSessionInfo = _sessionService.GetDeviceSessions();
                response.Settings = _settingsService.Get();
            }

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }
    }
}
