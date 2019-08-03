using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SerialPortTestDemo
{
    public partial class Form1 : Form
    {
        private SerialPortComImplement serialPort;

        bool bAccpet = false;
        private Thread getRecevice;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            serialPort = new SerialPortComImplement("COM3", 9600, 8);
            if (serialPort.IsOpen)
            {
                serialPort.Close();
                serialPort.Open();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (serialPort.IsOpen)
            {
                try
                {
                    serialPort.Write(txtSend.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (btnRecv.Text == "接收")
            {
                if (serialPort.IsOpen)
                {
                    //使用委托以及多线程进行
                    bAccpet = true;
                    getRecevice = new Thread(new ThreadStart(testDelegate));
                    getRecevice.Start();
                    btnRecv.Text = "停止接收";
                }
            }
            else
            {
                bAccpet = false;
                try
                {   //停止主监听线程
                    if (null != getRecevice)
                    {
                        if (getRecevice.IsAlive)
                        {
                            if (!getRecevice.Join(100))
                            {
                                //关闭线程
                                getRecevice.Abort();
                            }
                        }
                        getRecevice = null;
                    }
                }
                catch { }

                btnRecv.Text = "接收";
            }
        }

        private void testDelegate()
        {
            reaction r = new reaction(fun);
            r();
        }

        delegate void DelegateAcceptData();
        void fun()
        {
            while (bAccpet)
            {
                AcceptData();
            }
        }

        delegate void reaction();
        void AcceptData()
        {
            if (txtReceiveData.InvokeRequired)
            {
                try
                {
                    DelegateAcceptData ddd = new DelegateAcceptData(AcceptData);
                    this.Invoke(ddd, new object[] { });
                }
                catch { }
            }
            else
            {
                try
                {
                    string strReceive = serialPort.ReadLine();
                    if (!String.IsNullOrEmpty(strReceive))
                    {
                        txtReceiveData.AppendText(strReceive);
                    }
                }
                catch (Exception ex) { }
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (serialPort.IsOpen)
            {
                serialPort.Close();
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            try
            {
                serialPort.Open();//打开串口
            }
            catch(Exception ex)
            {
                MessageBox.Show("打开串口失败!");
            }
            
        }
    }
}
