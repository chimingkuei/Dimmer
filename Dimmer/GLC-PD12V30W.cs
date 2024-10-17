using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Dimmer
{
    class PD12V30W
    {

        public SerialPort serialPort = new SerialPort();

        /// <summary>
        /// PD12V30W("COM5", 19200, 8, 0, 1);
        /// </summary>
        public PD12V30W(string _PortName, int _BaudRate, int _DataBits, int _Parity, int _StopBits)
        {
            serialPort.PortName = _PortName;
            serialPort.BaudRate = _BaudRate;
            serialPort.DataBits = _DataBits;
            serialPort.Parity = (Parity)_Parity;
            serialPort.StopBits = (StopBits)_StopBits;
        }

        public void Connect()
        {
            try
            {
                serialPort.Open();
            }
            catch
            {
                MessageBox.Show("連線失敗!", "警告");
            }

        }

        public void DisConnect()
        {
            serialPort.Close();
        }

        private string OneChannelLRC(int led_value, int ch)
        {
            string Temp = (1 + 6 + ch + led_value).ToString("X");
            string Data_sum = Temp.Length >= 2 ? Temp.Substring(Temp.Length - 2, 2) : (1 + 6 + ch + led_value).ToString("X2");
            string LRC = (255 - Convert.ToInt32(Data_sum, 16) + 1).ToString("X2");
            return LRC;
        }

        private string OneChannelProtocalFormat(int led_value, int ch)
        {
            string Header = ":";
            string command = "0106000" + ch.ToString() + "00" + led_value.ToString("X2");
            string LRC = OneChannelLRC(led_value, ch);
            string msg = Header + command + LRC + "\r\n";
            //Console.WriteLine(msg);
            return msg;
        }

        public void OneChannelSetBrightness(int led_value, int ch)
        {
            string msg = OneChannelProtocalFormat(led_value, ch);
            byte[] buf = Encoding.Default.GetBytes(msg);
            serialPort.Write(buf, 0, buf.Length);
        }

        private string TwoChannelLRC(int led1_value, int led2_value)
        {
            string Temp = (1 + 16 + 1 + 2 + 4 + led1_value + led2_value).ToString("X");
            string Data_sum = Temp.Length >= 2 ? Temp.Substring(Temp.Length - 2, 2) : (1 + 16 + 1 + 2 + 4 + led1_value + led2_value).ToString("X2");
            string LRC = (255 - Convert.ToInt32(Data_sum, 16) + 1).ToString("X2");
            return LRC;
        }

        private string TwoChannelProtocalFormat(int led1_value, int led2_value)
        {
            string Header = ":";
            string command = "01100001000204" + led1_value.ToString("X2") + led2_value.ToString("X2");
            string LRC = TwoChannelLRC(led1_value, led2_value);
            string msg = Header + command + LRC + "\r\n";
            //Console.WriteLine(msg);
            return msg;
        }

        public void TwoChannelSetBrightness(int led1_value, int led2_value)
        {
            string msg = TwoChannelProtocalFormat(led1_value, led2_value);
            byte[] buf = Encoding.Default.GetBytes(msg);
            serialPort.Write(buf, 0, buf.Length);
        }

        #region Upgrade
        private string OneChannel(int led_value, int ch)
        {
            string Temp = (1 + 6 + ch + led_value).ToString("X");
            return Temp.Length >= 2 ? Temp.Substring(Temp.Length - 2, 2) : (1 + 6 + ch + led_value).ToString("X2");
        }

        private string TwoChannel(int led1_value, int led2_value)
        {
            string Temp = (1 + 16 + 1 + 2 + 4 + led1_value + led2_value).ToString("X");
            return Temp.Length >= 2 ? Temp.Substring(Temp.Length - 2, 2) : (1 + 16 + 1 + 2 + 4 + led1_value + led2_value).ToString("X2");
        }

        private string LRCCall(int led_value, int ch_or_ledvalue, Func<int, int, string> fun)
        {
            return (255 - Convert.ToInt32(fun(led_value, ch_or_ledvalue), 16) + 1).ToString("X2");
        }

        private string ProtocalFormat(int led_value, int ch_or_ledvalue, int type)
        {
            string Header = ":";
            string command = "";
            string LRC = "";
            if (type == 1)
            {
                command = "0106000" + ch_or_ledvalue.ToString() + "00" + led_value.ToString("X2");
                LRC = LRCCall(led_value, ch_or_ledvalue, OneChannel);
            }
            else if (type == 2)
            {
                command = "01100001000204" + led_value.ToString("X2") + ch_or_ledvalue.ToString("X2");
                LRC = LRCCall(led_value, ch_or_ledvalue, TwoChannel);
            }
            return Header + command + LRC + "\r\n";
        }

        public void SetBrightness(int led_value, int ch_or_ledvalue, int type)
        {
            string msg = ProtocalFormat(led_value, ch_or_ledvalue, type);
            byte[] buf = Encoding.Default.GetBytes(msg);
            serialPort.Write(buf, 0, buf.Length);
        }
        #endregion

    }
}
