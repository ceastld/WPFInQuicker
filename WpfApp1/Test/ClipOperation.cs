using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApp1.Test
{
    public class ClipOperation
    {
        public static object GetClipBoardData()
        {
            string type = "";
            object data = null;
            if (Clipboard.ContainsText())
            {
                type = "text"; data = Clipboard.GetText();
            }
            else if (Clipboard.ContainsFileDropList())
            {
                type = "file";
                data = string.Join("\r\n", Clipboard.GetFileDropList().Cast<string>());
            }
            return new { type, data };
        }

    }
}
