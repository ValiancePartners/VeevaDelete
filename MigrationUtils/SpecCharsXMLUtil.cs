using System;
using System.Collections;
using System.Collections.Specialized;
using System.Xml;

namespace TRUmigrate_1
{
	/// <summary>
	/// Summary description for SpecCharsXMLUtil.
	/// </summary>
	public class SpecCharsXMLUtil:XMLUtil
	{
		public SpecCharsXMLUtil(string xmlFilePath)
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
		/// Get a map of defined regular expressions
		/// </summary>
		/// <returns>ListDictionary of regular expressions and corresponding user-friendly names</returns>
		public ListDictionary ReadRegExpressions()
		{
			ListDictionary ldRegExpr = new ListDictionary();
			try
			{
				//get regular expression nodes
				XmlNode sCharNode = xmlDoc.SelectSingleNode("SpecialCharacters");
				XmlNode exprsNode = sCharNode.SelectSingleNode("RegExpressions");
				XmlNodeList exprsNodeList = exprsNode.SelectNodes("regexpr");

				for (int i=0;i<exprsNodeList.Count;i++)
				{
					//get regular expression
					string expr = exprsNodeList[i].Attributes["expr"].InnerText;
					//get its description
					string name = exprsNodeList[i].InnerText;
					//store in ListDictionary
					ldRegExpr.Add(name, expr);
				}
			}
			catch(Exception e)
			{
				string exc = e.StackTrace;
			}

			return ldRegExpr;
		}

		/// <summary>
		/// Get a mpa of defined special characters
		/// </summary>
		/// <returns>ListDictionary of special characters and corresponding user-friendly names</returns>

		public ListDictionary ReadSpecChars()
		{
			ListDictionary ldChars = new ListDictionary();
			try
			{
				//get special characters nodes
				XmlNode sCharNode = xmlDoc.SelectSingleNode("SpecialCharacters");
				XmlNode exprsNode = sCharNode.SelectSingleNode("Characters");
				XmlNodeList exprsNodeList = exprsNode.SelectNodes("name");

				for (int i=0;i<exprsNodeList.Count;i++)
				{
					//get special characters
					string expr = exprsNodeList[i].Attributes["char"].InnerText;
					//get its description
					string name = exprsNodeList[i].InnerText;
					//store in ListDictionary
					ldChars.Add(name, expr);
				}
			}
			catch(Exception e)
			{
				string exc = e.StackTrace;
			}

			return ldChars;
		}


	}
}
