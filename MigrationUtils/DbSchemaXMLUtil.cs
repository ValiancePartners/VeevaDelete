//-----------------------------------------------------------------------
// <copyright file="DbSchemaXMLUtil.cs" company="Valiance Partners">
//     Copyright (c) Valiance Partners.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Xml;
using System.Collections;

namespace TRUmigrate_1
{
	/// <summary>
	/// Utility for writing and reading database schema into/from an XML file
	/// 2.2 SP3
	///		AK, 8/2/07, BUGAWARE #229, date formatting fix. 
	///		Do not throw a fatal error when schema file cannot be loaded, should be able to continue execution
	/// </summary>
	public class DbSchemaXMLUtil:XMLUtil
	{

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="xmlFilePath">File system path of XML file</param>
		public DbSchemaXMLUtil(string xmlFilePath)
		{
			try
			{
				xmlDoc.Load(xmlFilePath);
			}
			catch(System.IO.FileNotFoundException nfe)
			{
				//AK, 8/02/07, BUGAWARE #229, when schema file cannot be found, do not throw a fatal error
				//log an error and continue
				throw new TRUMigrateException("Schema file " + xmlFilePath + " cannot be found.", nfe);
				//throw new TRUmigrateException(Constants.FATAL_ERROR, 
				//	"DbSchema - Internal XML cannot be found. " +
				//	"Please reinstall an application or restore a missing file", nfe);
				//end changes by AK on 8/2/07
			}
			catch(System.Xml.XmlException xmle)
			{
				//AK, 8/02/07, BUGAWARE #229, when schema file cannot be loaded, do not throw a fatal error
				//log an error and continue
				throw new TRUMigrateException("Error occured when loading " + xmlFilePath + " schema file.", xmle);
				//throw new TRUmigrateException(Constants.FATAL_ERROR, 
				//	"Error occured when loading DbSchema - Internal XML. " +
				//	"Please reinstall an application or replace a corrupted file", xmle);
				//end changes by AK on 8/2/07
			}
			catch (Exception e)
			{
				//AK, 8/02/07, BUGAWARE #229, do not throw a fatal error on general schema xml exception
				//log an error and continue
				throw new TRUMigrateException("Error occured when reading or loading " + xmlFilePath + " schema file.", e);
				//throw new TRUmigrateException(Constants.FATAL_ERROR, 
				//	"Error occured when reading or loading DbSchema - Internal XML. " +
				//	"Please reinstall an application or replace a corrupted file", e);
				//end changes by AK on 8/2/07

			}
		}

		/// <summary>
		/// Write database schema from sorted list into an XML
		/// </summary>
		/// <param name="htDbSchema">DB schema hashtable </param>
		/// <param name="iConnType">Type of connection as an integer</param>
		public void WriteDbSchema(SortedList slDbSchema, int iConnType)
		{

			try
			{
				XmlNode topTablesNode = xmlDoc.SelectSingleNode("tables");
				//set connection type
				XmlNode connTypeNode = topTablesNode.SelectSingleNode("conn_type");
				connTypeNode.InnerText = iConnType.ToString();

				//get empty sample table node
				XmlNode sTableNode = topTablesNode.SelectSingleNode("table");
				//get empty sample table name node
				XmlNode sNameNode = sTableNode.SelectSingleNode("name");
				//get empty sample column node
				XmlNode sColumnNode = sTableNode.SelectSingleNode("column");

				//get sortedlist keys collection
				IList keyList = slDbSchema.GetKeyList();
			
				//loop through schema hashtable and load data
				for (int i=0;i<keyList.Count;i++)
				{
					//sorted list key is table name
					string tableName = (string)keyList[i];
					//create new table node
					XmlNode tableNode = sTableNode.CloneNode(false);
					//create new table name node
					XmlNode nameNode = sNameNode.CloneNode(false);
					//set name node value to table name
					nameNode.InnerText = tableName;
					//insert name node into XML as a table child
					tableNode.AppendChild(nameNode);

					//get table columns definition
					ArrayList columnsData = (ArrayList)slDbSchema[tableName];
					//loop through columns definitions
					for (int j=0;j<columnsData.Count;j++)
					{
						//get field name and type array
						string[] fieldData = (string[])columnsData[j];

						//create column node and set its value
						XmlNode columnNode = sColumnNode.CloneNode(false);
						columnNode.InnerText = fieldData[0];

						//get datatype attribute value
						columnNode.Attributes["datatype"].InnerText=fieldData[1];

						//AK, 4/4/06, store length in schema
						if (fieldData.Length>2)
						{
							XmlAttribute attrRep = xmlDoc.CreateAttribute("length");
							columnNode.Attributes.Append(attrRep);
							columnNode.Attributes["length"].InnerText = fieldData[2];
						}
						//AK, 10/03/05, if has repeating flag, write it to schema
						//AK, 4/4/06, change from length==3 to length==4
						//added extra element for field length
						if (fieldData.Length==4)
						{
							XmlAttribute attrRep = xmlDoc.CreateAttribute("repeating");
							columnNode.Attributes.Append(attrRep);
							columnNode.Attributes["repeating"].InnerText = fieldData[3];
						}
						//end changes by AK on 10/03/05

						//insert column node into XML as a table child
						tableNode.AppendChild(columnNode);
					}	//end column loop
				
					//insert table node into XML
					topTablesNode.AppendChild(tableNode);
				}//end table loop
			}
			catch(Exception e)
			{
				string exc = e.StackTrace;
			}

		}

		/// <summary>
		/// Read DB Schema from XML into a hashtable
		/// </summary>
		/// <returns>DB schema hashtable</returns>
		public SortedList ReadDbSchema(ref int iConnType)
		{
			SortedList slDbSchema = new SortedList();
			try
			{
				//get top node
				XmlNode topTablesNode = xmlDoc.SelectSingleNode("tables");
				//get connection type
				XmlNode connTypeNode = topTablesNode.SelectSingleNode("conn_type");
				iConnType = Convert.ToInt32(connTypeNode.InnerText);

				//get tables nodes list
				XmlNodeList tablesList = topTablesNode.SelectNodes("table");

				//loop through table nodes, first node is empty, start at second one
				for (int i=1;i<tablesList.Count;i++)
				{
					//get table node
					XmlNode tableNode = tablesList[i];
					//get table name
					string tableName = tableNode.SelectSingleNode("name").InnerText;
					//get columns nodes
					XmlNodeList columnsList = tableNode.SelectNodes("column");
				
					//loop through column
					ArrayList columnsData = new ArrayList();
					for (int j=0;j<columnsList.Count;j++)
					{
						XmlNode columnNode = columnsList[j];
						//add column name and datatype to array
						//AK, 10/03/05, add repeating flag to column data
						string repeating="";
						if (columnNode.Attributes["repeating"]!=null)
							repeating= columnNode.Attributes["repeating"].InnerText;
						//AK, 9/28/06, BUGAWARE #160, add length attribute value 
						string length="";
						if (columnNode.Attributes["length"]!=null)
							length=columnNode.Attributes["length"].InnerText;
						string[] fieldData = {columnNode.InnerText,columnNode.Attributes["datatype"].InnerText, length, repeating};
						//string[] fieldData = {columnNode.InnerText,columnNode.Attributes["datatype"].InnerText, repeating};
						//end changes by AK on 9/28/06
						//string[] fieldData = {columnNode.InnerText,columnNode.Attributes["datatype"].InnerText};
						//end changes by AK on 10/03/05

						//add array to columns data
						columnsData.Add(fieldData);
					}//end columns loop

					//add table and columns data to hashtable
					slDbSchema.Add(tableName, columnsData);
				}
			}
			catch(Exception e)
			{
				string exc = e.StackTrace;
			}
			
			return slDbSchema;
		}

		/// <summary>
		/// Reads connection type from dm schema
		/// </summary>
		/// <returns>Connection type index</returns>
		public int GetConnectionType()
		{
			try
			{
				XmlNode topTablesNode = xmlDoc.SelectSingleNode("tables");
				//get connection type
				XmlNode connTypeNode = topTablesNode.SelectSingleNode("conn_type");
				return Convert.ToInt32(connTypeNode.InnerText);
			}
			catch(Exception e)
			{
				string exc = e.StackTrace;
				return 0;
			}

		}

	}
}
