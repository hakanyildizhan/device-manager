// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using DeviceManager.Service;
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
