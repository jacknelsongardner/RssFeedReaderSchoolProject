using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BingMapsWPF
{
    internal class DirectoryItem
    {
        //variables
      
        public String name;
        public String backurl;
        public String url;
        public String type;

        //constructors
        public DirectoryItem()
        { 
            //do nothing
        }
        

        public DirectoryItem(String n, String u, String b, String t)
        {
            name = n;
            type = t;
            url = u;
            backurl = b;
        }

        public DirectoryItem(String n, String u, String t)
        {
            name = n;
            type = t;
            url = u;
        }

    }
}
