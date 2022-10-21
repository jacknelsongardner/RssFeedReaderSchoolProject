using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BingMapsWPF
{
    internal class Folder
    {


        //VARIABLES

        public List<Folder> elements = new List<Folder>();
        public String path;
        public String parentPath;
        public String kind;
        public String name;

        //CONSTRUCTORS
        public Folder(String dir,String pdir, String n,String k)
        {
            path = dir;
            parentPath = pdir;
            kind = k;
            name = n;

        }


        //METHODS
        public void addElement(Folder elem)
        {

            if (kind != "folder")
            {
                elements.Add(elem);
            }
            else
            { 
                //does nothing...
            }
        }

        public void changeName(String n)
        {
            name = n;
        }
       




    }
}
