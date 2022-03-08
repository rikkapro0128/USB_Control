using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UsbLibrary;

namespace USB_Control
{
    public partial class Form1 : Form
    {
        byte[] readbuff = new byte[65];
        byte[] writebuff = new byte[65];
        public Form1()
        {
            InitializeComponent();
        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.usbHidPort2.VendorId = 0x04d8;
            this.usbHidPort2.ProductId = 0x0001;
            this.usbHidPort2.CheckDevicePresent();

            if (this.usbHidPort2.SpecifiedDevice != null)
            {
                this.usbHidPort2.SpecifiedDevice.SendData(writebuff);
            }
            textBox_VID.Text = usbHidPort1.VendorId.ToString("x4");
            textBox_PID.Text = usbHidPort1.ProductId.ToString("x4");
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            DialogResult answer = MessageBox.Show("Do you want to exit the program?", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (answer == DialogResult.Yes)
            {
                this.Close();
            }
        }

        private void ON_Click(object sender, EventArgs e)
        {
            writebuff[1] = 1;
            if (this.usbHidPort2.SpecifiedDevice != null)
                this.usbHidPort2.SpecifiedDevice.SendData(writebuff);
            else
            {
                MessageBox.Show("Device not found. Please reconnect USB device to use.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void OFF_Click(object sender, EventArgs e)
        {
            writebuff[1] = 0;
            if (this.usbHidPort2.SpecifiedDevice != null)
                this.usbHidPort2.SpecifiedDevice.SendData(writebuff);
            else
            {
                MessageBox.Show("Device not found. Please reconnect USB device to use.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void usbHidPort1_OnDataRecieved(object sender, DataRecievedEventArgs args)
        {
            if (InvokeRequired)
            {
                try
                {
                    Invoke(new DataRecievedEventHandler(usbHidPort1_OnDataRecieved), new object[] { sender, args });
                }
                catch { }

            }
            else
            {
                readbuff = args.data;
                toolStripStatusLabel_InforDevice.Text = "New Receieved Data";
                textBox_SW.Text = readbuff[1].ToString();
                if (readbuff[9] == 'O')
                {
                    //ovalShape1.BackColor = Color.Red;
                    pictureBox2.Visible = false;
                }
                else if (readbuff[9] == 'F')
                {
                    //ovalShape1.BackColor = Color.DarkGray;
                    pictureBox2.Visible = true;
                }
            }
        }

        private void usbHidPort1_OnDataSend(object sender, EventArgs e)
        {
            toolStripStatusLabel_InforDevice.Text = "Data sent";
        }

        private void usbHidPort1_OnDeviceArrived(object sender, EventArgs e)
        {
            toolStripStatusLabel_InforDevice.Text = "USB Connected";
        }

        private void usbHidPort1_OnDeviceRemoved(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new EventHandler(usbHidPort1_OnDeviceRemoved), new object[] { sender, e });

            }
            else
            {
                toolStripStatusLabel_InforDevice.Text = "USB Removed";
            }
        }

        private void usbHidPort1_OnSpecifiedDeviceArrived(object sender, EventArgs e)
        {
            toolStripStatusLabel_InforDevice.Text = "Device Detected";
            textBox_Status.Text = "Connected!";
            textBox_Status.BackColor = Color.Lime;
            textBox_SW.Text = "0";
            //ovalShape1.BackColor = Color.DarkGray;
            pictureBox2.Visible = true;
        }

        private void usbHidPort1_OnSpecifiedDeviceRemoved(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new EventHandler(usbHidPort1_OnSpecifiedDeviceRemoved), new object[] { sender, e });

            }
            else
            {
                toolStripStatusLabel_InforDevice.Text = "Device Disconnected";
                textBox_Status.Text = "Disconnected!!";
                textBox_Status.BackColor = Color.Red;
            }
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            usbHidPort2.RegisterHandle(Handle);
        }

        protected override void WndProc(ref Message m)
        {
            usbHidPort2.ParseMessages(ref m);
            base.WndProc(ref m);
        }
    }
}
