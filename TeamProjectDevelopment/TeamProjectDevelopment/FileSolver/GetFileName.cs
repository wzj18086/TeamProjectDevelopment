using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamProjectDevelopment.FileSolver
{
    class GetFileName
    {
        //获取文件名字
        public static String getFileName(String path)
        {
            DirectoryInfo folder = new DirectoryInfo(path);
            String DbName;
            try
            {
                DbName = Path.GetFileName(folder.GetFiles("*.mdb")[0].Name);
            }
            catch
            {
                return null;
            }

            return DbName;
        }

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
