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


namespace AutomaticUpdate
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class NewConfigWindow : Window
    {
        String localPath = ConfigurationSettings.AppSettings["localPath"];
        String databaseCon = ConfigurationSettings.AppSettings["databaseCon"];
        public NewConfigWindow()
        {
            InitializeComponent();
            FillDataGrid();
        }


        int n = 2;
        private void FillDataGrid()
        {
            string ConString = System.Configuration.ConfigurationManager.ConnectionStrings["ConString"].ConnectionString;
            string CmdString = string.Empty;
            using (OleDbConnection con = new OleDbConnection(ConString))
            {
                CmdString = "SELECT * FROM config1";
                OleDbCommand cmd = new OleDbCommand(CmdString, con);
                OleDbDataAdapter sda = new OleDbDataAdapter(cmd);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                grdEmployee.ItemsSource = dt.DefaultView;
            }

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
            asd = extension.Text;
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
            string ConString = System.Configuration.ConfigurationManager.ConnectionStrings["ConString"].ConnectionString;
            OleDbConnection con = new OleDbConnection(ConString);
            con.Open();
            OleDbCommand cmd = new OleDbCommand();
            cmd.Connection = con;

            string sqlcmd2 = "select MAX(id) From config1";
            string str = "create table config" + n++ + "(id counter,名称 varchar(80),文件大小 varchar(80),创建日期 datetime,修改日期 datetime,路径 varchar(100),版本号 varchar(80));";

            cmd = new OleDbCommand(str, con);
            cmd.ExecuteNonQuery();
            cmd.CommandText = sqlcmd2;
            int num = Convert.ToInt32(cmd.ExecuteScalar());
            Console.WriteLine(num);
            cmd.Dispose();
            con.Close();
            con.Dispose();
            con = new OleDbConnection(ConString);
            con.Open();
            cmd = new OleDbCommand();
            cmd.Connection = con;
            string sqlcmd1 = "insert into config1 values(" + ++num + ",'" + FileName + "'," + FileSize + ",'" + CreationTime + "','" + LastWriteTime + "','" + FileAddress + "','" + asd + "')";
            cmd.CommandText = sqlcmd1;
            FileAddress = "0";
            cmd.ExecuteNonQuery();
            cmd.Dispose();
            con.Close();
            con.Dispose();
            NewConfigWindow Window = new NewConfigWindow();
            this.Close();
            Window.Show();
        }
            private void Button_Click(object sender, RoutedEventArgs e)
        {
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
    }
}
