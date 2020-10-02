using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;
using RestUtility.Api;
using RestUtility.VeevaVaultXml;

namespace TextTable.Reader
{

    public class VaultXmlDataReader : TextDataReader
    {
        private IEnumerator< List<string[]> > dataPageEnumerator;
        private List<string[]> dataPage;
        private int pageRow;

        //private static System.Collections.Specialized.OrderedDictionary headerNoVersion = new System.Collections.Specialized.OrderedDictionary
        //{ { VaultXmlHelper.IdFieldName, typeof(string).GetType().Name } };
        //private static System.Collections.Specialized.OrderedDictionary headerWithVersion = new System.Collections.Specialized.OrderedDictionary
        //{
        //    { VaultXmlHelper.IdFieldName, typeof(string).GetType().Name },
        //    { VaultXmlHelper.MajorVersionFieldName, typeof(int).GetType().Name },
        //    { VaultXmlHelper.MinorVersionFieldName, typeof(int).GetType().Name }
        //};


        //bool haveWithVersion;

        /// <summary>
        /// Gets the filename of the csv file being read
        /// </summary>
        public string Filename { get; private set; }

        /// <summary>
        /// Creates an instance of CSV reader
        /// </summary>
        /// <param name="connection">Connecton to vault.</param>
        /// <param name="queyr">Vault query to run</param>
        public VaultXmlDataReader(IRestConnection connection, string query)
        {
            if (!(connection != null))
            {
                throw new ArgumentNullException(nameof(connection));
            }

            if (!(query != null))
            {
                throw new ArgumentNullException(nameof(query));
            }

            Contract.EndContractBlock();

            this.dataPageEnumerator = VaultXmlHelper.GetQueryPages(connection, query).GetEnumerator();
            this.AdvancePage();
            string []fieldNames = this.ReadFields();
            this.SetFieldNames(fieldNames);
            IEnumerable<string> dupNames = DuplicateFieldNames(fieldNames);
            if (dupNames.Any())
            {
                throw new TextDataReaderException(string.Format(
                    "Cannot create a Vault XML reader instance with duplicate header\nDuplicate tag names <{0}> found in data row.",
                    string.Join("><", dupNames)));
            }
        }

        private bool AdvancePage()
        {
            if (this.dataPageEnumerator.MoveNext())
            {
                this.dataPage = this.dataPageEnumerator.Current;
                this.pageRow = 0;
                return true;
            }
            return false;
        }
        public override void Close()
        {
            this.dataPage = null;
            this.dataPageEnumerator = null;
        }

        public override bool IsClosed
        {
            get { return this.dataPageEnumerator == null; }
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (!this.IsClosed)
                {
                    this.Close();
                }
            }
        }
        protected override sealed string[] ReadFields()
        {
            string[] result = null;
            bool more = this.pageRow < this.dataPage.Count;
            if (!more)
            {
                more = this.AdvancePage();
            }
            if (more)
            {
                result = this.dataPage[this.pageRow++];
                //if (currentItem.WithVersion != this.haveWithVersion)
                //{
                //    throw new ItemSourceException(string.Format(
                //        "Inconsistent request list - {0}version number supplied for item :{1}",
                //        currentItem.WithVersion ? string.Empty : "no ",
                //        currentItem.Id));
                //}
                //this.fieldValues[0] = currentItem.Id;
                //if (this.haveWithVersion)
                //{
                //    this.fieldValues[1] = currentItem.MajorVersion.ToString();
                //    this.fieldValues[2] = currentItem.MinorVersion.ToString();
                //}            
            }
            return result;
        }
    }
}
