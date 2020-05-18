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

namespace DeviceManager.Api.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdministrationController : Controller
    {
        private readonly ISettingsService _settingsService;
        private readonly ILogService _logService;
        private readonly IDeviceService _deviceService;
        private readonly ISessionService _sessionService;

        public AdministrationController(
            ISettingsService settingsService,
            ILogService<AdministrationController> logService,
            IDeviceService deviceListService,
            ISessionService sessionService)
        {
            _settingsService = settingsService;
            _logService = logService;
            _deviceService = deviceListService;
            _sessionService = sessionService;
        }

        // GET: Index
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Home");
        }

        // GET: Settings
        public async Task<ActionResult> Settings()
        {
            ViewBag.ActivePage = "Settings";

            var settings = await _settingsService.GetDetailedAsync();
            return View(settings);
        }

        // GET: Import
        public ActionResult Import()
        {
            ViewBag.ActivePage = "Import";
            return View();
        }

        // GET: Review
        public ActionResult Review()
        {
            ViewBag.ActivePage = "Review";
            var hardwareList = _deviceService.GetDevices().OrderBy(d => d.DeviceGroup).ToList();
            return View(hardwareList);
        }

        // POST: UpdateSettings
        /// <summary>
        /// Updates relevant server settings.
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateSettings(SettingsDetail settings)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ActivePage = "Settings";
                return View("Settings", settings);
            }

            await _settingsService.UpdateAsync(settings);
            return RedirectToAction("Settings");
        }

        // POST: UpdateHardware
        /// <summary>
        /// This method parses the information on Edit Hardware modal dialog in order to update the corresponding item.
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateHardware(DeviceDetail device)
        {
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    success = false,
                    formErrors = ModelState.Select(kvp => new { key = kvp.Key, errors = kvp.Value.Errors.Select(e => e.ErrorMessage) })
                });
            }

            bool result = await _deviceService.UpdateDevice(device);

            return Json(new
            {
                success = result
            });
        }

        // POST: DeleteHardware
        /// <summary>
        /// This method parses the information on Delete Hardware modal dialog in order to update the corresponding item.
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteHardware(DeviceDetail device)
        {
            bool result = await _deviceService.DeactivateDevice(device.Id);

            return Json(new
            {
                success = result
            });
        }

        // POST: AddHardware
        /// <summary>
        /// This method parses the information on Add New Hardware modal dialog in order to add a new item.
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddHardware(DeviceDetail device)
        {
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    success = false,
                    formErrors = ModelState.Select(kvp => new { key = kvp.Key, errors = kvp.Value.Errors.Select(e => e.ErrorMessage) })
                });
            }

            bool result = await _deviceService.AddDevice(device);

            return Json(new
            {
                success = result
            });
        }

        /// <summary>
        /// Shows a modal dialog for editing a hardware item.
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UpdateHardwarePopup(string itemId)
        {
            int deviceId;
            bool idIsValid = int.TryParse(itemId, out deviceId);

            if (!idIsValid)
            {
                return RedirectToAction("Review");
            }

            var device = _deviceService.GetDevice(deviceId);
            return PartialView("~/Views/Administration/Partial/HardwareUpdate.cshtml", device);
        }

        /// <summary>
        /// Shows a modal dialog for deleting a hardware item.
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteHardwarePopup(string itemId)
        {
            int deviceId;
            bool idIsValid = int.TryParse(itemId, out deviceId);

            if (!idIsValid)
            {
                return RedirectToAction("Review");
            }

            var device = _deviceService.GetDevice(deviceId);
            return PartialView("~/Views/Administration/Partial/HardwareDelete.cshtml", device);
        }

        /// <summary>
        /// Shows a modal dialog for adding a new hardware item.
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddHardwarePopup()
        {
            return PartialView("~/Views/Administration/Partial/HardwareAdd.cshtml");
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

                // if name, primary address or hardware info is null or empty, discard those items
                List<Hardware> invalidRows = hardwareList
                    .Where(r => string.IsNullOrWhiteSpace(r.Name) ||
                                string.IsNullOrWhiteSpace(r.PrimaryAddress) ||
                                string.IsNullOrWhiteSpace(r.HardwareInfo))
                    .ToList();

                if (invalidRows.Any())
                {
                    invalidRows.ForEach(invalidRow => hardwareList.Remove(invalidRow));
                }

                return Json(new
                {
                    Error = false,
                    Message = RenderRazorViewToString("~/Views/Administration/Partial/HardwareListPreview.cshtml", hardwareList.ToHardwareInfo()),
                    DiscardedRowWarning = invalidRows.Any() ? "Some rows were discarded. Make sure that each row has its hardware name, primary address and hardware info filled and not set to an empty string." : ""
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
            success &= await _deviceService.Import(hardwareList.ToDeviceImport());

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