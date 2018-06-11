using AutomaticUpdate;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TeamProjectDevelopment
{
    /// <summary>
    /// import.xaml 的交互逻辑
    /// </summary>
    public partial class import : Window
    {
        static string dataBaseAddress;
        static string SaveAddress;
        static String databaseCon = ConfigurationSettings.AppSettings["databaseCon"];
        public import()
        {
            InitializeComponent();
        }
        
        public OleDbConnection getConn(String conStr)
        {
            OleDbConnection oleDbConnection = new OleDbConnection(conStr);
            return oleDbConnection;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Multiselect = false;
            openFileDialog.Filter = "AllFiles|*.*";
            openFileDialog.ShowDialog();
            dataBaseAddress = openFileDialog.FileName;
            string constring = databaseCon + dataBaseAddress;
            Console.WriteLine(constring);
            OleDbConnection connection = getConn(constring);
            String selectString = "select * from config1";
            OleDbCommand command = new OleDbCommand(selectString, connection);
            connection.Open();
            OleDbDataAdapter adapter = new OleDbDataAdapter();
            DataSet dataTable = new DataSet();
            //OleDbCommand command = new OleDbCommand(selectString, connection);
            adapter.SelectCommand = command;
            adapter.Fill(dataTable);
            dataTable.Tables[0].PrimaryKey = new DataColumn[] { dataTable.Tables[0].Columns[0] };
            dg.ItemsSource = dataTable.Tables[0].DefaultView;
            DataRow dr = dataTable.Tables[0].NewRow();
            connection.Close();
            
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //MainWindow window = new MainWindow();
            //window.Show();
            this.Close();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {

            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
            //openFileDialog.Multiselect = false;
            saveFileDialog.Filter = "mdb|*.*";
            saveFileDialog.ShowDialog();
            SaveAddress = saveFileDialog.FileName;
            String version = extension.Text;
            FileInfo file = new FileInfo(@"..\otherDbs\" + version + ".mdb");
            file.CopyTo(SaveAddress);
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}
