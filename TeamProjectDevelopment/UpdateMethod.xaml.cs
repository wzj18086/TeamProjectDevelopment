using AutomaticUpdate;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.IO;

using System.Windows;

using System.Configuration;

namespace AutomaticUpdate
{
    /// <summary>
    /// UpdateMethod.xaml 的交互逻辑
    /// </summary>
    public partial class UpdateMethod : Window
    {
        //获取配置文件中的路径
        String localPath = ConfigurationSettings.AppSettings["localPath"];
        String databaseCon = ConfigurationSettings.AppSettings["databaseCon"];
        public UpdateMethod()
        {
            InitializeComponent();
        }

        //选择整体更新
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Close();
            EntireUpdate();
            this.Close();
            mainWindow.Show();
        }

        //整体更新
        private void EntireUpdate()
        {
            String serverPath = ConfigurationSettings.AppSettings["serverAddress"];
            try
            {
                if (File.Exists("address.txt"))
                {
                    FileStream file = new FileStream("address.txt", FileMode.Open);
                    StreamReader reader1 = new StreamReader(file);
                    String temp = reader1.ReadLine();
                    serverPath = temp;
                }
            }
            catch (IOException)
            {

            }
            
            String serverDbName = MainWindow.GetFileName(serverPath);
            OleDbDataReader reader = MainWindow.DbConnect(serverPath);
            while (reader.Read())
            {
                String tempStr = (String)reader["path"];
                String originFile = serverPath+"\\\\" + (String)reader["名称"];
                Console.WriteLine("success");
                copyFile(originFile, tempStr);
            }

            //最后复制配置文件
            Console.WriteLine("success2");
            
            String localDbName = MainWindow.GetFileName(localPath);
            copyFile(serverPath + "\\" + serverDbName, localPath+"\\" + serverDbName);
            File.Delete(localPath+"\\"+localDbName);//删除本地配置文件
            MessageBox.Show("更新完成", "提示", MessageBoxButton.OK);

        }
        //部分更新
        private int PartUpdate()
        {
            int i = 0;//用来标识是否存在差异
            String localDbName = MainWindow.GetFileName(localPath);
            String localConStr = databaseCon+localPath+"\\"+localDbName;
            OleDbConnection localConnection = getConn(localConStr);
            String localSelectString = "select * from config1";
            OleDbCommand localCommand = new OleDbCommand(localSelectString, localConnection);
            localConnection.Open();
            OleDbDataReader localReader = localCommand.ExecuteReader();
            List<String> localFileNames = new List<string>();
            Console.WriteLine("part update");
            while (localReader.Read())
            {
                localFileNames.Add((String)localReader["名称"]);
                Console.WriteLine((String)localReader["名称"]);
            }
            localReader.Close();


            String serverPath = ConfigurationSettings.AppSettings["serverAddress"];
            try
            {
                if (File.Exists("address.txt"))
                {
                    FileStream file = new FileStream("address.txt", FileMode.Open);
                    StreamReader reader = new StreamReader(file);
                    String temp = reader.ReadLine();
                    serverPath = temp;
                }
            }
            catch (IOException)
            {

            }
            OleDbDataReader newLocalReader = localCommand.ExecuteReader();
            String serverDbName = MainWindow.GetFileName(serverPath); 
            OleDbDataReader serverReader = MainWindow.DbConnect(serverPath);
            while (serverReader.Read())
            {
                String serverFileName = (String)serverReader["名称"];
                Console.WriteLine(serverFileName);
                while (newLocalReader.Read())
                {
                    Console.WriteLine("localreader success");
                    String localFileName = (String)newLocalReader["名称"];
                    Console.WriteLine(localFileNames.Contains(serverFileName));
                    if (!localFileNames.Contains(serverFileName))
                    {
                        //当本地数据库不包含文件名时，直接复制服务器上的文件到本地
                        String tempStr = (String)serverReader["path"];
                        String originFile = serverPath +"\\"+ serverFileName;
                        copyFile(originFile, tempStr);

                        copyFile(serverPath + "\\" + serverDbName, localPath+"\\" + serverDbName);
                        i++;

                        Console.WriteLine("name not equals ,copy finish");
                        break;
                    }
                    else if (localFileName.Equals(serverFileName))
                    {
                        //当文件名相同时，继续比较服务器和本地文件的大小和路径，只要有不一样的，就复制文件
                        String localFileSize = (String)newLocalReader["文件大小"];
                        String localFilePath = (String)newLocalReader["path"];
                        String serverFileSize = (String)serverReader["文件大小"];
                        String serverFilePath = (String)serverReader["path"];

                       
                        if (!localFileSize.Equals(serverFileSize) || !localFilePath.Equals(serverFilePath))
                        {
                            String tempStr = (String)serverReader["path"];
                            String originFile = serverPath + serverFileName;
                            copyFile(originFile, tempStr);

                            copyFile(serverPath + "\\" + serverDbName, localPath+"\\" + serverDbName);
                            i++;
                            Console.WriteLine("name equals,copy finish");
                        }
                    }
                }
                newLocalReader.Close();
                newLocalReader = localCommand.ExecuteReader();
            }
            if(newLocalReader!=null)
                newLocalReader.Close();
            if (localReader != null)
                localReader.Close();
            if(serverReader!=null)
                serverReader.Close();
            if(localConnection!=null)
                localConnection.Close();
            MessageBox.Show("更新完成", "提示", MessageBoxButton.OK);
            return i;
        }
        private OleDbConnection getConn(String conStr)
        {

            OleDbConnection oleDbConnection = new OleDbConnection(conStr);
            return oleDbConnection;
        }

        private void copyFile(String path, String targetDir)
        {

            try
            {
                File.Copy(path, targetDir, true);
            }
            catch (IOException e)
            {
                Console.WriteLine("error");
            }

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            
            int i=PartUpdate();
            if(i!=0)
            {
                
                String localDbName = MainWindow.GetFileName(localPath);
                File.Delete(localPath+"\\" + localDbName);
            }
            this.Close();
            
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Close();
        }
    }
}
