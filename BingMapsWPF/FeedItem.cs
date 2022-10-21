using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BingMapsWPF
{
    internal class FeedItem
    {
        //variables
        public String content;
        public String title;
        public String summary;
        public String url;
        public String dateString;

        //variables for date, time, etc of article
        public int hour;
        public int minute;

        public int day;
        public int month;
        public int year;

        public System.DateTime date;

        public List<String> tempList1;
        public List<String> tempList2;
        public List<String> tempList3;




        //constructors
        public FeedItem()
        { 
            //do nothing
        }
        
        public FeedItem(String t, String u)
        {
            url = u;
            title = t;  
        }

        public FeedItem(String t, String u, String d)
        {
            title = t;
            url = u;
            dateString = d;


            //splitting up the date in spaces
            tempList1 = dateString.Split(" ").ToList<String>();

            //splitting up teh month,year,day date
            tempList2 = tempList1[0].Split("/").ToList<String>();

            month = int.Parse(tempList2[0]);
            day = int.Parse(tempList2[1]);
            year = int.Parse(tempList2[2]);

            //splitting up the hour,minute date
            tempList3 = tempList1[1].Split(":").ToList<String>();

            hour = int.Parse(tempList3[0]);
            minute = int.Parse(tempList3[1]);

            date = new System.DateTime(year,month,day,hour,minute,0,0);


        }

        public FeedItem(String t, String u, String d, String s)
        {
            url = u;
            title = t;
            dateString = d;
            summary = s;
        }

        public FeedItem(String t, String u, String d, String s, String c)
        {
            dateString = d;
            url = u;
            content = c;
            title = t;
            summary = s;
        }

        //getters





    }
}
