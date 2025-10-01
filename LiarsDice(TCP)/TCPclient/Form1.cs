using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Drawing;
using System.Drawing.Text;
using System.Media;
using Microsoft.VisualBasic;
using System.Threading.Tasks;

namespace TCPclient;


public partial class Form1 : Form
{


    int[] Dices;

    TextBox IP;
    TextBox Port;
    TextBox Name;
    Button Connect;


    Button bidButton;
    Button callButton;


    NumericUpDown numberOfDice;
    ComboBox faceValue;
    
    private Bitmap[] diceImages = new Bitmap[6];

    public Form1()
    {
        this.Height = 800;
        this.Width = 1200;
        this.Text = "Cups and Dice (client)";

        IP = new TextBox();
        IP.Text = "127.0.0.1";
        IP.Left = 985;
        IP.Top = 70;
        IP.Height = 50;
        IP.Width = 175; 

        Port = new TextBox();
        Port.Text = "5678";
        Port.Left = 985;
        Port.Top = 125;
        Port.Height = 50;
        Port.Width = 175; 

        Name = new TextBox();
        Name.Text = "Player1";
        Name.Left = 985;
        Name.Top = 15;
        Name.Height = 50;
        Name.Width = 175; 

        Connect = new Button();
        Connect.Text = "Connect to server";
        Connect.Click += ConnectToServer;
        Connect.Left = 15;
        Connect.Top = 15;
        Connect.Height = 50;
        Connect.Width = 350;


        bidButton = new Button();
        bidButton.Text = "bid";
        bidButton.Click += bidButton_click;
        bidButton.Left = 15;
        bidButton.Top = 475;
        bidButton.Height = 50;
        bidButton.Width = 175;

        callButton = new Button();
        callButton.Text = "call";
        callButton.Click += callButton_click;
        callButton.Left = 15;
        callButton.Top = 545;
        callButton.Height = 50;
        callButton.Width = 175;


        numberOfDice = new NumericUpDown();
        numberOfDice.Minimum = 1;
        numberOfDice.Maximum = 5;
        numberOfDice.DecimalPlaces = 0;
        numberOfDice.Value = 1;
        numberOfDice.Left = 400;
        numberOfDice.Top = 475;
        numberOfDice.Height = 50;
        numberOfDice.Width = 175;
        

        faceValue = new ComboBox();
        faceValue.Items.Add("1");
        faceValue.Items.Add("2");
        faceValue.Items.Add("3");
        faceValue.Items.Add("4");
        faceValue.Items.Add("5");
        faceValue.Items.Add("6");
        faceValue.SelectedIndex = 0;
        faceValue.Left = 200;
        faceValue.Top = 475;
        faceValue.Height = 50;
        faceValue.Width = 175;
        
        this.Controls.Add(IP);
        this.Controls.Add(Port);
        this.Controls.Add(Name);
        this.Controls.Add(Connect);

        this.Controls.Add(bidButton);
        this.Controls.Add(callButton);
        this.Controls.Add(numberOfDice);
        this.Controls.Add(faceValue);
        
    }

    private TcpListener listener;
    private TcpClient client;
    private NetworkStream stream;
    private Thread receiveThread;

    private String ReceiveData;

    private int port;
    private string IPaddress;

    private void Form1_load(object sender, EventArgs e)
    {
        checkBidButton();

    }



    private void ConnectToServer(object sender, EventArgs e)
    {
        if (Connect.Text == "Connect to server")
        {
            try
            {

                Connect.Text = "Connecting...";

                IPaddress = IP.Text;
                port = int.Parse(Port.Text);

                client = new TcpClient();
                client.Connect(IPaddress, port);
                stream = client.GetStream();

                byte[] outData = Encoding.Unicode.GetBytes($"JOIN(n{Name.Text})");
                stream.Write(outData, 0, outData.Length);

                receiveThread = new Thread(ReceiveDataLoop)
                {
                    IsBackground = true
                };
                receiveThread.Start();

                Connect.Text = "Disconnect";
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Error: " + ex.Message);
                Connect.Text = "Connect to server";
                return;
            }
        }


        else if (Connect.Text == "Disconnect")
        {
            Disconnect();
            Connect.Text = "Connect to server";
        }
    }

    private void ReceiveDataLoop()
    {
        try
        {
            byte[] buffer = new byte[1024];


            while (client != null && client.Connected)
            {
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                if (bytesRead > 0)
                {
                    string message = Encoding.Unicode.GetString(buffer, 0, bytesRead);
                    Invoke(new Action(() =>
                    {
                        //MessageBox.Show("Received: " + message);
                    }));
                }
            }
        }
        catch (Exception ex)
        {
            Invoke(new Action(() =>
            {
                //MessageBox.Show("Disconnected: " + ex.Message);
                Disconnect();
            }));
        }
    }

    private void Disconnect()
    {
        stream?.Close();
        client?.Close();
    }

    private string SendRequest(string request)
    {
        if (client == null || !client.Connected)
            throw new Exception("Not connected to server");

        NetworkStream stream = client.GetStream();

        // Send request
        byte[] outData = Encoding.Unicode.GetBytes(request);
        stream.Write(outData, 0, outData.Length);

        // Wait for response
        byte[] buffer = new byte[1024];
        int bytesRead = stream.Read(buffer, 0, buffer.Length); // BLOCKS until data arrives
        string response = Encoding.Unicode.GetString(buffer, 0, bytesRead).Trim();

        return response;
    }

    private void Dice_Paint(object sender, PaintEventArgs e, int DiceValue)
    {
        //Fix this later
        e.Graphics.DrawImage(diceImages[DiceValue], new Point(0, 0));
    }

    private int[] GetServerDice()
    {
        string response;
        string[] subResponse;

        if (client?.Connected == true)
        {
            response = SendRequest($"GETDICE(n{Name.Text})");
            // Process the response to extract dice values

            /*string message = $"GETDICE(n{Name.Text})";
            byte[] outData = Encoding.Unicode.GetBytes(message);
            stream.Write(outData, 0, outData.Length);*/

            subResponse = response.Split('(');
            string DataType = subResponse[0];
            string Data = subResponse[1].TrimEnd(')');

            string[] DiceValues = Data.Split(',');
            int[] numbers = Array.ConvertAll(DiceValues, int.Parse);

            return numbers;
        }

        

        return new int[] { 1, 2, 3, 4, 5 };
    }

    private int PlayerCount()
    {
        string response;
        string[] subResponse;



        if (client?.Connected == true)
        {
            response = SendRequest($"GETPLAYERCOUNT(n{Name.Text})");
            // Process the response to extract player count

            /*string message = $"GETPLAYERCOUNT(n{Name.Text})";
            byte[] outData = Encoding.Unicode.GetBytes(message);
            stream.Write(outData, 0, outData.Length);*/

            subResponse = response.Split('(');
            string DataType = subResponse[0];
            string Data = subResponse[1].TrimEnd(')');

            string PlayerCount = Data;

            return int.Parse(PlayerCount);
        }


        return 2;
    }

    private void checkBidButton()
    {
        //Fix this later
        bidButton.Enabled = false;
    }

    private void bidButton_click(object sender, EventArgs e)
    {
        //bid button clicked

        if (client?.Connected == true)
        {
            string message = $"BID(c{numberOfDice.Value} v{faceValue.SelectedItem} n{Name.Text})";
            byte[] outData = Encoding.Unicode.GetBytes(message);
            stream.Write(outData, 0, outData.Length);
        }
    }

    private void callButton_click(object sender, EventArgs e)
    {
        //call button clicked

        if (client?.Connected == true)
        {
            string message = $"CALL(n{Name.Text})";
            byte[] outData = Encoding.Unicode.GetBytes(message);
            stream.Write(outData, 0, outData.Length);
        }
    }
}






