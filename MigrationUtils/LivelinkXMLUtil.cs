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
	/// Performs all the xml parsing for LivelinkConnectionUtil
	/// </summary>
	public class LivelinkXMLUtil:XMLUtil
	{
		public LivelinkXMLUtil(string xmlFilePath)
		{
			try
			{
				xmlDoc.Load(xmlFilePath);
			}
			catch(System.IO.FileNotFoundException nfe)
			{
				throw new TRUMigrateException("Livelink - Internal XML cannot be found. " +
					"Please reinstall an application or restore a missing file", nfe);
			}
			catch(System.Xml.XmlException xmle)
			{
				throw new TRUMigrateException("Error occured when loading Livelink - Internal XML. " +
					"Please reinstall an application or replace a corrupted file", xmle);
			}
			catch (Exception e)
			{
				throw new TRUMigrateException("Error occured when reading or loading Livelink - Internal XML. " +
					"Please reinstall an application or replace a corrupted file", e);

			}
		}

		public ArrayList GetSubTypes() 
		{
			ArrayList results = new ArrayList();
			XmlNode subTypesNode = xmlDoc.SelectSingleNode("/Livelink/Subtypes");
			XmlNodeList subTypes = subTypesNode.ChildNodes;
			for (int i=0; i<subTypes.Count; i++) 
			{
				string temp = subTypes.Item(i).Name.ToString();
				results.Add(subTypes.Item(i).Name.ToString());
			}
			return results;
		}

		public ArrayList GetBaseAttributes(string subType) 
		{
			ArrayList results = new ArrayList();
			XmlNode subTypesNode = xmlDoc.SelectSingleNode("/Livelink/Subtypes/" + subType + "/Attributes");
			XmlNodeList subTypes = subTypesNode.ChildNodes;
			int tasdfk = subTypes.Count;
			for (int i=0; i<subTypes.Count; i++) 
			{
				results.Add(subTypes.Item(i).InnerText.ToString());
			}
			return results;

		}

		public ArrayList GetFolderTypes() 
		{
			ArrayList results = new ArrayList();
			XmlNode subTypesNode = xmlDoc.SelectSingleNode("/Livelink/FolderTypes");
			XmlNodeList subTypes = subTypesNode.ChildNodes;
			for (int i=0; i<subTypes.Count; i++) 
			{
				string attrName = subTypes.Item(i).InnerText.ToString();
				results.Add(attrName);
			}
			return results;

		}

		public ArrayList GetBaseAttributesWithInfo(string subType) 
		{
			ArrayList results = new ArrayList();
			XmlNode subTypesNode = xmlDoc.SelectSingleNode("/Livelink/Subtypes/" + subType + "/Attributes");
			XmlNodeList subTypes = subTypesNode.ChildNodes;
			int tasdfk = subTypes.Count;
			for (int i=0; i<subTypes.Count; i++) 
			{
				string attrName = subTypes.Item(i).InnerText.ToString();
				XmlAttributeCollection attrs = subTypes.Item(i).Attributes;
				XmlNode dataType = attrs.GetNamedItem("dataType");
				string attrType = dataType.Value;
				XmlNode lengthAttr = attrs.GetNamedItem("length");
				string attrLength = lengthAttr.Value;
				string[] attrInfo = {attrName,attrType,attrLength};
				results.Add(attrInfo);
			}
			return results;

		}
	}

}

