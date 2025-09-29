using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Drawing;
using System.Drawing.Text;
using System.Media;
using Microsoft.VisualBasic;

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
        int playerCount = PlayerCount();
        Dices = GetServerDice();

        this.Height = 800;
        this.Width = 1200;
        this.Text = "Cups and Dice (client)";

        IP = new TextBox();
        IP.Text = "127.0.0.1";

        Port = new TextBox();
        Port.Text = "5678";

        Name = new TextBox();
        Name.Text = "Player1";

        Connect = new Button();
        Connect.Text = "Connect to server";
        Connect.Click += ConnectToServer;


        bidButton = new Button();
        bidButton.Text = "bid";
        bidButton.Click += bidButton_click;

        callButton = new Button();
        callButton.Text = "call";
        callButton.Click += callButton_click;


        numberOfDice = new NumericUpDown();
        numberOfDice.Minimum = 1;
        numberOfDice.Maximum = playerCount * Dices.Length;
        numberOfDice.DecimalPlaces = 0;
        numberOfDice.Value = 1;
        

        faceValue = new ComboBox();
        faceValue.Items.Add("1");
        faceValue.Items.Add("2");
        faceValue.Items.Add("3");
        faceValue.Items.Add("4");
        faceValue.Items.Add("5");
        faceValue.Items.Add("6");
        faceValue.SelectedIndex = 0;

        
        this.Controls.Add(IP);
        this.Controls.Add(Port);
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

                byte[] outData = Encoding.Unicode.GetBytes(Name.Text);
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
                MessageBox.Show("Error: " + ex.Message);
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
                        MessageBox.Show("Received: " + message);
                    }));
                }
            }
        }
        catch (Exception ex)
        {
            Invoke(new Action(() =>
            {
                MessageBox.Show("Disconnected: " + ex.Message);
                Disconnect();
            }));
        }
    }

    private void Disconnect()
    {
        stream?.Close();
        client?.Close();
    }



    private void Dice_Paint(object sender, PaintEventArgs e, int DiceValue)
    {
        //Fix this later
        e.Graphics.DrawImage(diceImages[DiceValue], new Point(0, 0));
    }

    private int[] GetServerDice()
    {
        //Fix this later
        return new int[] { 1, 2, 3, 4, 5 };
    }

    private int PlayerCount()
    {
        //Fix this later
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
            string message = $"BID {numberOfDice.Value} {faceValue.SelectedItem} {Name.Text}";
            byte[] outData = Encoding.Unicode.GetBytes(message);
            stream.Write(outData, 0, outData.Length);
        }
    }

    private void callButton_click(object sender, EventArgs e)
    {
        //call button clicked

        if (client?.Connected == true)
        {
            string message = $"CALL {Name.Text}";
            byte[] outData = Encoding.Unicode.GetBytes(message);
            stream.Write(outData, 0, outData.Length);
        }
    }
}






