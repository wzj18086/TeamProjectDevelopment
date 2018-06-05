using AutomaticUpdate;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Configuration;
using TeamProjectDevelopment;
using ADOX;

namespace AutomaticUpdate
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    
    public partial class NewConfigWindow : Window
    {
        static List<MyFile> files = new List<MyFile>();
        int id = 0;
        String localPath = ConfigurationSettings.AppSettings["localPath"];
        String databaseCon = ConfigurationSettings.AppSettings["databaseCon"];
        String otherDbs= ConfigurationSettings.AppSettings["otherDbs"];
        static String address = ConfigurationSettings.AppSettings["address"];
        public NewConfigWindow()
        {
            InitializeComponent();
            FillDataGrid();
        }


        int n = 2;
        private void FillDataGrid()
        {
            grdEmployee.ItemsSource = files;

        }
        static string FileAddress;
        static string FileName;
        static long FileSize;
        static DateTime LastWriteTime;
        static DateTime CreationTime;
        static String asd;
        static string SaveAddress;

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            insertFile();
            grdEmployee.ItemsSource = null;
            grdEmployee.ItemsSource = files;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CreateDb();
            insertData();
            MainWindow Window = new MainWindow();
            this.Close();
            Window.Show();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Multiselect = false;
            openFileDialog.Filter = "AllFiles|*.*";
            openFileDialog.ShowDialog();
            SaveAddress = openFileDialog.FileName;
            //string ConString = System.Configuration.ConfigurationManager.ConnectionStrings["ConString"].ConnectionString;
            FileInfo file = new FileInfo(SaveAddress);
            file.CopyTo("D:\\3.mdb");
        }

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
                System.IO.FileInfo file = new System.IO.FileInfo(FileAddress);
                myFile.ID = id;
                myFile.FileName = file.Name;
                myFile.FileSize = file.Length;
                myFile.CreateTime = file.CreationTime;
                myFile.ModifiedTime = file.LastWriteTime;
                id++;
                files.Add(myFile);
            }
        }
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            CreateDb();
            insertData();
        }
        private void insertData()
        {
            String versionNum = extension.Text;
            String conString = databaseCon + otherDbs + versionNum + ".mdb";
            OleDbConnection connection = new OleDbConnection(conString);
            connection.Open();
            foreach(MyFile file in files)
            {
                String sqlcmd = "insert into config1 values(" + file.ID + ",'" + file.FileName + "'," + file.FileSize + ",'" + file.CreateTime + "','" + file.ModifiedTime + "','" + file.Path + "','" + versionNum + "')";
                OleDbCommand command = new OleDbCommand(sqlcmd,connection);
                command.ExecuteNonQuery();   
            }
            connection.Close();
            

        }
        
    }
}
