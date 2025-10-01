using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace TCPserver;

public partial class Form1 : Form
{
    private Button myButton1;

    private TextBox myTextbox2;
    private TextBox myTextbox3;

    private Label myLabel2;
    private Label myLabel3;

    public Form1()
    {
        this.Text = "Table (server)";
        this.Width = 400;
        this.Height = 600;

        myButton1 = new Button();
        myButton1.Text = "start";
        myButton1.Left = 15;
        myButton1.Top = 415;
        myButton1.Height = 50;
        myButton1.Width = 350;
        //myButton1.BackColor = Color.Red;
        myButton1.Click += btn_click;

        myTextbox2 = new TextBox();
        myTextbox2.Text = "5678";
        myTextbox2.Left = 190;
        myTextbox2.Top = 70;
        myTextbox2.Height = 50;
        myTextbox2.Width = 175;

        myTextbox3 = new TextBox();
        myTextbox3.Text = "";
        myTextbox3.Left = 15;
        myTextbox3.Top = 150;
        myTextbox3.Height = 250;
        myTextbox3.Width = 350;

        myTextbox3.AcceptsReturn = true;
        myTextbox3.AcceptsTab = true;
        myTextbox3.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
        myTextbox3.Multiline = true;


        myLabel2 = new Label();
        myLabel2.Text = "Port:";
        myLabel2.Left = 15;
        myLabel2.Top = 70;

        myLabel3 = new Label();
        myLabel3.Text = "Server log:";
        myLabel3.Left = 25;
        myLabel3.Top = 125;
        myLabel3.Height = 50;


        this.Controls.Add(myButton1);

        this.Controls.Add(myTextbox2);
        this.Controls.Add(myTextbox3);

        this.Controls.Add(myLabel2);
        this.Controls.Add(myLabel3);
    }

    private TcpListener listener;
    private TcpClient client;
    private int port;
    private bool serverRunning = false;

    private void Log(string message)
    {
        if (myTextbox3.InvokeRequired)
        {
            myTextbox3.Invoke(new Action(() => myTextbox3.AppendText(message + Environment.NewLine)));
        }
        else
        {
            myTextbox3.AppendText(message + Environment.NewLine);
        }
    }

    private void btn_click(object sender, EventArgs e)
    {
        if (!serverRunning)
        {
            Log("Server started.");
            serverRunning = true;
            myButton1.Text = "Listening...";
            Thread listenerThread = new Thread(StartServer);
            listenerThread.IsBackground = true;
            listenerThread.Start();
        }
        else
        {
            StopServer();
        }
    }

    private void StartServer()
    {
        port = int.Parse(myTextbox2.Text);
        listener = new TcpListener(IPAddress.Any, port);
        listener.Start();

        while (serverRunning)
        {
            try
            {
                client = listener.AcceptTcpClient();
                Log("Client connected.");

                Thread clientThread = new Thread(() => HandleClient(client));
                clientThread.IsBackground = true;
                clientThread.Start();
            }
            catch (SocketException)
            {
                break; // Listener was stopped
            }
        }
    }

    private void HandleClient(TcpClient client)
    {
        NetworkStream stream = client.GetStream();
        byte[] buffer = new byte[1024];

        try
        {
            while (client.Connected)
            {
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                if (bytesRead == 0) break; // Client disconnected

                string message = Encoding.Unicode.GetString(buffer, 0, bytesRead);
                Log("Client: " + message);

                string respone = ProcessRequest(message);

                byte[] response = Encoding.Unicode.GetBytes("Server received: " + message);
                stream.Write(response, 0, response.Length);
            }
        }
        catch (Exception ex)
        {
            Log("Client error: " + ex.Message);
        }
        finally
        {
            client.Close();
            Log("Client disconnected.");
        }
    }

    private void StopServer()
    {
        serverRunning = false;
        listener?.Stop();
        myButton1.Text = "start";
        Log("Server stopped.");
    }

    private string ProcessRequest(string message)
    {
        Random rand = new Random();
        string[] request = message.Split('(');

        switch (request[0])
        {
            case "GETDICE":
                // Return dice values
                int[] diceValues = new int[5];

                for (int i = 0; i < 5; i++)
                {
                    diceValues[i] = rand.Next(1, 7);
                }

                string.Join(", ", diceValues);
            return $"DICE ({string.Join(", ", diceValues)})";

                
            case "GETPLAYERCOUNT":
                // Return player count
                return "PLAYERCOUNT(4)"; // Example response


            case "CALL":
                // Handle call action
                return "CALLACK (Success)"; // Example response


            case "BID":
                // Handle bid action
                return "BIDACK (Success)"; // Example response


            default:
                return "ERROR (Unknown command)";
        }
    }
}
