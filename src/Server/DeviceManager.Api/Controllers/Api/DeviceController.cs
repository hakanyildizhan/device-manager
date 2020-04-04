using DeviceManager.Service;
using DeviceManager.Service.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
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
        [HttpGet]
        public HttpResponseMessage Get()
        {
            IEnumerable<Device> deviceList = _deviceListService.GetDevices();
            return Request.CreateResponse(HttpStatusCode.OK, deviceList);
        }
    }
}