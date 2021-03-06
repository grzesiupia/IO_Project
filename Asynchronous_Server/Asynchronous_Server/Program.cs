﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;



public class SynchronousSocketListener
{

    // Incoming data from the client.  
    public static string data = null;

    public static void StartListening()
    {
        // Data buffer for incoming data.  
        byte[] bytes = new Byte[1024];

        // Establish the local endpoint for the socket.  
        // Dns.GetHostName returns the name of the
        // host running the application.  
        IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
        IPAddress ipAddress = ipHostInfo.AddressList[0];
        IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);

        // Create a TCP/IP socket.  
        Socket listener = new Socket(ipAddress.AddressFamily,
            SocketType.Stream, ProtocolType.Tcp);

        //Initialize database
        DBConnect db_users = new DBConnect();

        db_users.Initialize();

        // Bind the socket to the local endpoint and
        // listen for incoming connections. 
        try
        {
            listener.Bind(localEndPoint);
            listener.Listen(10);

            byte[] msg;

            // Start listening for connections.  
            while (true)
            {
                Console.WriteLine("Waiting for a connection...");
                
                // Program is suspended while waiting for an incoming connection.  
                Socket handler = listener.Accept();
                data = null;

                // An incoming connection needs to be processed.  
                while (true)
                {
                    data = "";
                    int bytesRec = handler.Receive(bytes);
                    data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                    if(data.IndexOf("<EOF>") > -1)
                    {
                        Console.WriteLine("Data recived: {0}", data);
                        break;
                    }
                    else if(data.IndexOf("<TST>") > -1)
                    {
                        Console.WriteLine("Data recived: {0}", data);
                        msg = Encoding.ASCII.GetBytes("Test connection passed<TST>");
                        handler.Send(msg);
                    }
                    else if(data.IndexOf("<ECO>") > -1)
                    {
                        Console.WriteLine("Data to Echo: {0}", data);
                        msg = Encoding.ASCII.GetBytes(data);
                        handler.Send(msg);
                    }
                    else if(data.IndexOf("<REG>") > -1)
                    {
                        bool alreadyExist = false;
                        Console.WriteLine("Passes to register: {0}", data);
                        
                        //data="username;password<REG>
                     
                        string[] subs = data.Split(';');
                        
                        //subs[0] = username
                        //subs[1] = password<REG>
                        
                        int index = subs[1].IndexOf("<");
                        if (index > 0)
                            subs[1] = subs[1].Substring(0, index);
                        
                        //subs[0] = username
                        //subs[1] = password
                        
                        Console.WriteLine(subs[0] + " " + subs[1]);
                        List<string>[] pass = db_users.Select();
                        Console.WriteLine("user_id   password   balance");
                        
                        for (int j = 0; j < pass[0].Count; j++)
                        {

                            Console.WriteLine(pass[0][j] + "   " + pass[1][j] + "   " + pass[2][j]);
                            if (pass[0][j] == subs[0])
                            {
                                alreadyExist = true;
                            }
                        }
                        if (alreadyExist == false)
                        {
                            Console.WriteLine("Registered Succesfully<REG>");
                            db_users.Insert(subs[0], subs[1]);
                            msg = Encoding.ASCII.GetBytes("Registered Succesfully<REG>");
                            handler.Send(msg);
                        }
                        else
                        {
                            Console.WriteLine("Username already exist<REG>");
                            msg = Encoding.ASCII.GetBytes("Username already exist<REN>");
                            handler.Send(msg);
                        }

                    }
                    else if(data.IndexOf("<LOG>") > -1)
                    {
                        bool alreadyExist = false;

                        //data="username;password<REG>
                        string userBalance = "";
                        string[] subs = data.Split(';');
                        
                        //subs[0] = username
                        //subs[1] = password<REG>
                        
                        int index = subs[1].IndexOf("<");
                        if (index > 0)
                            subs[1] = subs[1].Substring(0, index);
                        
                        //subs[0] = username
                        //subs[1] = password
                        
                        Console.WriteLine("Loging user: {0}", subs[0]);
                        List<string>[] pass = db_users.Select();
                        Console.WriteLine("user_id   password");
                        
                        for (int j = 0; j < pass[0].Count; j++)
                        {
                            if (pass[0][j] == subs[0])
                            {
                                if (pass[1][j] == subs[1])
                                {
                                    alreadyExist = true;
                                    userBalance = pass[2][j];
                                }
                            }
                        }
                        if (alreadyExist == false)
                        {
                            Console.WriteLine("Check Username/Password<LON>");
                            msg = Encoding.ASCII.GetBytes("Check Username/Password<LON>");
                            handler.Send(msg);
                        }
                        else
                        {
                            Console.WriteLine("Logged in successfully<LOG>");
                            msg = Encoding.ASCII.GetBytes(userBalance + "<LOG>");
                            handler.Send(msg);
                        }
                    }
                    else if(data.IndexOf("<DEP>") > -1)
                    {
                        //data="user;toDeposit<DEP>

                        string beforeString = "";
                        string afterString = "";

                        int beforeInt = 0;
                        int afterInt = 0;

                        string[] subs = data.Split(';');

                        //subs[0] = user
                        //subs[1] = toDeposit<DEP>

                        int index = subs[1].IndexOf("<");
                        if (index > 0)
                            subs[1] = subs[1].Substring(0, index);

                        //subs[0] = username
                        //subs[1] = toDeposit
                        
                        List<string>[] pass = db_users.Select();
                        
                        for(int i = 0; i < pass[0].Count; i++)
                        {
                            if(pass[0][i] == subs[0])
                            {
                                beforeString = pass[2][i];
                            }
                        }

                        beforeInt = int.Parse(beforeString);

                        afterInt = beforeInt + int.Parse(subs[1]);
                        afterString = afterInt.ToString();

                        db_users.UpdateBalance(afterString, subs[0]);

                        msg = Encoding.ASCII.GetBytes(afterString + "<DEP>");
                        handler.Send(msg);
                    }
                    else if(data.IndexOf("<WTD>") > -1)
                    {
                        //data="user;toWithdraw<WTD>

                        string beforeString = "";
                        string afterString = "";

                        int beforeInt = 0;
                        int afterInt = 0;

                        string[] subs = data.Split(';');

                        //subs[0] = user
                        //subs[1] = toWithdraw<WTD>

                        int index = subs[1].IndexOf("<");
                        if (index > 0)
                            subs[1] = subs[1].Substring(0, index);

                        //subs[0] = username
                        //subs[1] = toWithdraw
                        
                        List<string>[] pass = db_users.Select();
                        for (int i = 0; i < pass[0].Count; i++)
                        {
                            if (pass[0][i] == subs[0])
                            {
                                beforeString = pass[2][i];
                            }
                        }

                        beforeInt = int.Parse(beforeString);

                        if ((beforeInt - int.Parse(subs[1])) >= 0)
                        {
                            afterInt = beforeInt - int.Parse(subs[1]);
                            afterString = afterInt.ToString();

                            db_users.UpdateBalance(afterString, subs[0]);

                            msg = Encoding.ASCII.GetBytes(afterString + "<WTD>");
                            handler.Send(msg);
                        }
                        else
                        {
                            msg = Encoding.ASCII.GetBytes("You dont have enough money<WTN>");
                            handler.Send(msg);
                        }
                    }
                    else if(data.IndexOf("<TRS>") > -1)
                    {
                        //data="user;usertoTransfer;moneyToTransfer<TRS>

                        bool userToTransferExist = false;


                        string userToTransferBalanceStr = "";
                        string beforeString = "";
                        string afterString = "";

                        int userToTransferBalanceInt = 0;
                        int beforeInt = 0;
                        int afterInt = 0;

                        string[] subs = data.Split(';');

                        //subs[0] = user
                        //subs[1] = userToTransfer
                        //subs[2] = moneyTotransfer<TRS>

                        int index = subs[2].IndexOf("<");
                        if (index > 0)
                            subs[2] = subs[2].Substring(0, index);

                        //subs[0] = user
                        //subs[1] = userToTransfer
                        //subs[2] = moneyToTransfer


                        List<string>[] pass = db_users.Select();
                        for (int i = 0; i < pass[0].Count; i++)
                        {
                            if (pass[0][i] == subs[0])
                            {
                                beforeString = pass[2][i];
                            }
                            if(pass[0][i] == subs[1])
                            {
                                userToTransferExist = true;
                                userToTransferBalanceStr = pass[2][i];
                            }
                        }

                        beforeInt = int.Parse(beforeString);

                        if ((beforeInt - int.Parse(subs[2])) >= 0)
                        {
                            if(userToTransferExist == true)
                            {
                                afterInt = beforeInt - int.Parse(subs[2]);
                                afterString = afterInt.ToString();

                                userToTransferBalanceInt = int.Parse(userToTransferBalanceStr);
                                userToTransferBalanceInt = userToTransferBalanceInt + int.Parse(subs[2]);
                                userToTransferBalanceStr = userToTransferBalanceInt.ToString();

                                db_users.UpdateBalance(userToTransferBalanceStr, subs[1]);
                                db_users.UpdateBalance(afterString, subs[0]);

                                msg = Encoding.ASCII.GetBytes(afterString + "<TRS>");
                                handler.Send(msg);
                            }
                            else
                            {
                                msg = Encoding.ASCII.GetBytes("This user don't exist<TRN>");
                                handler.Send(msg);
                            }

                        }
                        else
                        {
                            msg = Encoding.ASCII.GetBytes("You don't have enough money<TRQ>");
                            handler.Send(msg);
                        }
                    }
                }

                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }

        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }

        Console.WriteLine("\nPress ENTER to continue...");
        Console.Read();

    }


    public static int Main(String[] args)
    {

        StartListening();
        return 0;
    }
}