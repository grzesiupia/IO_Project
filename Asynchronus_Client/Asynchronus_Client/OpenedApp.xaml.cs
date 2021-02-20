using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Asynchronous_Client
{
    /// <summary>
    /// Interaction logic for OpenedApp.xaml
    /// </summary>
    public partial class OpenedApp : Page
    {
        private static ManualResetEvent sendDone = new ManualResetEvent(false);
        private static ManualResetEvent receiveDone = new ManualResetEvent(false);


        string user;
        
        AsynchronousClient server;
        public OpenedApp(AsynchronousClient server, string user)
        {
            this.user = user;
            this.server = server;
            InitializeComponent();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            String data = "Closing client<EOF>";
            server.Send(data);
            server.StopClient();
        }
        private void TextBox_GotKeyboardFocus(object sender, RoutedEventArgs e)
        {
            TextBox txtBox = sender as TextBox;

            if (txtBox.Text == "0")
                txtBox.Text = string.Empty;

        }
        private void DepositButton_Click(object sender, RoutedEventArgs e)
        {
            string toDeposit = DepositTextBox.Text.ToString();
            String  data = user + ";" + toDeposit + "<DEP>";
            server.Send(data);
            sendDone.WaitOne(1000);
            server.Receive();
            receiveDone.WaitOne(500);
            string recived = server.receivedData;

            if (recived.IndexOf("<DEP>") > -1)
            {
                int index = recived.IndexOf("<");
                if (index > 0)
                    recived = recived.Substring(0, index);

                BalancetextBox.Text = recived;

            }

            DepositTextBox.Text = "0";
        }
    }
}
