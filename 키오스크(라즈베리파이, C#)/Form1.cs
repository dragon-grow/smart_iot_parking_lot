using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace piform
{

    public partial class Form1 : Form
    {
        public string carNum = "";
        public MqttClient client;
        public string clientId;
        public string search = "";
        public parkingLot []carList = new parkingLot[3];


        public Form1()
        {
            InitializeComponent();
            string BrokerAddress = "192.168.0.2";       //브로커 아이피 설정
            carList[0] = new parkingLot();      
            carList[1] = new parkingLot();
            carList[2] = new parkingLot();

            client = new MqttClient(BrokerAddress);

            // register a callback-function (we have to implement, see below) which is called by the library when a message was received
            client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;

            // use a unique id as client id, each time we start the application
            clientId = Guid.NewGuid().ToString();

            client.Connect(clientId);
            label4.Text = client.IsConnected.ToString();
            

            // subscribe to the topic with QoS 2
            client.Subscribe(new string[] { "parkinglot/1/num" }, new byte[] { 2 });   // we need arrays as parameters because we can subscribe to different topics with one call
            client.Subscribe(new string[] { "parkinglot/2/num" }, new byte[] { 2 });   // we need arrays as parameters because we can subscribe to different topics with one call
            client.Subscribe(new string[] { "parkinglot/3/num" }, new byte[] { 2 });   // we need arrays as parameters because we can subscribe to different topics with one call
            client.Subscribe(new string[] { "parkinglot/1/status" }, new byte[] { 2 });   // we need arrays as parameters because we can subscribe to different topics with one call
            client.Subscribe(new string[] { "parkinglot/2/status" }, new byte[] { 2 });   // we need arrays as parameters because we can subscribe to different topics with one call
            client.Subscribe(new string[] { "parkinglot/3/status" }, new byte[] { 2 });   // we need arrays as parameters because we can subscribe to different topics with one call
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //carNum = "";
            //textBox1.Text = carNum;
            bool find = false;
            if(textBox1.Text.Length < 4)
            {
                MessageBox.Show("Please enter a four-digit number.");
                return;
            }
            for (int i=0; i<3; i++)
            {
                if((carList[i].carNumber.Contains(textBox1.Text)))
                {
                    find = true;
                    search = carNum;
                    carNum = "";
                    textBox1.Text = carNum;
                    Form2 newForm = new Form2(this);
                    newForm.Location = new Point(this.Location.X, this.Location.Y);
                    newForm.ShowDialog();
                    break;
                }
            }
            if (!find)
            {
                carNum = "";
                textBox1.Text = carNum;
                MessageBox.Show("No matching vehicle found....");
            }
        }

        private void button_1_Click(object sender, EventArgs e)
        {
            if (carNum.Length >= 4)
            {
                carNum = "";
                textBox1.Text = carNum;
                MessageBox.Show("Only 4 digits can be inquired.");
                return;
            }
            carNum += "1";
            textBox1.Text = carNum;
        }

        private void button_2_Click(object sender, EventArgs e)
        {
            if (carNum.Length >= 4)
            {
                carNum = "";
                textBox1.Text = carNum;
                MessageBox.Show("Only 4 digits can be inquired.");
                return;
            }
            carNum += "2";
            textBox1.Text = carNum;
        }

        private void button_3_Click(object sender, EventArgs e)
        {
            if (carNum.Length >= 4)
            {
                carNum = "";
                textBox1.Text = carNum;
                MessageBox.Show("Only 4 digits can be inquired.");
                return;
            }
            carNum += "3";
            textBox1.Text = carNum;
        }

        private void button_4_Click(object sender, EventArgs e)
        {
            if (carNum.Length >= 4)
            {
                carNum = "";
                textBox1.Text = carNum;
                MessageBox.Show("Only 4 digits can be inquired.");
                return;
            }
            carNum += "4";
            textBox1.Text = carNum;
        }

        private void button_5_Click(object sender, EventArgs e)
        {
            if (carNum.Length >= 4)
            {
                carNum = "";
                textBox1.Text = carNum;
                MessageBox.Show("Only 4 digits can be inquired.");
                return;
            }
            carNum += "5";
            textBox1.Text = carNum;
        }

        private void button_6_Click(object sender, EventArgs e)
        {
            if (carNum.Length >= 4)
            {
                carNum = "";
                textBox1.Text = carNum;
                MessageBox.Show("Only 4 digits can be inquired.");
                return;
            }
            carNum += "6";
            textBox1.Text = carNum;
        }

        private void button_7_Click(object sender, EventArgs e)
        {
            if (carNum.Length >= 4)
            {
                carNum = "";
                textBox1.Text = carNum;
                MessageBox.Show("Only 4 digits can be inquired.");
                return;
            }
            carNum += "7";
            textBox1.Text = carNum;
        }

        private void button_8_Click(object sender, EventArgs e)
        {
            if (carNum.Length >= 4)
            {
                carNum = "";
                textBox1.Text = carNum;
                MessageBox.Show("Only 4 digits can be inquired.");
                return;
            }
            carNum += "8";
            textBox1.Text = carNum;
        }

        private void button_9_Click(object sender, EventArgs e)
        {
            if (carNum.Length >= 4)
            {
                carNum = "";
                textBox1.Text = carNum;
                MessageBox.Show("Only 4 digits can be inquired.");
                return;
            }
            carNum += "9";
            textBox1.Text = carNum;
        }

        private void button_0_Click(object sender, EventArgs e)
        {
            if (carNum.Length >= 4)
            {
                carNum = "";
                textBox1.Text = carNum;
                MessageBox.Show("Only 4 digits can be inquired.");
                return;
            }
            carNum += "0";
            textBox1.Text = carNum;
        }

        private void button_B_Click(object sender, EventArgs e)
        {
            int lastNumIndex = carNum.Length;
            if (lastNumIndex == 0)
                return;
            carNum = carNum.Substring(0, lastNumIndex - 1);
            textBox1.Text = carNum;
        }

        private void button_C_Click(object sender, EventArgs e)
        {
            carNum = "";
            textBox1.Text = carNum;
        }
        /////////////////////////////////////////////////////////////////////////////////////////////
        protected override void OnClosed(EventArgs e)
        {
            client.Disconnect();
            base.OnClosed(e);
        }
        // this code runs when a message was received
        void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            string ReceivedMessage = Encoding.UTF8.GetString(e.Message);
            if (e.Topic == "parkinglot/1/num")
            {
                carList[0].carNumber = ReceivedMessage;
            }
           if (e.Topic == "parkinglot/2/num")
            {
                carList[1].carNumber = ReceivedMessage;

            }
            if (e.Topic == "parkinglot/3/num")
            {
                carList[2].carNumber = ReceivedMessage;
            }
            if (e.Topic == "parkinglot/1/status")
            {
                if(ReceivedMessage == "off")
                    carList[0] = new parkingLot();
            }
            if (e.Topic == "parkinglot/2/status")
            {
                if (ReceivedMessage == "off")
                    carList[1] = new parkingLot();
            }
            if (e.Topic == "parkinglot/3/status")
            {
                if (ReceivedMessage == "off")
                    carList[2] = new parkingLot();
            }
        }
    }
}
