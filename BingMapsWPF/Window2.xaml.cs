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
using System.Windows.Shapes;
using System.IO;

namespace BingMapsWPF
{
    /// <summary>
    /// Interaction logic for Window2.xaml
    /// </summary>
    public partial class Window2 : Window
    {

        //please make TempDirectory equal to 

        String oldDirectory;
        String LibraryName;
        Boolean readyToClose;
        public Window2()
        {
            InitializeComponent();
        }

        private void Enter_Click(object sender, RoutedEventArgs e)
        {
            oldDirectory = TempDirectory.Text;
            LibraryName = LibraryNameBox.Text;
            readyToClose = false;

           
            TempDirectory.Text = TempDirectory.Text + LibraryName;
            readyToClose = true;

            


            if (LibraryName == null || LibraryName.Length == 0 || LibraryName == "")
            {
                ERROR.Visibility = Visibility.Visible;
                ERROR.Text = "Library name can't be empty";
                readyToClose = false;
            }
            else if (Directory.Exists(TempDirectory.Text))
            {
                ERROR.Visibility = Visibility.Visible;
                ERROR.Text = "A directory with this name already exists";
                
                TempDirectory.Text = oldDirectory;
                
                LibraryName = null;
                LibraryNameBox.Text = null;
            
                readyToClose = false;

            }


            if (readyToClose == true)
            {
                Directory.CreateDirectory(TempDirectory.Text);
                this.Close();
            }
            


        }

        //this silences errors from the webview. It doesn't quite solve our problem, but it allows the program to be usable...
        

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            backstatus.Content = "yes";
            this.Close();
        }
    }
}
