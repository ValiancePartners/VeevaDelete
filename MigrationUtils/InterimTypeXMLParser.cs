using System;
using System.Xml;
using System.Collections.Specialized;
using System.Collections;
using System.Text;

namespace TRUmigrate_1
{
    /// <summary>
    /// AK, 4/22/16, Mantis #1315, add a parser for Interim Datatype Translation - Internal XML
    /// Converts generic user-friendly SQL data types to an interim datatsource-specific types
    /// </summary>

    public class InterimTypeXMLParser : XMLUtil
    {

        /// <summary>
		/// Constructor
		/// </summary>
		/// <param name="xmlFilePath">File system path of XML file</param>
        public InterimTypeXMLParser(string xmlFilePath)
		{
			try
			{
				xmlDoc.Load(xmlFilePath);
			}
			catch(System.IO.FileNotFoundException nfe)
			{
				throw new TRUMigrateException(xmlFilePath + " is not found", nfe);
			}
			catch(System.Xml.XmlException xmle)
			{
				throw new TRUMigrateException("Error occured when loading " + xmlFilePath, xmle);
			}
			catch (Exception e)
			{
				throw new TRUMigrateException("Error occured when loading " + xmlFilePath, e);

			}
		}

        public ListDictionary InterimDataTypes(int dbType)
        {
            ListDictionary ldDataTypes = new ListDictionary();

            try
            {
                //get connection top nodes
                XmlNodeList connTypes = xmlDoc.SelectSingleNode("Connections").SelectNodes("interim_conn_type");

                //store connection types in ArrayList
                for (int i = 0; i < connTypes.Count; i++)
                {
                    XmlNode connTypeNode = connTypes.Item(i);
                    //get index
                    int connIndex = Convert.ToInt32(connTypeNode.Attributes["index"].InnerText);
                    //if index equals interim db type param, get data
                    if (connIndex == dbType)
                    {
                        //get all child nodes
                        XmlNodeList dataTypes = connTypeNode.SelectNodes("column");
                        for (int j = 0; j < dataTypes.Count; j++)
                        {
                            //data source datatype name
                            string sourceDatatype = dataTypes.Item(j).Attributes["datatype"].InnerText;
                            //coresponding interim datatype name
                            string interimDatatype = dataTypes.Item(j).Attributes["translatesto"].InnerText;
                            //store in dictionary
                            if (!ldDataTypes.Contains(sourceDatatype))
                                ldDataTypes.Add(sourceDatatype, interimDatatype);
                        }
                        break;
                    }
                }

            }
            catch (Exception e)
            {
                string exc = e.StackTrace;
            }

            //return connection types
            return ldDataTypes;

        }
    }
}
