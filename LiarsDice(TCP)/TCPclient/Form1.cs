using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TCPclient;


public partial class Form1 : Form
{
    Label[] dices = new Label[5];

    Button bidButton;
    Button callButton;

    TextBox numberOfDice;
    ComboBox faceValue;

    public Form1()
    {
        this.Height = 800;
        this.Width = 1200;
        this.Text = "Cups and Dice (client)";

        for (int i  = 0 ; i < dices.Length - 1; i++)
        {
            dices[i] = new Label();
            dices[i].Text = "Die";
            dices[i].Height = 100;
            dices[i].Width = 100;
            dices[i].BorderStyle = BorderStyle.FixedSingle;

            this.Controls.Add(dices[i]);
        }

        bidButton = new Button();
        bidButton.Text = "bid";
        bidButton.Click += bidButton_click;


        callButton = new Button();
        callButton.Text = "call";
        callButton.Click += callButton_click;


        numberOfDice = new TextBox();
        numberOfDice.Text = "";


        faceValue = new ComboBox();
        faceValue.Items.Add("1");
        faceValue.Items.Add("2");
        faceValue.Items.Add("3");
        faceValue.Items.Add("4");
        faceValue.Items.Add("5");
        faceValue.Items.Add("6");
        faceValue.SelectedIndex = 0;


        this.Controls.Add(bidButton);
        this.Controls.Add(callButton);
        this.Controls.Add(numberOfDice);
        this.Controls.Add(faceValue);
    }

    private TcpClient client;
    private int port;

    private void Form1_load(object sender, EventArgs e)
    {
        checkBidButton();
    }

    private void checkBidButton()
    {
        bidButton.Enabled = false;
    }

    private void bidButton_click(object sender, EventArgs e)
    {

    }

    private void callButton_click(object sender, EventArgs e)
    {
        //call button clicked
    }
}






