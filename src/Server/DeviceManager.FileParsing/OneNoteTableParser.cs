using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DeviceManager.FileParsing
{
    public class OneNoteTableParser
    {
        private XmlDocument _doc;
        private XmlNamespaceManager _xmlnsManager;

        public OneNoteTableParser(string schema)
        {
            _doc = new XmlDocument();
            _doc.LoadXml(schema);

            _xmlnsManager = new XmlNamespaceManager(_doc.NameTable);
            _xmlnsManager.AddNamespace("one", "http://schemas.microsoft.com/office/onenote/2013/onenote");
        }

        public List<Hardware> ParseHardwareTable()
        {
            XmlElement root = _doc.DocumentElement;
            XmlNodeList tableNodes = root.SelectNodes("//one:Table", _xmlnsManager);
            List<Hardware> hardwareList = new List<Hardware>();

            foreach (XmlNode tableNode in tableNodes)
            {
                List<XmlNode> rowNodes = tableNode.SelectNodes(".//one:Row", _xmlnsManager).Cast<XmlNode>().ToList();
                string currentGroup = "Default group";

                for (int i = 1; i < rowNodes.Count(); i++)
                {
                    // empty row
                    if (string.IsNullOrEmpty(rowNodes[i].ChildNodes[0].InnerText) && 
                        string.IsNullOrEmpty(rowNodes[i].ChildNodes[3].InnerText))
                    {
                        continue;
                    }

                    // group row
                    if (!string.IsNullOrEmpty(rowNodes[i].ChildNodes[0].InnerText) &&
                        string.IsNullOrEmpty(rowNodes[i].ChildNodes[3].InnerText))
                    {
                        currentGroup = GetCellValue(rowNodes[i].ChildNodes[0].InnerText);
                        continue;
                    }

                    Hardware hardware = new Hardware();
                    hardware.Group = currentGroup;
                    hardware.Name = GetCellValue(rowNodes[i].ChildNodes[0].InnerText);
                    hardware.PrimaryAddress = GetCellValue(rowNodes[i].ChildNodes[1].InnerText);
                    hardware.SecondaryAddress = GetCellValue(rowNodes[i].ChildNodes[2].InnerText);
                    hardware.HardwareInfo = GetCellValue(rowNodes[i].ChildNodes[3].InnerText);
                    hardware.ConnectedModuleInfo = GetCellValue(rowNodes[i].ChildNodes[4].InnerText);

                    if (!string.IsNullOrEmpty(hardware.Name) && !string.IsNullOrEmpty(hardware.HardwareInfo))
                    {
                        hardwareList.Add(hardware);
                    }
                }
            }
            return hardwareList;
        }

        private string GetCellValue(string cellContent)
        {
            XmlDocument cellXml = new XmlDocument();
            try
            {
                cellXml.LoadXml(cellContent);
            }
            catch (XmlException)
            {
                return cellContent;
            }

            return cellXml.InnerText;
        }
    }
}
