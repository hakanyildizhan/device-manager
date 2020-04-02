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

        // GET api/refresh
        [HttpGet]
        public HttpResponseMessage Get()
        {
            try
            {
                RefreshData data = new RefreshData();
                //Settings settings = _settingsService.Get();

                // determine if device list was updated since last refresh request
                int refreshInterval = int.Parse(_settingsService.Get(ServiceConstants.Settings.REFRESH_INTERVAL));
                DateTime lastDeviceListUpdate = DateTimeHelpers.Parse(_settingsService.Get(ServiceConstants.Settings.LAST_DEVICE_LIST_UPDATE));
                
                data.FullUpdateRequired = DateTime.UtcNow.Subtract(TimeSpan.FromSeconds(refreshInterval)) < lastDeviceListUpdate;

                if (!data.FullUpdateRequired)
                {
                    data.DeviceSessionInfo = _sessionService.GetDeviceSessions();
                    data.Settings = _settingsService.Get();
                }
                
                return Request.CreateResponse(HttpStatusCode.OK, data);
            }
            catch (Exception) //TODO: log
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Error");
            }
        }
    }
}
