using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Dimmer
{
    enum ProductModel
    {
        GLCPD12V30W, GLCPD24V24W, GLCLFB2350
    }
    enum ChannelControl
    {
        OneCh, TwoCh, FourCh
    }

    public class Parameter
    {
        public int LED1_val { get; set; }
        public int LED2_val { get; set; }
        public int LED3_val { get; set; }
        public int LED4_val { get; set; }
        public bool Ch1_Rd_val { get; set; }
        public bool Ch2_Rd_val { get; set; }
        public bool Ch3_Rd_val { get; set; }
        public bool Ch4_Rd_val { get; set; }
        public bool OneCh_val { get; set; }
        public bool TwoCh_val { get; set; }
        public bool FourCh_val { get; set; }
        public bool One_Channel_val { get; set; }
        public bool Two_Channel_val { get; set; }
        public bool GLC_PD12V30W_val { get; set; }
        public bool GLC_PD24V24W_val { get; set; }
        public bool GLC_LFB2350_val { get; set; }
        public string PortName_val { get; set; }
        public int BaundRate_val { get; set; }
        public int ByteSize_val { get; set; }
        public int Parity_val { get; set; }
        public int StopBit_val { get; set; }
    }
    
    public partial class MainWindow : Window
    {
        
        public MainWindow()
        {
            InitializeComponent();
        }

        #region Function
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show("請問是否要關閉？", "確認", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                e.Cancel = false;
            }
            else
            {
                e.Cancel = true;
            }
        }

        private ProductModel SelectModel()
        {
            if ((bool)GLC_PD12V30W.IsChecked)
            {
                selectedModel = ProductModel.GLCPD12V30W;
            }
            else if ((bool)GLC_PD24V24W.IsChecked)
            {
                selectedModel = ProductModel.GLCPD24V24W;
            }
            else if ((bool)GLC_LFB2350.IsChecked)
            {
                selectedModel = ProductModel.GLCLFB2350;
            }
            return selectedModel;
        }

        private ChannelControl SelectChannel()
        {
            if ((bool)OneCh.IsChecked)
            {
                selectedChannel = ChannelControl.OneCh;
            }
            else if ((bool)TwoCh.IsChecked)
            {
                selectedChannel = ChannelControl.TwoCh;
            }
            else if ((bool)FourCh.IsChecked)
            {
                selectedChannel = ChannelControl.FourCh;
            }
            return selectedChannel;
        }

        #region Config
        private void LoadConfig()
        {
            List<Parameter> Parameter_info = Config.Load();
            LED1.Text = Parameter_info[0].LED1_val.ToString();
            LED2.Text = Parameter_info[0].LED2_val.ToString();
            LED3.Text = Parameter_info[0].LED3_val.ToString();
            LED4.Text = Parameter_info[0].LED4_val.ToString();
            Ch1_Rd.IsChecked = Parameter_info[0].Ch1_Rd_val;
            Ch2_Rd.IsChecked = Parameter_info[0].Ch2_Rd_val;
            Ch3_Rd.IsChecked = Parameter_info[0].Ch3_Rd_val;
            Ch4_Rd.IsChecked = Parameter_info[0].Ch4_Rd_val;
            OneCh.IsChecked = Parameter_info[0].OneCh_val;
            TwoCh.IsChecked = Parameter_info[0].TwoCh_val;
            FourCh.IsChecked = Parameter_info[0].FourCh_val;
            GLC_PD12V30W.IsChecked = Parameter_info[0].GLC_PD12V30W_val;
            GLC_PD24V24W.IsChecked = Parameter_info[0].GLC_PD24V24W_val;
            GLC_LFB2350.IsChecked = Parameter_info[0].GLC_LFB2350_val;
            PortName.Text = Parameter_info[0].PortName_val.ToString();
            BaundRate.Text = Parameter_info[0].BaundRate_val.ToString();
            ByteSize.Text = Parameter_info[0].ByteSize_val.ToString();
            Parity.Text = Parameter_info[0].Parity_val.ToString();
            StopBit.Text = Parameter_info[0].StopBit_val.ToString();
        }

        private void SaveConfig()
        {
            List<Parameter> Parameter_config = new List<Parameter>()
                        {
                            new Parameter() { LED1_val = Convert.ToInt32(LED1.Text),
                                              LED2_val = Convert.ToInt32(LED2.Text),
                                              LED3_val = Convert.ToInt32(LED3.Text),
                                              LED4_val = Convert.ToInt32(LED4.Text),
                                              Ch1_Rd_val = (bool)Ch1_Rd.IsChecked,
                                              Ch2_Rd_val = (bool)Ch2_Rd.IsChecked,
                                              Ch3_Rd_val = (bool)Ch3_Rd.IsChecked,
                                              Ch4_Rd_val = (bool)Ch4_Rd.IsChecked,
                                              OneCh_val = (bool)OneCh.IsChecked,
                                              TwoCh_val = (bool)TwoCh.IsChecked,
                                              FourCh_val = (bool)FourCh.IsChecked,
                                              GLC_PD12V30W_val = (bool)GLC_PD12V30W.IsChecked,
                                              GLC_PD24V24W_val = (bool)GLC_PD24V24W.IsChecked,
                                              GLC_LFB2350_val = (bool)GLC_LFB2350.IsChecked,
                                              PortName_val=PortName.Text,
                                              BaundRate_val = Convert.ToInt32(BaundRate.Text),
                                              ByteSize_val = Convert.ToInt32(ByteSize.Text),
                                              Parity_val = Convert.ToInt32(Parity.Text),
                                              StopBit_val = Convert.ToInt32(StopBit.Text)
                            }
                        };
            Config.Save(Parameter_config);
            Logger.WriteLog("儲存參數!", 1, richTextBoxGeneral);
        }
        #endregion
        #endregion

        #region Parameter and Init
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadConfig();
        }
        BaseLogRecord Logger = new BaseLogRecord();
        BaseConfig<Parameter> Config = new BaseConfig<Parameter>();
        GLCPD12V30W GLCPD12V30W;
        GLCPD24V24W GLCPD24V24W;
        GLCLFB2350 GLCLFB2350;
        ProductModel selectedModel;
        ChannelControl selectedChannel;
        #endregion

        #region Main Screen
        private void Main_Btn_Click(object sender, RoutedEventArgs e)
        {
            switch ((sender as Button).Name)
            {
                case nameof(Connect):
                    {
                        SelectModel();
                        switch (selectedModel)
                        {
                            case ProductModel.GLCPD12V30W:
                                GLCPD12V30W = new GLCPD12V30W(PortName.Text, Convert.ToInt32(BaundRate.Text), Convert.ToInt32(ByteSize.Text), Convert.ToInt32(Parity.Text), Convert.ToInt32(StopBit.Text));
                                GLCPD12V30W.Connect();
                                break;

                            case ProductModel.GLCPD24V24W:
                                GLCPD24V24W = new GLCPD24V24W(PortName.Text, Convert.ToInt32(BaundRate.Text), Convert.ToInt32(ByteSize.Text), Convert.ToInt32(Parity.Text), Convert.ToInt32(StopBit.Text));
                                GLCPD24V24W.Connect();
                                break;

                            case ProductModel.GLCLFB2350:
                                GLCLFB2350 = new GLCLFB2350(PortName.Text, Convert.ToInt32(BaundRate.Text), Convert.ToInt32(ByteSize.Text), Convert.ToInt32(Parity.Text), Convert.ToInt32(StopBit.Text));
                                GLCLFB2350.Connect();
                                break;
                        }
                        break;
                    }
                case nameof(DisConnect):
                    {
                        SelectModel();
                        switch (selectedModel)
                        {
                            case ProductModel.GLCPD12V30W:
                                GLCPD12V30W.DisConnect();
                                break;

                            case ProductModel.GLCPD24V24W:
                                break;

                            case ProductModel.GLCLFB2350:
                                GLCLFB2350.DisConnect();
                                break;
                        }
                        break;
                    }
                case nameof(Light_Up):
                    {
                        SelectModel();
                        SelectChannel();
                        switch (selectedModel)
                        {
                            case ProductModel.GLCPD12V30W:
                                switch (selectedChannel)
                                {
                                    case ChannelControl.OneCh:
                                        if ((bool)Ch1_Rd.IsChecked)
                                        {
                                            GLCPD12V30W.OneChannelSetBrightness(Convert.ToInt32(LED1.Value), 1);
                                        }
                                        else if ((bool)Ch2_Rd.IsChecked)
                                        {
                                            GLCPD12V30W.OneChannelSetBrightness(Convert.ToInt32(LED2.Value), 2);
                                        }
                                        else if ((bool)Ch3_Rd.IsChecked)
                                        {
                                            GLCPD12V30W.OneChannelSetBrightness(Convert.ToInt32(LED3.Value), 3);
                                        }
                                        else if ((bool)Ch4_Rd.IsChecked)
                                        {
                                            GLCPD12V30W.OneChannelSetBrightness(Convert.ToInt32(LED4.Value), 4);
                                        }
                                        break;
                                    case ChannelControl.TwoCh:
                                        GLCPD12V30W.TwoChannelSetBrightness(Convert.ToInt32(LED1.Value), Convert.ToInt32(LED2.Value));
                                        break;
                                    case ChannelControl.FourCh:
                                        GLCPD12V30W.FourChannelSetBrightness(Convert.ToInt32(LED1.Value), Convert.ToInt32(LED2.Value), Convert.ToInt32(LED3.Value), Convert.ToInt32(LED4.Value));
                                        break;
                                }
                                break;
                            case ProductModel.GLCPD24V24W:
                                switch (selectedChannel)
                                {
                                    case ChannelControl.OneCh:
                                        if ((bool)Ch1_Rd.IsChecked)
                                        {
                                            GLCPD24V24W.OneChannelSetBrightness(Convert.ToInt32(LED1.Value), 1);
                                        }
                                        else if ((bool)Ch2_Rd.IsChecked)
                                        {
                                            GLCPD24V24W.OneChannelSetBrightness(Convert.ToInt32(LED2.Value), 2);
                                        }
                                        else if ((bool)Ch3_Rd.IsChecked)
                                        {
                                            GLCPD24V24W.OneChannelSetBrightness(Convert.ToInt32(LED3.Value), 3);
                                        }
                                        else if ((bool)Ch4_Rd.IsChecked)
                                        {
                                            GLCPD24V24W.OneChannelSetBrightness(Convert.ToInt32(LED4.Value), 4);
                                        }
                                        break;
                                    case ChannelControl.TwoCh:
                                        GLCPD24V24W.TwoChannelSetBrightness(Convert.ToInt32(LED1.Value), Convert.ToInt32(LED2.Value));
                                        break;
                                    case ChannelControl.FourCh:
                                        GLCPD24V24W.FourChannelSetBrightness(Convert.ToInt32(LED1.Value), Convert.ToInt32(LED2.Value), Convert.ToInt32(LED3.Value), Convert.ToInt32(LED4.Value));
                                        break;
                                }
                                break;
                            case ProductModel.GLCLFB2350:
                                switch (selectedChannel)
                                {
                                    case ChannelControl.OneCh:
                                        GLCLFB2350.OneChannelSetBrightness(Convert.ToInt32(LED1.Value), 1);
                                        break;
                                    case ChannelControl.TwoCh:
                                        MessageBox.Show("This product model only has 1 channel!", "Confirm", MessageBoxButton.OK, MessageBoxImage.Question);
                                        break;
                                    case ChannelControl.FourCh:
                                        MessageBox.Show("This product model only has 1 channel!", "Confirm", MessageBoxButton.OK, MessageBoxImage.Question);
                                        break;
                                }
                                break;
                        }
                        break;
                    }
                case nameof(Save_Config):
                    {
                        SaveConfig();
                        break;
                    }
            }
        }
        #endregion


    }
}
