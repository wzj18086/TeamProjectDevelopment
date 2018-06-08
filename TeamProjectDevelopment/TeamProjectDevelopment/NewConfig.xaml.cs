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
using System.Windows.Controls;
using Microsoft.Win32;

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
        static string FileAddress;
        static string FileName;
        static long FileSize;
        static DateTime LastWriteTime;
        static DateTime CreationTime;
        static String asd;
        static string SaveAddress;


        public NewConfigWindow()
        {
            InitializeComponent();
            FillDataGrid();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            insertFile();
            
            grdEmployee.ItemsSource = null;
            grdEmployee.ItemsSource = files;
            

        }

        //确定
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CreateDb();
            insertData();
            MainWindow Window = new MainWindow();
            this.Close();
            Window.Show();
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
            
            FileInfo file = new FileInfo(@"..\otherDbs\"+version+".mdb");
            
           
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

        //绑定数据
        private void FillDataGrid()
        {
            grdEmployee.ItemsSource = files;

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
            for(int i=0;i< files.Count;i++)
            {
                MyFile file = files[i];
                String updateMethod=GetUpdateMethod(i);
                String sqlcmd = "insert into config1 values(" + file.ID + ",'" + file.FileName + "'," + file.FileSize + ",'" + file.CreateTime + "','" + file.ModifiedTime + "','" + file.Path + "','" + versionNum + "','" +updateMethod +"')";
                OleDbCommand command = new OleDbCommand(sqlcmd,connection);
                command.ExecuteNonQuery();
                command = null;
            }
            connection.Dispose();
            connection.Close();
            
            

        }
        private String  GetUpdateMethod(int i)
        {
            FrameworkElement item = grdEmployee.Columns[0].GetCellContent(grdEmployee.Items[i]);
            DataGridTemplateColumn temp = grdEmployee.Columns[0] as DataGridTemplateColumn;
            object c = temp.CellTemplate.FindName("updateMethod", item);
            ComboBox b = c as ComboBox;
            return b.Text;
        }
        

    }
}
