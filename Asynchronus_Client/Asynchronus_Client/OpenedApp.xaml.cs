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
        public OpenedApp(AsynchronousClient server, string user, string balance)
        {
            this.user = user;
            this.server = server;
            InitializeComponent();

            BalancetextBox.Text = balance;
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

            if(txtBox.Text == "0" || txtBox.Text == "User")
                txtBox.Text = string.Empty;

        }
        private void DepositButton_Click(object sender, RoutedEventArgs e)
        {
            string toDeposit;
            if (int.Parse(DepositTextBox.Text) < 0)
            {
                string message = "Value not valid.";
                string caption = "Error";
                MessageBoxButton buttons = MessageBoxButton.OK;
                MessageBoxImage icon = MessageBoxImage.Error;
                MessageBox.Show(message, caption, buttons, icon);
            }
            else
            {
                if (DepositTextBox.Text == "")
                {
                    toDeposit = "0";
                }
                else
                    toDeposit = DepositTextBox.Text.ToString();

                String data = user + ";" + toDeposit + "<DEP>";
                server.Send(data);
                sendDone.WaitOne(500);
                server.Receive();
                receiveDone.WaitOne(500);
                string recived = server.receivedData;

                if (recived.IndexOf("<DEP>") > -1)
                {
                    int index = recived.IndexOf("<");
                    if (index > 0)
                        recived = recived.Substring(0, index);

                    BalancetextBox.Text = recived;

                    string message = "Deposit done successfully.";
                    string caption = "Success";
                    MessageBoxButton buttons = MessageBoxButton.OK;
                    MessageBoxImage icon = MessageBoxImage.Information;
                    MessageBox.Show(message, caption, buttons, icon);

                }
            }
            DepositTextBox.Text = "0";
        }
        private void WithdrawButton_Click(object sender, RoutedEventArgs e)
        {
            string toWithdraw;
            if (int.Parse(WithdrawTextBox.Text) < 0)
            {
                string message = "Value not valid.";
                string caption = "Error";
                MessageBoxButton buttons = MessageBoxButton.OK;
                MessageBoxImage icon = MessageBoxImage.Error;
                MessageBox.Show(message, caption, buttons, icon);
            }
            else
            {
                if (WithdrawTextBox.Text == "")
                {
                    toWithdraw = "0";
                }
                else
                    toWithdraw = WithdrawTextBox.Text.ToString();

                String data = user + ";" + toWithdraw + "<WTD>";
                server.Send(data);
                sendDone.WaitOne(500);
                server.Receive();
                receiveDone.WaitOne(500);
                string recived = server.receivedData;

                if (recived.IndexOf("<WTD>") > -1)
                {
                    int index = recived.IndexOf("<");
                    if (index > 0)
                        recived = recived.Substring(0, index);

                    BalancetextBox.Text = recived;

                    string message = "Withdraw done successfully.";
                    string caption = "Success";
                    MessageBoxButton buttons = MessageBoxButton.OK;
                    MessageBoxImage icon = MessageBoxImage.Information;
                    MessageBox.Show(message, caption, buttons, icon);

                }
                else if (recived.IndexOf("<WTN>") > -1)
                {
                    string message = "Not enough money on your account.";
                    string caption = "Error";
                    MessageBoxButton buttons = MessageBoxButton.OK;
                    MessageBoxImage icon = MessageBoxImage.Error;
                    MessageBox.Show(message, caption, buttons, icon);
                }
            }
            WithdrawTextBox.Text = "0";
        }
        private void TransferButton_Click(object sender, RoutedEventArgs e)
        {
            string moneyToTransfer, userToTransfer;
            if (int.Parse(TransferMoneyTextBox.Text) < 0)
            {
                string message = "Value not valid.";
                string caption = "Error";
                MessageBoxButton buttons = MessageBoxButton.OK;
                MessageBoxImage icon = MessageBoxImage.Error;
                MessageBox.Show(message, caption, buttons, icon);
            }
            else
            {
                if (TransferMoneyTextBox.Text == "")
                {
                    moneyToTransfer = "0";
                }
                else
                    moneyToTransfer = TransferMoneyTextBox.Text.ToString();

                if (TransferUserTextBox.Text == "")
                {
                    userToTransfer = "0";
                }
                else
                    userToTransfer = TransferUserTextBox.Text.ToString();

                String data = user + ";" + userToTransfer + ";" + moneyToTransfer + "<TRS>";
                server.Send(data);
                sendDone.WaitOne(500);
                server.Receive();
                receiveDone.WaitOne(500);
                string recived = server.receivedData;

                if (recived.IndexOf("<TRS>") > -1)
                {
                    int index = recived.IndexOf("<");
                    if (index > 0)
                        recived = recived.Substring(0, index);

                    BalancetextBox.Text = recived;

                    string message = "Transfer done successfully.";
                    string caption = "Success";
                    MessageBoxButton buttons = MessageBoxButton.OK;
                    MessageBoxImage icon = MessageBoxImage.Information;
                    MessageBox.Show(message, caption, buttons, icon);
                }
                else if (recived.IndexOf("<TRN>") > -1)
                {
                    string message = "This user don't exist.";
                    string caption = "Error";
                    MessageBoxButton buttons = MessageBoxButton.OK;
                    MessageBoxImage icon = MessageBoxImage.Error;
                    MessageBox.Show(message, caption, buttons, icon);
                }
                else if(recived.IndexOf("<TRQ>") > -1)
                {
                    string message = "You dont have enough money.";
                    string caption = "Error";
                    MessageBoxButton buttons = MessageBoxButton.OK;
                    MessageBoxImage icon = MessageBoxImage.Error;
                    MessageBox.Show(message, caption, buttons, icon);
                }
            }
        }

    }
}
