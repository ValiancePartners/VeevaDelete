//-----------------------------------------------------------------------
// <copyright file="MappingRulesXMLUtil.cs" company="Valiance Partners">
//     Copyright (c) Valiance Partners.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Xml;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;

namespace TRUmigrate_1
{

    /// <summary>
    /// Reads and writes TRUCompare configuration from/into XML file
    /// 3.0 HF 3
    ///     AK, 10/31/12, add functionality for pre and post processing commands on Export
    /// 3.1
    ///    KD, 01/16/2012 reads query caching values Mantis #315
    /// 4.0
    ///     AK, 11/14/13, read pre and post processing commands from the project file
    ///     KD, 11/1/13, read connection data from project file
    /// 4.1
    ///     AK, 4/8/15, Mantis #1118, add local variables functionality
    /// </summary>
    public class MappingRulesXMLUtil : XMLUtil
    {

        //connection constants - current connection selected for setup
        private const int SOURCE = 1;
        private const int TARGET = 2;
        private const int SUPPLEMENTAL = 3;

        string delimiter = "|";


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="xmlFilePath">Local file path of configuration file</param>
        public MappingRulesXMLUtil(string xmlFilePath)
        {
            try
            {
                xmlDoc.Load(xmlFilePath);
            }
            catch (System.IO.FileNotFoundException nfe)
            {
                throw new TRUMigrateException(xmlFilePath + " is not found", nfe);
            }
            catch (System.Xml.XmlException xmle)
            {
                throw new TRUMigrateException("Error occured when loading " + xmlFilePath, xmle);
            }
            catch (Exception e)
            {
                throw new TRUMigrateException("Error occured when loading " + xmlFilePath, e);

            }
        }


        /// <summary>
        /// Get a list of all queries stored in configuration
        /// </summary>
        /// <returns>ListDictionary of queries</returns>
        public ListDictionary ReadQueryData()
        {
            ListDictionary ldQueries = new ListDictionary();

            try
            {
                string dataNodeName = "SourceData";

                //get Source Data node
                XmlNode sourceDataNode = xmlDoc.SelectSingleNode("CompareRule").SelectSingleNode(dataNodeName);

                //get query nodes list
                XmlNodeList queryNodeList = sourceDataNode.ChildNodes;

                //loop through query nodes
                for (int i = 0; i < queryNodeList.Count; i++)
                {
                    XmlNode queryNode = queryNodeList[i];
                    string query = queryNode.SelectSingleNode("query").InnerText;

                    //get counter attribute if exists
                    XmlAttribute sQueryCounterAttr = queryNodeList[i].Attributes["counter"];
                    if (sQueryCounterAttr != null)
                    {
                        int queryId = Convert.ToInt32(sQueryCounterAttr.InnerText);
                        ldQueries.Add(queryId, query);
                    }

                }
            }
            catch (Exception e)
            {
                string exc = e.StackTrace;
            }


            return ldQueries;

        }

        /// <summary>
        /// AK, 7/8/14, TM 4.0, added functionality to assign preconditions to a query, read query preconditions from XML 
        /// </summary>
        /// <returns></returns>
        public Hashtable ReadQueryPreconditions()
        {
            Hashtable htQueryPreCond = new Hashtable();

            try
            {
                string dataNodeName = "SourceData";

                //get Source Data node
                XmlNode sourceDataNode = xmlDoc.SelectSingleNode("CompareRule").SelectSingleNode(dataNodeName);

                //get query nodes list
                XmlNodeList queryNodeList = sourceDataNode.ChildNodes;

                //loop through query nodes
                for (int i = 0; i < queryNodeList.Count; i++)
                {
                    //array list to store pre-conditions
                    ArrayList queryPreConds = new ArrayList();
                    //query node
                    XmlNode queryNode = queryNodeList[i];
                    //pre-conditions parent node
                    XmlNode preConditionsNode = queryNode.SelectSingleNode("pre-conditions");

                    //get counter attribute if exists
                    XmlAttribute sQueryCounterAttr = queryNode.Attributes["counter"];
                    if (sQueryCounterAttr != null && preConditionsNode !=null)
                    {
                        //set query ID
                        int queryId = Convert.ToInt32(sQueryCounterAttr.InnerText);
                        //get all preconditions for a query
                        XmlNodeList preConditionsNodes = preConditionsNode.SelectNodes("pre-condition");
                        //loop through preconditions
                        for (int j = 0; j < preConditionsNodes.Count; j++)
                        {
                            //get precondition as a string
                            string preCond = ReadPreconditionNode(preConditionsNodes[j]);
                            //add to array of preconditions
                            queryPreConds.Add(preCond);
                        }
                        //add to return hash as query id - array of preconditions
                        if (!htQueryPreCond.Contains(queryId))
                            htQueryPreCond.Add(queryId, queryPreConds);

                    }

                }
            }
            catch (Exception e)
            {
                string exc = e.StackTrace;
            }


            return htQueryPreCond;
        }

        /// <summary>
        /// KD, 01/16/2012 reads query caching values Mantis #315
        /// </summary>
        /// <returns>hashtable of query id-query caching pairs</returns>
        public Hashtable ReadQueryCachingValue()
        {
            Hashtable htQueryCaching = new Hashtable();

            try
            {
                string dataNodeName = "SourceData";
                //if (isTargetQuery)
                //    dataNodeName = "TargetData";

                //get Source Data node
                XmlNode sourceDataNode = xmlDoc.SelectSingleNode("CompareRule").SelectSingleNode(dataNodeName);

                //get query nodes list
                XmlNodeList queryNodeList = sourceDataNode.ChildNodes;

                //loop through query nodes
                for (int i = 0; i < queryNodeList.Count; i++)
                {
                    XmlNode queryNode = queryNodeList[i];
                    ////get type
                    //string queryType = queryNode.v;
                    //get query caching attribute
                    XmlAttribute sQueryCachingAttr = queryNodeList[i].Attributes["isQueryCaching"];
                    XmlAttribute sQueryCounterAttr = queryNodeList[i].Attributes["counter"];
                    if (sQueryCachingAttr != null && sQueryCounterAttr != null)
                    {
                        //get query id from counter attribute
                        string queryId = sQueryCounterAttr.InnerText;
                        string queryCachingValue = sQueryCachingAttr.InnerText;
                        htQueryCaching.Add(Convert.ToInt32(queryId), Convert.ToInt32(queryCachingValue));
                    }
                }

            }
            catch (Exception e)
            {
                string exc = e.StackTrace;

            }

            return htQueryCaching;
        }

        /// <summary>
        /// Reads query ids and types (primary, secondary, supplemental
        /// </summary>
        /// <returns>hashtable of query id-query type pairs</returns>
        public Hashtable ReadQueryTypes()
        {
            Hashtable htQueryTypes = new Hashtable();

            try
            {
                string dataNodeName = "SourceData";

                //get Source Data node
                XmlNode sourceDataNode = xmlDoc.SelectSingleNode("CompareRule").SelectSingleNode(dataNodeName);

                //get query nodes list
                XmlNodeList queryNodeList = sourceDataNode.ChildNodes;

                //loop through query nodes
                for (int i = 0; i < queryNodeList.Count; i++)
                {
                    XmlNode queryNode = queryNodeList[i];
                    //get type
                    string queryType = queryNode.Name;
                    //get counter attribute
                    XmlAttribute sQueryCounterAttr = queryNodeList[i].Attributes["counter"];
                    if (sQueryCounterAttr != null)
                    {
                        //get query id from counter attribute
                        string queryId = sQueryCounterAttr.InnerText;
                        htQueryTypes.Add(Convert.ToInt32(queryId), queryType);
                    }
                }

            }
            catch (Exception e)
            {
                string exc = e.StackTrace;
            }

            return htQueryTypes;
        }

        /// <summary>
        /// AK, 3/8/16, Mantis #1298, read custom table names if specified
        /// </summary>
        /// <returns></returns>
        public Hashtable ReadCustomTableNames()
        {
            Hashtable htTableNames = new Hashtable();

            try
            {
                string dataNodeName = "SourceData";

                //get Source Data node
                XmlNode sourceDataNode = xmlDoc.SelectSingleNode("CompareRule").SelectSingleNode(dataNodeName);

                //get query nodes list
                XmlNodeList queryNodeList = sourceDataNode.ChildNodes;

                //loop through query nodes
                for (int i = 0; i < queryNodeList.Count; i++)
                {
                    XmlNode queryNode = queryNodeList[i];
                    //get user table name node if exists
                    XmlNode userTableNameNode = queryNode.SelectSingleNode("userTableName");
                    string userTableName = "";
                    if (userTableNameNode != null)
                    {
                        userTableName = userTableNameNode.InnerText;
                    }
                    //get counter attribute
                    XmlAttribute sQueryCounterAttr = queryNodeList[i].Attributes["counter"];
                    if (sQueryCounterAttr != null)
                    {
                        //get query id from counter attribute
                        string queryId = sQueryCounterAttr.InnerText;
                        //store id and custom table name in the return hash, do not add blank values
                        if (!String.IsNullOrEmpty(userTableName))
                            htTableNames.Add(queryId, userTableName);
                    }
                }

            }
            catch (Exception e)
            {
                string exc = e.StackTrace;
            }
            return htTableNames;
        }

        /// <summary>
        /// Reads primary key value from config file
        /// </summary>
        /// <param name="xmlTagName">XML node name - source or target</param>
        /// <param name="elementName">ID node name</param>
        /// <returns>Primary key rule value</returns>
        public string ReadPrimaryKey(string xmlTagName, string elementName)
        {
            try
            {
                //get top ID node
                XmlNode idNode = xmlDoc.SelectSingleNode("CompareRule").SelectSingleNode("IdentificationRules");
                //get source or target node
                XmlNode primaryKeyNode = idNode.SelectSingleNode(xmlTagName);
                //get id node
                XmlNode elementNode = primaryKeyNode.SelectSingleNode(elementName);

                if (elementName.Equals("plugin"))
                    return ReadPluginNode(elementNode);
                else //return node text
                    return elementNode.InnerText;
            }
            catch (Exception e)
            {
                string exc = e.StackTrace;
                return "";
            }

        }

        /// <summary>
        /// Reads config rules global settings
        /// </summary>
        /// <param name="xmlTagName">Global setting XML node name</param>
        /// <returns>Global setting value</returns>
        public string ReadGlobalSettings(string xmlTagName)
        {
            //****
            //AK, 9/5/19, Mantis #1813, revert code back to original, changes cause exception
            //****

            ////MP, 09 / 06 / 2018, Mantis #1613, handle null reference
            try
            {
                ////end of changes by MP pon 09 / 06 / 2018
            //get mapping node
            XmlNode mappingNode = xmlDoc.SelectSingleNode("CompareRule").SelectSingleNode("Mappings");
                //get global settings node
                XmlNode globalSettingsNode = mappingNode.SelectSingleNode("GlobalSettings");
                //get given setting node
                XmlNode settingNode = globalSettingsNode.SelectSingleNode(xmlTagName);

                //return setting value
                //// MP, 09/06/2018, Mantis #1613, handle null reference
                return settingNode?.InnerText ?? string.Empty;
                //// end of changes by MP pon 09/06/2018
                //// MP, 09/06/2018, Mantis #1613, handle null reference
            }
            catch (Exception e)
            {
                string exc = e.StackTrace;
                return "";
            }
            //// end of changes by MP pon 09/06/2018

        }


        //AK, 3/16/10, write global setting - to store last run time for synch mode runs
        public void WriteGlobalSettings(string xmlTagName, string tagValue)
        {
            try
            {
                //get mapping node
                XmlNode mappingNode = xmlDoc.SelectSingleNode("CompareRule").SelectSingleNode("Mappings");
                //get global settings node
                XmlNode globalSettingsNode = mappingNode.SelectSingleNode("GlobalSettings");
                //get given setting node
                //AK, 1/14/09, create node if not found
                XmlNode settingNode = GetNode(ref globalSettingsNode, xmlTagName);
                //XmlNode settingNode = globalSettingsNode.SelectSingleNode(xmlTagName);
                //end changes by AK on 1/14/09

                //set global setting data
                settingNode.InnerText = tagValue;
            }
            catch (Exception e)
            {
                string exc = e.StackTrace;
            }
        }


        public ArrayList ReadRepeatingAttrsGroups()
        {
            ArrayList repeatingGroups = new ArrayList();

            // MP, 09/06/2018, Mantis #1613, handle null reference 
            //try
            //{
            // end of changed by MP on 09/02/2018

            //get Repeating Groups node
            XmlNode repAttrsGroupsNode = xmlDoc.SelectSingleNode("CompareRule").SelectSingleNode("RepeatingAttrsGroups");

            if(repAttrsGroupsNode != null)
            {
                    //get groups node list
                XmlNodeList groupNodeList = repAttrsGroupsNode.SelectNodes("RepAttrGroup");

                //loop through groups nodes
                for (int i = 0; i < groupNodeList.Count; i++)
                {
                    //attr nodes list
                    XmlNodeList attrsNode = groupNodeList[i].ChildNodes;
                    //loop through attr nodes
                    ArrayList attrs = new ArrayList();
                    for (int j = 0; j < attrsNode.Count; j++)
                    {
                        string attr = attrsNode[j].InnerText;
                        attrs.Add(attr);
                    }
                    //add group to all groups array
                    repeatingGroups.Add(attrs);
                }

            }

            // MP, 09/06/2018, Mantis #1613, handle null reference 
            //catch (Exception e)
            //{
            //    string exc = e.StackTrace;
            //    return repeatingGroups;
            //}
            // end of changed by MP on 09/02/2018

            return repeatingGroups;
        }

        /// <summary>
        /// Reads all mapping rules
        /// </summary>
        /// <returns>Sorted List of mapping rules data</returns>
        // AK, 6/13/12, 3.0 HF 1, make rules collection a sorted list
        public SortedList ReadAllMappingRules()
        //public Hashtable ReadAllMappingRules()
        //end changes by AK on 6/13/12
        {
            // AK, 6/13/12, 3.0 HF 1, make rules collection a sorted list
            //Hashtable htMappingRules = new Hashtable();
            SortedList htMappingRules = new SortedList();
            //end changes by AK on 6/13/12

            try
            {
                XmlNode mappingsNode = xmlDoc.SelectSingleNode("CompareRule").SelectSingleNode("Mappings").SelectSingleNode("MappingRules");
                XmlNodeList mappingsNodeList = mappingsNode.SelectNodes("mapping");

                for (int i = 0; i < mappingsNodeList.Count; i++)
                {
                    //get mapping node
                    XmlNode mappingNode = mappingsNodeList[i];
                    //get rule counter to use as ID
                    string ruleId = mappingNode.Attributes["counter"].InnerText;

                    Hashtable htRule = new Hashtable();
                    XmlNodeList mappingParamsList = mappingNode.ChildNodes;

                    //get all rule nodes and store in hashtable
                    for (int j = 0; j < mappingParamsList.Count; j++)
                    {
                        string name = mappingParamsList[j].Name;
                        //if node has children, store values in array
                        if (name.Equals("source-values") || name.Equals("pre-conditions"))
                        {
                            XmlNodeList paramsChildrenList = mappingParamsList[j].ChildNodes;
                            ArrayList vals = new ArrayList();

                            for (int k = 0; k < paramsChildrenList.Count; k++)
                            {
                                if (name.Equals("pre-conditions"))
                                    vals.Add(ReadPreconditionNode(paramsChildrenList[k]));
                                else
                                    vals.Add(paramsChildrenList[k].InnerText);
                            }
                            htRule.Add(name, vals);
                        }
                        else
                        {
                            string val = mappingParamsList[j].InnerText;
                            htRule.Add(name, val);
                        }
                    }

                    //add hashtable with rule details to hashtable storing all rules
                    //use counter as ID
                    htMappingRules.Add(Convert.ToInt32(ruleId), htRule);

                }
            }
            catch (Exception e)
            {
                string exc = e.StackTrace;
            }

            return htMappingRules;


        }


        /// <summary>
        /// Reads target object type ffrom config XML
        /// </summary>
        /// <returns></returns>
        public string ReadTargetObjectType()
        {
            try
            {
                //get target node
                XmlNode mappingNode = xmlDoc.SelectSingleNode("CompareRule").SelectSingleNode("Mappings");
                XmlNode targetTableNode = mappingNode.SelectSingleNode("TargetTable");

                //return its value
                return targetTableNode.InnerText;
            }
            catch (Exception e)
            {
                string exc = e.StackTrace;
                return "";
            }

        }


        //US, 7/15/11, function to read Content Type for SharePoint       
        public string ReadTargetContentType()
        {
            try
            {
                //get target node
                XmlNode mappingNode = xmlDoc.SelectSingleNode("CompareRule").SelectSingleNode("Mappings");
                XmlNode targetTableNode = mappingNode.SelectSingleNode("TargetTable");

                //return content type from SharePoint target object type "Doclib.ContentType"             
                return targetTableNode.InnerText.Split(".".ToCharArray())[1];
            }
            catch (Exception e)
            {
                string exc = e.StackTrace;
                return "";
            }

        }
        //end of changes 7/15/11

        //US, 7/15/11, function to read DocumentLibrary Type for SharePoint       
        public string ReadTargetDocumentLibrary()
        {
            try
            {
                //get target node
                XmlNode mappingNode = xmlDoc.SelectSingleNode("CompareRule").SelectSingleNode("Mappings");
                XmlNode targetTableNode = mappingNode.SelectSingleNode("TargetTable");

                //return content type from SharePoint target object type "Doclib.ContentType"             
                return targetTableNode.InnerText.Split(".".ToCharArray())[0];
            }
            catch (Exception e)
            {
                string exc = e.StackTrace;
                return "";
            }

        }
        //end of changes 7/15/11

        /// <summary>
        /// Reads folder comparison rule from config XML
        /// </summary>
        /// <param name="pathtype">Ref to path type</param>
        /// <param name="allPermutations">AK, 5/28/09, added permutations flag parameter</param>
        /// <returns>Folder info</returns>
        public Hashtable ReadFolderLocation(ref string pathtype, ref bool allPermutations)
        {
            Hashtable htFolderLocation = new Hashtable();
            try
            {
                //get folder locations node
                XmlNode mappingNode = xmlDoc.SelectSingleNode("CompareRule").SelectSingleNode("Mappings");
                XmlNode folderNode = mappingNode.SelectSingleNode("FolderLocations");

                //get pathtype node
                XmlNode pathtypeNode = folderNode.SelectSingleNode("pathtype");
                //set pathtype
                pathtype = pathtypeNode.InnerText;

                //if plugin
                if (pathtype.Equals("plugin"))
                {
                    //get filename
                    XmlNode pluginNode = folderNode.SelectSingleNode("plugin");
                    string plugininfo = ReadPluginNode(pluginNode);
                    htFolderLocation.Add("plugininfo", plugininfo);
                }
                else
                {
                    if (pathtype.Equals("exact_match"))
                    {
                        //AK, 8/15/08, add exact match support
                        htFolderLocation.Add("pathval", "exact_match");
                        //end changes by AK on 8/15/08
                        //no values needed

                    }
                    else
                    {
                        if (pathtype.Equals("derived"))
                        {
                            //get pathvalue
                            XmlNode pathvalNode = folderNode.SelectSingleNode("pathvalue");
                            //add to hashtable
                            htFolderLocation.Add("pathval", pathvalNode.InnerText);
                            //AK, 5/28/09, read processing attribute value if defined
                            XmlAttribute attrProcessing = pathvalNode.Attributes["processing"];
                            if (attrProcessing != null)
                            {
                                string processing = attrProcessing.InnerText;
                                if (processing.ToUpper().StartsWith("REC"))
                                    allPermutations = true;
                                else
                                    allPermutations = false;
                            }
                            else
                                allPermutations = false;
                            //end changes by AK on 5/28/09
                        }
                    }
                }
            }
            catch (Exception e)
            {
                string exc = e.StackTrace;
            }

            //return folder info
            return htFolderLocation;
        }


        //AK, 12/4/08, add param exportTo for location where content will be exported
        public ArrayList ReadContentFormat(ref string primaryRend, ref string exportTo)
        {
            ArrayList formats = new ArrayList();

            // MP, 09/06/2018, Mantis #1613, handle null reference
            //try
            //{
            // end of changes by MP on 09/06/2018
            //get content locations node
            XmlNode mappingNode = xmlDoc.SelectSingleNode("CompareRule").SelectSingleNode("Mappings");
            XmlNode contentNode = mappingNode.SelectSingleNode("ContentLocations");
            //get renditions node
            XmlNode rendsNode = contentNode.SelectSingleNode("Renditions");

            //get all rendition nodes
            // MP, 09/06/2018, Mantis #1613, handle null reference
            if (rendsNode != null)
            {
                // end of changes by MP on 09/06/2018

                XmlNodeList rendNodeList = rendsNode.SelectNodes("Rendition");
                for (int i = 0; i < rendNodeList.Count; i++)
                {
                    //get format and path nodes
                    XmlNode formatNode = rendNodeList[i].SelectSingleNode("format");
                    XmlNode pathNode = rendNodeList[i].SelectSingleNode("contentpath");

                    //if primary, set ref value
                    XmlAttribute primaryAttr = formatNode.Attributes["primary"];
                    if (primaryAttr != null)
                    {
                        //get text
                        string attrtext = primaryAttr.InnerText;
                        if (attrtext.Equals("true"))
                            primaryRend = formatNode.InnerText;
                    }

                    //get format and path values
                    string format = formatNode.InnerText;
                    formats.Add(format);
                }

                //Ak, 12/4/08, get exportTo value - a local location where content will be exported
                XmlNode localLocNode = contentNode.SelectSingleNode("exportTo");
                    if (localLocNode == null)
                        exportTo = "default";
                    else
                        exportTo = localLocNode.InnerText;
            }
            // MP, 09/06/2018, Mantis #1613, handle null reference
            //catch (Exception e)
            //{
            //    string exc = e.StackTrace;
            //}
            // end of changes by MP on 09/06/2018

                //return array of formats
                return formats;

        }


        /// <summary>
        /// Reads content locations
        /// </summary>
        /// <returns>Array of content locations</returns>
        public ArrayList ReadContentLocation()
        {
            ArrayList contentLocations = new ArrayList();

            // MP, 09/06/2018, Mantis #1613, handle null reference
            //try
            //{
            // end of changes by MP on 09/06/2018
            //get content locations node
            XmlNode mappingNode = xmlDoc.SelectSingleNode("CompareRule").SelectSingleNode("Mappings");
            XmlNode contentNode = mappingNode.SelectSingleNode("ContentLocations");
            //get renditions node
            XmlNode rendsNode = contentNode.SelectSingleNode("Renditions");

            //get all rendition nodes
            //get all rendition nodes
            // MP, 09/06/2018, Mantis #1613, handle null reference
            if (rendsNode != null)
            {
                // end of changes by MP on 09/06/2018
                XmlNodeList rendNodeList = rendsNode.SelectNodes("Rendition");
                for (int i = 0; i < rendNodeList.Count; i++)
                {
                    //get format and path nodes
                    XmlNode formatNode = rendNodeList[i].SelectSingleNode("format");
                    XmlNode pathNode = rendNodeList[i].SelectSingleNode("contentpath");

                    //get format and path values
                    string format = formatNode.InnerText;
                    string path = pathNode.InnerText;
                    //store as "-" delimited string in array
                    contentLocations.Add(format + "-" + path);
                }
            }
            // MP, 09/06/2018, Mantis #1613, handle null reference
            //catch (Exception e)
            //{
            //    string exc = e.StackTrace;
            //}
            // end of changes by MP on 09/06/2018

            //return array of content locations
            return contentLocations;
        }

        protected string ReadPluginNode(XmlNode parentNode)
        {
            string library = "";
            string className = "";
            string methodName = "";
            string parameters = "";

            //get plugin nodes
            XmlNode libraryNode = parentNode.SelectSingleNode("library");
            XmlNode classNode = parentNode.SelectSingleNode("class");
            XmlNode methodNode = parentNode.SelectSingleNode("method");
            XmlNode paramsNode = parentNode.SelectSingleNode("parameters");

            if (libraryNode != null)
                library = libraryNode.InnerText;
            if (classNode != null)
                className = classNode.InnerText;
            if (methodNode != null)
                methodName = methodNode.InnerText;
            if (paramsNode != null)
                parameters = paramsNode.InnerText;

            return library + delimiter + className + delimiter + methodName + delimiter + parameters;

        }

        protected string ReadPreconditionNode(XmlNode parentNode)
        {
            string andor = "";
            string field = "";
            string op = "";
            string val = "";

            //get precondition nodes
            XmlAttribute andorAttr = parentNode.Attributes["andor"];

            XmlNode fieldNode = parentNode.SelectSingleNode("field");
            XmlNode opNode = parentNode.SelectSingleNode("op");
            XmlNode valNode = parentNode.SelectSingleNode("value");

            if (andorAttr != null)
                andor = andorAttr.InnerText;
            if (fieldNode != null)
                field = fieldNode.InnerText;
            if (opNode != null)
                op = opNode.InnerText;
            if (valNode != null)
                val = valNode.InnerText;

            /*if (field.StartsWith("{") && field.EndsWith("}"))
                field = field.Substring(1, field.Length-2);
            if (val.StartsWith("{") && val.EndsWith("}"))
                val = val.Substring(1, val.Length-2);
            */

            return andor + delimiter + field + delimiter + op + delimiter + val;

        }


        public string ReadSchemaLocation(string connTagName)
        {
            string schemaPath = "";
            //get Connections node
            XmlNode connsNode = xmlDoc.SelectSingleNode("CompareRule").SelectSingleNode("Connections");
            //get top connection node
            XmlNode connNode = connsNode.SelectSingleNode(connTagName);

            //get schema node
            XmlNode schemaNode = connNode.SelectSingleNode("schema-filepath");
            if (schemaNode != null)
            {
                schemaPath = schemaNode.InnerText;
            }

            return schemaPath;
        }

        /// <summary>
        /// Reads all source column datatypes
        /// </summary>
        /// <returns>Hashtable of source column and datatype</returns>
        public Hashtable ReadSourceColumnDataType()
        {
            Hashtable htSourceColumnDataType = new Hashtable();

            try
            {
                XmlNode sourceColumns = xmlDoc.SelectSingleNode("CompareRule").SelectSingleNode("SourceDataType");
                XmlNodeList sourceColumnsList = sourceColumns.SelectNodes("column");

                for (int i = 0; i < sourceColumnsList.Count; i++)
                {
                    //get source column node
                    XmlNode columnNode = sourceColumnsList[i];
                    //xml node values - sourcecolumn:datatype

                    string[] columns = columnNode.InnerText.Split(new char[] { ':' });
                    //AK, 6/10/15, Mantis #1167, check if field already added, verify value count just in case
                    if (!htSourceColumnDataType.ContainsKey(columns[0].ToString()) && columns.Length == 2)
                        htSourceColumnDataType.Add(columns[0].ToString(), columns[1].ToString());
                    //htSourceColumnDataType.Add(columns[0].ToString(), columns[1].ToString());
                    //end changes by AK on 6/10/15
                }
            }
            catch (Exception e)
            {
                string exc = e.StackTrace;
            }

            return htSourceColumnDataType;


        }

        //AK, 2/26/09, add a function to read DCM query attributes
        //now stores an option for recursive or parallel processing of multi-valued attributes within a query
        /// <summary>
        /// Get a collection of query options stored in query attributes
        /// </summary>
        /// <returns></returns>
        public ListDictionary ReadDCMQueriesOptions()
        {
            ListDictionary ldDCMQueriesOptions = new ListDictionary();

            try
            {
                //get Source Data node
                XmlNode sourceDataNode = xmlDoc.SelectSingleNode("CompareRule").SelectSingleNode("Post-Processing").SelectSingleNode("Conversion-Queries");

                //get query nodes list
                XmlNodeList queryNodeList = sourceDataNode.ChildNodes;

                //loop through query nodes
                for (int i = 0; i < queryNodeList.Count; i++)
                {
                    XmlNode queryNode = queryNodeList[i];

                    //get counter attribute if exists
                    XmlAttribute sQueryCounterAttr = queryNodeList[i].Attributes["counter"];
                    //get parallel vs recursive attribute
                    XmlAttribute sQueryProcessingAttr = queryNodeList[i].Attributes["processing"];
                    string processing = "";
                    if (sQueryProcessingAttr != null)
                    {
                        processing = sQueryProcessingAttr.InnerText;
                    }
                    //add to string[] in case more attributes are added in the future
                    string[] attrs = { processing };
                    //add attrs to hash if query id is known
                    if (sQueryCounterAttr != null)
                    {
                        int queryId = Convert.ToInt32(sQueryCounterAttr.InnerText);
                        ldDCMQueriesOptions.Add(queryId, attrs);
                    }

                }
            }
            catch (Exception e)
            {
                string exc = e.StackTrace;
            }


            return ldDCMQueriesOptions;

        }
        //end changes by AK on 2/26/09



        /// <summary>
        /// Get a list of all queries for converting the migrated document to DCM Compliant document
        /// </summary>
        /// <returns>ListDictionary of DCM conversion queries</returns>
        public ListDictionary ReadDCMQueries()
        {
            ListDictionary ldDCMQueries = new ListDictionary();

            try
            {
                //get Source Data node
                XmlNode sourceDataNode = xmlDoc.SelectSingleNode("CompareRule").SelectSingleNode("Post-Processing").SelectSingleNode("Conversion-Queries");

                //get query nodes list
                XmlNodeList queryNodeList = sourceDataNode.ChildNodes;

                //loop through query nodes
                for (int i = 0; i < queryNodeList.Count; i++)
                {
                    XmlNode queryNode = queryNodeList[i];
                    string query = queryNode.InnerText;

                    //get counter attribute if exists
                    XmlAttribute sQueryCounterAttr = queryNodeList[i].Attributes["counter"];
                    if (sQueryCounterAttr != null)
                    {
                        int queryId = Convert.ToInt32(sQueryCounterAttr.InnerText);
                        ldDCMQueries.Add(queryId, query);
                    }

                }
            }
            catch (Exception e)
            {
                string exc = e.StackTrace;
            }


            return ldDCMQueries;

        }

        /// <summary>
        /// Reads target document class from config XML
        /// </summary>
        /// <returns></returns>
        public string ReadTargetDocumentClass()
        {
            try
            {
                //get target node
                XmlNode mappingNode = xmlDoc.SelectSingleNode("CompareRule").SelectSingleNode("Post-Processing").SelectSingleNode("Auto-Name");
                XmlNode targetTableNode = mappingNode.SelectSingleNode("DocumentClass");

                //return its value
                return targetTableNode.InnerText;
            }
            catch (Exception e)
            {
                string exc = e.StackTrace;
                return "";
            }

        }

        //AK, 11/26/08, read zero padding parameter
        /// <summary>
        /// Reads zero padding from config XML
        /// </summary>
        /// <returns></returns>
        public string ReadAutoNameZeroPadding()
        {
            try
            {
                //get target node
                XmlNode mappingNode = xmlDoc.SelectSingleNode("CompareRule").SelectSingleNode("Post-Processing").SelectSingleNode("Auto-Name");
                XmlNode zeroPaddingNode = mappingNode.SelectSingleNode("zero-padding");

                //return its value
                return zeroPaddingNode.InnerText;
            }
            catch (Exception e)
            {
                string exc = e.StackTrace;
                return "";
            }

        }


        /// <summary>
        /// Reads auto-name assignment from config XML
        /// </summary>
        /// <returns></returns>
        public string ReadDCMAutoNameAssignment()
        {
            try
            {
                //get target node
                XmlNode mappingNode = xmlDoc.SelectSingleNode("CompareRule").SelectSingleNode("Post-Processing").SelectSingleNode("Auto-Name");
                XmlNode targetTableNode = mappingNode.SelectSingleNode("Assignment");

                //return its value
                return targetTableNode.InnerText;
            }
            catch (Exception e)
            {
                string exc = e.StackTrace;
                return "";
            }

        }

        /// <summary>
        /// Reads auto-name custom prefix,suffix and query from config XML
        /// </summary>
        /// <returns></returns>
        public Hashtable ReadDCMCustomNameConfig()
        {
            Hashtable htDCMCustomName = new Hashtable();

            try
            {
                //get target node
                XmlNode mappingNode = xmlDoc.SelectSingleNode("CompareRule").SelectSingleNode("Post-Processing").SelectSingleNode("Auto-Name").SelectSingleNode("Custom");
                //get custom auto-name nodes list
                XmlNodeList customNodeList = mappingNode.ChildNodes;

                //loop through custom nodes
                for (int i = 0; i < customNodeList.Count; i++)
                {
                    XmlNode customNode = customNodeList[i];
                    htDCMCustomName.Add(customNode.Name, customNode.InnerText);
                }

            }
            catch (Exception e)
            {
                string exc = e.StackTrace;
            }
            return htDCMCustomName;
        }

        /// <summary>
        /// Reads all relation data
        /// </summary>
        /// <returns>Hashtable of object type relation data</returns>
        public Hashtable ReadAllRelationRules()
        {
            Hashtable htRelationRules = new Hashtable();

            try
            {
                XmlNode relationsNode = xmlDoc.SelectSingleNode("CompareRule").SelectSingleNode("Post-Processing").SelectSingleNode("Relations");
                XmlNodeList relationsNodeList = relationsNode.SelectNodes("Relation");

                for (int i = 0; i < relationsNodeList.Count; i++)
                {
                    //get relation node
                    XmlNode relationNode = relationsNodeList[i];
                    //get rule counter to use as ID
                    string ruleId = relationNode.Attributes["counter"].InnerText;

                    Hashtable htRelation = new Hashtable();
                    XmlNodeList relationParamsList = relationNode.ChildNodes;

                    //get all rule nodes and store in hashtable
                    for (int j = 0; j < relationParamsList.Count; j++)
                    {
                        string name = relationParamsList[j].Name;
                        string val = relationParamsList[j].InnerText;
                        htRelation.Add(name, val);
                    }

                    //add hashtable with rule details to hashtable storing all rules
                    //use counter as ID
                    htRelationRules.Add(Convert.ToInt32(ruleId), htRelation);

                }
            }
            catch (Exception e)
            {
                string exc = e.StackTrace;
            }

            return htRelationRules;


        }

        /// <summary>
        /// Reads Virtual document tags US, 7/5/07, Fusion changes
        /// </summary>
        /// <param name="xmlTagName">VDoc XML node name</param>
        /// <returns>Global setting value</returns>
        public string ReadVDocQuery(string xmlTagName)
        {
            try
            {
                //get node
                XmlNode VDocNode = xmlDoc.SelectSingleNode("CompareRule").SelectSingleNode("SourceVDoc");

                //JE 05MAR19 Mantis 1699 - add null check to avoid exception, created returnVal and added if block around the set innertext lines below
                //get given setting node
                string returnVal = null;
                if (VDocNode != null)
                { 
                    XmlNode vQueryNode = VDocNode.SelectSingleNode(xmlTagName);
                    if (vQueryNode != null)
                    {
                        returnVal = vQueryNode.InnerText;
                    }
                }
                //return setting value
                //return vQueryNode.InnerText;
                return returnVal;
                //end changes JE on 05MAR19
            }
            catch (Exception e)
            {
                string exc = e.StackTrace;
                return "";
            }
        }

        //AK, 8/6/08, read supplemental connections from configuration
        //the function sets a hashtable of supplemental connection indexes and corresponding connection types, 
        //hashtable of supplemental connection indexes and corresponding connection strings 
        //and another hashtable of supplemental connection indexes and corresponding connection names
        public bool ReadSupplementalConnections(ref Hashtable htSuppConnTypes, ref Hashtable htSuppConnStrings, ref Hashtable htSuppConnNames)
        {

            try
            {
                //get supplemental connections node list
                XmlNodeList suppConnNodes = xmlDoc.SelectNodes("/CompareRule/Connections/SupplementalConnection");
                //loop
                for (int i = 0; i < suppConnNodes.Count; i++)
                {
                    //get supp conn node
                    XmlNode suppConnNode = suppConnNodes[i];
                    //check if type is set
                    string type = suppConnNode.SelectSingleNode("type").InnerText;
                    if (type == null || type.Length < 1)
                    {
                        //incomplete configuration
                        return false;
                    }

                    //get connection type
                    int connType = Convert.ToInt32(suppConnNode.SelectSingleNode("type").InnerText);
                    //get connection name
                    string connName = suppConnNode.SelectSingleNode("type_name").InnerText;
                    //get connection string
                    string connStr = suppConnNode.SelectSingleNode("connection-string").InnerText;
                    //get conn index
                    XmlAttribute connIndexAttr = suppConnNode.Attributes["connIndex"];
                    if (connIndexAttr != null)	//cannot continue if index is not set
                    {
                        //get conn index
                        int connIndex = Convert.ToInt32(connIndexAttr.InnerText);

                        //store all data in hashtables with connIndex as a key
                        //connection strings
                        htSuppConnStrings.Add(connIndex, connStr);
                        //connection types
                        htSuppConnTypes.Add(connIndex, connType);
                        //connection names
                        htSuppConnNames.Add(connIndex, connName);
                    }
                    else
                    {
                        //incomplete configuration
                        return false;
                    }
                }
            }
            catch (Exception exc)
            {
                string e = exc.StackTrace;
                return false;
            }

            return true;

        }


        //AK, 1/17/17, Mantis #1421, add this function to also read supplemental connection schemas
        public bool ReadSupplementalConnections(ref Hashtable htSuppConnTypes, ref Hashtable htSuppConnStrings, ref Hashtable htSuppConnNames, 
            ref Hashtable htSuppConnSchemas)
        {

            try
            {
                //get supplemental connections node list
                XmlNodeList suppConnNodes = xmlDoc.SelectNodes("/CompareRule/Connections/SupplementalConnection");
                //loop
                for (int i = 0; i < suppConnNodes.Count; i++)
                {
                    //get supp conn node
                    XmlNode suppConnNode = suppConnNodes[i];
                    //check if type is set
                    string type = suppConnNode.SelectSingleNode("type").InnerText;
                    if (type == null || type.Length < 1)
                    {
                        //incomplete configuration
                        return false;
                    }

                    //get connection type
                    int connType = Convert.ToInt32(suppConnNode.SelectSingleNode("type").InnerText);
                    //get connection name
                    string connName = suppConnNode.SelectSingleNode("type_name").InnerText;
                    //get connection string
                    string connStr = suppConnNode.SelectSingleNode("connection-string").InnerText;
                    //get conn index
                    XmlAttribute connIndexAttr = suppConnNode.Attributes["connIndex"];

                    //AK, 1/17/17, Mantis #1421, get conn schema
                    string connSchemaStr = suppConnNode.SelectSingleNode("schema-filepath").InnerText;

                    if (connIndexAttr != null)	//cannot continue if index is not set
                    {
                        //get conn index
                        int connIndex = Convert.ToInt32(connIndexAttr.InnerText);

                        //store all data in hashtables with connIndex as a key
                        //connection strings
                        htSuppConnStrings.Add(connIndex, connStr);
                        //connection types
                        htSuppConnTypes.Add(connIndex, connType);
                        //connection names
                        htSuppConnNames.Add(connIndex, connName);
                        ////AK, 1/17/17, Mantis #1421, store schema file path in hashtable
                        htSuppConnSchemas.Add(connIndex, connSchemaStr);
                    }
                    else
                    {
                        //incomplete configuration
                        return false;
                    }
                }
            }
            catch (Exception exc)
            {
                string e = exc.StackTrace;
                return false;
            }

            return true;

        }
        //AK, 8/6/08, read supplemental queries info from configuration
        //returns a hashtable containing query index as key and conn index as value
        public Hashtable ReadSupplementalQueriesInfo()
        {
            Hashtable htSuppQueries = new Hashtable();
            try
            {
                XmlNodeList supplementalQueryXml = xmlDoc.SelectNodes("/CompareRule/SourceData/Supplemental");
                if (supplementalQueryXml != null)
                {
                    for (int i = 0; i < supplementalQueryXml.Count; i++)
                    {
                        XmlNode suppQueryNode = supplementalQueryXml[i];
                        //get conn index attribute
                        XmlAttribute connIndexAttr = suppQueryNode.Attributes["connIndex"];
                        //get query counter attribute
                        XmlAttribute queryCounter = suppQueryNode.Attributes["counter"];
                        if (connIndexAttr != null && queryCounter != null)
                        {
                            int connIndex = Convert.ToInt32(connIndexAttr.InnerText);
                            int queryId = Convert.ToInt32(queryCounter.InnerText);
                            //store in return hashtable
                            htSuppQueries.Add(queryId, connIndex);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                string exc = e.StackTrace;
                return null;
            }
            return htSuppQueries;
        }

        //AK, 8/7/08, read primary docbase info from XML
        /// <summary>
        /// Reads connections data
        /// </summary>
        /// <param name="selectedConnection">Source, target or supplemental connection index</param>
        /// <param name="datasource">Ref to connection type</param>
        /// <param name="iConnType">Ref to connection type index</param>
        /// <param name="connStr">Ref to connection string</param>
        /// <returns>Hashtable containing connection information</returns>
        public Hashtable ReadConnectionData(ref string datasource, ref int iConnType,
            ref string connStr, string appPath, string connection)
        {
            Hashtable htConnData = new Hashtable();

            try
            {

                //get Connections node
                XmlNode connsNode = xmlDoc.SelectSingleNode("CompareRule").SelectSingleNode("Connections");

                //get top connection node
                XmlNode topConnNode = connsNode.SelectSingleNode(connection);

                //get connection type
                XmlNode typeNode = topConnNode.SelectSingleNode("type");
                if (typeNode.InnerText.Length > 0)
                    iConnType = Convert.ToInt32(typeNode.InnerText);
                else
                    iConnType = -1;

                //get connection type name
                XmlNode typeNameNode = topConnNode.SelectSingleNode("type_name");
                datasource = typeNameNode.InnerText;

                //get conn string
                XmlNode connStrNode = topConnNode.SelectSingleNode("connection-string");
                connStr = connStrNode.InnerText;

                ConnXMLParser connXmlParser = new ConnXMLParser(appPath + "Connections - Internal.xml");
                string passwordParam = connXmlParser.FindPasswordParam(datasource);

                //AK, 1/17/17, Mantis #1421, add schema location to the connection data
                XmlNode schemaFileNode = topConnNode.SelectSingleNode("schema-filepath");
                if (schemaFileNode != null)
                    htConnData.Add("schema_filepath", schemaFileNode.InnerText);
                //end changes by AK on 1/17/17

                //get parameters
                XmlNodeList paramsListNode = topConnNode.SelectNodes("params");
                if (paramsListNode != null)
                {
                    for (int i = 0; i < paramsListNode.Count; i++)
                    {
                        XmlNode paramsNode = paramsListNode[i];
                        XmlNode paramName = paramsNode.SelectSingleNode("param_name");
                        XmlNode paramValue = paramsNode.SelectSingleNode("param_value");

                        string strParamName = paramName.InnerText;
                        string strParamValue = paramValue.InnerText;
                        //decrypt password
                        if (strParamName.Equals(passwordParam))
                            strParamValue = CryptoUtil.DecryptString(strParamValue, "saltstring");
                        htConnData.Add(strParamName, strParamValue);
                    }
                }
            }
            catch (Exception e)
            {
                string exc = e.StackTrace;
            }

            //return connection parameters stored as name-value pairs
            return htConnData;
        }

        //kd, 11/1/13, read connection codes from XML
        /// <summary>
        /// Reads connections connection
        /// </summary>
        /// <param name="selectedConnection">Source, target or supplemental connection index</param>        
        /// <returns>Hashtable containing connection information</returns>
        public Hashtable ReadConnectionCodes(string connection)
        {
            Hashtable htConnCodes = new Hashtable();

            try
            {

                //get Connections node
                XmlNode connsNode = xmlDoc.SelectSingleNode("CompareRule").SelectSingleNode("Connections");

                //get top connection node

                if (connection.ToLower() != "supplementalconnection".ToLower())
                {
                    XmlNode topConnNode = connsNode.SelectSingleNode(connection);


                    //get connection type name
                    XmlNode useProjectNode = topConnNode.SelectSingleNode("use_project");
                    if (useProjectNode != null)
                    {
                        if (useProjectNode.InnerText.Trim().ToLower().Equals("true"))
                        {
                            XmlNode conCodeNode = topConnNode.SelectSingleNode("connection_code");
                            if (conCodeNode != null)
                                htConnCodes.Add(1, conCodeNode.InnerText.Trim());
                        }
                    }
                }
                else
                {
                    XmlNodeList supConNodes = connsNode.SelectNodes(connection);
                    for (int i = 0; i < supConNodes.Count; i++)
                    {
                        XmlNode suppConnNode = supConNodes[i];
                        XmlAttribute connIndexAttr = suppConnNode.Attributes["connIndex"];


                        if (connIndexAttr != null)	//cannot continue if index is not set
                        {
                            //get conn index
                            int connIndex = Convert.ToInt32(connIndexAttr.InnerText);

                            XmlNode useProjectNode = suppConnNode.SelectSingleNode("use_project");
                            if (useProjectNode != null)
                            {
                                if (useProjectNode.InnerText.Trim().ToLower().Equals("true"))
                                {
                                    XmlNode conCodeNode = suppConnNode.SelectSingleNode("connection_code");
                                    if (conCodeNode != null)
                                        htConnCodes.Add(connIndex, conCodeNode.InnerText.Trim());
                                }
                            }



                        }
                    }

                }




            }
            catch (Exception e)
            {
                string exc = e.StackTrace;
            }

            //return connection parameters stored as name-value pairs
            return htConnCodes;
        }


        //kd, 11/1/13, read connection data from project file
        /// <summary>
        /// Reads connections connection
        /// </summary>
        /// <param name="selectedConnection">Source, target or supplemental connection index</param>        
        /// <returns>Hashtable containing connection information</returns>
        public Hashtable ReadConnectionDataFromProjectFile(string projectFile, ref Hashtable htConnParams)
        {

            Hashtable htConnCodes = new Hashtable();

            XmlDocument xmlProjDoc = new XmlDocument();
            xmlProjDoc.Load(projectFile);




            try
            {

                XmlNodeList ConnNodes = xmlProjDoc.SelectNodes("/ValianceProject/SharedConnections/Connection");

                for (int i = 0; i < ConnNodes.Count; i++)
                {
                    //get supp conn node
                    XmlNode ConnNode = ConnNodes[i];

                    //AK, 3/13/15, Mantis #1090, should be connection_code
                    if (ConnNode.SelectSingleNode("connection_code") != null)
                        //if (ConnNode.SelectSingleNode("connectioncode") != null)
                    //end changes by AK on 3/13/15
                    {
                        Hashtable htConnData = new Hashtable();
                        Hashtable htProjectParams = new Hashtable();
                        //get connection type
                        int iConnType;
                        XmlNode typeNode = ConnNode.SelectSingleNode("type");
                        if (typeNode.InnerText.Length > 0)
                            iConnType = Convert.ToInt32(typeNode.InnerText);
                        else
                            iConnType = -1;

                        htConnData.Add("type", iConnType);

                        //get connection type name
                        XmlNode typeNameNode = ConnNode.SelectSingleNode("type_name");
                        htConnData.Add("type_name", typeNameNode.InnerText);

                        //get conn string
                        XmlNode connStrNode = ConnNode.SelectSingleNode("connection-string");
                        htConnData.Add("connection-string", connStrNode.InnerText);

                        //AK, 1/17/17, Mantis #1421, add schema location to the connection data
                        XmlNode schemaFileNode = ConnNode.SelectSingleNode("schema-filepath");
                        if (schemaFileNode != null)
                            htConnData.Add("schema_filepath", schemaFileNode.InnerText);
                        //end changes by AK on 1/17/17

                        //AK, 3/13/15, Mantis #1090, should be connection_code
                        string connectionCode = ConnNode.SelectSingleNode("connection_code").InnerText;
                        //string connectionCode = ConnNode.SelectSingleNode("connectioncode").InnerText;
                        //end changes by AK on 3/13/15

                        if (!htConnCodes.ContainsKey(connectionCode))
                        {
                            //AK, 3/13/15, Mantis #1090, should be connection_code
                            htConnCodes.Add(ConnNode.SelectSingleNode("connection_code").InnerText, htConnData);
                            //htConnCodes.Add(ConnNode.SelectSingleNode("connectioncode").InnerText, htConnData);
                            //end changes by AK on 3/13/15
                        }

                        //get parameters
                        XmlNodeList paramsListNode = ConnNode.SelectNodes("params");
                        if (paramsListNode != null)
                        {
                            for (int K = 0; K < paramsListNode.Count; K++)
                            {
                                XmlNode paramsNode = paramsListNode[K];
                                XmlNode paramName = paramsNode.SelectSingleNode("param_name");
                                XmlNode paramValue = paramsNode.SelectSingleNode("param_value");

                                string strParamName = paramName.InnerText;
                                string strParamValue = paramValue.InnerText;

                                if (strParamName.Trim().ToLower().Equals("password"))
                                {
                                    strParamValue = CryptoUtil.DecryptString(strParamValue, "saltstring");
                                }

                                //decrypt password

                                htProjectParams.Add(strParamName, strParamValue);
                            }

                            if (!htConnParams.ContainsKey(connectionCode))
                            {
                                htConnParams.Add(connectionCode, htProjectParams);
                            }
                        }


                    }



                }




            }
            catch (Exception e)
            {
                string exc = e.StackTrace;
            }

            //return connection parameters stored as name-value pairs
            return htConnCodes;
        }

        /// <summary>
        /// AK, 3/10/16, Mantis #1298, read project code from project file
        /// </summary>
        /// <param name="projectFile"></param>
        /// <returns></returns>
        public string ReadProjectCode(string projectFile)
        {
            string projectCode = "";

            try
            {
                //load project file
                XmlDocument xmlProjDoc = new XmlDocument();
                xmlProjDoc.Load(projectFile);
                //get project code node
                XmlNode projCodeNode = xmlProjDoc.SelectSingleNode("/ValianceProject/projectCode");
                //get its value
                if (projCodeNode != null)
                {
                    projectCode = projCodeNode.InnerText;
                }

            }
            catch
            {
                //return empty string
                return "";
            }
            return projectCode;
        }


        //kd, 11/1/13, read connection codes from XML
        /// <summary>
        /// Reads Global Variables from Project File
        /// </summary>
        /// <param name="selectedConnection">Source, target or supplemental connection index</param>        
        /// <returns>Hashtable containing connection information</returns>
        public String ReplaceGlobalVariablesFromProjectFile(string projectFile)
        {


            try
            {
                Hashtable htGlobal = new Hashtable();

                XmlDocument xmlProjDoc = new XmlDocument();
                xmlProjDoc.Load(projectFile);

                // string xmlMapping = xmlDoc.ToString();

                StringWriter stringWriter = new StringWriter();
                XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
                xmlTextWriter.Formatting = Formatting.Indented;
                xmlDoc.WriteTo(xmlTextWriter);
                string xmlMapping = "";

                xmlMapping = stringWriter.ToString();
                xmlTextWriter.Flush();
                xmlTextWriter.Close();





            

                XmlNodeList GlobalNodes = xmlProjDoc.SelectNodes("/ValianceProject/global_variables/global_variable");

                for (int i = 0; i < GlobalNodes.Count; i++)
                {
                    //get supp conn node
                    XmlNode GlobalNode = GlobalNodes[i];

                    if (GlobalNode.SelectSingleNode("name") != null)
                    {
                        string globalVarName = GlobalNode.SelectSingleNode("name").InnerText;
                        string globalVarValue = GlobalNode.SelectSingleNode("value").InnerText;
                        
                        //AK, 5/12/15, replace special XML characters
                        //&lt; (<), &amp; (&), &gt; (>), &quot; ("), and &apos; (').
                        globalVarValue = globalVarValue.Replace("&", "&amp;");
                        globalVarValue = globalVarValue.Replace("<", "&lt;");
                        globalVarValue = globalVarValue.Replace(">", "&gt;");
                        globalVarValue = globalVarValue.Replace("\"", "&quot;");
                        globalVarValue = globalVarValue.Replace("'", "&apos;");
                        //end changes by AK on 5/12/15

                        if (xmlMapping.Contains(globalVarName))
                        {
                            //Regex regexText = new Regex(globalVarName, RegexOptions.IgnoreCase);

                            xmlMapping = xmlMapping.Replace(globalVarName, globalVarValue);
                            
                            //xmlMapping = regexText.Replace(xmlMapping, GlobalNode.SelectSingleNode("value").InnerText).ToString();
                            //xmlMapping = regexText.Replace(xmlMapping, globalVarValue).ToString();
                            //xmlDoc = (XmlDocument)xmlDoc.LoadXml(xmlMapping); 
                            XmlTextReader xreader = new XmlTextReader(new System.IO.StringReader(xmlMapping));
                            xreader.Read();
                            if (xreader != null)
                            {
                                XmlDocument xmlDocTemp= new XmlDocument();
                                                                
                                xmlDocTemp.Load(xreader);

                                if (xmlDocTemp.HasChildNodes)
                                {
                                    xmlDoc = xmlDocTemp;
                                }
                                
                                
                            }
                        }

                       

                    }



                }




            }
            catch (Exception e)
            {
                string exc = e.StackTrace;
                return e.Message.ToString();
            }

            return "";
            //return connection parameters stored as name-value pairs

        }


        //JE 01/06/2017 Mantis 1414 - This func copied from ReplaceGlobalVariablesFromProjectFile and ReplaceLocalVariables...
        // to offer something that actually returns a string with replaced vals 
        /// <summary>
        /// Reads Global Variables from Project File and Local Variables from configFile
        /// </summary>
        /// <param name="strToGetReplacements">String which will get values replaced where they are found</param>        
        /// <param name="projectFile">String Filename of TSPX file containing global variables</param>        
        /// <param name="htLocalInstanceVars">Hashtable of Local Instance values, 'default' where there is a single instance</param>        
        /// <param name="configFile">String for config xml file</param>        
        /// <returns>String containing variables replaced with their values as defined in the project and config</returns>
        public String ReplaceLocalAndGlobalVariables(string strToGetReplacements, string projectFile, Hashtable htLocalInstanceVars, string configFile)
        {
            string strReturn;
            
            try
            {
                string strReturnTemp;
            
                //do globals
                Hashtable htGlobal = new Hashtable();
                XmlDocument xmlProjDoc = new XmlDocument();
                xmlProjDoc.Load(projectFile);
                XmlNodeList GlobalNodes = xmlProjDoc.SelectNodes("/ValianceProject/global_variables/global_variable");
                for (int i = 0; i < GlobalNodes.Count; i++)
                {
                    XmlNode GlobalNode = GlobalNodes[i];
                    if (GlobalNode.SelectSingleNode("name") != null)
                    {
                        string globalVarName = GlobalNode.SelectSingleNode("name").InnerText;
                        string globalVarValue = GlobalNode.SelectSingleNode("value").InnerText;

                        //JE commented out below character replacements anticipating this func used mostly for connection params
                        //AK, 5/12/15, replace special XML characters
                        //&lt; (<), &amp; (&), &gt; (>), &quot; ("), and &apos; (').
                         //globalVarValue = globalVarValue.Replace("&", "&amp;");
                         //globalVarValue = globalVarValue.Replace("<", "&lt;");
                         //globalVarValue = globalVarValue.Replace(">", "&gt;");
                         //globalVarValue = globalVarValue.Replace("\"", "&quot;");
                         //globalVarValue = globalVarValue.Replace("'", "&apos;");
                        //end changes by AK on 5/12/15

                        if (strToGetReplacements.Contains(globalVarName))
                        {
                            strToGetReplacements = strToGetReplacements.Replace(globalVarName, globalVarValue);
                        }
                    }
                }
                strReturnTemp = strToGetReplacements;

                //do locals
                if (htLocalInstanceVars != null)
                {
                    IEnumerator ikeys = htLocalInstanceVars.Keys.GetEnumerator();
                    for (int i = 0; i < htLocalInstanceVars.Count; i++)
                    {
                        ikeys.MoveNext();
                        string varName = ikeys.Current.ToString();
                        string varValue = (string)htLocalInstanceVars[ikeys.Current];

                        //JE commented out below character replacements anticipating this func used mostly for connection params
                        //AK, 5/12/15, replace special XML characters
                        //&lt; (<), &amp; (&), &gt; (>), &quot; ("), and &apos; (').
                        //varValue = varValue.Replace("&", "&amp;");
                        //varValue = varValue.Replace("<", "&lt;");
                        //varValue = varValue.Replace(">", "&gt;");
                        //varValue = varValue.Replace("\"", "&quot;");
                        //varValue = varValue.Replace("'", "&apos;");
                        //end changes by AK on 5/12/15

                        if (strReturnTemp.Contains(varName))
                        {
                            strReturnTemp = strReturnTemp.Replace(varName, varValue);
                        }
                    }
                }
                strReturn = strReturnTemp;
            }
            catch (Exception e)
            {
                string exc = e.StackTrace;
                return e.Message.ToString();
            }
            return strReturn;
            //return connection parameters string
        }


        //AK, 6/1/10, read rules to create merged PDFs from Qumas attachments
        public SortedList ReadQumasContentRules(ref Hashtable htQumasRuleRows)
        {
            SortedList qumasRules = new SortedList();

            try
            {
                XmlNode contentNode = xmlDoc.SelectSingleNode("CompareRule").SelectSingleNode("Mappings").SelectSingleNode("ContentLocations");
                //get Qumas rules node
                XmlNode qumasRulesNode = contentNode.SelectSingleNode("qumas-merged-pdf-rules");

                //if parent node exists
                if (qumasRulesNode != null)
                {
                    XmlNodeList qumasRulesNodeList = qumasRulesNode.SelectNodes("qumas-content-rule");
                    if (qumasRulesNodeList != null)
                    {
                        //loop through rules nodes
                        for (int i = 0; i < qumasRulesNodeList.Count; i++)
                        {
                            XmlNode ruleNode = qumasRulesNodeList[i];
                            //get values
                            string andOr = "";
                            string field = "";
                            string op = "";
                            string val = "";
                            string mainOrAttach = "";

                            //get rule parameters
                            XmlNode andOrNode = ruleNode.SelectSingleNode("andOr");
                            XmlNode fieldNode = ruleNode.SelectSingleNode("field");
                            XmlNode opNode = ruleNode.SelectSingleNode("op");
                            XmlNode valNode = ruleNode.SelectSingleNode("value");
                            XmlNode mainOrAttachNode = ruleNode.SelectSingleNode("mainOrAttach");

                            //get row-rule ID pair
                            XmlAttribute ruleIDAttr = ruleNode.Attributes["ruleID"];
                            XmlAttribute rowIDAttr = ruleNode.Attributes["rowID"];
                            if (ruleIDAttr != null && rowIDAttr != null)
                            {
                                if (!htQumasRuleRows.ContainsKey(Convert.ToInt32(rowIDAttr.InnerText)))
                                    htQumasRuleRows.Add(Convert.ToInt32(rowIDAttr.InnerText), Convert.ToInt32(ruleIDAttr.InnerText));
                            }

                            if (andOrNode != null)
                                andOr = andOrNode.InnerText;
                            if (fieldNode != null)
                                field = fieldNode.InnerText;
                            if (opNode != null)
                                op = opNode.InnerText;
                            if (valNode != null)
                                val = valNode.InnerText;
                            if (mainOrAttachNode != null)
                                mainOrAttach = mainOrAttachNode.InnerText;

                            string[] rule = new string[] { andOr, field, op, val, mainOrAttach };
                            //store in return hash
                            qumasRules.Add(i + 1, rule);

                        }
                    }
                }
            }
            catch (Exception e)
            {
                string exc = e.StackTrace;
            }
            return qumasRules;
        }
        //end changes by AK on 6/1/10

        //AK, 10/31/12, 3.0 HF 3, read pre-processing or post-processing commands
        public SortedList GetPrePostCommands(string nodeName)
        {
            SortedList commands = new SortedList();
            try
            {
                //get mapping node
                XmlNode mappingNode = xmlDoc.SelectSingleNode("CompareRule").SelectSingleNode("Mappings");
                //get pre- or post- processing node
                XmlNode processingNode = mappingNode.SelectSingleNode(nodeName);
                //if exists, get commands
                if (processingNode != null)
                {
                    //get commands
                    XmlNodeList commandsList = processingNode.SelectNodes("command");
                    if (commandsList != null)
                    {
                        //store commands in sorted list with index attr value as key
                        for (int i = 0; i < commandsList.Count; i++)
                        {
                            XmlNode commandNode = commandsList[i];
                            //get command index
                            string index = commandNode.Attributes["index"].InnerText;
                            //get connection index and append to sql command if specified
                            XmlAttribute connIndexAttr = commandNode.Attributes["connIndex"];
                            if (connIndexAttr != null)
                            {
                                string connIndex = commandNode.Attributes["connIndex"].InnerText;
                                string cmd = connIndex + "-" + commandNode.InnerText;
                                commands.Add(index, cmd);
                            }
                            else
                            {
                                //just use node text
                                string cmd = commandNode.InnerText;
                                commands.Add(index, cmd);
                            }
                        }
                    }
                }

            }

            catch (Exception e)
            {
                string exc = e.StackTrace;
                return null;
            }
            return commands;


        }
        //end changes by AK on 10/31/12

        //AK, 11/13/13, 4.0, read pre/post processing commands from the project file
        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectFile"></param>
        /// <param name="configFile"></param>
        /// <param name="type">export or import</param>
        /// <returns></returns>
        public SortedList ReadPrePostCommandsFromProjectFile(string projectFile, string type, string procType)
        {
            //sorted list to store commands
            SortedList slCommands = new SortedList();
            //load project file
            XmlDocument xmlProjDoc = new XmlDocument();
            xmlProjDoc.Load(projectFile);
            //counter for now
            int c = 0;

            try
            {
                //get a map of connection codes and corresponding connections from config
                ArrayList codeMap = GetConnCodesTypesMap(type);
                //get pre/post processing node
                XmlNode processingNode = xmlProjDoc.SelectSingleNode("/ValianceProject/PrePostProcessing");
                if (processingNode != null)
                {
                    //get a list of nodes
                    XmlNodeList procNodes = processingNode.SelectNodes("pre_post_process");
                    if (procNodes != null)
                    {
                        //loop through nodes, store commands in sorted list with index attr value as key
                        for (int i = 0; i < procNodes.Count; i++)
                        {
                            XmlNode procNode = procNodes[i];
                            //get type
                            XmlNode procTypeNode = procNode.SelectSingleNode("type");
                            //get pre or post process
                            XmlNode prePostNode = procNode.SelectSingleNode("pre_post");
                            if (procTypeNode != null)
                            {
                                //compare to type needed
                                if (procTypeNode.InnerText.ToUpper().Equals(type.ToUpper()) &&
                                    prePostNode.InnerText.ToUpper().Equals(procType.ToUpper()))
                                {
                                    //continue, get connection code
                                    XmlNode connCodeNode = procNode.SelectSingleNode("connection_code");
                                    //get command node
                                    XmlNode commandNode = procNode.SelectSingleNode("command");
                                    if (connCodeNode != null && commandNode != null)
                                    {
                                        string connCode = connCodeNode.InnerText;
                                        string command = commandNode.InnerText;
                                        //look for this conn code in connections map
                                        for (int j = 0; j < codeMap.Count; j++)
                                        {
                                            string codeAndIndex = (string)codeMap[j];
                                            //get conn code
                                            string code = codeAndIndex.Substring(0, codeAndIndex.IndexOf(":::"));
                                            //get conn index
                                            string index = codeAndIndex.Substring(codeAndIndex.IndexOf(":::") + 3);
                                            //if matches, add to return
                                            if (code.Equals(connCode))
                                            {
                                                c++;
                                                if (type.ToUpper().Equals("IMPORT"))
                                                    slCommands.Add(c.ToString(), command);
                                                else
                                                    slCommands.Add(c.ToString(), index + "-" + command);
                                            }
                                        }

                                    }
                                }
                            }
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                string e = ex.Message;
            }

            return slCommands;
        }

        //AK, 11/13/13, get a map of connection codes and corresponding indexes to use for pre/post processing command
        public ArrayList GetConnCodesTypesMap(string type)
        {
            ArrayList connTypes = new ArrayList();

            try
            {
                if (type.ToUpper().Equals("EXPORT"))
                {
                    //get supplemental connections node list
                    XmlNodeList suppConnNodes = xmlDoc.SelectNodes("/CompareRule/Connections/SupplementalConnection");
                    //loop
                    if (suppConnNodes != null)
                    {
                        for (int i = 0; i < suppConnNodes.Count; i++)
                        {
                            //get supp conn node
                            XmlNode suppConnNode = suppConnNodes[i];
                            //get connection code node
                            XmlNode connCodeNode = suppConnNode.SelectSingleNode("connection_code");
                            //get conn index attr
                            XmlAttribute connIndexAttr = suppConnNode.Attributes["connIndex"];
                            if (connCodeNode != null && connIndexAttr != null)	//cannot continue if index is not set
                            {
                                //get conn index
                                string connIndex = connIndexAttr.InnerText;
                                //get conn code
                                string connCode = connCodeNode.InnerText;
                                //add to array
                                connTypes.Add(connCode + ":::" + connIndex);
                            }
                        }
                    }
                    //get source connection
                    XmlNode primaryConnNode = xmlDoc.SelectSingleNode("/CompareRule/Connections/SourceConnection");
                    if (primaryConnNode != null)
                    {
                        //get connection code node
                        XmlNode connCodeNode = primaryConnNode.SelectSingleNode("connection_code");
                        if (connCodeNode != null)
                        {
                            //get conn code
                            string connCode = connCodeNode.InnerText;
                            //add to array
                            connTypes.Add(connCode + ":::primary");
                        }

                    }

                }

                //get target connection
                if (type.ToUpper().Equals("IMPORT"))
                {
                    XmlNode primaryConnNode = xmlDoc.SelectSingleNode("/CompareRule/Connections/TargetConnection");
                    if (primaryConnNode != null)
                    {
                        //get connection code node
                        XmlNode connCodeNode = primaryConnNode.SelectSingleNode("connection_code");
                        if (connCodeNode != null)
                        {
                            //get conn code
                            string connCode = connCodeNode.InnerText;
                            //add to array
                            connTypes.Add(connCode + ":::primary");
                        }

                    }
                }

            }
            catch (Exception exc)
            {
                string e = exc.StackTrace;
            }
            return connTypes;
        }

        //AK, 7/11/14, TM 4.0, add function to read new content rules
        public ArrayList ReadNewContentRules(ref string exportTo)
        {
            ArrayList contentRules = new ArrayList();

            try
            {
                //get content locations node
                XmlNode mappingNode = xmlDoc.SelectSingleNode("CompareRule").SelectSingleNode("Mappings");
                XmlNode contentNode = mappingNode.SelectSingleNode("ContentLocations");
                //get rules node
                XmlNodeList rulesNodes = contentNode.SelectNodes("ContentRule");

                //loop through rules nodes
                for (int i = 0; i < rulesNodes.Count; i++)
                {
                    //store rule details in hash
                    Hashtable htRuleDetails = new Hashtable();
                    //vars to hold rule parameters
					//AK, 8/18/17, Mantis #1475, add "enabled"               
                    string index = "", source_type = "", source_value = "", target_type = "", target_value = "", enabled = "";
                    //JE 09152017 Mantis 1458 - vars for TrackWise content export options
                    string source_config_form_user = "",
                        source_use_config_form_layout = "",
                        source_incl_fields_not_on_form = "",
                        source_incl_fields_blank = "",
                        source_content_image = "",
                        source_headerfields = "",
                        source_incl_activity_id = "",
                        source_include_minimum_count = "",
                        source_minimum_count_value = "",
                        source_incl_performedby_id = "",
                        source_include_pr_workflow = "",
                        source_external_file_store_host = "";
                        
                    //AK, 1/13/17, Mantis #1420, read mapping "exportto" if defined on the rule level
                    string ruleExportTo = "";
                    //end changes by AK on 1/13/17
                    //JE 5/24/18 Mantis 1556 - crystal report content type parameters
                    string content_format = "";
                    string report_params = "";
                    //end changes JE 5/24/18
                    try
                    {
                        //get rule node
                        XmlNode ruleNode = rulesNodes[i];
                        //get its index
                        XmlAttribute indexAttr = ruleNode.Attributes["index"];
                        if (indexAttr != null)
                            index = indexAttr.InnerText;
                        else
                        {
                            //assign a counter
                            index = i.ToString();
                        }

                        //AK, 8/18/17, Mantis #1475, read "enabled" flag
                        XmlNode enabledNode = ruleNode.SelectSingleNode("enabled");
                        if (enabledNode != null)
                        {
                            enabled = enabledNode.InnerText;
                            htRuleDetails.Add("enabled", enabled);
                        }
                        //end changes by AK on 8/18/17


                        //get rule details
                        //AK, 1/13/17, Mantis #1420, read exportto if defined on a rule level
                        XmlNode exportToNode = ruleNode.SelectSingleNode("exportTo");
                        if (exportToNode != null)
                        {
                            ruleExportTo = exportToNode.InnerText;
                            htRuleDetails.Add("ruleExportTo", ruleExportTo);
                        }
                        //end changes by AK on 1/13/17
                        //source
                        XmlNode sourceNode = ruleNode.SelectSingleNode("Source");
                        source_type = sourceNode.SelectSingleNode("content_type").InnerText ;

                        //AK, 6/16/15, Mantis #1179, add an option to export text as many-to-one into a file 
                        if (source_type.Equals("Source Field - Many-to-One"))
                        {
                            //get XML under content_value - source values and target delimiter
                            //get a list of source_value nodes and store in array
                            ArrayList alSourceValues = new ArrayList();
                            XmlNode sourceValues = sourceNode.SelectSingleNode("content_value").SelectSingleNode("source-values");
                            XmlNodeList lstSourceValues = sourceValues.SelectNodes("source-value");
                            for (int k = 0; k < lstSourceValues.Count; k++)
                            {
                                string srcValue = lstSourceValues[k].InnerText;
                                alSourceValues.Add(srcValue);
                            }
                            //store source value array in return hash
                            htRuleDetails.Add("source_values", alSourceValues);
                            //get target delimiter value
                            string targetDel = "";
                            XmlNode targetDelNode = sourceNode.SelectSingleNode("content_value").SelectSingleNode("target-delimiter");
                            if (targetDelNode != null)
                                targetDel = targetDelNode.InnerText;
                            //store target delimiter in return hash
                            htRuleDetails.Add("target-delimiter", targetDel);
                        }
                        // JE 09/15/2017 Mantis 1460 - export content from trackwise to pdf
                        else if (source_type.Equals("TrackWise PR"))
                        {
                            source_config_form_user = sourceNode.SelectSingleNode("config_form_user").InnerText;
                            source_use_config_form_layout = sourceNode.SelectSingleNode("use_config_form_layout").InnerText;
                            source_incl_fields_not_on_form = sourceNode.SelectSingleNode("include_fields_not_on_form").InnerText;
                            source_incl_fields_blank = sourceNode.SelectSingleNode("include_fields_blank").InnerText;
                            source_content_image = sourceNode.SelectSingleNode("content_image").InnerText;
                            source_headerfields = sourceNode.SelectSingleNode("headerfields").InnerText;
                            //JE 12FEB19 Mantis 1677 - add Include Only Fields Longer Than option
                            source_include_minimum_count = sourceNode.SelectSingleNode("include_minimum_count").InnerText;
                            source_minimum_count_value = sourceNode.SelectSingleNode("minimum_count_value").InnerText;
                            //end changes JE on 12FEB19
                            //JE 13FEB19 Mantis 1681 - add workflow option
                            source_include_pr_workflow = sourceNode.SelectSingleNode("include_pr_workflow").InnerText;
                            //end changes JE on 13FEB19
                        }
                        else if (source_type.Equals("TrackWise PR History") || source_type.Equals("TrackWise Audit Trail"))
                        {
                            source_content_image = sourceNode.SelectSingleNode("content_image").InnerText;
                            source_headerfields = sourceNode.SelectSingleNode("headerfields").InnerText;
                            //JE 8FEB19 Mantis 1673 - add activity id as optional audit item field
                            source_incl_activity_id = sourceNode.SelectSingleNode("include_activity_id").InnerText;
                            //JE 12FEB19 Mantis 1678 - add performed by id as optional part of "Performed By" value
                            source_incl_performedby_id = sourceNode.SelectSingleNode("include_performed_by_user_id").InnerText;
                        }
                        //JE 26FEB19 Mantis 1566 & 1690 - support "native" file attachment export, including externally stored file attachments
                        else if (source_type.Equals("Primary Content"))
                        {
                            XmlNode xnFileStoreHost = sourceNode.SelectSingleNode("external_file_store_host");
                            if (xnFileStoreHost != null) source_external_file_store_host = xnFileStoreHost.InnerText;
                        }
                        else if (source_type.Equals("Source Field"))
                        {
                            XmlNode xnFileStoreHost = sourceNode.SelectSingleNode("external_file_store_host");
                            if (xnFileStoreHost != null) source_external_file_store_host = xnFileStoreHost.InnerText;
                            source_value = sourceNode.SelectSingleNode("content_value").InnerText;
                        }
                        //end changes JE on 26FEB19
                        //JE 5/24/18 Mantis 1556 - crystal report content parameters
                        else if (source_type.Equals("Crystal Report"))
                        {
                            source_value = sourceNode.SelectSingleNode("content_value").InnerText;
                            content_format = sourceNode.SelectSingleNode("content_format").InnerText;
                            XmlNode reportparams = sourceNode.SelectSingleNode("params");
                            string crParamKey;
                            string crParamVal;
                            string strParamConcat;
                            XmlNodeList crParams = reportparams.SelectNodes("param");
                            for (int iCr = 0; iCr < crParams.Count; iCr++)
                            {
                                XmlNode crParam = crParams.Item(iCr);
                                crParamKey = crParam.SelectSingleNode("paramkey").InnerText;
                                crParamVal = crParam.SelectSingleNode("value").InnerText;
                                strParamConcat = crParamKey + ":" + crParamVal + ";";
                                report_params = report_params + strParamConcat;
                            }
                        }
                        //end changes JE 5/24/18
                        else
                        {
                            source_value = sourceNode.SelectSingleNode("content_value").InnerText;
                        }
                        //source_value = sourceNode.SelectSingleNode("content_value").InnerText;
                        //end changes by AK on 6/16/15
                        //target
                        XmlNode targetNode = ruleNode.SelectSingleNode("Target");
                        target_type = targetNode.SelectSingleNode("content_type").InnerText ;
                        target_value = targetNode.SelectSingleNode("content_value").InnerText;

                        //pre-conditions
                        XmlNode preConds = ruleNode.SelectSingleNode("pre-conditions");
                        ArrayList alPreConditions = new ArrayList();
                        if (preConds != null)
                        {
                            XmlNodeList preCondList = preConds.ChildNodes;

                            for (int k = 0; k < preCondList.Count; k++)
                            {
                                alPreConditions.Add(ReadPreconditionNode(preCondList[k]));
                            }
                        }

                        //store in hash
                        htRuleDetails.Add("index", index);
                        htRuleDetails.Add("source_type", source_type);
                        htRuleDetails.Add("source_value", source_value);
                        htRuleDetails.Add("target_type", target_type);
                        htRuleDetails.Add("target_value", target_value);
                        htRuleDetails.Add("pre-conditions", alPreConditions);
                        htRuleDetails.Add("config_form_user", source_config_form_user);
                        htRuleDetails.Add("use_config_form_layout", source_use_config_form_layout);
                        htRuleDetails.Add("include_fields_not_on_form", source_incl_fields_not_on_form); 
                        htRuleDetails.Add("include_fields_blank", source_incl_fields_blank);
                        htRuleDetails.Add("content_image", source_content_image);
                        htRuleDetails.Add("headerfields", source_headerfields);
                        //JE 5/24/18 Mantis 1556 - crystal report elements
                        htRuleDetails.Add("content_format", content_format);
                        htRuleDetails.Add("report_params", report_params);
                        //end changes JE 5/24/18
                        //JE 8FEB19 Mantis 1673 - add optional activity id
                        htRuleDetails.Add("include_activity_id", source_incl_activity_id);
                        //JE 12FEB19 Mantis 1677 - add option to Include Only Fields Longer Than
                        htRuleDetails.Add("include_minimum_count", source_include_minimum_count);
                        htRuleDetails.Add("minimum_count_value", source_minimum_count_value);
                        //JE 12FEB19 Mantis 1678 - add option to Include User ID in Performed By
                        htRuleDetails.Add("include_performed_by_user_id", source_incl_performedby_id);
                        //JE 13FEB19 Mantis 1681 - add workflow option
                        htRuleDetails.Add("include_pr_workflow", source_include_pr_workflow);
                        //JE 26FEB19 Mantis 1690 - add external file store host
                        htRuleDetails.Add("external_file_store_host", source_external_file_store_host); 
                    }
                    catch (Exception e)
                    {
                        //exception will occur if the rule is not configured correctly, store error and continue
                        htRuleDetails.Add("ERROR",e.Message);
                    }


                    //add hash to return array
                    contentRules.Add(htRuleDetails);
                }


                //get exportTo value - a local location where content will be exported
                XmlNode localLocNode = contentNode.SelectSingleNode("exportTo");
                if (localLocNode == null)
                    exportTo = "default";
                else
                    exportTo = localLocNode.InnerText;
            }
            catch (Exception e)
            {
                string exc = e.StackTrace;
            }


            return contentRules;
        }


        //AK, 4/8/15, Mantis #1118, add local variables functionality
        //configuration stores local variables and instance numbers and their corresponding values for each variable
        //find and return all variables and their values for the given instance
        public Hashtable ReadInstanceValues(string instanceNumber)
        {
            Hashtable instanceValues = new Hashtable();

            //get instances node

            try
            {
                //get content locations node
                XmlNode mappingNode = xmlDoc.SelectSingleNode("CompareRule").SelectSingleNode("Mappings");
                XmlNode localVarsNode = mappingNode.SelectSingleNode("LocalVariables");

                //exit if node is not found
                if (localVarsNode == null)
                    return instanceValues;


                //loop through variables
                XmlNodeList localVarsList = localVarsNode.SelectNodes("localvariable");

                //exit if node is not found
                if (localVarsList == null)
                    return instanceValues;


                for (int i = 0; i < localVarsList.Count; i++)
                {
                    //local var parent node
                    XmlNode localVarParent = localVarsList[i];
                    //get variable name
                    string varName = localVarParent.SelectSingleNode("variablename").InnerText;
                    //find instance and get the value
                    string varValue = "";
                    XmlNodeList localVarInstances = localVarParent.SelectNodes("instance");
                    for (int k = 0; k < localVarInstances.Count; k++)
                    {
                        //get instance node
                        XmlNode instanceNode = localVarInstances[k];
                        //get index node and its value
                        string index = instanceNode.SelectSingleNode("index").InnerText ;
                        //if matches index, get and store the value
                        if (index.Equals(instanceNumber))
                        {
                            varValue = instanceNode.SelectSingleNode("value").InnerText;
                            break;
                        }
                    }
                    if (!instanceValues.ContainsKey(varName))
                        instanceValues.Add(varName, varValue);
                    
                }

            }
            catch (Exception e)
            {
                string exc = e.StackTrace;
            }


            return instanceValues;
        }

        //AK, 4/14/15, Mantis #1118, add local variables functionality
        //configuration stores local variables and instance numbers and their corresponding values for each variable
        //find and return all variables and their default values if not instance number is passed
        public Hashtable ReadDefaultInstanceValues()
        {
            Hashtable instanceValues = new Hashtable();

            //get instances node

            try
            {
                //get content locations node
                XmlNode mappingNode = xmlDoc.SelectSingleNode("CompareRule").SelectSingleNode("Mappings");
                XmlNode localVarsNode = mappingNode.SelectSingleNode("LocalVariables");

                //exit if node is not found
                if (localVarsNode == null)
                    return instanceValues;


                //loop through variables
                XmlNodeList localVarsList = localVarsNode.SelectNodes("localvariable");

                //exit if node is not found
                if (localVarsList == null)
                    return instanceValues;


                for (int i = 0; i < localVarsList.Count; i++)
                {
                    //local var parent node
                    XmlNode localVarParent = localVarsList[i];
                    //get variable name
                    string varName = localVarParent.SelectSingleNode("variablename").InnerText;
                    //get its default value
                    string varValue = localVarParent.SelectSingleNode("default").InnerText;
                    if (varName != null && varValue != null && !instanceValues.ContainsKey(varName))
                        instanceValues.Add(varName, varValue);

                }

            }
            catch (Exception e)
            {
                string exc = e.StackTrace;
            }


            return instanceValues;
        }

        //AK, 4/15/15, Mantis #1118, replace local variables in the config
        public string ReplaceLocalVariables(Hashtable localVars, string configFile)
        {
            try
            {
                //read xml into a reader
                //AK, 6/10/15, Mantis #1118, read from xmlDoc, not config file, global variables already replaced
                StringWriter stringWriter = new StringWriter();
                XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
                xmlTextWriter.Formatting = Formatting.Indented;
                xmlDoc.WriteTo(xmlTextWriter);
                string xmltext = stringWriter.ToString();
                xmlTextWriter.Flush();
                xmlTextWriter.Close();
                //StreamReader reader = new StreamReader(configFile);
                //string xmltext = reader.ReadToEnd();
                //reader.Close();
                //end changes by AK on 6/10/15

                //replace variables
                IEnumerator ikeys = localVars.Keys.GetEnumerator();
                for (int i = 0; i < localVars.Count; i++)
                {
                    ikeys.MoveNext();

                    string varName = ikeys.Current.ToString();
                    string varValue = (string)localVars[ikeys.Current];

                    //AK, 5/12/15, replace special XML characters
                    //&lt; (<), &amp; (&), &gt; (>), &quot; ("), and &apos; (').
                    varValue = varValue.Replace("&", "&amp;");
                    varValue = varValue.Replace("<", "&lt;");
                    varValue = varValue.Replace(">", "&gt;");
                    varValue = varValue.Replace("\"", "&quot;");
                    varValue = varValue.Replace("'", "&apos;");
                    //end changes by AK on 5/12/15

                    xmltext = xmltext.Replace(varName, varValue);
                }

                //load resulting XML into the xmlDoc
                XmlDocument updatedXmlDoc = new XmlDocument();
                updatedXmlDoc.LoadXml(xmltext);
                //reset current xml
                xmlDoc = updatedXmlDoc;

                ////DEBUG
                //string filepath = @"C:\Projects\TRUconsole\Projects\Configs\TEMP_XML_" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".xml"; 
                //StreamWriter sw = new StreamWriter(filepath);
                //sw.Write(xmltext);
                //sw.Flush();
                //sw.Close();
                ////END DEBUG

                //return empty string, no error
                return "";
            }
            catch (Exception ex)
            {
                //return error
                return ex.Message;
            }


        }
        //end changes by AK on 4/15/15
    }
}


