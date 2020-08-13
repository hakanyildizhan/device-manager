// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using System;
using System.IO;

namespace DeviceManager.FileParsing
{
    public static class ParserFactory
    {
        public static IParser CreateParser(string filePath)
        {
            string extension = new FileInfo(filePath).Extension;
            if (extension == ".xlsx")
            {
                return new OpenXmlParser(filePath);
            }
            else if (extension == ".csv")
            {
                return new CsvParser(filePath);
            }
            else if (extension == ".txt")
            {
                return new TextFileParser(filePath);
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
