using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
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
    public class Parameter
    {
        public int LED1_val { get; set; }
        public int LED2_val { get; set; }
        public bool One_Channel_val { get; set; }
        public bool Two_Channel_val { get; set; }
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

        #region Config
        private void LoadConfig()
        {
            List<Parameter> Parameter_info = Config.Load();
            LED1.Text = Parameter_info[0].LED1_val.ToString();
            LED2.Text = Parameter_info[0].LED2_val.ToString();
            One_Channel.IsChecked = Parameter_info[0].One_Channel_val;
            Two_Channel.IsChecked = Parameter_info[0].Two_Channel_val;
        }

        private void SaveConfig()
        {
            List<Parameter> Parameter_config = new List<Parameter>()
                        {
                            new Parameter() { LED1_val = Convert.ToInt32(LED1.Text),
                                              LED2_val = Convert.ToInt32(LED2.Text),
                                              One_Channel_val= (bool)One_Channel.IsChecked,
                                              Two_Channel_val= (bool)Two_Channel.IsChecked
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
        PD12V30W Do;
        #endregion

        #region Main Screen
        private void Main_Btn_Click(object sender, RoutedEventArgs e)
        {
            switch ((sender as Button).Name)
            {
                case nameof(Connect):
                    {
                        Do = new PD12V30W("COM9", 19200, 8, 0, 1);
                        Do.Connect();
                        break;
                    }
                case nameof(DisConnect):
                    {
                        Do.DisConnect();
                        break;
                    }
                case nameof(Light_Up):
                    {
                        if ((bool)One_Channel.IsChecked)
                        {
                            Do.OneChannelSetBrightness(Convert.ToInt32(LED1.Value), 1);
                        }
                        else if ((bool)Two_Channel.IsChecked)
                        {
                            Do.TwoChannelSetBrightness(Convert.ToInt32(LED1.Value), Convert.ToInt32(LED2.Value));
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
