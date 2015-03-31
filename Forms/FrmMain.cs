using System;
using System.Collections.Generic;
using System.Windows.Forms;
//using UsbHid;
//using UsbHid.USB.Classes.Messaging;
using System.ComponentModel;
using System.IO.Ports;

using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Threading;
using System.IO;
using System.Timers;
using System.Reflection;
using PCComm;

namespace USB2550HidTest.Forms
{


    public partial class FrmMain : Form
    {
        public string ACTUATOR_EXCEL_FILENAME = "BooksLog.xlsx"; 
        object WithEvents;
        System.IO.Ports.SerialPort SPort = new System.IO.Ports.SerialPort();


    //    public UsbHidDevice Device;
        int lastlength=0,i=0, sent=0;
        string rack;

        public FrmMain()
        {
            InitializeComponent();
        }
        
        private void FrmMainLoad(object sender, EventArgs e)
        {
            // For Each sp As String In My.Computer.Ports.SerialPortNames
           // portsel.Items.Add(sp)


            string[] ports = SerialPort.GetPortNames();
            foreach (string port in ports)
            {
                cmbport.Items.Add(port);
            }

           
            Timer1.Start();

            //Device = new UsbHidDevice(0x04D8, 0x01FF);
            //Device.OnConnected += DeviceOnConnected;
            //Device.OnDisConnected += DeviceOnDisConnected;
            //Device.DataReceived += DeviceDataReceived;
            //Device.Connect();
        }


        private void DeviceDataReceived(byte[] data)
        {
            AppendText(ByteArrayToString(data));
        }

        private void AppendText(string p)
        {
            ThreadSafe(() => textBox1.AppendText(p + Environment.NewLine));
        }

        private void DeviceOnDisConnected()
        {
            //ThreadSafe(() => checkBox1.Enabled = false);

        }

        private void DeviceOnConnected()
        {
          // ThreadSafe(() => checkBox1.Enabled = true);
        }

        private void ThreadSafe(MethodInvoker method)
        {
            if (InvokeRequired)
                Invoke(method);
            else
                method();
        }


        private static string ByteArrayToString(ICollection<byte> input)
        {
            var result = string.Empty;

            if (input != null && input.Count > 0)
            {
                var isFirst = true;
                foreach (var b in input)
                {
                    result += isFirst ? string.Empty : ",";
                    result += b.ToString("X2");
                    isFirst = false;
                }
            }
            return result;
        }

        private void Button1Click(object sender, EventArgs e)
        {
            //if (!Device.IsDeviceConnected)
            //{
            //    MessageBox.Show("Device not Connected");
            //    return;
            //}
  
            //var command = new CommandMessage(0x86, new byte[] { 0, 1, 2, 3, 4, 5, 6, 7 });
            //Device.SendMessage(command);
        }


        private void DynamicAutoShapeChange(string name)
        {
            try
            {
                excel_helper.excel_init(System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) +
                                         @"\" + ACTUATOR_EXCEL_FILENAME);

                string result = excel_helper.Find_ActValue_Frm_Date(name, 1);
                string bookname = excel_helper.excel_getValue("C"+result);
                MessageBox.Show(bookname);
                //string barcode = excel_helper.excel_getValue("A3");
                //string bookname = excel_helper.excel_getValue("C3");
                //MessageBox.Show(barcode,bookname);
                //MessageBox.Show(bookname);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: SND ACTUATOR VALUE DELTADIS: " + ex.ToString());
            }
        }


        private void Timer1_Tick(object sender, EventArgs e)
        {
            if (textBox1.Text.Length >= lastlength+1)
            {
                 // Write here the actual code if compare matched
                 lastlength= textBox1.Text.Length;
            }
            else
            {
                i++;
                if(i==4)
                {
                    i = 0;
                    lastlength = 0;
                   string chattextbox = textBox1.Text;
                   chattextbox = chattextbox.Replace("\r\n", string.Empty);
                    textBox1.Text = String.Empty;
                    txtdisp.Text = String.Empty;
                    txtdisp.Text = chattextbox;
                    if (chattextbox.Length > 1)
                    {
                     //   DynamicAutoShapeChange(chattextbox);
                        try
                        {
                            excel_helper.excel_init(System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) +
                                                     @"\" + ACTUATOR_EXCEL_FILENAME);

                            string result = excel_helper.Find_ActValue_Frm_Date(chattextbox, 1);
                            string bookname = excel_helper.excel_getValue("C" + result);
                            rack = excel_helper.excel_getValue("E" + result);
                            lbl1.Text = bookname;
                            lbl2.Text = rack;
                            //MessageBox.Show("Rack No."+rack);
                            //if(SPort.IsOpen == true)
                            //{
                            //    SPort.Write(rack+'#');
                            //}
                            //else
                            //{
                            //    MessageBox.Show("Error: Port Closed");
                            //    SPort.Close();
                            //    btnclose.Enabled = false;
                            //    btnoopen.Enabled = true;
                            //}
                            
                        }
                        catch (NullReferenceException ex)
                        {
                            lbl1.Text = "Not Found";
                            lbl2.Text = "---";
                            DialogResult dialogResult = MessageBox.Show("Did not find any match" + "\n" + " Would you like to add?", "Error", MessageBoxButtons.YesNo);
                            if (dialogResult == DialogResult.Yes)
                            {
                                Program.Form1.Show();
                            }
                        }
                    }

                }
            }

        }

    
        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnoopen_Click(object sender, EventArgs e)
        {
            if (SPort.IsOpen == false)
            {
                try
                {
                    SPort.PortName = (string)cmbport.SelectedItem;
                    SPort.Open();
                    btnclose.Enabled = true;
                    btnoopen.Enabled = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error:" + ex);
                }
            }
            else 
            {
                MessageBox.Show("Port Already pen");
            }
        }

        private void btnclose_Click(object sender, EventArgs e)
        {
            if (SPort.IsOpen == true)
            {
                try
                {
                    SPort.Close();
                    btnoopen.Enabled = true;
                    btnclose.Enabled = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error:" + ex);
                }
            }
        }

        private void btnadd_Click_1(object sender, EventArgs e)
        {

            Program.Form1.Show();
        }

        private void btngo_Click(object sender, EventArgs e)
        {
            if (SPort.IsOpen == true)
            {
                SPort.Write("R");
                Thread.Sleep(20);
                SPort.Write("a");
                Thread.Sleep(20);
                SPort.Write("c");
                Thread.Sleep(20);
                SPort.Write("k");
                Thread.Sleep(20);
                if(rack=="Rack1")
                {
                    SPort.Write("1");
                }
                if (rack == "Rack2")
                {
                    SPort.Write("2");
                }
                if (rack == "Rack3")
                {
                    SPort.Write("3");
                }
                Thread.Sleep(20);
                SPort.Write("&");
               // Thread.Sleep(1500);

                //SPort.Write("R");
                //Thread.Sleep(20);
                //SPort.Write("a");
                //Thread.Sleep(20);
                //SPort.Write("c");
                //Thread.Sleep(20);
                //SPort.Write("k");
                //Thread.Sleep(20);
                //SPort.Write("4");
                //Thread.Sleep(20);
                //SPort.Write("&");
                //Thread.Sleep(20);

            }
            else
            {
                MessageBox.Show("Error: Port Closed");
                SPort.Close();
                btnclose.Enabled = false;
                btnoopen.Enabled = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SPort.Write("o");
            Thread.Sleep(20);
            SPort.Write("p");
            Thread.Sleep(20);
            SPort.Write("e");
            Thread.Sleep(20);
            SPort.Write("&");
            Thread.Sleep(20);
        }

        private void btnclohand_Click(object sender, EventArgs e)
        {
            SPort.Write("c");
            Thread.Sleep(20);
            SPort.Write("l");
            Thread.Sleep(20);
            SPort.Write("o");
            Thread.Sleep(20);
            SPort.Write("&");
            Thread.Sleep(20);
        }

        private void btnstop_Click(object sender, EventArgs e)
        {
            SPort.Write("s");
            Thread.Sleep(20);
            SPort.Write("t");
            Thread.Sleep(20);
            SPort.Write("o");
            Thread.Sleep(20);
            SPort.Write("&");
            Thread.Sleep(20);
        }

      
    }
}
