// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using DeviceManager.Api.Model;
using DeviceManager.Service;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace DeviceManager.Api.Controllers
{
    public class HomeController : Controller
    {
        private readonly IClientService _userService;
        private readonly IDeviceService _deviceListService;
        private readonly ISettingsService _settingsService;


        public HomeController(
            IClientService userService, 
            IDeviceService deviceListService,
            ISettingsService settingsService)
        {
            _userService = userService;
            _deviceListService = deviceListService;
            _settingsService = settingsService;
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

        //GET: Info
        public ActionResult Info()
        {
            Dictionary<string, string> settings = _settingsService.Get();
            ServerStats model = new ServerStats()
            {
                LastDeviceListUpdate = settings[ServiceConstants.Settings.LAST_DEVICE_LIST_UPDATE],
                ServerVersion = settings[ServiceConstants.Settings.VERSION]
            };

            ViewBag.ActivePage = "Info";
            return View(model);
        }
    }
}