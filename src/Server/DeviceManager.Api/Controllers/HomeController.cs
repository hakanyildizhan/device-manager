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
        private readonly IUserService _userService;
        private readonly IDeviceService _deviceListService;
        private readonly ISessionService _sessionService;

        public HomeController(IUserService userService, IDeviceService deviceListService, ISessionService sessionService)
        {
            _userService = userService;
            _deviceListService = deviceListService;
            _sessionService = sessionService;
        }

        // GET: Home
        public ActionResult Index()
        {
            StatusPageViewModel model = new StatusPageViewModel()
            {
                UserList = _userService.GetUserInfo(),
                HardwareList = _deviceListService.GetDevices().OrderBy(d => d.DeviceGroup).ToList()
            };

            ViewBag.ActivePage = "Index";
            return View(model);
        }

        // GET: Manage
        public ActionResult Manage()
        {
            ViewBag.ActivePage = "Manage";
            return View();
        }

        // POST: Dashboard/File/UploadFile
        [HttpPost]
        public JsonResult UploadFile(HttpPostedFileBase file)
        {
            // Verify that the user selected a file
            if (file == null || file.ContentLength == 0)
            {
                return Json(new
                {
                    Error = true,
                    Message = ""
                });
            }

            try
            {
                // extract only the filename
                string fileName = Path.GetFileName(file.FileName);
                string generatedName = fileName.Insert(file.FileName.IndexOf('.'), "_" + DateTime.UtcNow.ToString("yyyyMMdd-HHmmss"));
                var path = Path.Combine(Server.MapPath("~/App_Data"), generatedName);
                file.SaveAs(path);

                string pageName = Request.Form["pageName"].ToString();
                IParser parser = ParserFactory.CreateParser(path, pageName);
                IList<Hardware> hardwareList = parser.Parse();

                return Json(new
                {
                    Error = false,
                    Message = RenderRazorViewToString("Partial/HardwareListPreview", hardwareList.ToHardwareInfo())
                });
            }
            catch (Exception e)
            {
                return Json(new
                {
                    Error = true,
                    Message = ""
                });
            }
        }

        [HttpPost]
        public async Task<JsonResult> ImportData(IEnumerable<HardwareInfo> hardwareList)
        {
            bool success = await _sessionService.EndActiveSessionsAsync();
            success &= await _deviceListService.Import(hardwareList.ToDeviceImport());

            return Json(new
            {
                Error = !success
            });
        }

        private string RenderRazorViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }

        // GET: Settings
        public ActionResult Settings()
        {
            ViewBag.ActivePage = "Settings";
            return View();
        }
    }
}