using DeviceManager.Api.Model;
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
    public class HomeController : Controller
    {
        private readonly IUserService _userService;
        private readonly IDeviceListService _deviceListService;

        public HomeController(IUserService userService, IDeviceListService deviceListService)
        {
            _userService = userService;
            _deviceListService = deviceListService;
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
        public async Task<JsonResult> UploadFile(HttpPostedFileBase file)
        {
            // Verify that the user selected a file
            if (file != null && file.ContentLength > 0)
            {
                // extract only the filename
                string fileName = Path.GetFileName(file.FileName);
                string generatedName = fileName.Insert(file.FileName.IndexOf('.'), "_" + DateTime.UtcNow.ToString("yyyyMMdd-HHmmss"));
                // store the file inside ~/App_Data/uploads folder
                var path = Path.Combine(Server.MapPath("~/App_Data/uploads"), generatedName);
                file.SaveAs(path);
            }

            if (true)
            {

            }

            // redirect back to the index action to show the form once again
            return Json(new
            {
                FailedLines = ""
            });

            /*
            var file = Request.Form.Files[0];
            string webRootPath = _hostingEnvironment.WebRootPath;
            string filePath = Path.Combine(webRootPath, "Temp", file.FileName.Insert(file.FileName.IndexOf('.'), "_" + DateTime.UtcNow.ToString("yyyyMMdd-HHmmss")));
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(fileStream);
            }

            string dataType = Request.Form["DataType"].ToString();

            try
            {
                if (dataType == "Offer")
                {
                    var parser = new ExcelDataParser<OfferUploadDto>(filePath);
                    var items = parser.Parse();
                    var uploadInfo = new UploadInfoDto() { FileName = file.FileName };
                    var result = await _offerPostService.AddUploads(items, uploadInfo);
                    return Json(new
                    {
                        FailedLines = result.FailedItemCount,
                        UploadResult = result.UploadResult.ToString(),
                        Error = result.Error,
                        SessionId = result.SessionId,
                        SucceededLines = result.SucceededItemCount
                    });
                }

                else //if (dataType == "Demand")
                {
                    var parser = new ExcelDataParser<DemandUploadDto>(filePath);
                    var items = parser.Parse();
                    var uploadInfo = new UploadInfoDto() { FileName = file.FileName };
                    var result = await _demandPostService.AddUploads(items, uploadInfo);
                    return Json(new
                    {
                        FailedLines = result.FailedItemCount,
                        UploadResult = result.UploadResult.ToString(),
                        Error = result.Error,
                        SessionId = result.SessionId,
                        SucceededLines = result.SucceededItemCount
                    });
                }
            }
            catch (Exception ex)
            {
                string errorMessage = "";

                if (ex is MissingColumnException || ex is ParsingException)
                {
                    errorMessage = ex.Message;
                }
                else
                {
                    errorMessage = "An error occurred. Please try again later.";
                }
                return Json(new
                {
                    FailedLines = 0,
                    UploadResult = UploadResult.Failed.ToString(),
                    Error = errorMessage,
                    SessionId = "-",
                    SucceededLines = 0
                });
            }
            */
        }

        // GET: Settings
        public ActionResult Settings()
        {
            ViewBag.ActivePage = "Settings";
            return View();
        }
    }
}