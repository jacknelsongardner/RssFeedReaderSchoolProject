// 4/9/2022
// Team Nathaniel 
namespace BingMapsWPF
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Xml.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    internal class XMLManager
    {
        // Constructor for the XMLManager
        // that manages creation of an XMLManager instance
        public XMLManager()
        {

        }

        /// <summary>
        /// Loads an XML documents and creates
        /// a spreadsheet based off of its data.
        /// </summary>
        /// <param name="xmlReader">an xml reader of the XML file to lead</param>
        public TreeItem Load(XmlReader xmlReader)
        {
            TreeItem previousNode = null;   // Used to store the the xml tag read before the current one
            TreeItem currentNode = null; // Use to keep track of current node
            TreeItem rootNode = null;   // Used to keep track of the root node for the tree

            xmlReader.MoveToContent();  // Skip reading the doc type and whitespace until first element

            string xmlDocString = xmlReader.ReadOuterXml();     //Read all the XML document into a string var
            XElement xmlTree = GenerateXMLTree(xmlDocString);   // Create an XElement tree based off the xml doc as a string

            IEnumerable<XElement> xNodes = xmlTree.DescendantsAndSelf();

            ArrayList treeNodes = new ArrayList();

            foreach (XElement node in xNodes) { 

                // If the node is a directory
                if (node.Name == "directory") {
                    // Check if the node has an attribute
                    if (node.FirstAttribute != null)
                    {
                        // If it is the root node
                        if (node.FirstAttribute.Value == "ROOT")
                        {
                            TreeItem root = new TreeItem("root", String.Empty, String.Empty);    // Create the tree node
                            root.makeRoot();

                            root.id = node.Attribute("id").Value;

                            currentNode = root;
                            previousNode = root;

                            rootNode = root;            // Update rootNode to hold a reference to the root node of the tree
                            treeNodes.Add(currentNode); // Add the current node (in this case root) to tree nodes arraylist
                        }
                        else 
                        {
                            TreeItem directory = new TreeItem("directory", String.Empty, node.FirstAttribute.Value);    // Create a node for the directory (type,url,name)   

                            directory.id = node.Attribute("id").Value;

                            currentNode = directory;
                            treeNodes.Add(currentNode); // Add the current node to tree nodes arraylist
                        }
                    }
                    else
                    {
                        TreeItem directory = new TreeItem("directory", String.Empty, String.Empty);    // Create an empty node
                        directory.id = node.Attribute("id").Value;
                        currentNode = directory;
                        treeNodes.Add(currentNode); // Add the current node to tree nodes arraylist
                    }

                    // Check if the node is a child node
                    if (node.Parent != null) { 
                        
                        //Find the parent node from the TreeItem Array list
                        foreach (TreeItem treeItem in treeNodes ) {

                            if (node.Parent.Attribute("name") != null)
                            {
                                // If the id of the XElement node is the same as the treeItem's
                                if (node.Parent.Attribute("id").Value == treeItem.id)
                                {
                                    treeItem.addChild(currentNode);   // Add the node as a child      
                                }
                            }
                        }
                    }
                }

                if (node.Name == "feed") {

                    if (node.FirstAttribute != null)
                    {
                        previousNode = currentNode;
                        TreeItem feed = new TreeItem("feed", node.Attribute("url").Value, node.Attribute("name").Value);    // Create a node for the feed (type,url,name)   
                        feed.id = node.Attribute("id").Value;
                        currentNode = feed;
                    }
                    else
                    {
                        previousNode = currentNode;
                        TreeItem feed = new TreeItem("feed", String.Empty, String.Empty);    // Create an empty feed node
                        feed.id = node.Attribute("id").Value;
                        currentNode = feed;
                    }

                    // Check if the node is a child node
                    if (node.Parent != null)
                    {
                        //Find the parent node from the TreeItem Array list
                        foreach (TreeItem treeItem in treeNodes)
                        {
                            if (node.Parent.Attribute("name") != null)
                            {
                                // If the id of the XElement node is the same as the treeItem's
                                if (node.Parent.Attribute("id").Value == treeItem.id)
                                {
                                    treeItem.addChild(currentNode);   // Add the node as a child      
                                }
                            }
                        }
                    }
                }

                if (node.Name == "topic")
                {

                    if (node.FirstAttribute != null)
                    {
                        previousNode = currentNode;
                        TreeItem feed = new TreeItem("topic", node.Attribute("keywords").Value, node.Attribute("name").Value);    // Create a node for the feed (type,url,name)   
                        feed.id = node.Attribute("id").Value;
                        currentNode = feed;

                        treeNodes.Add(currentNode); // Add the current node to tree nodes arraylist
                    }
                    else
                    {
                        previousNode = currentNode;
                        TreeItem feed = new TreeItem("feed", String.Empty, String.Empty);    // Create an empty feed node
                        feed.id = node.Attribute("id").Value;
                        currentNode = feed;

                        treeNodes.Add(currentNode); // Add the current node to tree nodes arraylist
                    }

                    // Check if the node is a child node
                    if (node.Parent != null)
                    {
                        //Find the parent node from the TreeItem Array list
                        foreach (TreeItem treeItem in treeNodes)
                        {
                            if (node.Parent.Attribute("name") != null)
                            {
                                // If the id of the XElement node is the same as the treeItem's
                                if (node.Parent.Attribute("id").Value == treeItem.id)
                                {
                                    treeItem.addChild(currentNode);   // Add the node as a child      
                                }
                            }
                        }
                    }
                }
                Console.WriteLine(node.Parent);
                Console.WriteLine(node.NextNode);
                Console.WriteLine(node.PreviousNode);
            }

            //Recreate the XElement Tree as a TreeItem
                            



            /*
            // Read the XML file
            while (xmlReader.Read())
            {
                
                // Depending on the type of input read 
                switch (xmlReader.NodeType)
                {
                    // If it is text between two tags
                    case XmlNodeType.Text:
                        // If the string read is not empty
                        if (xmlReader.HasValue)
                        {
                            // Do something here if necessary
                        }
                        break;

                    // If it is a tag 
                    case XmlNodeType.Element:

                        // When enocuntering the tree tag, create a root node
                        /*if (xmlReader.Name == "tree")
                        {

                            TreeItem root = new TreeItem("root", String.Empty, String.Empty);    // Create an empty directory node
                            root.makeRoot();
                            currentNode = root;
                            previousNode = root;

                            rootNode = root;    // Update rootNode to hold a reference to the root node of the tree
                        }

                        // WHen a directory tag is read
                        if (xmlReader.Name == "directory")
                        {
                            if (xmlReader.HasAttributes)
                            {
                                // When enocuntering the tag named ROOT, create a root node
                                if (xmlReader.GetAttribute("name") == "ROOT")
                                {

                                    TreeItem root = new TreeItem("root", String.Empty, String.Empty);    // Create an empty directory node
                                    root.makeRoot();
                                    currentNode = root;
                                    previousNode = root;

                                    rootNode = root;    // Update rootNode to hold a reference to the root node of the tree
                                }
                                else
                                {
                                    TreeItem directory = new TreeItem("directory", String.Empty, xmlReader.GetAttribute("name"));    // Create a node for the directory (type,url,name)   
                                    currentNode = directory;
                                }

                                //XmlReader subTree = xmlReader.ReadSubtree();

                            }
                            else
                            {
                                TreeItem directory = new TreeItem("directory", String.Empty, String.Empty);    // Create an empty directory node    
                                currentNode = directory;
                            }

                            //Set current node as a child of the previous node
                            if (previousNode != null)
                            {
                                // Do not add the root/any other node as a child of itself
                                if (currentNode != previousNode)
                                {
                                    previousNode.addChild(currentNode);
                                }

                                XmlReader descendantReader = xmlReader;                       // Make a copy of xmlReader to add child nodes to current node

                                // If the tree has child nodes
                                if (descendantReader.ReadToDescendant("directory") || descendantReader.ReadToDescendant("feed") != false)
                                {
                                    
                                }

                                //Otherwise just update previous node
                                else
                                {
                                    previousNode = currentNode;
                                }
                                
                                // Add all directory tags as directory type treeitems
                                while (descendantReader.ReadToDescendant("directory"))
                                {
                                    previousNode = currentNode;
                                    TreeItem directory = new TreeItem("directory", String.Empty, descendantReader.GetAttribute("name"));    // Create a node for the directory (type,url,name)   
                                    currentNode = directory;
                                    previousNode.addChild(currentNode);
                                }

                                descendantReader = xmlReader;                       // Reset descendant reader to current element (to prep for reading feed children)

                                // Add all feed tags as feed type treeitems
                                while (descendantReader.ReadToDescendant("feed"))
                                {
                                    previousNode = currentNode;
                                    TreeItem feed = new TreeItem("feed", String.Empty, descendantReader.GetAttribute("name"));    // Create a node for the directory (type,url,name)   
                                    currentNode = feed;
                                    previousNode.addChild(currentNode);
                                }
                            }
                            else
                            {
                                XmlReader descendantReader = xmlReader;                       // Make a copy of xmlReader to add child nodes to current node

                                // If the tree has child nodes
                                if (descendantReader.ReadToDescendant("directory") || descendantReader.ReadToDescendant("feed") != false)
                                {
                                    
                                }

                                //Otherwise just update previous node
                                else
                                {
                                    previousNode = currentNode;
                                } 
                                // Add all directory tags as directory type treeitems
                                while (descendantReader.ReadToDescendant("directory"))
                                {
                                    previousNode = currentNode;
                                    TreeItem directory = new TreeItem("directory", String.Empty, descendantReader.GetAttribute("name"));    // Create a node for the directory (type,url,name)   
                                    currentNode = directory;
                                    previousNode.addChild(currentNode);
                                }

                                descendantReader = xmlReader;                       // Reset descendant reader to current element (to prep for reading feed children)

                                // Add all feed tags as feed type treeitems
                                while (descendantReader.ReadToDescendant("feed"))
                                {
                                    previousNode = currentNode;
                                    TreeItem feed = new TreeItem("feed", String.Empty, descendantReader.GetAttribute("name"));    // Create a node for the directory (type,url,name)   
                                    currentNode = feed;
                                    previousNode.addChild(currentNode);
                                }
                            }
                        }

                        // When a feed tag is read
                        if (xmlReader.Name == "feed")
                        {
                            if (xmlReader.HasAttributes)
                            {
                                previousNode = currentNode;
                                TreeItem feed = new TreeItem("feed", xmlReader.GetAttribute("url"), xmlReader.GetAttribute("name"));    // Create a node for the feed (type,url,name)   
                                currentNode = feed;
                            }
                            else
                            {
                                previousNode = currentNode;
                                TreeItem feed = new TreeItem("feed", String.Empty, String.Empty);    // Create an empty feed node
                                currentNode = feed;
                            }

                            // Add parent node of feed if feed is a child
                            if (previousNode != null)
                            {
                                previousNode.addChild(currentNode);
                            }

                        }
                        /*
                        // When an article tag is read
                        else if (xmlReader.Name == "article")
                        {
                            if (xmlReader.HasAttributes)
                            {
                                TreeItem directory = new TreeItem("directory", null, xmlReader.GetAttribute("name"));    // Create a node for the directory (type,url,name)   
                            }
                            else
                            {
                                TreeItem directory = new TreeItem("directory", null, null);    // Create an empty directory node    
                            }

                        }
                        
                        break;

                        //case XmlNodeType.EndElement:
                        //    previousNode = currentNode;
                        //    break;
                }
            }*/
            return rootNode;        //Returns the TreeItem tree
        }

        /*
        // Recursively reads and generates nodes for 
        // the subtree of root
        private void RecursiveLoad(XmlReader subTree) { 
            if(subTree != null)
            {

            }
        }*/

        
        // Takes an XML document as a string and converts it
        // to an XElement tree
        private XElement GenerateXMLTree(string xmlString)
        {
            XElement xmlTree = XElement.Parse(xmlString);
            return xmlTree;
        }

        /// <summary>
        /// Writes the info of the spreadsheet to
        /// an XML file.
        /// </summary>
        /// <param name="xmlWriter">an xml writer that outputs to an xml doc</param>
        /// <param name="tree"> the tree data structure storing all the RSSfeed data</param>
        public void Save(XmlWriter xmlWriter, TreeItem tree)
        {
            // Create the spreadsheet tag
            xmlWriter.WriteStartElement("tree");                    // Start tag to denote the tree, i.e <tree>

            RecursiveSave(xmlWriter, tree);                         // Use the recursive function to generate all the xml for the tree's nodes

            xmlWriter.WriteEndElement();                            // Write </tree> to close out RSSFeed tree
        }

        // Recursively adds tags to all the children of the directory node
        private void RecursiveSave(XmlWriter xmlWriter, TreeItem currentNode)
        {
            // If a node has children
            if (currentNode.getChildren() != null)
            {

                if (currentNode.getChildren().Count != 0)
                {

                    GenerateXMLOpeningTags(xmlWriter, currentNode);    //create xml opening tag for the  parent node

                    // For every child call the recursive save function on them
                    foreach (TreeItem child in currentNode.getChildren())
                    {                         
                        RecursiveSave(xmlWriter, child);                   //call its child
                    }
                    
                    // This runs after the recursive call
                    // Closes out the nodes that ran calls on their children
                    // but not the children themselves
                    if (currentNode.getChildren().Count != 0) {
                        xmlWriter.WriteEndElement();                                // Close the tag for parent nodes 
                    }
                    
                }
                else
                {
                    // Just generate the tags for the node if it has no children
                    GenerateXMLOpeningTags(xmlWriter, currentNode);
                    xmlWriter.WriteEndElement();                                // Close the tag 
                }
                
            }
            
        }

        // Generates the opening tags for each node type
        // Does not close them as that would not allow for accurate
        // parsing of child nodes during the loading of an xml file
        private void GenerateXMLOpeningTags(XmlWriter xmlWriter, TreeItem node)
        {
            // Determine if the node is a feed or a directory
            switch (node.getType())
            {

                case "directory":
                    xmlWriter.WriteStartElement("directory");                   // Open a new directory element tag <directory>
                    xmlWriter.WriteAttributeString("name", node.getName());     // Add the directory name attribute to the element
                    xmlWriter.WriteAttributeString("id", node.id);              // Add the unique id attribute
                    break;

                case "feed":
                    xmlWriter.WriteStartElement("feed");                          // Add a new feed element tag <feed>
                    xmlWriter.WriteAttributeString("name", node.getName());          // Add the feed name attribute to the cell
                    xmlWriter.WriteAttributeString("url", node.getUrl());        // Add the link to the feed as an attribute to the cell
                    xmlWriter.WriteAttributeString("id", node.id);   // Add the unique id attribute
                    /*
                    // Find the children of each feed (i.e. articles) and create a tag for them nested
                    // under the RSSfeed tag
                    foreach (TreeItem article in node.getChildren()) {
                        xmlWriter.WriteStartElement("article");                        // Add a new article element tag <article>
                        xmlWriter.WriteAttributeString("name", "article name");        // Add the article name attribute to the cell
                        xmlWriter.WriteAttributeString("link", "article link");        // Add the link to the article as an attribute to the cell
                        xmlWriter.WriteEndElement();
                    }
                    */
                    break;

                case "topic":
                    xmlWriter.WriteStartElement("topic");                          // Add a new feed element tag <feed>
                    xmlWriter.WriteAttributeString("name", node.getName());          // Add the feed name attribute to the cell
                    xmlWriter.WriteAttributeString("keywords", (node.getKeyWords().Aggregate((x,y) => x+ " " +y)));        // Take all the topic's keywords and aggregate them into a single string
                    xmlWriter.WriteAttributeString("id", node.id);   // Add the unique id attribute
                    break;
            }
        }
    }
}
