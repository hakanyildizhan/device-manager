using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OneNoteApplication = Microsoft.Office.Interop.OneNote.Application;

namespace DeviceManager.FileParsing
{
    public class OneNoteParser : IParser
    {
        private string _filePath;
        private string _pageName;

        public OneNoteParser(string filePath, string pageName)
        {
            _filePath = filePath;
            _pageName = pageName;
        }
        public IList<Hardware> Parse()
        {
            string strID;
            OneNoteApplication app = new OneNoteApplication();
            app.OpenHierarchy(_filePath, string.Empty, out strID);

            string schema;
            app.GetHierarchy(strID, Microsoft.Office.Interop.OneNote.HierarchyScope.hsChildren, out schema);
            OneNoteXmlManager xmlMgr = new OneNoteXmlManager(schema);
            bool pageExists = xmlMgr.PageExists(_pageName);

            if (!pageExists) return null;

            string pageContent;
            string pageId = xmlMgr.GetPageId(_pageName);
            app.GetPageContent(pageId, out pageContent);

            OneNoteTableParser tableParser = new OneNoteTableParser(pageContent);
            return tableParser.ParseHardwareTable();
        }
    }
}
