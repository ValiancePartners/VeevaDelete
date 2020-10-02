using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Net;
using System.IO;
using System.Collections;
using System.Data;
using System.Data.OleDb;

namespace VeevaDelete
{
    class ConnectionUtil: IDBConnectionUtil
    {
        private string defaultURL = "";
        private string sessionID = "";
        private bool xmlError = false;
        private string xmlApiError = "";

        public event EventHandler<LimitArgs> LimitEventOne;

        public event EventHandler<LimitArgs> LimitUpdate;
        private void OnLimitUpdate(object sender, LimitArgs e)
        {
            if (LimitUpdate != null)
            {
                LimitUpdate(sender, e);
            }
        }
        public bool OpenConnection(string[] connParams)
        {
            bool status = false;

            try
            {
                string URL = connParams[0];
                string userName = connParams[1];
                string password = connParams[2];
                string version = connParams[3];

                //Constructing Default URL to use with Queries 

                StringBuilder sBuilder = new StringBuilder();
                sBuilder.AppendFormat("{0}/api/{1}/", URL, version);
                defaultURL = sBuilder.ToString();

                sBuilder.Clear();

                sBuilder.AppendFormat("{0}auth?username={1}&password={2}", defaultURL, userName, password);

                string vaultURL = sBuilder.ToString();

                XDocument responseXml = executeVeevaAPI("POST", vaultURL);

                if (responseXml != null)
                {
                    //sessionId = responseXml.Descendants("VersionedAuthRestResult").Select(s => s.Element("sessionId").Value).FirstOrDefault();
                    sessionID = responseXml.Root.Element("sessionId").Value;
                    status = true;
                }
                else
                {
                    status = false;
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return status;
        }

        /// <summary>
        /// Method that connects to veevaVault
        /// </summary>
        /// <param name="method"></param>
        /// <param name="vaultURL"></param>
        /// <returns>XDocument with sessionID in it on success and null on failure</returns>
        private XDocument executeVeevaAPI(string method, string vaultURL)
        {
            string connURL;
          //  VeevaDelete vdObject = new VeevaDelete();
           //LimitUpdate += new EventHandler<LimitArgs>(vdObject.UpdateLimits);
            //Append Default URL to vaultURL if does not exitst. This the case with VQL Queries 
            if (vaultURL.IndexOf(defaultURL) < 0)
            {
                connURL = defaultURL + vaultURL;
            }
            else
            {
                connURL = vaultURL;
            }

            
            HttpWebRequest request;

            try
            {
                request = (HttpWebRequest)WebRequest.Create(connURL);
            }
            catch (Exception e)
            {
                throw e;
            }

           
            request.Method = method;
            request.Accept = "application/xml";

            request.Headers.Add("authorization", sessionID);

            request.ContentType = "application/x-www-form-urlencoded";
            request.Timeout = 300000;

            WebResponse response = null;
            string webErrorStatus = "";
            bool canExit = false;
            int count = 0;
            while (!canExit && count < 10)
            {
                try
                {
                    //Count tries.
                    count++;

                    //Get the response.
                    response = request.GetResponse();

                    //Set flag as success.
                    canExit = true;
                }
                catch (WebException webExcp)
                {
                    //Get the WebException status code.
                    WebExceptionStatus status = webExcp.Status;

                    //Take a break and try again.
                    System.Threading.Thread.Sleep(100);

                    //Save the failed status to log later.
                    if (string.IsNullOrEmpty(webErrorStatus))
                        webErrorStatus = status.ToString();
                }
            }

            Stream outputStream = response.GetResponseStream();

            StreamReader outputStreamReader = new StreamReader(outputStream);

            string responseStr = outputStreamReader.ReadToEnd();

            String dailyLimit = "-";
            String burstLimit = "-";
            int flag = 0;
            for(int i =0; i<response.Headers.Count; i++)
            {
                
                if (response.Headers.Keys[i] == "X-VaultAPI-BurstLimitRemaining")
                {
                    burstLimit = response.Headers[i];
                    flag = 1;
                }
                if (response.Headers.Keys[i] == "X-VaultAPI-DailyLimitRemaining")
                {
                    dailyLimit = response.Headers[i];
                    flag = 1;
                }

            }
            if (flag == 1)
            {
                //always call the wrapper function
                OnLimitUpdate(this, new LimitArgs(burstLimit, dailyLimit));
                
              //  LimitUpdate(this, new LimitArgs(burstLimit, dailyLimit));

            }
            response.Close();
            XDocument responseXML = null;

            if (!string.IsNullOrEmpty(responseStr))
            {

                responseXML = XDocument.Parse(responseStr);

                if (GetXMLStatus(responseXML) == false)
                {
                    responseXML = null;
                    xmlError = true;
                }
                else
                {
                    xmlError = false;
                }
            }

            //Return object.
            return responseXML;
        }

        private bool GetXMLStatus(XDocument responseXML)
        {
            bool status = false;

            //var result = responseXML.Element("VersionedAuthRestResult").Descendants();

            //string responseStatus = responseXML.Descendants("responseStatus").FirstOrDefault().Value;
            string responseStatus = responseXML.Root.Element("responseStatus").Value;
            try
            {
                if (responseStatus.ToUpper().Equals("SUCCESS"))
                {
                    status = true;
                    xmlApiError = "";
                }
                else
                {
                    //   string responseMessage = responseXML.Descendants("VersionedAuthRestResult").Select(s => s.Element("responseMessage").Value).FirstOrDefault();
                  //  string responseMessage = responseXML.Descendants().Select(s => s.Element("responseMessage").Value).FirstOrDefault();
                   // string responseMessage = responseXML.Root.Element("responseMessage").Value;
                    XElement errors = responseXML.Root.Elements("errors").FirstOrDefault();
                    string responseMessage = errors.Element("error").Element("type").Value;
                    responseMessage = responseMessage + "--" + errors.Element("error").Element("message").Value;
                    xmlApiError = responseMessage;
                    status = false;
                }

            }
            catch (Exception e)
            {
                throw e;
            }
            return status;
        }

        public bool TestQuery(string vql, ref int numberOfRecs)
        {
            //Remove any line breaks.
            vql = vql.Replace("\n", "");
            vql = vql.Replace("\r", "");

            //Use method to get record count to test query.
            numberOfRecs = GetRecordCount(vql);

            //Evaluate results.
            if (numberOfRecs < 0)
                return false;
            else
                return true;
        }

        public int GetRecordCount(string vql)
        {
            //Instantiate object.
            string recordCount = "-1";

            //Generate URL to return one item.
            string url = "query?q=" + vql;  //+ " limit 1";

            //API call.
            XDocument responseXml = executeVeevaAPI("GET", url);

            //Evaluate response.
            if (responseXml != null && xmlError == false)
            {

                //Parse XML.
                if (vql.ToLower().Contains(" limit ").Equals(true))
                {
                    recordCount = responseXml.Descendants("row").Count().ToString();
                }
                else
                {
                    XElement totalXElement = responseXml.Descendants("total").FirstOrDefault();
                    if(totalXElement != null)
                    {
                        recordCount = totalXElement.Value.ToString();
                    }
                    
                }
            }

            //Return count.
            return Convert.ToInt32(recordCount);
        }

        public Hashtable ExecuteSql(VeevaDelete.ConnectionObject connectionObject)
        {
            Hashtable htResults = null;


            int success = 0;
            int failure = 0;

            List<DocumentObject> docList = CreateDocObject(connectionObject);

                if (docList != null)
                {
                    htResults = new Hashtable();

                    int rowCount = docList.Count;

                    if (rowCount > 0)
                        VeevaDelete.updateLogs("Attempting to Delete " + rowCount + " id(s)...");
                    else
                        VeevaDelete.updateLogs("No id(s) found to delete.");
                    for (int i = 0; i < rowCount; i++)
                    {
                        DocumentObject docObj = docList[i];
                        bool status = ExecuteTypeSQL(connectionObject,docObj);
                        if (status)
                        {
                            success++;
                        }
                        else
                        {
                            failure++;
                        }
                    }
					// MP: 09-Jan-2018, Veeva Delete Item #1
                    htResults.Add(0, success);
                    htResults.Add(1, failure);
                }
            
            VeevaDelete.updateLogs("Deletion completed.");
            return htResults;
        }


        /// <summary>
        /// for every last stinking type, create a list of the items found in the source ( VQL or file )
        /// 
        /// </summary>
        /// <param name="connectionObject"></param>
        /// <returns></returns>
        public List<DocumentObject> CreateDocObject(VeevaDelete.ConnectionObject connectionObject)
        {
            // this appears to be local only and not used.
            //List<string> ids = new List<string>();

            string inputType = connectionObject.GetInputType();
            string inputPath = connectionObject.GetInputPath();
            List<DocumentObject> docList = new List<DocumentObject>();
            string vqlType = connectionObject.GetVqlType();
            bool withVersion = connectionObject.GetWithVersion();
            string objectName = connectionObject.GetObjectName();
            switch (inputType)
            {
                case "VQL":
                    string vql = connectionObject.GetVql();
                    string url = "query?q=" + vql;
                    XDocument responseXml = executeVeevaAPI("GET", url);
                    if (responseXml != null && xmlError == false)
                    {
                        string majorVersion = connectionObject.GetMajorVersion();
                        string minorVersion = connectionObject.GetMinorVersion();

                        List<XElement> rowValues = new List<XElement>(responseXml.Descendants("row"));
                        if (rowValues != null)
                        {
                            int rowCount = rowValues.Count;
                            for (int i = 0; i < rowCount; i++)
                            {
                                DocumentObject tempDocObject = new DocumentObject(rowValues[i].Value, majorVersion, minorVersion, vqlType, objectName);
                                docList.Add(tempDocObject);
                                //ids.Add(rowValues[i].Value);
                            }
                        }
                    }
                    break;
                case "csv":
                    var parser = new Microsoft.VisualBasic.FileIO.TextFieldParser(inputPath);
                    parser.TextFieldType = Microsoft.VisualBasic.FileIO.FieldType.Delimited;
                    parser.SetDelimiters(new string[] { "," });
                    if(!parser.EndOfData)
                    {
                        parser.ReadFields();
                    }
                    while (!parser.EndOfData)
                    {
                        var values= parser.ReadFields();
                        if (values.Count()>0)
                        {
                            string majorVersion = withVersion && values.Count() > 1 ? values[1] : null;
                            string minorVersion = withVersion && values.Count() > 2 ? values[2] : null;
                            DocumentObject tempDocObject = new DocumentObject(values[0], majorVersion, minorVersion, vqlType, objectName);
                            //ids.Add(values[0]);
                            docList.Add(tempDocObject);
                        }
                    }

                    break;
                case "Excel":
                    string excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source= " + inputPath + ";Extended Properties=\"Excel 12.0 XML;HDR=YES\"";
                    {
                        object []restrictions = null;
                        DataRowCollection dataRows = GetDataRows(excelConnectionString, restrictions);
                        docList = CreateDocObjectFromDataRows(dataRows, vqlType, withVersion, objectName);
                    }
                    break;
                case "Access":
                    string accessConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source= " + inputPath;
                    {
                        object[] restrictions = new string[4];
                        DataRowCollection dataRows = GetDataRows(accessConnectionString, restrictions);
                        docList = CreateDocObjectFromDataRows(dataRows, vqlType, withVersion, objectName);
                    }
                    break;
                }
                return docList;
            }


        private DataRowCollection GetDataRows(string connectionString, object []restrictions)
        {
                DataSet dataSet = new DataSet();
                string tableName;
                //restrictions[3] = "Table";
                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    conn.Open();
                    OleDbCommand cmd = new OleDbCommand();
                    cmd.Connection = conn;

                    //Get all sheets
                    DataTable docTables = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, restrictions);


                    DataRow dr = docTables.Rows[0];
                    tableName = dr["TABLE_NAME"].ToString();
                    cmd.CommandText = "SELECT * FROM [" + tableName + "]";
                    DataTable dt = new DataTable();
                    dt.TableName = tableName;

                    OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                    da.Fill(dt);

                    dataSet.Tables.Add(dt);

                    cmd = null;
                    conn.Close();
                }
                    
                DataRowCollection dataRows = dataSet.Tables[tableName].Rows;
                return dataRows;
        }

        public List<DocumentObject> CreateDocObjectFromDataRows(DataRowCollection dataRows,string vqlType, bool withVersion, string objectName)
        {
            List<DocumentObject> docList = new List<DocumentObject>(); 
            foreach (DataRow dr in dataRows)
            {
                if (dr.ItemArray.Count() > 0)
                {
                    string majorVersion = withVersion && dr.ItemArray.Count() > 1 ? dr[1].ToString() : null;
                    string minorVersion = withVersion && dr.ItemArray.Count() > 2 ? dr[2].ToString() : null;
                    DocumentObject tempDocObject = new DocumentObject(dr[0].ToString(), majorVersion, minorVersion, vqlType, objectName);
                }
            }
            return docList;
        }

        private string GetExcelConnectionString()
        {
            throw new NotImplementedException();
        }

        public bool ExecuteTypeSQL(VeevaDelete.ConnectionObject connectionObject,DocumentObject docObject)
        {
            string deleteURL = DeleteHelper.getDeleteURL(docObject);
            XDocument xmlDeleteResponse = executeVeevaAPI("DELETE", deleteURL);
            bool success_status;

            if (xmlError == false)
            {
                VeevaDelete.updateLogs("Document with id = " + docObject.GetID() + " has been succesfully deleted");
                success_status = true;
                
                
            }
            else
            {
                success_status = false;
                VeevaDelete.updateLogs("Failed to delete id : " + docObject.GetID());
                VeevaDelete.updateLogs("Failed url : " + defaultURL + deleteURL);
                VeevaDelete.updateLogs("API error : " + xmlApiError);
               
            }

            UpdateFile(connectionObject, success_status, docObject.GetID());

            return success_status;
        }

        private void UpdateFile(VeevaDelete.ConnectionObject connectionObject, bool success_status, string ID)
        {
            string inputType = connectionObject.GetInputType();
            string inputPath = connectionObject.GetInputPath();
            string vqlType = connectionObject.GetVqlType();
            switch (inputType)
            {
                case "VQL":
                    break;
                case "csv":
                    break;
                case "Excel":
                    string sheetName;
                     string excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source= " + inputPath + ";Extended Properties=\"Excel 12.0 XML;HDR=YES\"";

                     using (OleDbConnection conn = new OleDbConnection(excelConnectionString))
                     {
                        conn.Open();
                        OleDbCommand cmd = new OleDbCommand();
                        cmd.Connection = conn;

                        //Get all sheets
                        DataTable docSheets = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

                        DataRow dr = docSheets.Rows[0];
                        sheetName = dr["TABLE_NAME"].ToString();
                        string sql;
                        if (success_status == true)
                        {
                            sql = "Update [" + sheetName + "] set Status='Deleted' where ID=" + ID;
                        }
                        else
                        {
                            sql = "Update [" + sheetName + "] set Status='Error-Not Deleted' where ID='" + ID +"'";
                        }

                        cmd.CommandText = sql;
                        cmd.ExecuteNonQuery();
                        cmd = null;
                        conn.Close();
                    }

                    break;
                case "Access":

                    string tableName;
                    string accessConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source= " + inputPath;

                     using (OleDbConnection conn = new OleDbConnection(accessConnectionString))
                     {
                        conn.Open();
                        OleDbCommand cmd = new OleDbCommand();
                        cmd.Connection = conn;

                        //Get all sheets
                        string[] restrictions = new string[4];
                        restrictions[3] = "Table";
                        DataTable docSheets = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, restrictions);

                        DataRow dr = docSheets.Rows[0];
                        tableName = dr["TABLE_NAME"].ToString();
                        string sql;
                        if (success_status == true)
                        {
                            sql = "Update [" + tableName + "] set Status = 'Deleted' where ID ='" + ID + "'";
                        }
                        else
                        {
                            sql = "Update [" + tableName + "] set Status = 'Error-Not deleted' where ID ='" + ID + "'";
                        }

                        cmd.CommandText = sql;
                        cmd.ExecuteNonQuery();
                        cmd = null;
                        conn.Close();
                    }
                    break;
            }
        }



        public class DocumentObject
        {
            string id, majorVersion, minorVersion, docType, objectName;

            /// <summary>
            /// This is a data only class for holding some document information so it can be deleted.
            /// </summary>
            /// <param name="id"></param>
            /// <param name="majorVersion"></param>
            /// <param name="minorVersion"></param>
            /// <param name="docType"></param>
            /// <param name="objectName"></param>
            public DocumentObject(string id, string majorVersion, string minorVersion, string docType, string objectName)
            {
                this.id = id;
                this.majorVersion = majorVersion;
                this.minorVersion = minorVersion;
                this.docType = docType;
                this.objectName = objectName;
            }

            public string GetMajorVersion()
            {
                return this.majorVersion;
            }

            public string GetMinorVersion()
            {
                return this.minorVersion;
            }

            public string GetID()
            {
                return this.id;
            }

            public string GetDocType()
            {
                return this.docType;
            }

            public string GetObjectName()
            {
                return this.objectName;
            }

        }
    }
}
