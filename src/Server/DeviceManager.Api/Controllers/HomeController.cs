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
        public JsonResult UploadFile(UploadViewModel viewModel)
        {
            // Verify that the user selected a file
            if ((viewModel.File == null || viewModel.File.ContentLength == 0) && 
                (string.IsNullOrEmpty(viewModel.OneNotePath) || string.IsNullOrEmpty(viewModel.OneNotePageName)))
            {
                return Json(new
                {
                    Error = true,
                    Message = ""
                });
            }

            try
            {
                string path = string.Empty;

                if (viewModel.File != null)
                {
                    string fileName = Path.GetFileName(viewModel.File.FileName); // extract only the filename
                    string generatedName = fileName.Insert(viewModel.File.FileName.IndexOf('.'), "_" + DateTime.UtcNow.ToString("yyyyMMdd-HHmmss"));
                    path = Path.Combine(Server.MapPath("~/App_Data"), generatedName);
                    viewModel.File.SaveAs(path);
                }
                else
                {
                    path = viewModel.OneNotePath;
                }

                //string pageName = Request.Form["pageName"].ToString();
                IParser parser = ParserFactory.CreateParser(path, viewModel.OneNotePageName);
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