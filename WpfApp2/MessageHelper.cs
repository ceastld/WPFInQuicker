using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp2
{
    internal static class MessageHelper
    {
        public static void ShowMessage(string message) => Process.Start($"quicker:showmessage:{message}");

        internal static void ShowMessage(object obj) => ShowMessage(obj.ToString());
    }
}
