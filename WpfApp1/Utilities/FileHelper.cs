using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Utilities
{
    internal class FileHelper : Test.ExpEnvironment
    {
        /// <summary>
        /// 数字重命名文件，
        /// 1.png,2.png,3.png,以此类推
        /// </summary>
        public static void ReNameFilesByNumber()
        {
            List<string> files = (List<string>)_context.GetVarValue("fileList");
            int name_index = 0;
            for (int i = 0; i < files.Count; i++)
            {
                string x = files[i];
                if (!File.Exists(x)) continue;
                string dir = Path.GetDirectoryName(x);
                string ext = Path.GetExtension(x);
                while (true)
                {
                    var path_new = Path.Combine(dir, name_index + ext);
                    name_index++;
                    if (!File.Exists(path_new))
                    {
                        new FileInfo(x).MoveTo(path_new);
                        files[i] = path_new;
                        break;
                    }
                }
            }
        }
    }
}
