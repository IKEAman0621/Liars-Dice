using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TCPclient;


public partial class Form1 : Form
{
    TextBox textbox1;
    TextBox textbox2;
    TextBox textbox3;

    Label label1;
    Label label2;

    Button button1;

    public Form1()
    {
        this.Height = 600;
        this.Width = 400;
        this.Text = "Cups and Dice (client)";

        textbox1 = new TextBox();
        textbox1.Text = "127.0.0.1";
        textbox1.Top = 15;
        textbox1.Left = 215;
        textbox1.Width = 150;

        textbox2 = new TextBox();
        textbox2.Text = "5678";
        textbox2.Top = 65;
        textbox2.Left = 215;
        textbox2.Width = 150;

        textbox3 = new TextBox();
        textbox3.Top = 150;
        textbox3.Height = 80;
        textbox3.Left = 15;
        textbox3.Width = 350;
        textbox3.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
        textbox3.Multiline = true;
        textbox3.AcceptsReturn = true;
        textbox3.AcceptsTab = true;


        label1 = new Label();
        label1.Text = "IP-address";
        label1.Top = 15;
        label1.Left = 15;

        label2 = new Label();
        label2.Text = "Port";
        label2.Top = 65;
        label2.Left = 15;


        button1 = new Button();
        button1.Text = "send";
        button1.Top = 470;
        button1.Height = 50;
        button1.Left = 15;
        button1.Width = 350;
        button1.Click += btn_click;


        this.Controls.Add(textbox1);
        this.Controls.Add(textbox2);
        this.Controls.Add(textbox3);

        this.Controls.Add(label1);
        this.Controls.Add(label2);
        
        this.Controls.Add(button1);
    }

    private TcpClient client;
    private int port;

    private void btn_click(object sender, EventArgs e)
    {
        port = int.Parse(textbox2.Text);

        IPAddress address = IPAddress.Parse(textbox1.Text);
        client = new TcpClient();
        client.NoDelay = true;
        client.Connect(address, port);

        if (client.Connected)
        {
            byte[] outData = Encoding.Unicode.GetBytes(textbox3.Text);
            client.GetStream().Write(outData, 0, outData.Length);
            client.Close();
        }
    }
}






