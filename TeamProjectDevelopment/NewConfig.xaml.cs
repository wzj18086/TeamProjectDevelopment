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
        private void FillDataGrid()
        {
            
            
            String localDbName = MainWindow.GetFileName(localPath);
            String conStr = databaseCon+localPath + "\\" + localDbName;
            string CmdString = string.Empty;
            //OleDbConnection Con = new OleDbConnection(ConString);
            //Con.Open();
            using (OleDbConnection con = new OleDbConnection(conStr))
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
        static string FileName ;
        static long FileSize ;
        static DateTime LastWriteTime ;
        static DateTime CreationTime ;

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
           
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
            String conStr = databaseCon+localPath + "\\" + localDbName;
            OleDbConnection con = new OleDbConnection(conStr);
            con.Open();            
            OleDbCommand cmd = new OleDbCommand();
            cmd.Connection = con;

            string sqlcmd2 = "select MAX(id) From config1";
            cmd.CommandText = sqlcmd2;
            int num =Convert.ToInt32(cmd.ExecuteScalar());
            Console.WriteLine(num);
            cmd.Dispose();
            con.Close();
            con.Dispose();
            con = new OleDbConnection(conStr);
            con.Open();
            cmd = new OleDbCommand();
            cmd.Connection = con;
            string sqlcmd1 = "insert into config1 values(" + ++num + ",'" + FileName + "'," + FileSize + ",'" + CreationTime + "','" + LastWriteTime + "','" + FileAddress + "')";
            cmd.CommandText = sqlcmd1;
            FileAddress = "0";
            cmd.ExecuteNonQuery();
            cmd.Dispose();
            con.Close();
            con.Dispose();
            MainWindow Window = new MainWindow();
            this.Close();
            Window.Show();

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
