using System;
using System.Configuration;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace AutomaticUpdate
{
    /// <summary>
    /// setting.xaml 的交互逻辑
    /// </summary>
    public partial class Websetting : Window
    {
        static String address1 = ConfigurationSettings.AppSettings["address"];
        public Websetting()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }


        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //将新的服务器地址写入文件
            try
            {
                FileStream address = new FileStream(address1, FileMode.Create,FileAccess.ReadWrite);
                StreamWriter writer = new StreamWriter(address);
                writer.Write(AddressBox.Text);
                writer.Close();

            }
            catch(IOException)
            {
                
            }
            this.Close();
        }
    }
}