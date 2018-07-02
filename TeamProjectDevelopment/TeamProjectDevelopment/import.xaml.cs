using AutomaticUpdate;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Reflection;
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
using TeamProjectDevelopment.CreateConfig;

namespace TeamProjectDevelopment
{
    /// <summary>
    /// import.xaml 的交互逻辑
    /// </summary>
    public partial class import : Window
    {
        static string FileAddress;
        static string dataBaseAddress;
        string FileName;
        long FileSize;
        DateTime LastWriteTime;
        DateTime CreationTime;
        String ExtensionNumber;
        int localVersion;
        int serverVersion;

        String localPath = ConfigurationSettings.AppSettings["localPath"];
        static String serverPath = ConfigurationSettings.AppSettings["serverAddress"];
        static String databaseCon = ConfigurationSettings.AppSettings["databaseCon"];
        static String versionFile = ConfigurationSettings.AppSettings["versionFile"];
        static String otherDbs = ConfigurationSettings.AppSettings["otherDbs"];
        static String address = ConfigurationSettings.AppSettings["address"];
        static List<MyFile> files = new List<MyFile>();
        int id = 0;
        static String asd;
        static string SaveAddress;
        private ObservableCollection<ConfigFile> list { get; set; }
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
            /*
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Multiselect = false;
            openFileDialog.Filter = "AllFiles|*.*";
            openFileDialog.ShowDialog();
            dataBaseAddress = openFileDialog.FileName;
            string constring = databaseCon + dataBaseAddress;
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
            */
            InsertFile insertFile = new InsertFile();
            insertFile.insertFile(id, files);

            dg.ItemsSource = null;
            dg.ItemsSource = files;
            id++;

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //MainWindow window = new MainWindow();
            //window.Show();
            String versionNum = extension.Text;
            CreateDb createDb = new CreateDb();
            createDb.createDb(versionNum);
            InsertFile insertFile = new InsertFile();
            insertFile.insertData(versionNum, files, dg);
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

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            BindData();
        }


        //在datagrid中绑定数据库数据
        public void BindData()
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Multiselect = false;
            openFileDialog.Filter = "AllFiles|*.*";
            openFileDialog.ShowDialog();
            dataBaseAddress = openFileDialog.FileName;
            string constring = databaseCon + dataBaseAddress;

            using (OleDbConnection connection = getConn(constring))
            {
                connection.Open();
                OleDbDataAdapter adapter = new OleDbDataAdapter();
                DataSet dataTable = new DataSet();
                String selectString = "select * from config1";
                OleDbCommand command = new OleDbCommand(selectString, connection);
                adapter.SelectCommand = command;
                adapter.Fill(dataTable);
                var dt = dataTable.Tables[0];
                dataTable.Tables[0].PrimaryKey = new DataColumn[] { dataTable.Tables[0].Columns[0] };
                dg.ItemsSource = dataTable.Tables[0].DefaultView;

                DataRow dr = dataTable.Tables[0].NewRow();
                connection.Close();

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    //创建泛型对象  
                    MyFile _t = new MyFile();
                    _t = Activator.CreateInstance<MyFile>();
                    //获取对象所有属性  
                    PropertyInfo[] propertyInfo = _t.GetType().GetProperties();
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        foreach (PropertyInfo info in propertyInfo)
                        {
                            //属性名称和列名相同时赋值  
                            if (dt.Columns[j].ColumnName.ToUpper().Equals(info.Name.ToUpper()))
                            {
                                if (dt.Rows[i][j] != DBNull.Value)
                                {
                                    info.SetValue(_t, dt.Rows[i][j], null);
                                }
                                else
                                {
                                    info.SetValue(_t, null, null);
                                }
                                break;
                            }
                        }
                        id = i + 1;
                    }
                    files.Add(_t);
                }
            }

        }
    }
}
