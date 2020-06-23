// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using DeviceManager.Api.Model;
using DeviceManager.Service;
using System;
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
