using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;
using Microsoft.VisualBasic.FileIO;


namespace TextTable.Reader
{

    public class CsvDataReader : TextDataReader
    {
        private TextFieldParser csvReader;

        //private StreamReader file;
        private readonly string delimiter;

        private readonly bool firstRowHeader = true;

        private string[] pendingValues = null;

        /// <summary>
        /// Gets the filename of the csv file being read
        /// </summary>
        public string Filename { get; private set; }

        /// <summary>
        /// Creates an instance of CSV reader
        /// </summary>
        /// <param name="filePath">Path to the csv file.</param>
        /// <param name="delimiter">delimiter charecter used in csv file.</param>
        /// <param name="firstRowHeader">specify the csv got a header in first row or not. Default is true and if argument is false then auto header 'ROWthis.xx will be used as per the order of columns.</param>
        public CsvDataReader(string filePath, string delimiter = ",", bool firstRowHeader = true)
        {
            this.Filename = filePath;
            this.delimiter = delimiter;
            this.OpenParser();

            if (this.firstRowHeader == true)
            {
                string[] fieldNames = this.ReadFields();
                this.SetFieldNames(fieldNames);
                IEnumerable<string> dupNames = DuplicateFieldNames(fieldNames);
                if (dupNames.Any())
                {
                    throw new TextDataReaderException(string.Format(
                        "Cannot create a CSV reader instance with duplicate header\nDuplicate columns {0} found in CSV header. ", 
                        string.Join(this.delimiter, dupNames)));
                }
            }
            else
            {
                this.pendingValues = this.csvReader.ReadFields();
                string[] fieldNames = new string[this.pendingValues.Length];
                for(int i = 0;i < pendingValues.Length; ++i)
                {
                    fieldNames[i] = "COL_" + i.ToString();
                }
                this.SetFieldNames(fieldNames);
            }
        }

        private void OpenParser()
        {
            this.csvReader = new TextFieldParser(this.Filename);
            this.csvReader.SetDelimiters(new string[] { this.delimiter });
            this.csvReader.HasFieldsEnclosedInQuotes = true;
        }

        public override void Close()
        {
            this.csvReader.Close();
            this.csvReader.Dispose();
            this.csvReader = null;
        }
        public override bool IsClosed 
        {
             get { return this.csvReader == null; }
        }
        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.csvReader != null)
                {
                    this.csvReader.Dispose();
                    this.csvReader = null;
                }
            }
        }
        protected override sealed string[] ReadFields()
        {
            string[] result = this.pendingValues;
            if (result == null && !this.csvReader.EndOfData)
            {
                result = this.csvReader.ReadFields();
            }
            else
            {
                this.pendingValues = null;
            }
            return result;
        }

    }
}
