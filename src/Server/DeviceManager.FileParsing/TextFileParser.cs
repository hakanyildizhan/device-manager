// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DeviceManager.FileParsing
{
    public class TextFileParser : IParser
    {
        private string _filePath;

        public TextFileParser(string filePath)
        {
            _filePath = filePath;
        }

        public IList<DeviceItem> Parse()
        {
            var rowData = File.ReadAllLines(_filePath);
            var deviceItemList = new List<DeviceItem>();
            string currentGroup = "Default group";

            for (int i = 1; i < rowData.Count(); i++) // first row will be ignored as it is the header row
            {
                var rowCells = rowData[i].Split('\t');

                // empty row
                if (string.IsNullOrEmpty(rowCells[0]) &&
                    string.IsNullOrEmpty(rowCells[3]))
                {
                    continue;
                }

                // group row
                if (!string.IsNullOrEmpty(rowCells[0]) &&
                    string.IsNullOrEmpty(rowCells[3]))
                {
                    currentGroup = rowCells[0];
                    continue;
                }

                deviceItemList.Add(new DeviceItem()
                {
                    Group = currentGroup,
                    Name = rowCells[0],
                    PrimaryAddress = rowCells[1],
                    SecondaryAddress = rowCells[2],
                    HardwareInfo = rowCells[3],
                    ConnectedModuleInfo = rowCells[4]
                });
            }

            return deviceItemList;
        }
    }
}
