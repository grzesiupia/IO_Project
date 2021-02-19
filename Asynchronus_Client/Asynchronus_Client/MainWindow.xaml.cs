using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
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

        private static ManualResetEvent sendDone = new ManualResetEvent(false);
        private static ManualResetEvent receiveDone = new ManualResetEvent(false);

        public AsynchronousClient server = new AsynchronousClient();

        public IOProjectApp()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            server.StartClient();
            if (server.isConnected == true)
            {
                SolidColorBrush brush = new SolidColorBrush();
                brush.Color = Color.FromRgb(0, 255, 0);
                Diode.Fill = brush;
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            String data;
            data = "Closing client<EOF>";
            server.Send(data);
            server.StopClient();
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

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (server.isConnected == false)
            {
                string message = "You are not connected with server.";
                string caption = "Error";
                MessageBoxButton buttons = MessageBoxButton.OK;
                MessageBoxImage icon = MessageBoxImage.Error;
                MessageBox.Show(message, caption, buttons, icon);

            }
            else
            {
                String data;
                data = LoginBox.Text.ToString() + ";" + PasswordBox.Password.ToString() + "<LOG>";
                server.Send(data);
                sendDone.WaitOne(500);
                server.Receive();
                receiveDone.WaitOne(500);

                string recived = server.receivedData;
                Trace.WriteLine("Response received: " + recived);

                if (recived.IndexOf("<LON>") > -1)
                {
                    string message = "Check Username/Password.";
                    string caption = "Error";
                    MessageBoxButton buttons = MessageBoxButton.OK;
                    MessageBoxImage icon = MessageBoxImage.Error;
                    MessageBox.Show(message, caption, buttons, icon);

                }
                else if (recived.IndexOf("<LOG>") > -1)
                {
                    string message = "Logged in successfully.";
                    string caption = "Success";
                    MessageBoxButton buttons = MessageBoxButton.OK;
                    MessageBoxImage icon = MessageBoxImage.Information;
                    MessageBox.Show(message, caption, buttons, icon);
                }
            }
        }

        public void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            if (server.isConnected == false)
            {
                string message = "You are not connected with server.";
                string caption = "Error";
                MessageBoxButton buttons = MessageBoxButton.OK;
                MessageBoxImage icon = MessageBoxImage.Error;
                MessageBox.Show(message, caption, buttons, icon);

            }
            else
            {
                String data;
                data = LoginBox.Text.ToString() + ";" + PasswordBox.Password.ToString() + "<REG>";
                server.Send(data);
                sendDone.WaitOne(500);
                server.Receive();
                receiveDone.WaitOne(500);

                string recived = server.receivedData;
                Trace.WriteLine("Response received: " + recived);

                if (recived.IndexOf("<REN>") > -1)
                {
                    string message = "User with this name already exist.";
                    string caption = "Error";
                    MessageBoxButton buttons = MessageBoxButton.OK;
                    MessageBoxImage icon = MessageBoxImage.Error;
                    MessageBox.Show(message, caption, buttons, icon);

                }
                else if (recived.IndexOf("<REG>") > -1)
                {
                    string message = "User crated succesfully.";
                    string caption = "Success";
                    MessageBoxButton buttons = MessageBoxButton.OK;
                    MessageBoxImage icon = MessageBoxImage.Information;
                    MessageBox.Show(message, caption, buttons, icon);
                }
            }
        }
    }
}
