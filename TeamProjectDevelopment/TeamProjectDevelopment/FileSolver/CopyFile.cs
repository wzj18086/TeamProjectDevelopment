using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamProjectDevelopment.FileSolver
{
    class CopyFile
    {
        public void copyFile(String path, String targetDir)
        {

            try
            {
                File.Copy(path, targetDir, true);
                Console.WriteLine("success");
            }
            catch (IOException)
            {
                Console.WriteLine("error");
            }

        }

        //没有后缀名的文件名获取
        public static String GetFileNameWithonExtension(String path)
        {
            DirectoryInfo folder = new DirectoryInfo(path);
            String DbName;
            try
            {
                DbName = Path.GetFileNameWithoutExtension(folder.GetFiles("*.mdb")[0].Name);
            }
            catch
            {
                return null;
            }

            return DbName;
        }
    }
}
