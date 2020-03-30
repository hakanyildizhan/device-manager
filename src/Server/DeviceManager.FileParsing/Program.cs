using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DeviceManager.FileParsing
{
    class Program
    {
        static void Main(string[] args)
        {
            string oneNotePath = @"\\evosoft.com\dfsroot_nbg$\008_Projects\735035_SIRIUS_TIA_INT_ISTK\01_Projects\01_Starter\01_SoftStarter\Documentation\SoftStarter_Notebook\Hardware.one";
            string oneNotePageName = "New Rack Order";
            string excelFilePath = @"C:\Users\USER\Documents\test.xlsx";

            //IParser parser = new ExcelParser(excelFilePath);
            IParser parser = new OneNoteParser(oneNotePath, oneNotePageName);
            IList<Hardware> hardwareList = parser.Parse();
        }

        static void ParseExcel(string path)
        {
            IParser excelParser = new ExcelParser(path);
            IList<Hardware> hardwareList = excelParser.Parse();
        }
    }
}
