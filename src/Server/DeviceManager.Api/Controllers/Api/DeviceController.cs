// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using DeviceManager.Service;
using DeviceManager.Service.Model;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DeviceManager.Api.Controllers
{
    public class DeviceController : ApiController
    {
        private readonly IDeviceService _deviceListService;

        public DeviceController(IDeviceService deviceListService)
        {
            _deviceListService = deviceListService;
        }

        // GET api/device
        /// <summary>
        /// Gets all active devices, along with their availability information.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage Get()
        {
            IEnumerable<Device> deviceList = _deviceListService.GetDevices();
            return Request.CreateResponse(HttpStatusCode.OK, deviceList);
        }

        // POST api/device/getmydevices
        /// <summary>
        /// Gets all devices currently checked out to given user.
        /// </summary>
        /// <param name="domainUserName"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage GetMyDevices([FromBody] string domainUserName)
        {
            IEnumerable<DeviceDetail> deviceList = _deviceListService.GetUserDevices(domainUserName);
            return Request.CreateResponse(HttpStatusCode.OK, deviceList);
        }
    }
}