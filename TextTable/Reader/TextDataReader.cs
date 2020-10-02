using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;
using Microsoft.VisualBasic.FileIO;
   
namespace TextTable.Reader
{

    public abstract class TextDataReader : IDataReader, IDisposable
    {
        private string byteFieldValue;

        private byte[] byteFieldBytes;

        /// <summary>
        /// Returns an array of strings from the current line of csv file. Call Read() method to read the next line/record of csv file. 
        /// </summary>
        private string[] fieldValues;

        private int recordsaffected;
        private bool iscolumnlocked = false;

        private string[] fieldNames;

        /// <summary>
        /// Creates an instance of CSV reader
        /// </summary>
        /// <param name="filePath">Path to the csv file.</param>
        /// <param name="delimiter">delimiter charecter used in csv file.</param>
        /// <param name="firstRowHeader">specify the csv got a header in first row or not. Default is true and if argument is false then auto header 'ROWthis.xx will be used as per the order of columns.</param>
        public TextDataReader()
        {
            this.iscolumnlocked = false; //setting this to false since above read is called internally during constructor and actual user read() didnot start.
            //this.csvlinestring = "";
            this.fieldValues = null;
            this.recordsaffected = 0;
        }
        public bool Read()
        {
            this.fieldValues = this.ReadFields();
            bool result = this.fieldValues != null;
            if (result)
            {
                for (int i = 0; i < this.fieldValues.Length; i++)
                {
                    if (string.IsNullOrEmpty(this.fieldValues[i]))
                    {
                        this.fieldValues[i] = null;
                    }
                }
                ++this.recordsaffected;
            }
            if (this.iscolumnlocked == false)
                this.iscolumnlocked = true;
            return result;
        }

        /// <summary>
        /// Gets a value that indicates the depth of nesting for the current row.
        /// </summary>
        public int Depth
        {
            get { return 1; }
        }

        public DataTable GetSchemaTable()
        {
            DataTable t = new DataTable();
            t.Columns.AddRange(
                new System.Data.DataColumn[]
                {
                    new DataColumn { ColumnName="ColumnName"         , DataType = typeof(string      ) },
                    new DataColumn { ColumnName="ColumnOrdinal"      , DataType = typeof(int         ) },
                    new DataColumn { ColumnName="ColumnSize"         , DataType = typeof(int         ) },
                    new DataColumn { ColumnName="NumericPrecision"   , DataType = typeof(int         ) },
                    new DataColumn { ColumnName="NumericScale"       , DataType = typeof(int         ) },
                    new DataColumn { ColumnName="DataType"           , DataType = typeof(Type        ) },
                    new DataColumn { ColumnName="ProviderType"       , DataType = typeof(int         ) },
                    new DataColumn { ColumnName="IsLong"             , DataType = typeof(bool        ) },
                    new DataColumn { ColumnName="AllowDBNull"        , DataType = typeof(bool        ) },
                    new DataColumn { ColumnName="IsReadOnly"         , DataType = typeof(bool        ) },
                    new DataColumn { ColumnName="IsRowVersion"       , DataType = typeof(bool        ) },
                    new DataColumn { ColumnName="IsUnique"           , DataType = typeof(bool        ) },
                    new DataColumn { ColumnName="IsKey"              , DataType = typeof(bool        ) },
                    new DataColumn { ColumnName="IsAutoIncrement"    , DataType = typeof(bool        ) },
                    new DataColumn { ColumnName="BaseCatalogName"    , DataType = typeof(string      ) },
                    new DataColumn { ColumnName="BaseSchemaName"     , DataType = typeof(string      ) },
                    new DataColumn { ColumnName="BaseTableName"      , DataType = typeof(string      ) },
                    new DataColumn { ColumnName="BaseColumnName"     , DataType = typeof(string      ) },
                    new DataColumn { ColumnName="AutoIncrementSeed"  , DataType = typeof(long        ) },
                    new DataColumn { ColumnName="AutoIncrementStep"  , DataType = typeof(long        ) },
                    new DataColumn { ColumnName="DefaultValue"       , DataType = typeof(object      ) },
                    new DataColumn { ColumnName="Expression"         , DataType = typeof(string      ) },
                    new DataColumn { ColumnName="ColumnMapping"      , DataType = typeof(MappingType ) },
                    new DataColumn { ColumnName="BaseTableNamespace" , DataType = typeof(string      ) },
                    new DataColumn { ColumnName="BaseColumnNamespace", DataType = typeof(string      ) },
                });

            int fieldNo = 0;
            foreach (string fieldName in this.fieldNames)
            {
                DataRow r = t.NewRow();
                ++fieldNo;
                r.ItemArray = new object[]
                {
                    fieldName,
                    fieldNo,
                    -1,  //-1 if ColumnSize(or MaxLength) property of the DataColumn cannot be determined or is not relevant; otherwise, 0 or a positive integer that contains the MaxLength value.
                    null,//NumericPrecision null
                    null,// NumericScale null
                    typeof(string),// DataType
                    0,// ProviderType
                    false,// IsLong true if the data type of the column is String and its MaxLength property is -1.Otherwise, false.
                    true,// AllowDBNull
                    true,// IsReadOnly true
                    false,// IsRowVersion false
                    false,// IsUnique false
                    false,// IsKey false
                    false,// IsAutoIncrement false
                    null,// BaseCatalogName null
                    null,// BaseSchemaName  null
                    fieldName,// BaseTableName
                    string.Empty,// BaseColumnName
                    0,// AutoIncrementSeed
                    0,// AutoIncrementStep
                    null,// DefaultValue null
                    null,// Expression null
                    MappingType.Element,// ColumnMapping "Element"
                    string.Empty,// BaseTableNamespace
                    string.Empty// BaseColumnNamespace
                };
                t.Rows.Add(r);
            }
            return t;
        }

        public abstract bool IsClosed { get; }

        public bool NextResult()
        {
            return Read();
        }


        /// <summary>
        /// Retuens how many records read so far.
        /// </summary>
        public int RecordsAffected
        {
            get { return recordsaffected; }
        }

        /// <summary>
        /// Gets the number of columns in the current row.
        /// </summary>
        public int FieldCount
        {
            get { return this.fieldNames.Length; }
        }

        public bool GetBoolean(int i)
        {
            return Boolean.Parse(this.fieldValues[i]);
        }

        public byte GetByte(int i)
        {
            return Byte.Parse(this.fieldValues[i]);
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            string fieldValue = this.GetString(i);
            if(this.byteFieldValue != fieldValue)
            {
                this.byteFieldValue = fieldValue;
                Encoding u8 = Encoding.UTF8;
                this.byteFieldBytes = u8.GetBytes(this.GetString(i));
            }
            long extractLength = this.byteFieldBytes.Length - fieldOffset;
            if(length< extractLength)
            {
                extractLength = length;
            }
            if(extractLength > 0)
            {
                Array.Copy(this.byteFieldBytes, (int)fieldOffset, buffer, bufferoffset, (int)extractLength);
            }
            return extractLength;
        }

        public char GetChar(int i)
        {
            return Char.Parse(this.fieldValues[i]);
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        public IDataReader GetData(int i)
        {
            return (IDataReader)this;
        }

        public string GetDataTypeName(int i)
        {
            return typeof(string).GetType().Name;
        }

        public DateTime GetDateTime(int i)
        {
            return DateTime.Parse(this.fieldValues[i]);
        }

        public decimal GetDecimal(int i)
        {
            return Decimal.Parse(this.fieldValues[i]);
        }

        public double GetDouble(int i)
        {
            return Double.Parse(this.fieldValues[i]);
        }

        public Type GetFieldType(int i)
        {
            return typeof(String);
        }

        public float GetFloat(int i)
        {
            return float.Parse(this.fieldValues[i]);
        }

        public Guid GetGuid(int i)
        {
            return Guid.Parse(this.fieldValues[i]);
        }

        public short GetInt16(int i)
        {
            return Int16.Parse(this.fieldValues[i]);
        }

        public int GetInt32(int i)
        {
            return Int32.Parse(this.fieldValues[i]);
        }

        public long GetInt64(int i)
        {
            return Int64.Parse(this.fieldValues[i]);
        }

        public string GetName(int i)
        {
            return this.fieldNames[i];
        }

        public int GetOrdinal(string name)
        {
            return Array.IndexOf(this.fieldNames,name);
        }

        public string GetString(int i)
        {
            return this.fieldValues[i];
        }

        public object GetValue(int i)
        {
            return this.fieldValues[i];
        }

        public int GetValues(object[] values)
        {
            int columnCount = fieldValues.Length < values.Length ? fieldValues.Length : values.Length;
            Array.Copy(this.fieldValues, values, columnCount);
            return columnCount;
        }

        public bool IsDBNull(int i)
        {
            return string.IsNullOrWhiteSpace(this.fieldValues[i]);
        }

        public object this[string name]
        {
            get { return this.fieldValues[GetOrdinal(name)]; }
        }

        public object this[int i]
        {
            get { return this.GetValue(i); }
        }
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected abstract void Dispose(bool disposing);

        public abstract void Close();

        public static IEnumerable<string> DuplicateFieldNames(string[] fieldNames)
        {
            IEnumerable<string> dupNames = fieldNames.GroupBy(name => name).Where(group => group.Count() > 1).Select(group => group.Key);
            return dupNames;
        }

        protected void SetFieldNames(string[] fieldNames)
        {
            this.fieldNames = fieldNames;
        }

        protected abstract string[] ReadFields();


    }
}
