using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Diagnostics;

namespace TasktopSync_XMLUpdate_Proto
{
    class Program
    {
        static void Main(string[] args)
        {

            int MaxMappingID = 0;

            // Load master Tasktop xml
            XmlDocument doc = new XmlDocument();
            doc.Load("C:\\Scratch\\synchronizer_test.xml");
            XmlElement root = doc.DocumentElement;

            // Need to determine max values for mapping IDs as need to be incremented and set in the new mapping being added.
            // Extract the elements of task-mapping from primary doc - master config xml for Tasktop Sync
            XmlNodeList nodeList = doc.GetElementsByTagName("task-mapping");
            // Get Tasktop Sync config master task-mapping node 
            XmlNode masterTaskMapping = nodeList.Item(0);

            foreach (XmlNode node in nodeList)
            {
                XmlAttributeCollection nodeAttrs = node.Attributes;
                foreach (XmlAttribute attr in nodeAttrs)
                {
                    if (String.Compare(attr.Name, "id") == 0)
                    {
                        MaxMappingID = Math.Max(MaxMappingID, Convert.ToInt32(attr.Value));
                    }
                }
            }

            // Load entity sync type model xml
            XmlDocument modelDoc = new XmlDocument();
            modelDoc.Load("C:\\Scratch\\synchronizer_epic_model.xml");
            XmlElement modelRoot = modelDoc.DocumentElement;

            // NOT WORKING XmlNodeList targetNode = modelDoc.SelectNodes("/sync-model/task-mapping/repository/scope/field[@id='project']");

            // Update the task-mapping attributes, id and display-name, etc in the model xml
            XmlNodeList modelNodes = modelDoc.GetElementsByTagName("task-mapping");
            XmlNode modelNode = modelNodes.Item(0);
            XmlAttributeCollection attrList = modelNode.Attributes;

            foreach (XmlAttribute attr in attrList)
            {
                if (String.Compare(attr.Name, "id") == 0 && String.Compare(attr.Value, "999") == 0)
                {
                    attr.Value = (MaxMappingID + 1).ToString();
                    continue;
                }
                if (String.Compare(attr.Name, "display-name") == 0 && attr.Value.Contains("XXXX"))
                {
                    attr.Value = "ZxZxZ" + " Project JIRA Epics - HP QC System Requirements";
                    continue;
                }
            }

            //Update the scope/field id=project value
            XmlNodeList fieldNodes = modelDoc.GetElementsByTagName("field");

            foreach (XmlNode node in fieldNodes)
            {
                XmlAttributeCollection nodeattrs = node.Attributes;
                foreach (XmlAttribute attr in nodeattrs)
                {
                    if (String.Compare(attr.Name, "id") == 0 && String.Compare(attr.Value, "projectj") == 0)
                    {
                        attr.Value = "project";
                        node.Attributes["value"].Value = "Automation JIRA";
                        continue;
                    }
                    if (String.Compare(attr.Name, "id") == 0 && String.Compare(attr.Value, "projecth") == 0)
                    {
                        attr.Value = "project";
                        node.Attributes["value"].Value = "Automation HPQC";
                        continue;
                    }
                    if (String.Compare(attr.Name, "domain") == 0)
                    {
                        node.Attributes["value"].Value = "ALMTST-Automation";
                        continue;
                    }
                }
            }

            //Update the query/field id=project value
            XmlNodeList queryNodes = modelDoc.GetElementsByTagName("query");

            foreach (XmlNode node in queryNodes)
            {
                XmlAttributeCollection nodeattrs = node.Attributes;
                foreach (XmlAttribute attr in nodeattrs)
                {
                    if (String.Compare(attr.Name, "name") == 0 && String.Compare(attr.Value, "initializationj") == 0)
                    {
                        attr.Value = "initialization";
                        node.Attributes["value"].Value = "Automation All Epics";
                        continue;
                    }
                    if (String.Compare(attr.Name, "name") == 0 && String.Compare(attr.Value, "initializationh") == 0)
                    {
                        attr.Value = "initialization";
                        node.Attributes["value"].Value = "Automation All System Requirements";
                        continue;
                    }
                    if (String.Compare(attr.Name, "name") == 0 && String.Compare(attr.Value, "changesj") == 0)
                    {
                        attr.Value = "changes";
                        node.Attributes["value"].Value = "Automation Recent Epics";
                        continue;
                    }
                    if (String.Compare(attr.Name, "name") == 0 && String.Compare(attr.Value, "changesh") == 0)
                    {
                        attr.Value = "changes";
                        node.Attributes["value"].Value = "Automation Recent System Requirements";
                        continue;
                    }
                }
            }

            // Import the model node
            XmlNode impNode = doc.ImportNode(modelNode, true);


            //XmlElement newElement = doc.CreateElement("task-mapping", doc.DocumentElement.NamespaceURI);
            //newElement.SetAttribute("id", "21");
            //newElement.SetAttribute("display_name", "Automation Inserted Mapping");

            //XmlNode newNode = doc.CreateNode(XmlNodeType.Element, "task-mapping", doc.DocumentElement.NamespaceURI);
            //XmlAttribute xID = doc.CreateAttribute("id");
            //xID.Value = "21";

            //XmlAttribute xDN = doc.CreateAttribute("display_name");
            //xDN.Value = "Automation Inserted Mapping";

            //newNode.Attributes.Append(xID);
            //newNode.Attributes.Append(xDN);

            int lastNodeIndex = nodeList.Count - 1;
            XmlNode lastNode = nodeList.Item(lastNodeIndex);

            // Now must insert the new imported modified model node after the last task-mapping node in the Tasktop Sync master config xml
            lastNode.ParentNode.InsertAfter(impNode, lastNode);

            Console.WriteLine("Display the modified XML...");
            doc.Save("C:\\Scratch\\synchronizer_NEW.xml");

            Console.ReadKey();

        }
    }
}
