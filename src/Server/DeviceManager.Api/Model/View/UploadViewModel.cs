using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeviceManager.Api.Model
{
    public class UploadViewModel
    {
        public HttpPostedFileBase File { set; get; }
        public string OneNotePageName { set; get; }
        public string OneNotePath { set; get; }
    }
}