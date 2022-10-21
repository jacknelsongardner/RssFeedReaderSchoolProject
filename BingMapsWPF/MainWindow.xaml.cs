using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using System.Xml;
using System.Collections;

using Microsoft.Maps.MapControl;

//using WK.Libraries.BetterFolderBrowserNS;
using Ookii.Dialogs.Wpf;
//using this for downloading RSS feeds
using SimpleFeedReader;
//using this to silence the web browser script errors
using System.Reflection;

namespace BingMapsWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {


        //lists for the csv importing
        LinkedList<String> stateNames = new LinkedList<String>();
        LinkedList<String> cityNames = new LinkedList<String>();
        LinkedList<float> xLatitude = new LinkedList<float>();
        LinkedList<float> yLatitude = new LinkedList<float>();

        //a hashtable for storing the csv information
        Hashtable stateHashTable = new Hashtable();
        Hashtable cityHashTable = new Hashtable();

        Hashtable xlatitudeHashTable = new Hashtable();
        Hashtable ylatitudeHashTable = new Hashtable();

        // the offset date, used for pulling articles from a certain time frame
        System.TimeSpan offsetDate;

        //variables for how long ago the a

        //SETTINGS FOR XML
        XmlWriter writer;
        XmlReader reader;
        XMLManager manager = new XMLManager();


        //SETTINGS FOR SWITCHING 
        String directoryViewType = "feedFolders";
        //can also be set to keyFolders (folders based on keywords)


        //this is what will read and write the xml files
        XMLManager xmlManager = new XMLManager();

        //VARIABLES INSTANTIATED
        
        Boolean BrowserVisibility = true;
        Boolean MapVisibility = false;




        String TreeLabelMessage;
        String ListLabelMessage;

        String UserName;
        String Password;

        Boolean FirstTimeUser = true;
        Boolean FolderSelected;

        String saveDataDirectory;
        String SelectedDirectory;
        

        String tempDirectory;

        System.Xml.XmlReader RSSreader;
        System.Xml.XmlReader DataReader;
        System.Xml.XmlTextReader TextReader;


        System.Xml.Serialization.XmlSerializer XMLSaver;
        System.Xml.XmlWriter XMLCreator;

        List<FeedItem> tempFeedItems;
        List<DirectoryItem> tempDirectoryItems;

        FeedItem[] tempFeedArray;
        DirectoryItem[] tempDirectoryArray;

        //feed stuff
        List<FeedItem> selectedFeedItems = new List<FeedItem>();
        
        List<String> selectedFeedTitles = new List<String>();
        List<String> selectedFeedUrls = new List<String>();

        int selectedFeedIndex;
        
        //directory stuff
        List<DirectoryItem> selectedDirectoryItems = new List<DirectoryItem>();

        List<String> selectedDirectoryTitles = new List<String>();
        List<String> selectedDirectoryUrls = new List<String>();

        int selectedDirectoryIndex;

        String selectedDirectoryBack = "";

        FeedItem tempFeedItem;
        DirectoryItem tempDirectoryItem;

        string xmlFileNameDirectory = "rootDirectoryView.xml";
        string xmlFileNameTopics = "rootTopicView.xml";


        //DEBUGGING STRINGS, not vital to project AT ALL
        public String a;
        public String b;

        //this one automatically becomes a root of the main directory
        TreeItem directoryRoot = new TreeItem();

        //this one automatically becomes a root of the topics interface
        TreeItem topicsRoot = new TreeItem();

        TreeItem selectedTreeItem = new TreeItem("d",null,null);

        Microsoft.Maps.MapControl.WPF.MapLayer myMapPushPinLayer = new Microsoft.Maps.MapControl.WPF.MapLayer();

        


        //CONSTRUCTORS
        public MainWindow()
        {
            InitializeComponent();

            
            myMap.Children.Add(myMapPushPinLayer);



            //importing the csv file
            using (var reader = new StreamReader(@"uslocations_for_ProjectB.csv"))
            {
                reader.ReadLine();
                
                
                while (!reader.EndOfStream)
                {
                    
                    var line = reader.ReadLine();
                    var values = line.Split(',');

                    //OLD METHOD
                    stateNames.AddFirst(values[2]);
                    cityNames.AddFirst(values[3]);

                    xLatitude.AddFirst(float.Parse(values[5]));
                    yLatitude.AddFirst(float.Parse(values[6]));


                    //NEW METHOD
                    String stringToHash = values[2];
                    if (!stateHashTable.ContainsKey(stringToHash))
                    {
                        stateHashTable.Add(stringToHash, values[2]);

                        xlatitudeHashTable.Add(stringToHash, values[5]);
                        ylatitudeHashTable.Add(stringToHash, values[6]);
                    }

                    stringToHash = values[3];
                    if (!cityHashTable.ContainsKey(stringToHash))
                    {
                        cityHashTable.Add(stringToHash, values[3]);

                        xlatitudeHashTable.Add(stringToHash, values[5]);
                        ylatitudeHashTable.Add(stringToHash, values[6]);
                    }

                }

                

            }

            //testing the pushpin
            myPushPin.Location = new Microsoft.Maps.MapControl.WPF.Location(50, -120);

            //setting day, time, and hour to default 

            offsetDate = new System.TimeSpan(0, 12, 0, 0);

            //NEW IMPLEMENTATION
            //testing tree items, debug this to see how it works. (please keep in mind is not yet compatable with treeView, but works fine with the viewer we have)
            directoryRoot.addChild(new TreeItem("d", null, "folderOne"));
            directoryRoot.addChild(new TreeItem("d", null, "folderTwo"));
            selectedTreeItem = directoryRoot.getChild(0);

            directoryRoot.addChild(new TreeItem("d", null, "folderThree"));
            selectedTreeItem = selectedTreeItem.getParent();

            selectedTreeItem.addChild(new TreeItem("d", null, "folderFive"));
            selectedTreeItem.addChild(new TreeItem("d", null, "folderSix"));

            selectedTreeItem.addChild(new TreeItem("d", null, "folderFour"));
            
            System.Console.WriteLine(selectedTreeItem);
            System.Console.WriteLine(directoryRoot);

            //deleting results from test here...
            directoryRoot = new TreeItem();
            directoryRoot.makeRoot();
            directoryRoot.setParent(directoryRoot);

            selectedTreeItem = directoryRoot;


            //makes it so that the browser does not constantly give you errors
            TrySetSuppressScriptErrors(myWebBrowser, true);

            //Pulling saveData.XML file from project directory
            using (XmlReader reader = XmlReader.Create("saveData.xml"))
            {
                String tempString = "";
                
                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        //return only when you have START tag  
                        switch (reader.Name.ToString())
                        {
                            case "firstTime":
                                tempString = reader.ReadString();

                                if (tempString.Equals("yes"))
                                {
                                    FirstTimeUser = true;
                                }
                                else if (tempString.Equals("no"))
                                {
                                    FirstTimeUser = false;
                                }
                                else
                                {
                                    FirstTimeUser = true;
                                }

                                break;

                            case "directory":
                                saveDataDirectory = reader.ReadString();
                                break;
                        }

                    }


                }
            }

            


            //hiding empty directory
            DirectoryViewEmpty.Visibility = Visibility.Collapsed;
            DirectoryView.Visibility = Visibility.Visible;

            Console.WriteLine(saveDataDirectory);
            Console.WriteLine(FirstTimeUser.ToString());

            //setting the back directory to itself...
            selectedDirectoryBack = saveDataDirectory;


            
            //Setup for a first time user...
            if (FirstTimeUser == true)
            {
                Console.WriteLine("WELCOME FIRST TIME USER!!!");

                //Popup window welcoming the user...

                
                this.openWelcomeWindow();

                this.nameLibrary();

                //NEW
                selectedTreeItem = directoryRoot;


                SelectedDirectory = saveDataDirectory;
                FirstTimeUser = false;

                //WRITING TO XML savedata file

                //deleting old savedata file
                File.Delete("saveData.xml");
                

                //settings for XMLCreator
                var sts = new XmlWriterSettings()
                {
                    Indent = true,
                };


                //creating save data file
                XMLCreator = System.Xml.XmlWriter.Create("saveData.xml",sts);

                //creating new savedata file
                XMLCreator.WriteStartDocument();

                XMLCreator.WriteStartElement("ROOT");
                    XMLCreator.WriteStartElement("FeedItem");
                    XMLCreator.WriteElementString("firstTime", "no");
                    XMLCreator.WriteElementString("directory", saveDataDirectory);
                    XMLCreator.WriteEndElement();
                XMLCreator.WriteEndElement();

                XMLCreator.WriteEndDocument();
                XMLCreator.Close();


                //creating the xml files for the root
                writer = XmlWriter.Create(saveDataDirectory + xmlFileNameDirectory);
                manager.Save(writer, directoryRoot);
                writer.Close();

                writer = XmlWriter.Create(saveDataDirectory + xmlFileNameTopics);
                manager.Save(writer, directoryRoot);
                writer.Close();

            }
            //if the user is not a first timer...
            else if (FirstTimeUser == false)
            {
                Console.WriteLine("YOU'VE USED THIS PROGRAM BEFORE, HAVEN'T YOU...");
                SelectedDirectory = saveDataDirectory;


                //testing to see if there is no directory selected. If not, then theres text that says "no directory selected"
                if (saveDataDirectory != null)
                {
                    TreeLabel.Visibility = Visibility.Hidden;

                }
                else if (saveDataDirectory == null || saveDataDirectory == "")
                {

                    TreeLabel.Visibility = Visibility.Visible;

                }

                //loading the tree from the xml file for the topics directory and the 
                reader = XmlReader.Create(saveDataDirectory + xmlFileNameDirectory);
                directoryRoot = manager.Load(reader);
                selectedTreeItem = directoryRoot;       // Update selectedTreeItem, for accurate updating of the directory window

                reader = XmlReader.Create(saveDataDirectory + xmlFileNameTopics);
                topicsRoot = manager.Load(reader);

                //Update the screen to show loaded folder and rss tree
                updateDirectoryView();

                newDirectoryView.Visibility = Visibility.Visible;
                DirectoryViewEmpty.Visibility = Visibility.Collapsed;
                reader.Close();
            }

            updateDirectory();
        }

        //opening windows
        private void openWelcomeWindow()
        {
            Window1 window1 = new Window1();
            window1.ShowDialog();
        }
      
        private void openFolderWindow()
        {
                // Popup window using Ookii.Dialogs.Wpf, downloadable with this command in PM, Install-Package Ookii.Dialogs.Wpf
                var dialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
                if (dialog.ShowDialog(this).GetValueOrDefault())
                {
                    saveDataDirectory = dialog.SelectedPath;
                }
                Console.WriteLine(saveDataDirectory);
        }

        private void openRSSFeedWindow()
        { 
            



        }

        private void nameLibrary()
        {
            openFolderWindow();

            //Popup window asking for RSS FEED library name (with exceptions)
            Window2 window2 = new Window2();
            window2.TempDirectory.Text = saveDataDirectory + "\\";
            window2.ShowDialog();

            saveDataDirectory = window2.TempDirectory.Text;

            if (window2.backstatus.Content == "yes")
            {
                
                nameLibrary();
                window2.backstatus.Content = "no";
            }

            

        }




        //directory stuff
        //REDONE
        private void makeNewFolder(String folderName)
        {

            //NEW METHOD
            selectedTreeItem.addChild(new TreeItem("d", null, folderName));
            updateDirectoryView();

            //setting the xml writer
            writer = XmlWriter.Create(saveDataDirectory+ xmlFileNameDirectory);

            //saving to xml file
            manager.Save(writer, directoryRoot);
            writer.Close();




            tempDirectory = "";

            updateDirectoryView();

            //switching visibility of the directoryViews
            DirectoryViewEmpty.Visibility = Visibility.Collapsed;
            newDirectoryView.Visibility = Visibility.Visible;

            //DirectoryView.ItemsSource = selectedDirectoryUrls;
            //FeedView.ItemsSource = selectedFeedTitles;


        }
        //REDONE
        private void updateDirectoryView()
        {
            //NEW METHOD 
            newDirectoryView.ItemsSource = selectedTreeItem.getChildrenNames();
            newDirectoryView.Items.Refresh();

        }

        private void updateFeedView()
        {
            FeedView.ItemsSource = selectedFeedTitles;
            FeedView.Items.Refresh();
        }

        //NEW
        private List<FeedItem> downloadFeed(String url)
        {


            //variables for time

            System.DateTime todaysDate = DateTime.Now;


            //NEW METHOD
            var RSSparser = new FeedReader();
            var retrievedFeed = RSSparser.RetrieveFeed(url);
            
            var tempFeedItems = new List<FeedItem>();
            var tempFeedTitles = new List<String>();
            var tempFeedUrls = new List<String>();
            var tempFeedDates = new List<String>();

            //retriving feed stuff and adding it to a list
            foreach (var i in retrievedFeed)
            {
                
                tempFeedItem = new FeedItem(i.Title.ToString(), i.Uri.ToString(), i.Date.ToString());

                if (todaysDate - tempFeedItem.date <= offsetDate)
                {
                    tempFeedItems.Add(tempFeedItem);
                    Console.WriteLine($"{i.Date.ToString("g")}\t{i.Title}");

                    tempFeedTitles.Add(i.Title);
                    tempFeedUrls.Add(i.Uri.ToString());
                    tempFeedDates.Add(i.Date.ToString());
                }



                
            }

            selectedFeedItems = tempFeedItems;
            selectedFeedTitles = tempFeedTitles;
            selectedFeedUrls = tempFeedUrls;

            return tempFeedItems;
        }

        //NEW
        // n = name of RSS feed
        // url = RSSFeed url
        private void makeNewFeed(String n, String url)
        {

            //NEW METHOD
            selectedTreeItem.addChild(new TreeItem("f",url,n));
            updateDirectoryView();

            //setting the xml writer
            writer = XmlWriter.Create(saveDataDirectory + xmlFileNameDirectory);   // update the DirectoryView xml file (has data for the tree)

            //saving to xml file
            manager.Save(writer, directoryRoot);
            writer.Close();

            

        }

        //save the current directory
        //OLD METHOD...DELETE WHEN ABLE
        private void updateDirectory()
        {
            
            //OLD METHOD
            //importing feeds here...

            DirectoryInfo d = new DirectoryInfo(SelectedDirectory);

            FileInfo[] Files = d.GetFiles("*.xml"); //Getting Xml files
            string tempFileString = "";

            selectedDirectoryItems = new List<DirectoryItem>();
            selectedDirectoryUrls = new List<String>();
            selectedDirectoryTitles = new List<String>();

            selectedDirectoryIndex = 0;

            foreach (FileInfo file in Files)
            {
                tempFileString = tempFileString + ", " + file.Name;

                selectedDirectoryItems.Add(new DirectoryItem(file.Name, file.Directory.ToString(), "feed"));
                selectedDirectoryTitles.Add(System.IO.Path.GetFileNameWithoutExtension(file.Name) + " FEED");
                selectedDirectoryUrls.Add(file.Directory.ToString());


                

            }



            //importing directories here...

            Console.WriteLine("" + selectedDirectoryItems);
            Console.WriteLine(selectedDirectoryTitles);

            var Directories = Directory.GetDirectories(SelectedDirectory);
            string tempFolderString = "";
            DirectoryInfo tempDirectoryInfo = new DirectoryInfo(SelectedDirectory);


            foreach (String directory in Directories)
            {
                tempDirectoryInfo = new DirectoryInfo(directory);

                //for debugging
                tempFolderString = tempFolderString + ", " + tempDirectoryInfo.Name;

                //actual importing...
                tempFileString = tempFileString + ", " + tempDirectoryInfo.Name;

                selectedDirectoryItems.Add(new DirectoryItem(tempDirectoryInfo.Name, directory.ToString(), "folder"));
                selectedDirectoryTitles.Add(tempDirectoryInfo.Name + " FOLDER");
                selectedDirectoryUrls.Add(tempDirectoryInfo.ToString());


                

            }

            Console.WriteLine("" + selectedDirectoryItems);
            Console.WriteLine(selectedDirectoryTitles);

            DirectoryView.ItemsSource = selectedDirectoryTitles;
            DirectoryView.UpdateLayout();


            Console.WriteLine();


            


        }
        //go back to the parent directory
        private void goToParentDirectory()
        {
            if (!SelectedDirectory.Equals(saveDataDirectory))
            {
                //go back code here...
                SelectedDirectory = selectedDirectoryBack;
                selectedDirectoryBack = System.IO.Directory.GetParent(SelectedDirectory).FullName;
                Console.WriteLine("");
                updateDirectory();
                Console.WriteLine("");
            }

            //switching visibility of the directoryViews
            DirectoryViewEmpty.Visibility = Visibility.Collapsed;
            newDirectoryView.Visibility = Visibility.Visible;

            Console.WriteLine();

        }

        private void importXMLFeedList(String url)
        {
            
            tempFeedItem = new FeedItem();
            /*
            //parsing the selected feed into the 
            TextReader = new System.Xml.XmlTextReader(url);

            int I = TextReader.AttributeCount;

            var RSSparser = new FeedReader();
            var retrievedFeed = RSSparser.RetrieveFeed(url);

            //retriving feed stuff and adding it to a list
            foreach (var i in retrievedFeed)
            {
                tempFeedItem = new FeedItem(i.Title.ToString(), i.Uri.ToString());//may add more here);
                selectedFeedItems.Add(tempFeedItem);
                Console.WriteLine($"{i.Date.ToString("g")}\t{i.Title}");
            }
            */
            selectedFeedUrls = new List<String>();
            selectedFeedTitles = new List<String>();
            selectedFeedItems = new List<FeedItem>();

            var RSSparser = new FeedReader();
            var retrievedFeed = RSSparser.RetrieveFeed(url);

            tempFeedItem = new FeedItem();

            //parsing xml file
            XmlReader reader = XmlReader.Create(@url);
            using (reader)
            {

                String tempTitle = "";
                String tempUrl = "";

                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        //return only when you have START tag  
                        switch (reader.Name.ToString())
                        {
                            case "feed":
                                if (reader.HasAttributes)
                                {
                                    // Add title
                                    tempTitle = reader.GetAttribute("name");
                                    selectedFeedTitles.Add(tempTitle);

                                    // Add URL
                                    tempUrl = reader.GetAttribute("url");
                                    selectedFeedItems.Add(new FeedItem(tempTitle, tempUrl));

                                    //setting reference lists (may need to add more later on)

                                    selectedFeedUrls.Add(tempUrl);
                                    reader.MoveToNextAttribute();
                                }

                                break;
                        }
                        
                    }

                    
                }
            }

            Console.WriteLine();

            
          
            
        }

        private void changeWebView(int index)
        {
            if (FeedView.SelectedIndex != -1 && index != -1)
            {
                //ADDED METHOD


                selectedFeedIndex = index;
                FeedView.SelectedIndex = selectedFeedIndex;
                myWebBrowser.Navigate(selectedFeedUrls[selectedFeedIndex]);
            }
        }


        //this keeps the webview from spitting out javascript errors
        private static bool TrySetSuppressScriptErrors(WebBrowser webBrowser, bool value)
        {
            FieldInfo field = typeof(WebBrowser).GetField("_axIWebBrowser2", BindingFlags.Instance | BindingFlags.NonPublic);
            if (field != null)
            {
                object axIWebBrowser2 = field.GetValue(webBrowser);
                if (axIWebBrowser2 != null)
                {
                    axIWebBrowser2.GetType().InvokeMember("Silent", BindingFlags.SetProperty, null, axIWebBrowser2, new object[] { value });
                    return true;
                }
            }

            return false;
        }



     



        //events (buttons clicked, etc)

        //SWITCH BETWEEN READER VIEW AND MAP VIEW
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //RUNNING FINE HERE
            if (BrowserVisibility == true && MapVisibility == false)
            {
                myWebBrowser.Visibility = Visibility.Hidden;
                myMap.Visibility = Visibility.Visible;
                BrowserVisibility = false;
                MapVisibility = true;

            }
            else if (BrowserVisibility == false && MapVisibility == true)
            {
                myWebBrowser.Visibility = Visibility.Visible;
                myMap.Visibility = Visibility.Hidden;
                BrowserVisibility = true;
                MapVisibility = false;

            }
            //CHECKING FOR ERROR HERE
            else if (MapVisibility == true && BrowserVisibility == true)
            {
                Console.WriteLine("ERROR : BOTH MAP AND BROWSER ARE VISIBLE");
            }
            else if (MapVisibility == false && BrowserVisibility == false)
            {
                Console.WriteLine("ERROR : BOTH MAP AND BROWSER ARE INVISIBLE");
            }
            //ELSE....
            else
            {
                Console.WriteLine("ERROR : I DON'T KNOW WHAT THE HELL'S GOING ON");
            }
        }



        private void addRSSFeed_Click(object sender, RoutedEventArgs e)
        {


                //TODO : somehow, make sure that folder is selected. Perhaps turn this into a method...this can only be done once we have the tree/directory viewer working
                Window3 window3 = new Window3();
                window3.ShowDialog();


                makeNewFeed(window3.nameBox.Text, window3.linkBox.Text);
                //NEW METHOD FOR XML NEW UPDATE
                updateDirectoryView();

                newDirectoryView.Visibility = Visibility.Visible;
                DirectoryViewEmpty.Visibility = Visibility.Collapsed;

            //OLD METHOD
            //importXMLFeedList(SelectedDirectory + "\\" + window3.nameBox.Text + ".xml");




            //should be noted that this isn't very efficient...should be changed somehow, if possible in our timeframe :/
            /*
            for(int i = 0; i < selectedFeedItems.Count; i++)
            {

                selectedTitles.Add(selectedFeedItems[i].title);
                selectedUrls.Add(selectedFeedItems[i].url);
            }
            */

            DirectoryView.ItemsSource = selectedDirectoryTitles;
                FeedView.ItemsSource = selectedFeedTitles;


                selectedFeedIndex = 0;
                FeedView.SelectedIndex = 0;

                DirectoryView.Visibility = Visibility.Visible;
                DirectoryViewEmpty.Visibility = Visibility.Collapsed;

                //myWebBrowser.Navigate(selectedFeedUrls[0]);
                Console.WriteLine("");

                DirectoryView.ItemsSource = selectedDirectoryTitles;
                FeedView.ItemsSource = selectedFeedTitles;

                DirectoryView.Items.Refresh();
                FeedView.Items.Refresh();

            
            

        }

        //FINISHED
        private void addFolder_Click(object sender, RoutedEventArgs e)
        {
            //TODO : diddo above method :/

            Window4 window4 = new Window4();
            window4.ShowDialog();
            
            
            makeNewFolder(window4.nameBox.Text);
            updateDirectoryView();


            DirectoryView.ItemsSource = selectedDirectoryTitles;
            FeedView.ItemsSource = selectedFeedTitles;

            newDirectoryView.Visibility = Visibility.Visible;
            DirectoryViewEmpty.Visibility = Visibility.Collapsed;
        }

        private void FeedView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedFeedIndex = FeedView.SelectedIndex;
            changeWebView(FeedView.SelectedIndex);
            updatePushPinSelected();
        }

        //FINISHED
        private void deleteWhatever_Click(object sender, RoutedEventArgs e)
        {

            //NEW METHOD

            //deleting from tree
            if (newDirectoryView.Items.Count > 0)
            {
                if (newDirectoryView.SelectedIndex >= 0)
                {
                    selectedTreeItem.deleteChild(newDirectoryView.SelectedIndex);
                }
            }

            //updating the directory view to match
            updateDirectoryView();

            //setting the xml writer
            writer = XmlWriter.Create(saveDataDirectory + xmlFileNameDirectory);   // update the DirectoryView xml file (has data for the tree)

            //saving to xml file
            manager.Save(writer, directoryRoot);
            writer.Close();


        }


    

  
        //enter button clicked
        private void Enter_Click(object sender, RoutedEventArgs e)
        {
                //NEW METHOD
            
                if (newDirectoryView.Items.Count > 0)
                {
                    if (newDirectoryView.SelectedIndex >= 0)
                    {
                        if (selectedTreeItem.getChild(newDirectoryView.SelectedIndex).getType() == "feed" && selectedTreeItem.getChild(newDirectoryView.SelectedIndex).getUrl() != null)
                        {
                            downloadFeed(selectedTreeItem.getChild(newDirectoryView.SelectedIndex).getUrl());
                            updateFeedView();
                            updatePushPinAll();

                        }
                        else if (selectedTreeItem.getType() == "directory")
                        {
                            selectedTreeItem = selectedTreeItem.getChild(newDirectoryView.SelectedIndex);
                            updateDirectoryView();

                            if (selectedTreeItem.Count() == 0)
                            {
                                newDirectoryView.Visibility = Visibility.Collapsed;
                                DirectoryViewEmpty.Visibility = Visibility.Visible;
                            }
                            else if (selectedTreeItem.Count() >= 1)
                            {
                                newDirectoryView.Visibility = Visibility.Visible;
                                DirectoryViewEmpty.Visibility = Visibility.Collapsed;
                            }
                        }
                    }
                }

            myMapPushPinLayer.UpdateLayout();
            myMap.UpdateLayout();
            


        }

        private void updatePushPinAll()
        {
            //NEW METHOD
            float tempXlat = 0f;
            float tempYlat = 0f;


            //deleting all old pushpins:
            myMapPushPinLayer = new Microsoft.Maps.MapControl.WPF.MapLayer();
            myMap.Children.Clear();
            myMap.Children.Add(myMapPushPinLayer);

            //an insanely INEFFICIENT WAY to find out if the selected feed title contains a city name
            //fix this if at all possible

            
            foreach (String articleTitle in selectedFeedTitles)
            {
                List<String> tempList = articleTitle.Split(" ").ToList();

                foreach (String i in tempList)
                {
                    if (cityHashTable.ContainsValue(i))
                    {
                        Microsoft.Maps.MapControl.WPF.Pushpin tempPushPin = new Microsoft.Maps.MapControl.WPF.Pushpin();

                        tempXlat = float.Parse(xlatitudeHashTable[i].ToString());
                        tempYlat = float.Parse(ylatitudeHashTable[i].ToString());

                        tempPushPin.Content = articleTitle;

                        tempPushPin.Location = new Microsoft.Maps.MapControl.WPF.Location(tempXlat, tempYlat);

                        myMapPushPinLayer.AddChild(tempPushPin, new Microsoft.Maps.MapControl.WPF.Location(tempXlat, tempYlat), Microsoft.Maps.MapControl.WPF.PositionOrigin.BottomCenter);
                        break;
                    }

                    if (stateHashTable.ContainsValue(i))
                    {
                        Microsoft.Maps.MapControl.WPF.Pushpin tempPushPin = new Microsoft.Maps.MapControl.WPF.Pushpin();

                        tempXlat = float.Parse(xlatitudeHashTable[i].ToString());
                        tempYlat = float.Parse(ylatitudeHashTable[i].ToString());

                        tempPushPin.Content = selectedFeedTitles.ElementAt(selectedFeedIndex);

                        tempPushPin.Location = new Microsoft.Maps.MapControl.WPF.Location(tempXlat, tempYlat);

                        myMapPushPinLayer.AddChild(tempPushPin, new Microsoft.Maps.MapControl.WPF.Location(tempXlat, tempYlat), Microsoft.Maps.MapControl.WPF.PositionOrigin.BottomCenter);
                        break;
                    }
                }
            }


            //OLD METHOD
            /*
            float tempXlat = 0f;
            float tempYlat = 0f;

            
            //deleting all old pushpins:
            myMapPushPinLayer = new Microsoft.Maps.MapControl.WPF.MapLayer();
            myMap.Children.Clear();
            myMap.Children.Add(myMapPushPinLayer);

            //an insanely INEFFICIENT WAY to find out if the selected feed title contains a city name
            //fix this if at all possible
            foreach (var i in selectedFeedTitles)
            {


                int tempIndex = 0;
                foreach (var b in cityNames)
                {

                   
                    tempXlat = xLatitude.ElementAt(tempIndex);
                    tempYlat = yLatitude.ElementAt(tempIndex);

                    if (i.Contains(b))
                    {
                        Microsoft.Maps.MapControl.WPF.Pushpin tempPushPin = new Microsoft.Maps.MapControl.WPF.Pushpin();

                        tempPushPin.Content = i;
                        tempPushPin.Location = new Microsoft.Maps.MapControl.WPF.Location(tempXlat, tempYlat);
                        myMapPushPinLayer.AddChild(tempPushPin, new Microsoft.Maps.MapControl.WPF.Location(tempXlat, tempYlat), Microsoft.Maps.MapControl.WPF.PositionOrigin.BottomCenter);
                        break;
                    }

                    tempIndex++;
                }
                tempIndex = 0;
                foreach (var a in stateNames)
                {
                    
                    tempXlat = xLatitude.ElementAt(tempIndex);
                    tempYlat = yLatitude.ElementAt(tempIndex);

                    if (i.Contains(a))
                    {
                        Microsoft.Maps.MapControl.WPF.Pushpin tempPushPin = new Microsoft.Maps.MapControl.WPF.Pushpin();
                        
                        tempPushPin.Content = i;
                        tempPushPin.Location = new Microsoft.Maps.MapControl.WPF.Location(tempXlat,tempYlat);
                        myMapPushPinLayer.AddChild(new Microsoft.Maps.MapControl.WPF.Pushpin(), new Microsoft.Maps.MapControl.WPF.Location(tempXlat, tempYlat), Microsoft.Maps.MapControl.WPF.PositionOrigin.BottomCenter);
                        
                        
                        
                        
                        
                        break;
                    }

                    tempIndex++;

                }
            }

            */
            myMapPushPinLayer.UpdateLayout();
            myMap.UpdateLayout();
            myMapPushPinLayer.UpdateLayout();
            myMap.UpdateLayout();
            
        }

        private void updatePushPinSelected()
        {
            float tempXlat = 0f;
            float tempYlat = 0f;


            //deleting all old pushpins:
            myMapPushPinLayer = new Microsoft.Maps.MapControl.WPF.MapLayer();
            myMap.Children.Clear();
            myMap.Children.Add(myMapPushPinLayer);

            //an insanely INEFFICIENT WAY to find out if the selected feed title contains a city name
            //fix this if at all possible

            List<String> tempList = selectedFeedTitles.ElementAt(selectedFeedIndex).Split(" ").ToList();
            foreach (String i in tempList)
            {
                if (cityHashTable.ContainsValue(i))
                {
                    Microsoft.Maps.MapControl.WPF.Pushpin tempPushPin = new Microsoft.Maps.MapControl.WPF.Pushpin();

                    tempXlat = float.Parse(xlatitudeHashTable[i].ToString());
                    tempYlat = float.Parse(ylatitudeHashTable[i].ToString());

                    tempPushPin.Content = selectedFeedTitles.ElementAt(selectedFeedIndex);
                    
                    tempPushPin.Location = new Microsoft.Maps.MapControl.WPF.Location(tempXlat, tempYlat);

                    myMapPushPinLayer.AddChild(tempPushPin, new Microsoft.Maps.MapControl.WPF.Location(tempXlat, tempYlat), Microsoft.Maps.MapControl.WPF.PositionOrigin.BottomCenter);
                    break;
                }

                if (stateHashTable.ContainsValue(i))
                {
                    Microsoft.Maps.MapControl.WPF.Pushpin tempPushPin = new Microsoft.Maps.MapControl.WPF.Pushpin();

                    tempXlat = float.Parse(xlatitudeHashTable[i].ToString());
                    tempYlat = float.Parse(ylatitudeHashTable[i].ToString());

                    tempPushPin.Content = selectedFeedTitles.ElementAt(selectedFeedIndex);

                    tempPushPin.Location = new Microsoft.Maps.MapControl.WPF.Location(tempXlat, tempYlat);

                    myMapPushPinLayer.AddChild(tempPushPin, new Microsoft.Maps.MapControl.WPF.Location(tempXlat, tempYlat), Microsoft.Maps.MapControl.WPF.PositionOrigin.BottomCenter);
                    break;
                }
            }


            




            //OLD METHOD

            /*
            int tempIndex = 0;
                foreach (var b in cityNames)
                {
                    
                    tempXlat = xLatitude.ElementAt(tempIndex);
                    tempYlat = yLatitude.ElementAt(tempIndex);
                    if (selectedFeedTitles.ElementAt(selectedFeedIndex).Contains(b))
                    {
                        Microsoft.Maps.MapControl.WPF.Pushpin tempPushPin = new Microsoft.Maps.MapControl.WPF.Pushpin();

                        tempPushPin.Content = selectedFeedTitles.ElementAt(selectedFeedIndex);
                        tempPushPin.Location = new Microsoft.Maps.MapControl.WPF.Location(tempXlat, tempYlat);
                        myMapPushPinLayer.AddChild(tempPushPin, new Microsoft.Maps.MapControl.WPF.Location(tempXlat, tempYlat), Microsoft.Maps.MapControl.WPF.PositionOrigin.BottomCenter);
                        break;
                    }
                    tempIndex++;
                }
            tempIndex = 0;
                foreach (var a in stateNames)
                {

                    tempXlat = xLatitude.ElementAt(tempIndex);
                    tempYlat = yLatitude.ElementAt(tempIndex);
                    if (selectedFeedTitles.ElementAt(selectedFeedIndex).Contains(a))
                    {
                        Microsoft.Maps.MapControl.WPF.Pushpin tempPushPin = new Microsoft.Maps.MapControl.WPF.Pushpin();

                        tempPushPin.Content = selectedFeedTitles.ElementAt(selectedFeedIndex);
                        tempPushPin.Location = new Microsoft.Maps.MapControl.WPF.Location(tempXlat, tempYlat);
                        myMapPushPinLayer.AddChild(new Microsoft.Maps.MapControl.WPF.Pushpin(), new Microsoft.Maps.MapControl.WPF.Location(tempXlat, tempYlat), Microsoft.Maps.MapControl.WPF.PositionOrigin.BottomCenter);
                        break;
                    }
                    tempIndex++;
                }

            */

            //updating the pushpinlayer and mapview
            myMapPushPinLayer.UpdateLayout();
            myMap.UpdateLayout();
            myMapPushPinLayer.UpdateLayout();
            myMap.UpdateLayout();

        }

        private void goBack_Click(object sender, RoutedEventArgs e)
        {
            //NEW METHOD
            if (selectedTreeItem.isRoot == false)
            {
                selectedTreeItem = selectedTreeItem.getParent();
            }

            updateDirectoryView();

            newDirectoryView.Visibility = Visibility.Visible;
            DirectoryViewEmpty.Visibility = Visibility.Collapsed;

        }





        //THIS DOESN'T WORK FOR SOME REASON, SCRAPPING IT BUT KEEPING IT HERE just in case :/
        //program won't work without this addition sadly...oh well.
        private void DirectoryView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            selectedDirectoryIndex = DirectoryView.SelectedIndex;

        }


        //switch between the different directory views
        private void SwitchClick(object sender, RoutedEventArgs e)
        {
            //switch to the topic window
            TopicWindow topicWindow = new TopicWindow();
            this.Hide();
            topicWindow.ShowDialog();
            this.Show();
            
        }

        private void changeTimeInterval_Click(object sender, RoutedEventArgs e)
        {
            Window6 window6 = new Window6();
            window6.ShowDialog();

            offsetDate = new System.TimeSpan(int.Parse(window6.daysBox.Text), int.Parse(window6.hoursBox.Text), int.Parse(window6.minutesBox.Text), 0);

        }

        private void update_Click(object sender, RoutedEventArgs e)
        {
            if (newDirectoryView.Items.Count > 0)
            {
                if (newDirectoryView.SelectedIndex >= 0)
                {
                    if (selectedTreeItem.getChild(newDirectoryView.SelectedIndex).getType() == "feed" && selectedTreeItem.getChild(newDirectoryView.SelectedIndex).getUrl() != null)
                    {
                        downloadFeed(selectedTreeItem.getChild(newDirectoryView.SelectedIndex).getUrl());
                        updateFeedView();
                        updatePushPinAll();
                    }
                    else if (selectedTreeItem.getType() == "directory")
                    {
                        selectedTreeItem = selectedTreeItem.getChild(newDirectoryView.SelectedIndex);
                        updateDirectoryView();

                        if (selectedTreeItem.Count() == 0)
                        {
                            newDirectoryView.Visibility = Visibility.Collapsed;
                            DirectoryViewEmpty.Visibility = Visibility.Visible;
                        }
                        else if (selectedTreeItem.Count() >= 1)
                        {
                            newDirectoryView.Visibility = Visibility.Visible;
                            DirectoryViewEmpty.Visibility = Visibility.Collapsed;
                        }
                    }
                }
            }
        }
    }
}
