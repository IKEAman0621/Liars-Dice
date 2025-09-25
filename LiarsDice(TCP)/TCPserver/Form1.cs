using System.Net;
using System.Net.Sockets;
using System.Text;

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
        this.Text = "TCP server";
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
        myTextbox3.Height = 75;
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
        myLabel3.Text = "Message:";
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
    
    private void btn_click(object sender, EventArgs e)
    {
        myButton1.Text = "Listening";

        Task.Run(() =>
        {
            port = int.Parse(myTextbox2.Text);

            listener = new TcpListener(IPAddress.Any, port);
            listener.Start();

            while (true)
            {
                client = listener.AcceptTcpClient();

                byte[] inData = new byte[256];

                int byteSize = client.GetStream().Read(inData, 0, inData.Length);

                string message = Encoding.Unicode.GetString(inData, 0, byteSize);
                Invoke((Action)(() => myTextbox3.Text += message + Environment.NewLine));

                client.Close();
            }
        });
    } 
}
