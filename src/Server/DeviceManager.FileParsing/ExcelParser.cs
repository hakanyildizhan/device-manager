using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManager.FileParsing
{
    public class ExcelParser : IParser
    {
        private string _filePath;

        public ExcelParser(string filePath)
        {
            _filePath = filePath;
        }

        public virtual ExcelDataSetConfiguration ExcelDatasetConfiguration
        {
            get
            {
                return new ExcelDataSetConfiguration()
                {
                    // Gets or sets a value indicating whether to set the DataColumn.DataType 
                    // property in a second pass.
                    UseColumnDataType = true,
                    
                    // Gets or sets a callback to obtain configuration options for a DataTable. 
                    ConfigureDataTable = (tableReader) => new ExcelDataTableConfiguration()
                    {
                        // Gets or sets a value indicating the prefix of generated column names.
                        EmptyColumnNamePrefix = "EmptyColumn",

                        // Gets or sets a value indicating whether to use a row from the 
                        // data as column names.
                        UseHeaderRow = true,

                        // Gets or sets a callback to determine which row is the header row. 
                        // Only called when UseHeaderRow = true.
                        ReadHeaderRow = (rowReader) =>
                        {
                            // F.ex skip the first row and use the 2nd row as column headers:
                            // rowReader.Read();
                        },

                        // Gets or sets a callback to determine whether to include the 
                        // current row in the DataTable.
                        FilterRow = (rowReader) =>
                        {
                            return true;
                        },

                        // Gets or sets a callback to determine whether to include the specific
                        // column in the DataTable. Called once per column after reading the 
                        // headers.
                        FilterColumn = (rowReader, columnIndex) =>
                        {
                            return true;
                        }
                    }
                };
            }
        }

        public IList<Hardware> Parse()
        {
            try
            {
                using (var stream = File.Open(_filePath, FileMode.Open, FileAccess.Read))
                {
                    // Supports *.xls & *.xlsx files
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        var result = reader.AsDataSet(ExcelDatasetConfiguration).AsHardwareData();
                        return result;
                    }
                }
            }
            catch (IndexOutOfRangeException)
            {
                // TODO: log this
                throw new MissingColumnException();
            }
            catch (Exception)
            {
                // TODO: log this
                throw new ParsingException();
            }
        }
    }

    public static class ExtensionMethods
    {
        public static List<Hardware> AsHardwareData(this DataSet dataSet)
        {
            var offers = new List<Hardware>();
            string currentGroup = "Default group";

            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {
                // empty row
                if (string.IsNullOrEmpty(dataSet.Tables[0].Rows[i][0].ToString()) &&
                    string.IsNullOrEmpty(dataSet.Tables[0].Rows[i][3].ToString()))
                {
                    continue;
                }

                // group row
                if (!string.IsNullOrEmpty(dataSet.Tables[0].Rows[i][0].ToString()) &&
                    string.IsNullOrEmpty(dataSet.Tables[0].Rows[i][3].ToString()))
                {
                    currentGroup = dataSet.Tables[0].Rows[i][0].ToString();
                    continue;
                }

                offers.Add(new Hardware()
                {
                    Group = currentGroup,
                    Name = dataSet.Tables[0].Rows[i][0].ToString(),
                    PrimaryAddress = dataSet.Tables[0].Rows[i][1].ToString(),
                    SecondaryAddress = dataSet.Tables[0].Rows[i][2].ToString(),
                    HardwareInfo = dataSet.Tables[0].Rows[i][3].ToString(),
                    ConnectedModuleInfo = dataSet.Tables[0].Rows[i][4].ToString()
                });
            }

            return offers;
        }
    }
}
