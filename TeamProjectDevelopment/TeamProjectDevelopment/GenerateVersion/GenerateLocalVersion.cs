using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TeamProjectDevelopment.CreateConfig;
using TeamProjectDevelopment.FileSolver;

namespace TeamProjectDevelopment.GenerateVersion
{
    class GenerateLocalVersion
    {
        String localPath = ConfigurationSettings.AppSettings["localPath"];
        static String databaseCon = ConfigurationSettings.AppSettings["databaseCon"];
        static String versionFile = ConfigurationSettings.AppSettings["versionFile"];
        static String otherDbs = ConfigurationSettings.AppSettings["otherDbs"];
        static String address = ConfigurationSettings.AppSettings["address"];
        public void generateLocalVersion(String DbPath)
        {
            CopyFile copyFile = new CopyFile();

            String DbName = GetFileName.getFileName(DbPath);
            OleDbDataReader reader = CreateDb.DbConnect(DbPath);
            
            String path = versionFile;
            System.IO.Directory.CreateDirectory(path + "\\" + "本地版本" + Path.GetFileNameWithoutExtension(DbName));
            DirectoryInfo dir = new DirectoryInfo(path + "\\" + "本地版本" + Path.GetFileNameWithoutExtension(DbName));
            dir.Create();
            while (reader.Read())
            {
                String tempStr = (String)reader["path"];
                String targetDir = dir + "\\" + Path.GetFileName(tempStr);
                copyFile.copyFile(tempStr, targetDir);
            }
            copyFile.copyFile(DbPath + "\\" + DbName, dir + "\\" + DbName);
            MessageBox.Show("生成版本成功", "提示", MessageBoxButton.OK);
        }

        
    }
}
