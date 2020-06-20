// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using System.Collections.Generic;
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
