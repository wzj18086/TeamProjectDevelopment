using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Data.OleDb;
using System.Data;
using System.Collections.ObjectModel;
using System.IO;
using System.Configuration;
using AutomaticUpdate;

public class ConfigFile
{
    public String Filename { get; set; }
    public String VersionNum { get; set; }
    public String UpdateType { get; set; }
    public List<String> UpdateFiles { get; set; }
    public String Path { get; set; }
    public int LastModified { get; set; }
}

//创建连接数据库的类
public class DbHelper
{
    //获取配置文件中的路径
    static String localPath = ConfigurationSettings.AppSettings["localPath"];
    static String databaseCon= ConfigurationSettings.AppSettings["databaseCon"];

    public static OleDbConnection getCon()
    {
        String localDbName = MainWindow.GetFileName(localPath);
        String conStr = databaseCon+localPath + "\\" + localDbName;
        OleDbConnection connection = getConn(conStr);
        String selectString = "select * from config1";
        OleDbCommand command = new OleDbCommand(selectString, connection);
        return connection;
    }
    private static OleDbConnection getConn(String conStr)
    {
        OleDbConnection oleDbConnection = new OleDbConnection(conStr);
        return oleDbConnection;
    }
}



namespace AutomaticUpdate
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        String localPath = ConfigurationSettings.AppSettings["localPath"];
        static String databaseCon = ConfigurationSettings.AppSettings["databaseCon"];
        private ObservableCollection<ConfigFile> list { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            BindData();//绑定数据

        }

        //新建配置文件
        private void Setnewconfig(object sender, RoutedEventArgs e)
        {
            NewConfigWindow setting = new NewConfigWindow();
            setting.Show();
            this.Close();
        }
        //导入配置文件
        private void Import(object sender, RoutedEventArgs e)
        {

        }

        //修改网站
        private void ChangeAd(object sender, RoutedEventArgs e)
        {
            Websetting webwindow = new Websetting();
            webwindow.Show();

        }

        //在datagrid中绑定数据库数据
        public void BindData()
        {
            using (OleDbConnection connection = DbHelper.getCon())
            {
                connection.Open();
                OleDbDataAdapter adapter = new OleDbDataAdapter();
                DataTable dataTable = new DataTable();
                String selectString = "select * from config1";
                OleDbCommand command = new OleDbCommand(selectString, connection);
                adapter.SelectCommand = command;
                adapter.Fill(dataTable);
                dg.ItemsSource = dataTable.DefaultView;
            }
            
        }


        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }
        
        private void dg_SelectionChanged(object sender,RoutedEventArgs e)
        {

        }
        

        //生成版本
        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            String localDbName = GetFileName(localPath);
            OleDbDataReader reader = DbConnect(localPath);
            while (reader.Read())
            {
                String tempStr = (String)reader["path"];
                String targetDir = @"F:\test\" + Path.GetFileName(tempStr);
                copyFile(tempStr, targetDir);
            }
            copyFile(localPath+"\\" + localDbName, "F:\\test\\" + localDbName);
            MessageBox.Show("生成版本成功", "提示", MessageBoxButton.OK);
        }

        //复制文件
        private void copyFile(String path, String targetDir)
        {

            try
            {
                File.Copy(path, targetDir, true);
                Console.WriteLine("success");
            }
            catch (IOException)
            {
                Console.WriteLine("error");
            }

        }

        //窗口加载时进行更新检测，并进行更新
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //获取配置文件中服务器地址
            String serverPath = ConfigurationSettings.AppSettings["serverAddress"];

            //判断文件是否存在
            try
            {
                if (File.Exists("address.txt"))
                {
                    FileStream file = new FileStream("address.txt", FileMode.Open);
                    StreamReader reader = new StreamReader(file);
                    String temp = reader.ReadLine();
                    serverPath = temp;
                }
            }catch(IOException)
            {

            }
           ;
            String serverDbName = GetFileNameWithonExtension(serverPath);
            //Console.WriteLine(serverDbName);
            String localDbName = GetFileNameWithonExtension(localPath);

            if(int.Parse(localDbName) < int.Parse(serverDbName))
            {
                MessageBoxResult result = MessageBox.Show("当前有版本更新，需要更新吗？", "更新", MessageBoxButton.OKCancel);
                if(result==MessageBoxResult.OK)
                {
                    UpdateMethod updateMethod = new UpdateMethod();
                    updateMethod.Show();
                    this.Close();
                }
            }


        }

        //没有后缀名的文件名获取
        public static String GetFileNameWithonExtension(String path)
        {
            DirectoryInfo folder = new DirectoryInfo(path);
           
            String DbName = Path.GetFileNameWithoutExtension(folder.GetFiles("*.mdb")[0].Name);
            return DbName;
        }

        //连接数据库
        public static OleDbConnection getConn(String conStr)
        {
            OleDbConnection oleDbConnection = new OleDbConnection(conStr);
            return oleDbConnection;
        }

        //获取文件名字
        public static String GetFileName(String path)
        {
            DirectoryInfo folder = new DirectoryInfo(path);
            String DbName = Path.GetFileName(folder.GetFiles("*.mdb")[0].Name);
            return DbName;
        }

        //连接数据库
        public static OleDbDataReader DbConnect(String path)
        {
            String DbName = GetFileName(path);
            String conStr = databaseCon + path + "\\" + DbName;
            OleDbConnection connection = getConn(conStr);
            String selectString = "select * from config1";
            OleDbCommand command = new OleDbCommand(selectString, connection);
            connection.Open();
            OleDbDataReader reader = command.ExecuteReader();
            return reader;
        }
    }

}

