using AutomaticUpdate;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.IO;

using System.Windows;

using System.Configuration;
using System.Text;

namespace AutomaticUpdate
{
    /// <summary>
    /// UpdateMethod.xaml 的交互逻辑
    /// </summary>
    public partial class UpdateMethod : Window
    {
        //获取配置文件中的路径
        static String localPath = ConfigurationSettings.AppSettings["localPath"];
        static String databaseCon = ConfigurationSettings.AppSettings["databaseCon"];
        static String serverPath = ConfigurationSettings.AppSettings["serverAddress"];
        static String address = ConfigurationSettings.AppSettings["address"];
        public UpdateMethod()
        {
            InitializeComponent();
        }

        //选择整体更新
        private void Button_Click(object sender, RoutedEventArgs e)
        {   
            EntireUpdate();
        }

        //选择部分更新
    
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            PartUpdate();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        //整体更新具体实现
        private void EntireUpdate()
        {
            String serverDbName = MainWindow.GetFileName(serverPath);
            OleDbDataReader reader = MainWindow.DbConnect(serverPath);
            while (reader.Read())
            {
                String tempStr = (String)reader["path"];
                String originFile = serverPath+"\\" + (String)reader["fileName"];
                Console.WriteLine("success");
                copyFile(originFile, tempStr);
            }
            if(reader!=null)
                reader.Close();

            //最后复制服务器配置文件到本地，并删除原来的配置文件
            Console.WriteLine("success2");
            

            String localDbName = MainWindow.GetFileName(localPath);
            copyFile(serverPath + "\\" + serverDbName, localPath + "\\" + serverDbName);
            //File.Delete(localPath + "\\" + localDbName);
            SuccessTips();


        }
        //部分更新具体实现
        private void PartUpdate()
        {
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
                localFileNames.Add((String)localReader["fileName"]);
                Console.WriteLine((String)localReader["fileName"]);
            }
            localReader.Close();

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
                String serverFileName = (String)serverReader["fileName"];
                Console.WriteLine(serverFileName);
                while (newLocalReader.Read())
                {
                    Console.WriteLine("localreader success");
                    String localFileName = (String)newLocalReader["fileName"];
                    Console.WriteLine(localFileNames.Contains(serverFileName));
                    if (!localFileNames.Contains(serverFileName))
                    {
                        //当本地数据库不包含文件名时，直接复制服务器上的文件到本地
                        String tempStr = (String)serverReader["path"];
                        String originFile = serverPath +"\\"+ serverFileName;
                        copyFile(originFile, tempStr);

                        Console.WriteLine("name not equals ,copy finish");
                        break;
                    }
                    else if (localFileName.Equals(serverFileName))
                    {
                        //当文件名相同时，继续比较服务器和本地文件的大小和路径，只要有不一样的，就复制文件
                        String localFileSize = (String)newLocalReader["fileSize"];
                        String localFilePath = (String)newLocalReader["path"];
                        String serverFileSize = (String)serverReader["fileSize"];
                        String serverFilePath = (String)serverReader["path"];

                       
                        if (!localFileSize.Equals(serverFileSize) || !localFilePath.Equals(serverFilePath))
                        {
                            String tempStr = (String)serverReader["path"];
                            String originFile = serverPath + serverFileName;
                            copyFile(originFile, tempStr);
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
            copyFile(serverPath + "\\" + serverDbName, localPath + "\\" + serverDbName);
            //File.Delete(localPath + "\\" + localDbName);

            SuccessTips();
        }
        //连接数据库
        private OleDbConnection getConn(String conStr)
        {

            OleDbConnection oleDbConnection = new OleDbConnection(conStr);
            return oleDbConnection;
        }
        //复制文件
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

        //更新完之后提示
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
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
        }
    }
}
