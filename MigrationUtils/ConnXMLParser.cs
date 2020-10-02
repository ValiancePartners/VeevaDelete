//-----------------------------------------------------------------------
// <copyright file="ConnXMLParser.cs" company="Valiance Partners">
//     Copyright (c) Valiance Partners.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Xml;
using System.Collections;
using System.Collections.Specialized;
using System.Text;

namespace TRUmigrate_1
{
	/// <summary>
	/// A parser for internal connections definition XML
	/// The XML stores a list of available connections
	/// and defines connection parameters
	/// </summary>
	public class ConnXMLParser:XMLUtil
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="xmlFilePath">File system path of XML file</param>
		public ConnXMLParser(string xmlFilePath)
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


		/// <summary>
		/// Parses connections XML and retrieves the list of available connections
		/// </summary>
		/// <returns>ListDictionary of connection names and types pairs</returns>
		public ListDictionary ConnectionsList()
		{
			ListDictionary ldDataSources = new ListDictionary();

			try 
			{
				//get connection types list
				XmlNodeList typesList = xmlDoc.SelectSingleNode("types").SelectNodes("type");
			
				//store connection types in ArrayList
				for (int i=0;i<typesList.Count;i++)
				{
					string typeName = typesList.Item(i).SelectSingleNode("type_name").InnerText;
					string connType = typesList.Item(i).SelectSingleNode("conn_type").InnerText;
					int iConnType = Convert.ToInt32(connType);
					ldDataSources.Add(typeName, iConnType);
				}

			}
			catch(Exception e)
			{
				string exc = e.StackTrace;
			}

			//return connection types
			return ldDataSources;

		}
		/// <summary>
		/// Gets parameters required for the selected connection
		/// </summary>
		/// <param name="connType">Selected connection type</param>
		/// <returns>ListDictionary containing parameter label and correspoding
		/// connection string IDs pairs</returns>
		public ListDictionary GetConnParams(string connType)
		{
			ListDictionary ldParams = new ListDictionary();

			try
			{
				XmlNode paramsNode = FindParamsNode(connType);

				//get connection params
				if(paramsNode!=null)
				{
					//get parameters nodes
					XmlNodeList connParams = paramsNode.SelectNodes("param");

					//loop through parameter nodes
					for (int j=0;j<connParams.Count;j++)
					{
						//get param node
						XmlNode connParam = connParams.Item(j);
						string paramId="";
						//get parameter's connection string id
						if(connParam.SelectSingleNode("conn_str_id")!=null)
							paramId = connParam.SelectSingleNode("conn_str_id").InnerText;

						//get parameter's label
						string paramName = connParam.SelectSingleNode("param_name").InnerText;

						//store in hashtable
						ldParams.Add(paramId,paramName);
					}
				}
			}
			catch(Exception e)
			{
				string exc = e.StackTrace;
			}

			//return parameters collection
			return ldParams;
			
		}


		/// <summary>
		/// Builds data source connection string following connection string
		/// rules defined in XML for the given connection type
		/// </summary>
		/// <param name="htConnData">Hashtable containing connection info</param>
		/// <param name="connType">Connection Type</param>
		/// <param name="encryptPassword">Flag indicating whether the password should be encrypted</param>
		/// <returns>Connection string</returns>
		public string BuildConnString(Hashtable htConnData, string connType)
		{
			StringBuilder sbConnStr = new StringBuilder();

			try
			{


				//generate connection string
				IEnumerator enKeys = htConnData.Keys.GetEnumerator();

				for(int i=0;i<htConnData.Count;i++)
				{
					enKeys.MoveNext();
					//get connection string ID
					string connStrId = enKeys.Current.ToString();
					//get value
					string connStrVal = (string)htConnData[connStrId];


					//attach to connection string
					sbConnStr.Append(connStrId + "=" + connStrVal +";");	
				}

				//attach constant value to connection string if specified
				XmlNode paramsNode = FindParamsNode(connType);
				XmlNode constantNode = paramsNode.SelectSingleNode("constant");
				if (constantNode!=null)
				{
					sbConnStr.Append(constantNode.InnerText);
				}
			}
			catch(Exception e)
			{
				string exc = e.StackTrace;
			}

			return sbConnStr.ToString();
		}


		public string FindDatasourceParam(string connType)
		{
			string dsParam="";
			
			try
			{
				//find password parameter
				XmlNode paramsNode = FindParamsNode(connType);
				if(paramsNode!=null)
				{
					dsParam = FindParamName(paramsNode, "datasource");
				}
			}
			catch(Exception e)
			{
				string exc = e.StackTrace;
			}

			return dsParam;

		}

		private string FindParamName(XmlNode paramsNode, string attrName)
		{
			string name="";

			XmlNodeList paramsList = paramsNode.SelectNodes("param");
			//loop through connection parameters
			for (int i=0;i<paramsList.Count;i++)
			{
				XmlNode paramNode = paramsList[i];
				XmlNode paramName = paramNode.SelectSingleNode("conn_str_id");
				//get encrypt attribute
				XmlAttribute attr = paramName.Attributes[attrName];
				//if attribute found and is set to true, get it's text and exit params loop
				if (attr!=null && attr.InnerText.Equals("true"))
				{
					name = paramName.InnerText;
					break;
				}
			}

			return name;

			
		}

		/// <summary>
		/// Finds and returns params node for a given connection type
		/// </summary>
		/// <param name="connType">Connection Type</param>
		/// <returns>params node</returns>
		private XmlNode FindParamsNode(string connType)
		{
			XmlNode paramsNode = null;
			try
			{
				//get types nodes
				XmlNodeList typesList = xmlDoc.SelectSingleNode("types").SelectNodes("type");

				//loop through types
				for (int i=0;i<typesList.Count;i++)
				{
					//look for selected type
					string typeName = typesList.Item(i).SelectSingleNode("type_name").InnerText;
					//selected type found
					if (typeName.Equals(connType))
					{
						//get connection params node tree
						paramsNode = typesList.Item(i).SelectSingleNode("params");
						//exit for loop
						break;
					}
				}
			}
			catch(Exception e)
			{
				string exc = e.StackTrace;
			}
			
			return paramsNode;
		}

		//AK, 8/7/08, add to be able to read connection string from XML with password encrypted
		/// <summary>
		/// Finds the password parameter for the given connection type,
		/// i.e. the value to be encrypted before storing in configuration
		/// </summary>
		/// <param name="connType">connection type</param>
		/// <returns>Name of the password parameter</returns>
		public string FindPasswordParam(string connType)
		{
			string encryptedParam="";
			
			try
			{
				//find password parameter
				XmlNode paramsNode = FindParamsNode(connType);
				if(paramsNode!=null)
				{
					encryptedParam = FindParamName(paramsNode, "encrypt");
					/*XmlNodeList paramsList = paramsNode.SelectNodes("param");
					//loop through connection parameters
					for (int i=0;i<paramsList.Count;i++)
					{
						XmlNode paramNode = paramsList[i];
						XmlNode paramName = paramNode.SelectSingleNode("conn_str_id");
						//get encrypt attribute
						XmlAttribute encryptAttr = paramName.Attributes["encrypt"];
						//if attribute found and is set to true, get it's text and exit params loop
						if (encryptAttr!=null && encryptAttr.InnerText.Equals("true"))
						{
							encryptedParam = paramName.InnerText;
							break;
						}
					}*/
				}
			}
			catch(Exception e)
			{
				string exc = e.StackTrace;
				return "";
			}

			return encryptedParam;
		}


		//AK, 8/7/08, add to be able to read connection string from XML with password encrypted
		/// <summary>
		/// Returns a connection string with password decrypted
		/// </summary>
		/// <param name="connStr">conneciton string with encrypted password value</param>
		/// <param name="connType">connection type</param>
		/// <returns>conneciton string with encrypted password value</returns>
		public string GetConnStringDecrypted(string connStr, string connType)
		{
			try
			{
				string encrypted="";
				//get password parameter name
				string encryptParam=FindPasswordParam(connType);
			
				int iPos1=-1, iPos2=-1;
				//find param beginning position
				iPos1 = connStr.IndexOf(encryptParam);
				//find param ending position
				if (iPos1>-1 && encryptParam.Length>0)
					iPos2 = connStr.IndexOf(";", iPos1);
			
				if (iPos1>-1 && iPos2>-1)
				{
					//get encrypted value
					iPos1=iPos1+encryptParam.Length+1;
					encrypted = connStr.Substring(iPos1, iPos2-iPos1);
					if (encrypted.Length>0)
					{
						//decrypt password, hardcode saltstring
						string decrypted = CryptoUtil.DecryptString(encrypted, "saltstring");
						//replace in connection string
						connStr = connStr.Replace(encrypted,decrypted);
					}
				}
			}
			catch(Exception e)
			{
				string exc = e.StackTrace;
				return "";
			}
			return connStr;

		}


		//AK, 2/25/10 
		/// <summary>
		/// Get collection of command line params and their corresponding connection string labels
		/// </summary>
		/// <param name="connType">Connection type</param>
		/// <returns>A map of command line params as key and connection string param name as value</returns>
		public ListDictionary GetCommandLineParams(string connType)
		{
			ListDictionary ldParams = new ListDictionary();

			try
			{
				XmlNode paramsNode = FindParamsNode(connType);

				//get connection params
				if(paramsNode!=null)
				{
					//get parameters nodes
					XmlNodeList connParams = paramsNode.SelectNodes("param");

					//loop through parameter nodes
					for (int j=0;j<connParams.Count;j++)
					{
						//get param node
						XmlNode connParam = connParams.Item(j);
						string paramId="";
						//get parameter's connection string id
						if(connParam.SelectSingleNode("conn_str_id")!=null)
							paramId = connParam.SelectSingleNode("conn_str_id").InnerText;

                        //get parameter's command line abbreviation
                        //JE 03JAN19 Mantis 1646 - add null check to avoid NullReferenceException when node is null
                        XmlNode cmdLineParamNode = connParam.SelectSingleNode("command_line_param");
                        string cmdParamName = null;
                        if (cmdLineParamNode != null)
                        {
                            cmdParamName = connParam.SelectSingleNode("command_line_param").InnerText;
                            //store in hashtable
                            ldParams.Add(cmdParamName,paramId);
                        }
                        //string cmdParamName = connParam.SelectSingleNode("command_line_param").InnerText;
                        //store in hashtable
                        //moved line below to IF block above so we dont add a null key to ldParams
                        //ldParams.Add(cmdParamName,paramId);
                        //end changes JE on 03JAN19
                    }
                }
			}
			catch(Exception e)
			{
				string exc = e.StackTrace;
			}

			//return parameters collection
			return ldParams;
			
		}

        //Mkhan, 3/17/11 
        /// <summary>
        /// Get collection of command line params and their corresponding connection string labels 
        /// for interim DBs
        /// </summary>
        /// <param name="connType">Connection type</param>
        /// <returns>A map of command line params as key and connection string param name as value</returns>
        public ListDictionary GetInterimCommandLineParams(string connType)
        {
            ListDictionary ldParams = new ListDictionary();

            // MP, 01/28/2019, 00001643, handle null reference
            XmlNodeList connParams = FindParamsNode(connType)?.SelectNodes("param");
            if (connParams != null)
            //try
            // end changes by MP on 01/28/2019
            {
                // MP, 01/28/2019, 00001643, handle null reference
                //XmlNode paramsNode = FindParamsNode(connType);

                ////get connection params
                //if (paramsNode != null)
                //{
                //    //get parameters nodes
                //    XmlNodeList connParams = paramsNode.SelectNodes("param");
                // end changes by MP on 01/28/2019

                //loop through parameter nodes
                for (int j = 0; j < connParams.Count; j++)
                {
                    //get param node
                    XmlNode connParam = connParams.Item(j);
                    // MP, 01/28/2019, 00001643, handle null reference
                    string paramId = connParam.SelectSingleNode("conn_str_id")?.InnerText;
                    string cmdParamName = connParam.SelectSingleNode("interimdb_command_line_param")?.InnerText;
                    if (!string.IsNullOrEmpty(paramId) && !String.IsNullOrEmpty(cmdParamName))
                    {
                        //string paramId = "";
                        ////get parameter's connection string id
                        //if (connParam.SelectSingleNode("conn_str_id") != null)
                        //    paramId = connParam.SelectSingleNode("conn_str_id").InnerText;

                        ////get parameter's command line abbreviation
                        //string cmdParamName = connParam.SelectSingleNode("interimdb_command_line_param").InnerText;
                        // end changes by MP on 01/28/2019

                        //store in hashtable
                        ldParams.Add(cmdParamName, paramId);
                    }
                }
            }
            // MP, 01/28/2019, 00001643, handle null reference
            //catch (Exception e)
            //{
            //    string exc = e.StackTrace;
            //}
            // end changes by MP on 01/28/2019

            //return parameters collection
            return ldParams;

        }
	}
}
