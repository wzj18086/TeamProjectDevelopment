using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamProjectDevelopment.FileSolver;

namespace TeamProjectDevelopment.CreateConfig
{

    //创建连接数据库的类
    public class DbHelper
    {
        //获取配置文件中的路径
        static String localPath = ConfigurationSettings.AppSettings["localPath"];
        static String databaseCon = ConfigurationSettings.AppSettings["databaseCon"];
        static String serverPath = ConfigurationSettings.AppSettings["serverPath"];



        public static OleDbConnection getCon()
        {
            String localDbName = GetFileName.getFileName(localPath);
            String conStr = databaseCon + localPath + "\\" + localDbName;
            OleDbConnection connection = getConn(conStr);
            String selectString = "select * from config1";
            OleDbCommand command = new OleDbCommand(selectString, connection);
            return connection;
        }
        private static OleDbConnection getConn(String conStr)
        {
            OleDbConnection oleDbConnection = new OleDbConnection(conStr);
            return oleDbConnection;
        }
    }
}
