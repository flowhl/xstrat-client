using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace xstrat.Core
{
    public static class CSVHelper
    {
        public static DataTable LoadDataTableFromCSV(string pathToFile)
        {
            // Create a new DataTable to hold the data
            DataTable dataTable = new DataTable();

            // Read the CSV file into a stream
            using (StreamReader streamReader = new StreamReader(pathToFile))
            {
                // Read the header row and add columns to the DataTable
                string[] headers = streamReader.ReadLine().Split(',');
                foreach (string header in headers)
                {
                    dataTable.Columns.Add(header);
                }

                // Read the data rows and add them to the DataTable
                while (!streamReader.EndOfStream)
                {
                    string[] values = streamReader.ReadLine().Split(',');
                    DataRow dataRow = dataTable.NewRow();
                    for (int i = 0; i < headers.Length; i++)
                    {
                        dataRow[i] = values[i];
                    }
                    dataTable.Rows.Add(dataRow);
                }
            }

            return dataTable;
        }

        /// <summary>
        /// Returns true when complete, false when failed
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="pathToFile"></param>
        /// <param name="askToOverwrite"></param>
        /// <returns></returns>
        public static bool SaveDataTableToCSV(DataTable dataTable, string pathToFile, bool askToOverwrite = true)
        {
            if (askToOverwrite)
            {
                if (File.Exists(pathToFile))
                {
                    var res = MessageBox.Show("Override File?", $"File: {pathToFile} will be overwritten - continue?", MessageBoxButton.YesNoCancel);
                    if (res != MessageBoxResult.Yes) return false;
                }
            }

            try
            {

                // Write the header row to the CSV file
                using (StreamWriter streamWriter = new StreamWriter(pathToFile))
                {
                    string[] headers = dataTable.Columns.Cast<DataColumn>().Select(column => column.ColumnName).ToArray();
                    streamWriter.WriteLine(string.Join(",", headers));

                    // Write the data rows to the CSV file
                    foreach (DataRow dataRow in dataTable.Rows)
                    {
                        string[] values = dataRow.ItemArray.Select(value => value.ToString()).ToArray();
                        streamWriter.WriteLine(string.Join(",", values));
                    }
                }

            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
    }
}
