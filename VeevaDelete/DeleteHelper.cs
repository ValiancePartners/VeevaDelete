using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VeevaDelete
{
    class DeleteHelper
    {
        public static string getDeleteURL(ConnectionUtil.DocumentObject docObject)
        {
            string deleteURLformat = String.Empty;

            switch (docObject.GetDocType())
            {
                case "documents":
                    //case "radDocVer":
                    deleteURLformat = "objects/documents/{0}";
                    break;  

                case "binders":
                    // case "radBin":
                    //Delete binder.
                    deleteURLformat = "objects/binders/{0}";
                    break;

                //case "radObject":
                case "objects":

                    deleteURLformat = "vobjects/{1}/{0}";
                    break;

                // Doug 10/26/2017
                case "parents":
                    // testing the Grandparent-down API delete, a cascade of FUN!
                    // this is similar to "radObject" but it adds the suffix for cascadedelete
                    // vobjects/{object_name}/{object_record_id}/actions/cascadedelete
                    deleteURLformat = "vobjects/{1}/{0}/actions/cascadedelete";
                    break;

                default:
                    deleteURLformat = "--";
                    break;
            }
            
            //Return url.
            string majorVersion = docObject.GetMajorVersion();
            string minorVersion = docObject.GetMinorVersion();
            if (!string.IsNullOrEmpty(majorVersion))
            {
                deleteURLformat += "/versions/{2}/{3}";
            }
            string deleteURL=string.Format(deleteURLformat, docObject.GetID(), docObject.GetObjectName(), majorVersion, minorVersion);
            return deleteURL;
         }

        

        /// <summary>
        /// Method to calculate the number of records that match the VQL Query
        /// </summary>
        /// <param name="vql">VQL query</param>
        /// <returns>number of records</returns>
        public static int getCount(string vql,IDBConnectionUtil connUtil)
        {
          //  IDBConnectionUtil connUtil = new ConnectionUtil();
            int numberOfRecs = -1;
            if (connUtil.TestQuery(vql, ref numberOfRecs) == true)
            {
                return numberOfRecs;
            }
            else
            {
                return -1;
            }
            
        }
    }
}
