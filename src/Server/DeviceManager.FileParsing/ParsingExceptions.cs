using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
