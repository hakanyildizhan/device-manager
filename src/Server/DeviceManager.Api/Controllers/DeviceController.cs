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
        private readonly IDeviceListService _deviceListService;

        public DeviceController(IDeviceListService deviceListService)
        {
            _deviceListService = deviceListService;
        }

        // GET api/device
        [HttpGet]
        public HttpResponseMessage Get()
        {
            try
            {
                IEnumerable<Device> deviceList = _deviceListService.GetDevices();
                return Request.CreateResponse(HttpStatusCode.OK, deviceList);
            }
            catch (Exception)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Error");
            }
        }
    }
}