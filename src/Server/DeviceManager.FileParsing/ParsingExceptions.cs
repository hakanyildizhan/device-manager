// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using System;

namespace DeviceManager.FileParsing
{
    /// <summary>
    /// Thrown when there is a missing column in the uploaded file.
    /// </summary>
    public class MissingColumnException : Exception
    {
        static string exceptionMessage = "Possible missing column in the file";

        public MissingColumnException() : base(exceptionMessage) { }
        public MissingColumnException(Exception innerException) : base(exceptionMessage, innerException) { }
    }

    /// <summary>
    /// Thrown when an error occurs while parsing the data, possibly because of a mismatch between the number of columns of the template and the uploaded file.
    /// </summary>
    public class ParsingException : Exception
    {
        static string exceptionMessage = "Unknown parsing error. File might not be compatible with template";

        public ParsingException() : base(exceptionMessage) { }
        public ParsingException(Exception innerException) : base(exceptionMessage, innerException) { }
    }
}
