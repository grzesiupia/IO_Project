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
using Asynchronus_Client;

namespace Asynchronus_Client
{
    
    /// <summary>
    /// Interaction logic for IOProjectApp.xaml
    /// </summary>
    public partial class IOProjectApp : Window
    {
        public String username { get; set; }
        public String password { get; set; }

        AsynchronousClient server = new AsynchronousClient();

        public IOProjectApp()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            server.StartClient();
        }

        private void TextBox_GotKeyboardFocus(object sender, RoutedEventArgs e)
        {
            TextBox txtBox = sender as TextBox;
            
            if (txtBox.Text == "Login")
                txtBox.Text = string.Empty;
                
        }

        private void PasswordBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            PasswordBox passBox = sender as PasswordBox;
            if (passBox.Password == "Password")
                passBox.Password = string.Empty;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            String data = "";
            data = LoginBox.Text.ToString() + ";" + PasswordBox.Password.ToString();
            box.Text = data;
  
        }
    }
}
