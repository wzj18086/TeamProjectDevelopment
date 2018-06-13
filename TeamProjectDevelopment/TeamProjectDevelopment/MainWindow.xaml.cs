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
using System.Text;
using System.Threading.Tasks;
using TeamProjectDevelopment;
using ADOX;
using System.Windows.Controls;
using Microsoft.Win32;

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
        static List<MyFile> files = new List<MyFile>();
        int id = 0;
        static String asd;
        static string SaveAddress;
        private ObservableCollection<ConfigFile> list { get; set; }
        

public MainWindow()
        {
            InitializeComponent();
            BindData();//绑定数据

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
            }
            catch (IOException)
            {

            }
           ;
            String serverDbName = GetFileNameWithonExtension(serverPath);
            if (serverDbName == null)
                return;
            String localDbName = GetFileNameWithonExtension(localPath);
            if (AntoUpdateOrNot())
            {
                MessageBoxResult result = MessageBox.Show("当前软件需要更新", "提示", MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.OK)
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
            if (UpdateOrNot())
            {
                MessageBoxResult result = MessageBox.Show("当前有版本更新，需要更新吗？", "更新", MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.OK)
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


        //修改服务器地址
        private void ChangeAd(object sender, RoutedEventArgs e)
        {
            Websetting webwindow = new Websetting();
            webwindow.Show();

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

        
        //选择本地版本生成
        private void LocalVerisonClick(object sender, RoutedEventArgs e)
        {
            GenerateLocalVersion(localPath);
        }

        //选择其他版本生成
        private void OtherVersionClick(object sender, RoutedEventArgs e)
        {
            GenerateOhtherVersion();
        }


        //连接数据库
        public static OleDbConnection getConn(String conStr)
        {
            OleDbConnection oleDbConnection = new OleDbConnection(conStr);
            return oleDbConnection;
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

        //判断是否需要更新
        public bool UpdateOrNot()
        {
            String serverDbName =GetFileName(serverPath);
            if (serverDbName == null)
                return false;
            OleDbDataReader serverReader = DbConnect(serverPath);
            String localDbName = GetFileName(localPath);
            OleDbDataReader localReader = DbConnect(localPath);
            
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

        //软件自己更新自己
        private void AutoUpdate()
        {
            Process p = new Process();
            p.StartInfo.FileName = System.AppDomain.CurrentDomain.BaseDirectory + "TeamProjectDevelopment.exe";
            p.StartInfo.UseShellExecute = false;
            p.Start();
            Application.Current.Shutdown();
        }

        //判断是否更新
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



        
        //生成其他版本
        private void GenerateOhtherVersion()
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.RestoreDirectory=true;
            openFileDialog.Filter = "AllFiles|*.*";
            openFileDialog.InitialDirectory = @"D:\study\vsproject\TeamProjectDevelopment\TeamProjectDevelopment\TeamProjectDevelopment\bin";
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

                String path = versionFile;
                System.IO.Directory.CreateDirectory(path +"\\"+"版本"+Path.GetFileNameWithoutExtension(versionFilePath));
                DirectoryInfo dir = new DirectoryInfo(path + "\\" + "版本"+ Path.GetFileNameWithoutExtension(versionFilePath));
                dir.Create();

                while (reader.Read())
                {
                    String tempStr = (String)reader["path"];
                    String targetDir = dir + "\\" + Path.GetFileName(tempStr);
                    copyFile(tempStr, targetDir);
                }
                copyFile(versionFilePath, dir + "\\" + DbName);
                oleDbConnection.Close();
                MessageBox.Show("生成版本成功", "提示", MessageBoxButton.OK);
            }
            catch { }
            

        }

        //生成本地版本
        private void GenerateLocalVersion(String DbPath)
        {
            String DbName = GetFileName(DbPath);
            OleDbDataReader reader = DbConnect(DbPath);

            String path = versionFile;
            System.IO.Directory.CreateDirectory(path + "\\" + "本地版本" + Path.GetFileNameWithoutExtension(DbName));
            DirectoryInfo dir = new DirectoryInfo(path + "\\" + "本地版本" + Path.GetFileNameWithoutExtension(DbName));
            dir.Create();
            while (reader.Read())
            {
                String tempStr = (String)reader["path"];
                String targetDir =dir+"\\" + Path.GetFileName(tempStr);
                copyFile(tempStr, targetDir);
            }
            copyFile(DbPath + "\\" + DbName, dir+"\\" + DbName);
            MessageBox.Show("生成版本成功", "提示", MessageBoxButton.OK);
        }

        

        //更新
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
            String localDbName = GetFileName(localPath);
            copyFile(serverPath + "\\" + serverDbName, localPath + "\\" + serverDbName);
            if(serverDbName!=localDbName)
                File.Delete(localPath + "\\" + localDbName);
            SuccessTips();
        }

        //更新成功的提示
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

        //没有后缀名的文件名获取
        public static String GetFileNameWithonExtension(String path)
        {
            DirectoryInfo folder = new DirectoryInfo(path);
            String DbName;
            try
            {
                DbName = Path.GetFileNameWithoutExtension(folder.GetFiles("*.mdb")[0].Name);
            }
            catch
            {
                return null;
            }

            return DbName;
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
                        id = i+1;
                    }
                    files.Add(_t);
                }
            }  

        }


        //新建配置信息

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            insertFile();

            dg.ItemsSource = null;
            dg.ItemsSource = files;


        }

        //确定
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CreateDb();
            insertData();
        }

        //另存为
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {


            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
            //openFileDialog.Multiselect = false;
            saveFileDialog.Filter = "mdb|*.*";
            saveFileDialog.ShowDialog();
            SaveAddress = saveFileDialog.FileName;

            CreateDb();
            insertData();
            String version = extension.Text;

            FileInfo file = new FileInfo(@"..\otherDbs\" + version + ".mdb");


            file.CopyTo(SaveAddress);
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();




        }

        //创建数据库
        private void CreateDb()
        {

            ADOX.Catalog catalog = new ADOX.Catalog();
            String versionNum = extension.Text;
            try
            {
                catalog.Create(databaseCon + otherDbs + versionNum + ".mdb" + ";Jet OLEDB:Engine Type=5");
            }
            catch { }



            ADODB.Connection cn = new ADODB.Connection();
            cn.Open(databaseCon + otherDbs + versionNum + ".mdb", null, null, -1);
            catalog.ActiveConnection = cn;

            //创建表
            ADOX.Table table = new ADOX.Table();
            table.Name = "config1";

            //创建列
            ADOX.Column column = new ADOX.Column();
            column.ParentCatalog = catalog;
            column.Name = "ID";
            column.Type = DataTypeEnum.adInteger;
            column.DefinedSize = 9;
            column.Properties["AutoIncrement"].Value = true;
            table.Columns.Append(column, DataTypeEnum.adInteger, 9);
            // 设置为主键
            table.Keys.Append("FirstTablePrimaryKey", KeyTypeEnum.adKeyPrimary, column, null, null);

            table.Columns.Append("fileName", DataTypeEnum.adVarWChar, 0);
            table.Columns.Append("fileSize", DataTypeEnum.adInteger, 0);
            table.Columns.Append("createTime", DataTypeEnum.adDate, 0);
            table.Columns.Append("modifiedTime", DataTypeEnum.adDate, 0);
            table.Columns.Append("path", DataTypeEnum.adLongVarWChar, 0);
            table.Columns.Append("versionNum", DataTypeEnum.adInteger, 0);
            table.Columns.Append("updateMethod", DataTypeEnum.adVarWChar, 0);

            try
            {
                // 添加表
                catalog.Tables.Append(table);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            //此处一定要关闭连接，否则添加数据时候会出错

            table = null;
            catalog = null;
            //Application.DoEvents();

            cn.Close();


        }





        //新建配置文件上的导入功能
        private void insertFile()
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Multiselect = false;
            openFileDialog.Filter = "AllFiles|*.*";

            if ((bool)openFileDialog.ShowDialog())
            {
                MyFile myFile = new MyFile();


                FileAddress = openFileDialog.FileName;
                myFile.Path = openFileDialog.FileName;
                FileInfo file = new System.IO.FileInfo(FileAddress);
                myFile.ID = id;
                myFile.FileName = file.Name;
                myFile.FileSize = file.Length;
                myFile.CreateTime = file.CreationTime;
                myFile.ModifiedTime = file.LastWriteTime;
                id++;
                files.Add(myFile);
            }
        }

        private void insertData()
        {
            String versionNum = extension.Text;
            String conString = databaseCon + otherDbs + versionNum + ".mdb";
            OleDbConnection connection = new OleDbConnection(conString);
            connection.Open();
            for (int i = 0; i < files.Count; i++)
            {
                MyFile file = files[i];
                String updateMethod = GetUpdateMethod(i);
                String sqlcmd = "insert into config1 values(" + file.ID + ",'" + file.FileName + "'," + file.FileSize + ",'" + file.CreateTime + "','" + file.ModifiedTime + "','" + file.Path + "','" + versionNum + "','" + updateMethod + "')";
                OleDbCommand command = new OleDbCommand(sqlcmd, connection);
                command.ExecuteNonQuery();
                command = null;
            }
            connection.Dispose();
            connection.Close();



        }
        private String GetUpdateMethod(int i)
        {
            FrameworkElement item = dg.Columns[0].GetCellContent(dg.Items[i]);
            DataGridTemplateColumn temp = dg.Columns[0] as DataGridTemplateColumn;
            object c = temp.CellTemplate.FindName("updateMethod", item);
            ComboBox b = c as ComboBox;
            return b.Text;
        }

    }

}

