// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualBasic.FileIO;

namespace DeviceManager.FileParsing
{
    class CsvParser : IParser
    {
        private string _filePath;

        public CsvParser(string filePath)
        {
            _filePath = filePath;
        }

        public IList<DeviceItem> Parse()
        {
            TextFieldParser parser = new TextFieldParser(_filePath);
            parser.HasFieldsEnclosedInQuotes = true;
            parser.SetDelimiters(System.Globalization.CultureInfo.CurrentCulture.TextInfo.ListSeparator);

            var deviceItemList = new List<DeviceItem>();
            string currentGroup = "Default group";

            if (!parser.EndOfData) // first row will be ignored as it is the header row
            {
                parser.ReadLine();
            }

            while (!parser.EndOfData)
            {
                List<string> rowData = parser.ReadFields().ToList();

                // empty row
                if (string.IsNullOrEmpty(rowData[0]) &&
                    string.IsNullOrEmpty(rowData[3]))
                {
                    continue;
                }

                // group row
                if (!string.IsNullOrEmpty(rowData[0]) &&
                    string.IsNullOrEmpty(rowData[3]))
                {
                    currentGroup = rowData[0];
                    continue;
                }

                deviceItemList.Add(new DeviceItem()
                {
                    Group = currentGroup,
                    Name = rowData[0],
                    PrimaryAddress = rowData[1],
                    SecondaryAddress = rowData[2],
                    HardwareInfo = rowData[3],
                    ConnectedModuleInfo = rowData[4]
                });
            }

            return deviceItemList;
        }
    }
}
