using DeviceManager.Api.Model;
using DeviceManager.FileParsing;
using DeviceManager.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DeviceManager.Api.Controllers
{
    [Authorize]
    public class AdministrationController : Controller
    {
        private readonly ISettingsService _settingsService;
        private readonly ILogService _logService;
        private readonly IDeviceService _deviceListService;
        private readonly ISessionService _sessionService;

        public AdministrationController(
            ISettingsService settingsService, 
            ILogService<AdministrationController> logService,
            IDeviceService deviceListService,
            ISessionService sessionService)
        {
            _settingsService = settingsService;
            _logService = logService;
            _deviceListService = deviceListService;
            _sessionService = sessionService;
        }

        // GET: Index
        public ActionResult Index()
        {
            ViewBag.ActivePage = "Settings";
            Dictionary<string, string> settings = _settingsService.Get();
            return View(settings);
        }

        // GET: Settings
        public ActionResult Settings()
        {
            ViewBag.ActivePage = "Settings";
            Dictionary<string, string> settings = _settingsService.Get();
            return View(settings);
        }

        // GET: Import
        public ActionResult Import()
        {
            ViewBag.ActivePage = "Import";
            return View();
        }

        // POST: UploadFile
        [HttpPost]
        public JsonResult UploadFile(UploadViewModel viewModel)
        {
            // Validate inputs
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
                    path = Path.Combine(Utility.GetAppRoamingFolder(), generatedName);
                    viewModel.File.SaveAs(path);
                }
                else
                {
                    path = viewModel.OneNotePath;
                }

                IParser parser = ParserFactory.CreateParser(path, viewModel.OneNotePageName);
                IList<Hardware> hardwareList = parser.Parse();

                return Json(new
                {
                    Error = false,
                    Message = RenderRazorViewToString("Partial/HardwareListPreview", hardwareList.ToHardwareInfo())
                });
            }
            catch (Exception ex)
            {
                _logService.LogException(ex, "Error occured during file upload/processing");

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
    }
}