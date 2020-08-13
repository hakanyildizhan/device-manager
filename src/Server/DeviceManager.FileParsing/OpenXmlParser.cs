// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Collections.Generic;
using System.Linq;

namespace DeviceManager.FileParsing
{
    public class OpenXmlParser : IParser
    {
        private string _filePath;
        private SharedStringTable _sharedStringTable;

        public OpenXmlParser(string filePath)
        {
            _filePath = filePath;
        }

        public IList<DeviceItem> Parse()
        {
            using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Open(_filePath, false))
            {
                _sharedStringTable = spreadsheetDocument.WorkbookPart.SharedStringTablePart.SharedStringTable;
                WorkbookPart workbookPart = spreadsheetDocument.WorkbookPart;
                WorksheetPart worksheetPart = workbookPart.WorksheetParts.First();
                SheetData sheetData = worksheetPart.Worksheet.Elements<SheetData>().First();
                
                var rowData = sheetData.Elements<Row>().ToList();
                var deviceItemList = new List<DeviceItem>();
                string currentGroup = "Default group";

                for (int i = 1; i < rowData.Count(); i++) // first row will be ignored as it is the header row
                {
                    var rowCells = rowData[i].Elements<Cell>().ToList();

                    // empty row
                    if (string.IsNullOrEmpty(GetCellValue(rowCells, 0)) &&
                        string.IsNullOrEmpty(GetCellValue(rowCells, 3)))
                    {
                        continue;
                    }

                    // group row
                    if (!string.IsNullOrEmpty(GetCellValue(rowCells, 0)) &&
                        string.IsNullOrEmpty(GetCellValue(rowCells, 3)))
                    {
                        currentGroup = GetCellValue(rowCells, 0);
                        continue;
                    }

                    deviceItemList.Add(new DeviceItem()
                    {
                        Group = currentGroup,
                        Name = GetCellValue(rowCells, 0),
                        PrimaryAddress = GetCellValue(rowCells, 1),
                        SecondaryAddress = GetCellValue(rowCells, 2),
                        HardwareInfo = GetCellValue(rowCells, 3),
                        ConnectedModuleInfo = GetCellValue(rowCells, 4)
                    });
                }

                return deviceItemList;
            }
        }

        private string GetCellValue(IList<Cell> rowData, int index)
        {
            if (rowData.Count <= index || string.IsNullOrEmpty(rowData[index].InnerText))
            {
                return string.Empty;
            }

            return _sharedStringTable.ElementAt(int.Parse(rowData[index].InnerText)).InnerText;
        }
    }
}
