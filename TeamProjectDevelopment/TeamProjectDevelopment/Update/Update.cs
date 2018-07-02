using AutomaticUpdate;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.OleDb;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TeamProjectDevelopment.CreateConfig;
using TeamProjectDevelopment.FileSolver;

namespace TeamProjectDevelopment.Update
{
    public class Update
    {
        String localPath = ConfigurationSettings.AppSettings["localPath"];
        static String serverPath = ConfigurationSettings.AppSettings["serverAddress"];
        static String databaseCon = ConfigurationSettings.AppSettings["databaseCon"];
        static String versionFile = ConfigurationSettings.AppSettings["versionFile"];
        static String otherDbs = ConfigurationSettings.AppSettings["otherDbs"];
        static String address = ConfigurationSettings.AppSettings["address"];
        int localVersion;
        int serverVersion;
        String softwareName= Path.GetFileName(Assembly.GetEntryAssembly().Location);
        public void update(Window oldWindow)
        {
            //判断文件是否存在

            CopyFile copyFile = new CopyFile();
            String serverDbName = GetFileName.getFileName(serverPath);
            OleDbDataReader reader = CreateDb.DbConnect(serverPath);
            while (reader.Read())
            {
                String tempStr = (String)reader["fileName"];
                String originFile = serverPath + "\\" + tempStr;
                String updateMethod = (String)reader["updateMethod"];
                if (updateMethod.Equals("删除") && File.Exists(localPath + "\\" + tempStr))
                {
                    File.Delete(localPath + "\\" + tempStr);
                }
                else
                    copyFile.copyFile(originFile, localPath + "\\" + tempStr);
            }
            if (reader != null)
                reader.Close();

            //最后复制服务器配置文件到本地，并删除原来的配置文件
            String localDbName = GetFileName.getFileName(localPath);
            copyFile.copyFile(serverPath + "\\" + serverDbName, localPath + "\\" + localDbName);
            /*
            if (serverDbName != localDbName)
                File.Delete(localPath + "\\" + localDbName);
                */
            SuccessTips(oldWindow);
        }
        public void SuccessTips(Window oldWindow)
        {
            MessageBoxResult result = MessageBox.Show("更新完成", "提示", MessageBoxButton.OK);

            if (result == MessageBoxResult.OK)
            {
                String name = Path.GetFileName(Assembly.GetEntryAssembly().Location);
                Console.WriteLine("program_name:"+name);
                Process p = new Process();
                p.StartInfo.FileName = System.AppDomain.CurrentDomain.BaseDirectory + name;
                p.StartInfo.UseShellExecute = false;
                p.Start();
                Application.Current.Shutdown();

            }
        }
        public bool AntoUpdateOrNot()
        {
            //判断文件是否存在

            String serverDbName = GetFileName.getFileName(serverPath);
            if (serverDbName == null)
            {
                return false;
            }
            OleDbDataReader serverReader = CreateDb.DbConnect(serverPath);
            while (serverReader.Read())
            {
                if (serverReader["fileName"].Equals(softwareName))
                {
                    serverReader.Close();
                    return true;
                }
            }
            serverReader.Close();
            return false;
        }

        //软件自己更新自己
        public void AutoUpdate()
        {
            Process p = new Process();
            p.StartInfo.FileName = System.AppDomain.CurrentDomain.BaseDirectory + softwareName;
            p.StartInfo.UseShellExecute = false;
            p.Start();
            Application.Current.Shutdown();
        }

        //判断是否需要更新
        public bool UpdateOrNot()
        {
            String serverDbName = GetFileName.getFileName(serverPath);
            if (serverDbName == null)
                return false;
            //String DbName = GetFileName.getFileName(serverPath);
            String serverConStr = databaseCon + serverPath + "\\" + serverDbName;
            OleDbConnection serverConnection = new OleDbConnection(serverConStr);
            String serverSelectString = "select * from config1";
            OleDbCommand serverCommand = new OleDbCommand(serverSelectString, serverConnection);
            serverConnection.Open();
            OleDbDataReader serverReader = serverCommand.ExecuteReader();


            //OleDbDataReader serverReader = CreateDb.DbConnect(serverPath);
            //OleDbDataReader localReader = CreateDb.DbConnect(localPath);

            String localDbName = GetFileName.getFileName(localPath);
            String localConStr = databaseCon + localPath + "\\" + localDbName;
            OleDbConnection localConnection = new OleDbConnection(localConStr);
            String localSelectString = "select * from config1";
            OleDbCommand localCommand = new OleDbCommand(localSelectString, localConnection);
            localConnection.Open();
            OleDbDataReader localReader = localCommand.ExecuteReader();

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
            if (serverReader.Read())
            {
                serverVersion = (int)serverReader["versionNum"];
            }
            Console.WriteLine("localVersion:"+localVersion);
            Console.WriteLine("serverVersion:"+serverVersion);
            if (localVersion < serverVersion)
            {
                serverReader.Close();
                localReader.Close();
                localConnection.Close();
                serverConnection.Close();

                return true;
            }
            else
            {
                serverReader.Close();
                localReader.Close();
                localConnection.Close();
                serverConnection.Close();
                return false;
            }


        }

        public void SoftwareUpdate()
        {
            String filename = Assembly.GetExecutingAssembly().Location;
            File.Move(filename, filename + ".delete");
            File.Copy(serverPath + "\\" + softwareName, filename);

            String DbName = GetFileName.getFileName(serverPath);
            String conStr = databaseCon + serverPath + "\\" + DbName;
            OleDbConnection connection = CreateDb.getConn(conStr);
            connection.Open();
            String selectString = "delete from config1 where fileName=\""+softwareName+"\"";
            OleDbCommand command = new OleDbCommand(selectString, connection);
            command.ExecuteNonQuery();
            connection.Close();

            AutoUpdate();
        }

        public void IfUpdate(Window oldWindow)
        {

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
                    File.Copy(serverPath + "\\" + softwareName, filename);

                    String DbName = GetFileName.getFileName(serverPath);
                    String conStr = databaseCon + serverPath + "\\" + DbName;
                    OleDbConnection connection = CreateDb.getConn(conStr);
                    connection.Open();
                    String selectString = "delete from config1 where fileName=\""+softwareName+"\"";
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
                    update.update(oldWindow);
                }
            }
        }
    }
}
