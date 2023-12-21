using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LineExtractor
{
    public class StorageManager
    {
        public StorageManager() 
        {
            if (!System.IO.Directory.Exists(GetStoragePath()))
            {
                System.IO.Directory.CreateDirectory(GetStoragePath());
            }
        }

        public static string GetStoragePath()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "LineExtractor");
        }
    }
}
