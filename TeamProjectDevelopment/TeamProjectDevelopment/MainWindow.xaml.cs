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
using System.Diagnostics;
using System.Reflection;

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
    static String serverPath = ConfigurationSettings.AppSettings["serverPath"];
    
    

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
        static string FileAddress;
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
        static String otherDbs= ConfigurationSettings.AppSettings["otherDbs"];
        static String address = ConfigurationSettings.AppSettings["address"];
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
                DataSet dataTable = new DataSet();
                String selectString = "select * from config1";
                OleDbCommand command = new OleDbCommand(selectString, connection);
                adapter.SelectCommand = command;
                adapter.Fill(dataTable);
                dataTable.Tables[0].PrimaryKey = new DataColumn[] { dataTable.Tables[0].Columns[0] };
                dg.ItemsSource = dataTable.Tables[0].DefaultView;

                DataRow dr = dataTable.Tables[0].NewRow();
                connection.Close();
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

            //判断文件是否存在
            try
            {
                if (File.Exists(address))
                {
                    FileStream file = new FileStream(address, FileMode.Open);
                    StreamReader reader = new StreamReader(file);
                    String temp = reader.ReadLine();
                    serverPath = temp;
                    file.Close();
                    reader.Close();
                }
            }catch(IOException)
            {

            }
           ;
            String serverDbName = GetFileNameWithonExtension(serverPath);
            if (serverDbName == null)
                return;
            //Console.WriteLine(serverDbName);
            String localDbName = GetFileNameWithonExtension(localPath);
            if(AntoUpdateOrNot())
            {
                MessageBoxResult result = MessageBox.Show("当前软件需要更新", "提示", MessageBoxButton.OKCancel);
                if(result==MessageBoxResult.OK)
                {
                    String filename = Assembly.GetExecutingAssembly().Location;
                    File.Move(filename, filename + ".delete");
                    File.Copy(serverPath + "\\" + "TeamProjectDevelopment.exe", filename);
                    

                   
                    String DbName = GetFileName(serverPath);
                    String conStr = databaseCon + serverPath + "\\" + DbName;
                    OleDbConnection connection = getConn(conStr);
                    connection.Open();
                    String selectString = "delete from config1 where fileName=\"TeamProjectDevelopment.exe\"";
                    OleDbCommand command = new OleDbCommand(selectString, connection);
                    command.ExecuteNonQuery();
                    connection.Close();

                    AutoUpdate();
                }
                
            }
            if(UpdateOrNot())
            {
                MessageBoxResult result = MessageBox.Show("当前有版本更新，需要更新吗？", "更新", MessageBoxButton.OKCancel);
                if(result==MessageBoxResult.OK)
                {
                    /*
                    UpdateMethod updateMethod = new UpdateMethod();
                    updateMethod.Show();
                    this.Close();
                    */
                    Update();
                }
            }


        }

        //没有后缀名的文件名获取
        public static String GetFileNameWithonExtension(String path)
        {
            DirectoryInfo folder = new DirectoryInfo(path);
            String DbName;
            try
            {
                 DbName= Path.GetFileNameWithoutExtension(folder.GetFiles("*.mdb")[0].Name);
            }
            catch
            {
                return null;
            }
            
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
            String DbName;
            try
            {
                DbName = Path.GetFileName(folder.GetFiles("*.mdb")[0].Name);
            }
            catch
            {
                return null;
            }
            
            return DbName;
        }

        //连接数据库
        public static OleDbDataReader DbConnect(String path)
        {
            Console.WriteLine(path + "tetetetet");
            String DbName = GetFileName(path);
            String conStr = databaseCon + path + "\\"+ DbName;
            OleDbConnection connection = getConn(conStr);
            String selectString = "select * from config1";
            OleDbCommand command = new OleDbCommand(selectString, connection);
            connection.Open();
            OleDbDataReader reader = command.ExecuteReader();
            
            return reader;
        }
        public bool UpdateOrNot()
        {
            String serverDbName =GetFileName(serverPath);
            Console.WriteLine("updateornot" + serverPath);
            if (serverDbName == null)
                return false;
            OleDbDataReader serverReader = MainWindow.DbConnect(serverPath);
            String localDbName = MainWindow.GetFileName(localPath);
            OleDbDataReader localReader = MainWindow.DbConnect(localPath);
            
            if (localReader.Read())
            {
                try
                {
                    localVersion = (int)localReader["versionNum"];
                }
                catch
                {
                    localVersion = 1;
                }
                
            }
            if(serverReader.Read())
            {
                 serverVersion= (int)serverReader["versionNum"];
            }
            
            if (localVersion < serverVersion)
            {
                serverReader.Close();
                localReader.Close();
                return true;
            }
            else
            {
                serverReader.Close();
                localReader.Close();
                return false;
            }
                

        }
        private void AutoUpdate()
        {
            Process p = new Process();
            p.StartInfo.FileName = System.AppDomain.CurrentDomain.BaseDirectory + "TeamProjectDevelopment.exe";
            p.StartInfo.UseShellExecute = false;
            p.Start();
            Application.Current.Shutdown();
        }
        private bool AntoUpdateOrNot()
        {
            String serverDbName = MainWindow.GetFileName(serverPath);
            if(serverDbName==null)
            {
                return false;
            }
            OleDbDataReader serverReader = MainWindow.DbConnect(serverPath);
            while(serverReader.Read())
            {
                if(serverReader["fileName"].Equals("TeamProjectDevelopment.exe"))
                {
                    serverReader.Close();
                    return true;
                }
            }
            serverReader.Close();
            return false;
        }
        //导入新文件
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
            //ExtensionNumber = extension.Text;//版本号
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Multiselect = false;
            openFileDialog.Filter = "AllFiles|*.*";

            if ((bool)openFileDialog.ShowDialog())
            {
                FileAddress = openFileDialog.FileName;
                System.IO.FileInfo file = new System.IO.FileInfo(FileAddress);
                FileName = file.Name;
                FileSize = file.Length;
                LastWriteTime = file.LastWriteTime.ToUniversalTime();
                CreationTime = file.CreationTime.ToUniversalTime();


            }

            String localDbName = MainWindow.GetFileName(localPath);
            String conStr = databaseCon + localPath + "\\" + localDbName;
            OleDbConnection con = new OleDbConnection(conStr);
            con.Open();
            OleDbCommand cmd = new OleDbCommand();
            cmd.Connection = con;

            string sqlcmd2 = "select MAX(id) From config1";
            cmd.CommandText = sqlcmd2;
            int num = Convert.ToInt32(cmd.ExecuteScalar());
            Console.WriteLine(num);
            cmd.Dispose();
            con.Close();
            con.Dispose();
            con = new OleDbConnection(conStr);
            con.Open();
            cmd = new OleDbCommand();
            cmd.Connection = con;
            string sqlcmd1 = "insert into config1 values(" + ++num + ",'" + FileName + "'," + FileSize + ",'" + CreationTime + "','" + LastWriteTime + "','" + FileAddress + "','" + localVersion + "')";
            cmd.CommandText = sqlcmd1;
            FileAddress = "0";
            cmd.ExecuteNonQuery();
            cmd.Dispose();
            con.Close();
            con.Dispose();

            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        //修改
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            OleDbConnection connection = DbHelper.getCon();
            connection.Open();
            OleDbCommand cmd = connection.CreateCommand();

            cmd.CommandText = "UPDATE config1 Set [fileName]='" + ((DataRowView)this.dg.SelectedItem).Row.ItemArray[1].ToString().Trim() +
                    "', [fileSize]='" + ((DataRowView)this.dg.SelectedItem).Row.ItemArray[2].ToString().Trim() +
                    "', [createTime]='" + ((DataRowView)this.dg.SelectedItem).Row.ItemArray[3].ToString().Trim() +
                    "', [modifiedTime]='" + ((DataRowView)this.dg.SelectedItem).Row.ItemArray[4].ToString().Trim() +
                    "', [path]='" + ((DataRowView)this.dg.SelectedItem).Row.ItemArray[5].ToString().Trim() +
                    "', [versionNum]='" + ((DataRowView)this.dg.SelectedItem).Row.ItemArray[6].ToString().Trim() +
                    "' Where ID=" + ((DataRowView)this.dg.SelectedItem).Row.ItemArray[0].ToString().Trim();
            cmd.ExecuteNonQuery();

            connection.Close();
            BindData();
        }
        //删除
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            OleDbConnection connection = DbHelper.getCon();
            connection.Open();
            OleDbCommand cmd = connection.CreateCommand();
            string sql = "DELETE FROM config1 WHERE ID=" + ((DataRowView)this.dg.SelectedItem).Row.ItemArray[0].ToString().Trim();
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();

            connection.Close();
            BindData();
        }

        //生成其他版本
        private void GenerateOhtherVersion()
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.RestoreDirectory=true;
            openFileDialog.Filter = "AllFiles|*.*";
            openFileDialog.InitialDirectory = @"F:\vs2017 project\TeamProjectDevelopment\TeamProjectDevelopment\TeamProjectDevelopment\bin\otherDbs";
            openFileDialog.ShowDialog();
            String versionFilePath = openFileDialog.FileName;
            try
            {
                String DbName = Path.GetFileName(versionFilePath);
                String conString = databaseCon + versionFilePath;
                String sql = "select * from config1";
                OleDbConnection oleDbConnection = new OleDbConnection(conString);
                OleDbCommand command = new OleDbCommand(sql, oleDbConnection);
                oleDbConnection.Open();
                OleDbDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    String tempStr = (String)reader["path"];
                    String targetDir = versionFile + "\\" + Path.GetFileName(tempStr);
                    copyFile(tempStr, targetDir);
                }
                copyFile(versionFilePath, versionFile + "\\" + DbName);
                oleDbConnection.Close();
                MessageBox.Show("生成版本成功", "提示", MessageBoxButton.OK);
            }
            catch { }
            

        }
        private void GenerateLocalVersion(String DbPath)
        {
            String DbName = GetFileName(DbPath);
            OleDbDataReader reader = DbConnect(DbPath);
            while (reader.Read())
            {
                String tempStr = (String)reader["path"];
                String targetDir =versionFile+"\\" + Path.GetFileName(tempStr);
                copyFile(tempStr, targetDir);
            }
            copyFile(DbPath + "\\" + DbName, versionFile+"\\" + DbName);
            MessageBox.Show("生成版本成功", "提示", MessageBoxButton.OK);
        }

        private void LocalVerisonClick(object sender, RoutedEventArgs e)
        {
            GenerateLocalVersion(localPath);
        }

        private void OtherVersionClick(object sender, RoutedEventArgs e)
        {
            GenerateOhtherVersion();
        }
        private void Update()
        {
            String serverDbName = GetFileName(serverPath);
            OleDbDataReader reader = DbConnect(serverPath);
            while (reader.Read())
            {
                String tempStr = (String)reader["path"];
                String originFile = serverPath + "\\" + (String)reader["fileName"];
                Console.WriteLine("success");
                String updateMethod = (String)reader["updateMethod"];
                if (updateMethod.Equals("删除"))
                    File.Delete(tempStr);
                else
                    copyFile(originFile, tempStr);
            }
            if (reader != null)
                reader.Close();

            //最后复制服务器配置文件到本地，并删除原来的配置文件
            Console.WriteLine("success2");


            String localDbName = MainWindow.GetFileName(localPath);
            copyFile(serverPath + "\\" + serverDbName, localPath + "\\" + serverDbName);
            //File.Delete(localPath + "\\" + localDbName);
            SuccessTips();
        }

        private void SuccessTips()
        {
            MessageBoxResult result = MessageBox.Show("更新完成", "提示", MessageBoxButton.OK);

            if (result == MessageBoxResult.OK)
            {
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();

            }
        }
    }

}

