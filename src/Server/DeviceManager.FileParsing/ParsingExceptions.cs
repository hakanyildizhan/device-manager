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
        public MissingColumnException() : base("An error occurred while parsing the file. Please make sure there is no missing column on your file.")
        {
        }
    }

    /// <summary>
    /// Thrown when an error occurs while parsing the data, possibly because of a mismatch between the number of columns of the template and the uploaded file.
    /// </summary>
    public class ParsingException : Exception
    {
        public ParsingException() : base("An error occurred while parsing the file. Please make sure your file is compatible with the template.")
        {
        }
    }
}
