//-----------------------------------------------------------------------
// <copyright file="LivelinkXMLUtil.cs" company="Valiance Partners">
//     Copyright (c) Valiance Partners.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Xml;
using System.Collections;

namespace TRUmigrate_1
{
	/// <summary>
	/// Performs all the xml parsing for batch processing
	/// </summary>
	public class BatchXMLUtil:XMLUtil
	{
		public BatchXMLUtil(string xmlFilePath)
		{
			try
			{
				xmlDoc.Load(xmlFilePath);
			}
			catch(System.IO.FileNotFoundException nfe)
			{
				throw new TRUMigrateException(xmlFilePath + " Not found.", nfe);
			}
			catch(System.Xml.XmlException xmle)
			{
				throw new TRUMigrateException( 
					"Error occured when loading " + xmlFilePath + " " +
					"Please reinstall an application or replace a corrupted file", xmle);
			}
			catch (Exception e)
			{
				throw new TRUMigrateException( 
					"Error occured when reading or loading " + xmlFilePath + " " +
					"Please reinstall an application or replace a corrupted file", e);

			}
		}

		public ArrayList GetConfigFiles() 
		{
			ArrayList results = new ArrayList();
			//XmlNode configFilesNode = xmlDoc.SelectSingleNode("/BatchRun/ConfigFiles");
			XmlNodeList configFilePaths = xmlDoc.SelectNodes("/BatchRun/ConfigFiles/FilePaths");
			for(int i=0; i<configFilePaths.Count; i++)
			{
				XmlNode configPaths = configFilePaths[i];
				string configFile = configPaths.SelectSingleNode("configPath").InnerText;
				string accessDB = configPaths.SelectSingleNode("accessDbPath").InnerText;
				string[] filePaths = new string[]{configFile, accessDB};
				results.Add(filePaths);
			}
			return results;
		}

		public bool IsBatchFile()
		{
			XmlNode testForNode = xmlDoc.SelectSingleNode("/BatchRun/ConfigFiles");
			if (testForNode == null) 
			{
				return false;
			}
			else 
			{
				return true;
			}
		}
	}

}

