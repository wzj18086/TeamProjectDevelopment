using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Data.OleDb;
using System.Data;
using System.Collections.ObjectModel;
using System.IO;
using System.Configuration;
using TeamProjectDevelopment;
using System.Reflection;

using TeamProjectDevelopment;

using TeamProjectDevelopment.GenerateVersion;
using TeamProjectDevelopment.CreateConfig;
using TeamProjectDevelopment.Update;
using TeamProjectDevelopment.FileSolver;

public class ConfigFile
{
    public String Filename { get; set; }
    public String VersionNum { get; set; }
    public String UpdateType { get; set; }
    public List<String> UpdateFiles { get; set; }
    public String Path { get; set; }
    public int LastModified { get; set; }
}




namespace AutomaticUpdate
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
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
            Update update = new Update();
            String serverDbName = GetFileName.GetFileNameWithonExtension(serverPath);
            if (serverDbName == null)
                return;
            String localDbName = GetFileName.GetFileNameWithonExtension(localPath);
            if (update.AntoUpdateOrNot())
            {
                MessageBoxResult result = MessageBox.Show("当前软件需要更新", "提示", MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.OK)
                {
                    String filename = Assembly.GetExecutingAssembly().Location;
                    File.Move(filename, filename + ".delete");
                    File.Copy(serverPath + "\\" + "TeamProjectDevelopment.exe", filename);

                    String DbName = GetFileName.getFileName(serverPath);
                    String conStr = databaseCon + serverPath + "\\" + DbName;
                    OleDbConnection connection = CreateDb.getConn(conStr);
                    connection.Open();
                    String selectString = "delete from config1 where fileName=\"TeamProjectDevelopment.exe\"";
                    OleDbCommand command = new OleDbCommand(selectString, connection);
                    command.ExecuteNonQuery();
                    connection.Close();

                    update.AutoUpdate();
                }

            }
            if (update.UpdateOrNot())
            {
                MessageBoxResult result = MessageBox.Show("当前有版本更新，需要更新吗？", "更新", MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.OK)
                {
                    update.update(this);
                }
            }


        }

        private void Import(object sender, RoutedEventArgs e)
        {
            import import = new import();
            import.Show();
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
            GenerateLocalVersion generateLocalVersion = new GenerateLocalVersion();
            generateLocalVersion.generateLocalVersion(localPath);
        }

        //选择其他版本生成
        private void OtherVersionClick(object sender, RoutedEventArgs e)
        {
            GenerateOtherVersion generateOtherVersion = new GenerateOtherVersion();
            generateOtherVersion.generateOtherVersion();
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
                connection.Close();

                DataRow dr = dataTable.Tables[0].NewRow();

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
            
            InsertFile insertFile = new InsertFile();
            insertFile.insertFile(id,files);

            dg.ItemsSource = null;
            dg.ItemsSource = files;
            id++;


        }

        //确定
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            String versionNum = extension.Text;
            CreateDb createDb = new CreateDb();
            createDb.createDb(versionNum);
            InsertFile insertFile = new InsertFile();
            insertFile.insertData(versionNum,files,dg);
        }

        //另存为
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
            //openFileDialog.Multiselect = false;
            saveFileDialog.Filter = "mdb|*.*";
            saveFileDialog.ShowDialog();
            SaveAddress = saveFileDialog.FileName;

            CreateDb createDb = new CreateDb();
            createDb.createDb(extension.Text);
            InsertFile insertFile = new InsertFile();
            insertFile.insertData(extension.Text, files, dg);
            String version = extension.Text;

            FileInfo file = new FileInfo(@"..\otherDbs\" + version + ".mdb");


            file.CopyTo(SaveAddress);
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {

        }

        private void Import(object sender, RoutedEventArgs e)
        {
            import import = new import();
            import.Show();
        }
    }

}

