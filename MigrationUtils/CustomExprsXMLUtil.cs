using System;
using System.Collections;
using System.Collections.Specialized;
using System.Xml;

namespace TRUmigrate_1
{
	/// <summary>
	/// Summary description for CustomExprsXMLUtil.
	/// </summary>
	public class CustomExprsXMLUtil:XMLUtil
	{
		public CustomExprsXMLUtil(string xmlFilePath)
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
		/// Gets custom regular expression from xml
		/// </summary>
		/// <returns>ListDictionary of regular expressions and descriptions</returns>
		public ListDictionary ReadCustomRegExpr()
		{
			ListDictionary ldRegExpr = new ListDictionary();
			try
			{
				//get top node
				XmlNode sCharNode = xmlDoc.SelectSingleNode("SpecialCharacters");
				//get Regular Expressions node
				XmlNode exprsNode = sCharNode.SelectSingleNode("RegExpressions");
				//get list of reg exp nodes
				XmlNodeList exprsNodeList = exprsNode.SelectNodes("regexpr");

				for (int i=0;i<exprsNodeList.Count;i++)
				{
					//get regular expression
					string expr = exprsNodeList[i].Attributes["expr"].InnerText;
					//get its name
					string name = exprsNodeList[i].InnerText;
					//add to List Dictionary
					ldRegExpr.Add(name, expr);
				}
			}
			catch(Exception e)
			{
				string exc = e.StackTrace;
			}

			//return List Dictionary
			return ldRegExpr;

		}


		/// <summary>
		/// Reads custom special characters from xml
		/// </summary>
		/// <returns>ListDictionary of special characters and descriptions</returns>
		public ListDictionary ReadCustomSpecChars()
		{
			ListDictionary ldChars = new ListDictionary();
			try
			{
				//get top node
				XmlNode sCharNode = xmlDoc.SelectSingleNode("SpecialCharacters");
				//get Characters node
				XmlNode exprsNode = sCharNode.SelectSingleNode("Characters");
				//get list of character nodes
				XmlNodeList exprsNodeList = exprsNode.SelectNodes("name");

				for (int i=0;i<exprsNodeList.Count;i++)
				{
					//get special character
					string expr = exprsNodeList[i].Attributes["char"].InnerText;
					//get its name
					string name = exprsNodeList[i].InnerText;
					//add to listdictionary
					ldChars.Add(name, expr);
				}
			}
			catch(Exception e)
			{
				string exc = e.StackTrace;
			}

			//return List Dictionary of custom special characters
			return ldChars;

		}

	}
}
