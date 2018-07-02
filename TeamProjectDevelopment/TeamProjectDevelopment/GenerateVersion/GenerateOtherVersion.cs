using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TeamProjectDevelopment.FileSolver;

namespace TeamProjectDevelopment.GenerateVersion
{
    class GenerateOtherVersion
    {
        String localPath = ConfigurationSettings.AppSettings["localPath"];
        static String serverPath = ConfigurationSettings.AppSettings["serverAddress"];
        static String databaseCon = ConfigurationSettings.AppSettings["databaseCon"];
        static String versionFile = ConfigurationSettings.AppSettings["versionFile"];
        static String otherDbs = ConfigurationSettings.AppSettings["otherDbs"];
        static String address = ConfigurationSettings.AppSettings["address"];
        //生成其他版本
        public void generateOtherVersion()
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();


            openFileDialog.RestoreDirectory = true;
            openFileDialog.Filter = "AllFiles|*.*";
            openFileDialog.InitialDirectory = @"D:\study\vsproject\TeamProjectDevelopment\TeamProjectDevelopment\TeamProjectDevelopment\bin";
            openFileDialog.ShowDialog();
            String versionFilePath = openFileDialog.FileName;
            try
            {
                CopyFile copyFile = new CopyFile();
                String DbName = Path.GetFileName(versionFilePath);
                String conString = databaseCon + versionFilePath;
                String sql = "select * from config1";
                OleDbConnection oleDbConnection = new OleDbConnection(conString);
                OleDbCommand command = new OleDbCommand(sql, oleDbConnection);
                oleDbConnection.Open();
                OleDbDataReader reader = command.ExecuteReader();

                String path = versionFile;
                System.IO.Directory.CreateDirectory(path + "\\" + "版本" + Path.GetFileNameWithoutExtension(versionFilePath));
                DirectoryInfo dir = new DirectoryInfo(path + "\\" + "版本" + Path.GetFileNameWithoutExtension(versionFilePath));
                dir.Create();

                while (reader.Read())
                {
                    String tempStr = (String)reader["path"];
                    String targetDir = dir + "\\" + Path.GetFileName(tempStr);
                    copyFile.copyFile(tempStr, targetDir);
                }
                copyFile.copyFile(versionFilePath, dir + "\\" + DbName);
                oleDbConnection.Close();
                MessageBox.Show("生成版本成功", "提示", MessageBoxButton.OK);
            }
            catch { }


        }
    }
}
