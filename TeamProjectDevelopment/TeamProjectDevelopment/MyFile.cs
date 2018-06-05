using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamProjectDevelopment
{
    class MyFile
    {
        private int id;
        private String fileName;
        private long fileSize;
        private DateTime createTime;
        private DateTime modifiedTime;
        private String path;
        private int versionNum;
        public int ID
        {
            get { return id; }
            set { id = value; }
        }
        public String FileName
        {  get { return fileName; }
           set { fileName = value; }
        }
        public long FileSize
        {
            get { return fileSize; }
            set { fileSize = value; }
        }
        public DateTime CreateTime
        {
            get { return createTime; }
            set { createTime = value; }
        }
        public DateTime ModifiedTime
        {
            get { return modifiedTime; }
            set { modifiedTime = value; }
        }
        public String Path
        {
            get { return path; }
            set { path = value; }
        }
        public int VersionNum
        {
            get { return versionNum; }
            set { versionNum = value; }
        }
    }
}
