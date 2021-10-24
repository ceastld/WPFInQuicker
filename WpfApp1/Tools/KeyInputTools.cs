using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quicker.Modules.TextTools.Tools;
namespace WpfApp1.Tools
{
    public static class KeyInputTools
    {
        public static string GetKeyCode()
        {
            string keyCode = "";
            var win = new KeysInputWindow(KeysInputMode.CombinedKey);
            win.ShowDialog();
            return keyCode;
        }

    }
}
