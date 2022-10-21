using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BingMapsWPF
{
    internal class TreeItem
    {

        private String type;
        private String url;
        private String name;
        public Boolean isRoot = false;
        private TreeItem parentItem;

        private List<TreeItem> childrenItems = new List<TreeItem>();
        private List<String> childrenNames = new List<String>();

        private String childrenKeyWords;

        // Used to create a random number for a unique ID
        Random rand = new Random();
        public string id;
        
        //CREATORS
        public TreeItem()
        {
            this.makeRoot();
            id = rand.Next().ToString();
        }

        public TreeItem(String t, String u, String n)
        {
            name = n;
            type = t;
            url = u;

            id = rand.Next().ToString();

            if (type == "feed" || type == "f")
            {
                type = "feed";
            }
            else if (type == "directory" || type == "d")
            {
                url = null;
                type = "directory";
            }
            else if (type == "root" || type == "r")
            {
                type = "directory";
            }
            else if (type == "topic" || type == "t")
            {
                url = null;
                childrenKeyWords = u;
                type = "topic";
            }

        }


        //GETTERS
        public String getName()
        {
            return name;
        }

        public String getUrl()
        {

            return url;
        }

        public List<String> getKeyWords()
        {

            return childrenKeyWords.Split(" ").ToList<String>();
        }

        public int Count()
        {
            return childrenItems.Count();
        }

        public TreeItem getChild(int i)
        {
            return childrenItems[i];
        }

        public TreeItem getParent()
        {
            return parentItem;
        }

        public String getChildName(int i)
        {
            return childrenNames[i];
        }

        public String getType()
        {
            if (type == "feed" || type == "f")
            {
                return "feed";
            }
            else if (type == "d" || type == "directory")
            {
                return "directory";
            }
            else if (type == "t" || type == "topic")
            {
                return "topic";
            }
            else
            {
                return null;
            }
            
        }

        public List<String> getChildrenNames()
        {
            return childrenNames;
        }

        public List<TreeItem> getChildren()
        {
            return childrenItems;
        }


        //METHODS
        public void makeRoot()
        {
            isRoot = true;
            this.parentItem = this;
            url = null;
            name = "ROOT";
            type = "directory";
        }

        public void addChild(TreeItem t)
        {
            if (type != "f" && type != "feed")
            {
                t.parentItem = this;
                childrenItems.Add(t);
                childrenNames.Add(t.getName());
            }
        }

        public void deleteChild(TreeItem t)
        {
            if (type != "f" && type != "feed")
            {
                childrenItems.Remove(t);
                childrenNames.Remove(t.name);
            }
        }

        public void deleteChild(int i)
        {
            if (childrenItems.Count >= i + 1)
            {
                childrenItems.RemoveAt(i);
                childrenNames.RemoveAt(i);
            }
        }

        //SETTERS
        public void setParent(TreeItem t)
        {
            parentItem = t;
        }




    }
}
