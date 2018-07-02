using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace TeamProjectDevelopment.CreateConfig
{
    class InsertFile
    {

        String localPath = ConfigurationSettings.AppSettings["localPath"];
        static String databaseCon = ConfigurationSettings.AppSettings["databaseCon"];
        static String versionFile = ConfigurationSettings.AppSettings["versionFile"];
        static String otherDbs = ConfigurationSettings.AppSettings["otherDbs"];
        static String address = ConfigurationSettings.AppSettings["address"];


        //新建配置文件上的导入功能
        public void insertFile(int id,List<MyFile> files)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Multiselect = false;
            openFileDialog.Filter = "AllFiles|*.*";

            if ((bool)openFileDialog.ShowDialog())
            {
                MyFile myFile = new MyFile();


                String FileAddress = openFileDialog.FileName;
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
        public void insertData(String versionNum,List<MyFile> files,DataGrid dg)
        {
            String conString = databaseCon + otherDbs + versionNum + ".mdb";
            OleDbConnection connection = new OleDbConnection(conString);
            connection.Open();
            for (int i = 0; i < files.Count; i++)
            {
                MyFile file = files[i];
                String updateMethod = GetUpdateMethod(i,dg);
                if (updateMethod.Equals(""))
                    updateMethod = file.UpdateMethod;
                String sqlcmd = "insert into config1 values(" + file.ID + ",'" + file.FileName + "'," + file.FileSize + ",'" + file.CreateTime + "','" + file.ModifiedTime + "','" + file.Path + "','" + versionNum + "','" + updateMethod + "')";
                OleDbCommand command = new OleDbCommand(sqlcmd, connection);
                command.ExecuteNonQuery();
                command = null;
            }
            connection.Dispose();
            connection.Close();
        }

        private String GetUpdateMethod(int i,DataGrid dg)
        {
            FrameworkElement item = dg.Columns[0].GetCellContent(dg.Items[i]);
            DataGridTemplateColumn temp = dg.Columns[0] as DataGridTemplateColumn;
            object c = temp.CellTemplate.FindName("updateMethod", item);
            ComboBox b = c as ComboBox;
            return b.Text;
        }
    }
}
