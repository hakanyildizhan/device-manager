using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManager.FileParsing
{
    public static class ParserFactory
    {
        public static IParser CreateParser(string filePath, string pageName = null)
        {
            string extension = new FileInfo(filePath).Extension;
            if (extension == ".xls" || extension == ".xlsx")
            {
                return new ExcelParser(filePath);
            }
            else if (extension == ".one")
            {
                return new OneNoteParser(filePath, pageName);
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
