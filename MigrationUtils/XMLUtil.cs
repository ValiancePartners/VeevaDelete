//-----------------------------------------------------------------------
// <copyright file="XMLUtil.cs" company="Valiance Partners">
//     Copyright (c) Valiance Partners.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Xml;
using System.IO;

namespace TRUmigrate_1
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	public abstract class XMLUtil
	{

		/// <summary>
		/// XML Document object
		/// </summary>
		public XmlDocument xmlDoc = new XmlDocument();

		public XmlNodeList GetNodeTree(String nodeTreeName)
		{
			return xmlDoc.SelectNodes(nodeTreeName);
		}

		public void CreateFile(String localFilePath)
		{
			if (File.Exists(localFilePath)) File.Delete(localFilePath);

			//create new file stream
			FileStream fsxml = new FileStream(localFilePath,FileMode.Create,
				FileAccess.Write,
				FileShare.ReadWrite);

			//Save XML Document
			xmlDoc.Save(fsxml);

			//release file
			fsxml.Close();
		}



		/// <summary>
		/// Gets or creates an XML node and returns this node
		/// </summary>
		/// <param name="parentNode">parent XML Node</param>
		/// <param name="nodename">name of returned node</param>
		/// <returns>Single XML node</returns>
		public XmlNode GetNode (ref XmlNode parentNode, string nodename)
		{
			XmlNode node = parentNode.SelectSingleNode(nodename);
			if (node==null)
			{
				node = xmlDoc.CreateNode("element", nodename, "");
				parentNode.AppendChild(node);
			}
			return node;
		
		}


		public XmlAttribute GetAttribute(ref XmlNode parentNode, string attrname)
		{
			XmlAttribute attr = parentNode.Attributes[attrname];
			if (attr==null)
			{
				attr = xmlDoc.CreateAttribute(attrname);
				parentNode.Attributes.Append(attr);
			}

			return attr;
		}
	}
	
		

}
