
using ADOX;
using System;
using System.Configuration;
using System.Data.OleDb;

using System.Windows;
using TeamProjectDevelopment.FileSolver;

namespace TeamProjectDevelopment.CreateConfig
{
    class CreateDb
    {
        String localPath = ConfigurationSettings.AppSettings["localPath"];
        static String databaseCon = ConfigurationSettings.AppSettings["databaseCon"];
        static String versionFile = ConfigurationSettings.AppSettings["versionFile"];
        static String otherDbs = ConfigurationSettings.AppSettings["otherDbs"];
        static String address = ConfigurationSettings.AppSettings["address"];
        //创建数据库
        public void createDb(String versionNum)
        {

            ADOX.Catalog catalog = new ADOX.Catalog();
            //String versionNum = extension.Text;
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

        public static OleDbDataReader DbConnect(String path)
        {
            String DbName = GetFileName.getFileName(path);
            String conStr = databaseCon + path + "\\" + DbName;
            OleDbConnection connection = getConn(conStr);
            String selectString = "select * from config1";
            OleDbCommand command = new OleDbCommand(selectString, connection);
            connection.Open();
            OleDbDataReader reader = command.ExecuteReader();

            return reader;
        }

        public static OleDbConnection getConn(String conStr)
        {
            OleDbConnection oleDbConnection = new OleDbConnection(conStr);
            return oleDbConnection;
        }
    }
}
