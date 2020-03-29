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
            string oneNotePath = @"C:\Users\USER\Documents\OneNote Not Defterleri\Test Notebook\Yeni Bölüm 1.one";
            string oneNotePageName = "Sayfa 1";
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
