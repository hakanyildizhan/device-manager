using DeviceManager.Api.Model;
using DeviceManager.FileParsing;
using DeviceManager.Service;
using DeviceManager.Service.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using FromBody = System.Web.Http.FromBodyAttribute;

namespace DeviceManager.Api.Controllers
{
    public class HomeController : Controller
    {
        private readonly IClientService _userService;
        private readonly IDeviceService _deviceListService;


        public HomeController(
            IClientService userService, 
            IDeviceService deviceListService)
        {
            _userService = userService;
            _deviceListService = deviceListService;
        }

        // GET: Home
        public ActionResult Index()
        {
            StatusPageViewModel model = new StatusPageViewModel()
            {
                UserList = _userService.GetClientInfo(),
                HardwareList = _deviceListService.GetDevices().OrderBy(d => d.DeviceGroup).ToList()
            };

            ViewBag.ActivePage = "Index";
            return View(model);
        }

        
    }
}