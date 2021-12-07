using Quicker.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WpfApp1.Tools
{
    internal class Keyboarda
    {
        public bool GetKeyBoardList()
        {
            var keyDownList = new List<string>();
            if (!KeyboardHelper.IsAnyKeyDown()) return false;
            for (int i = 1; i < Enum.GetValues(typeof(Keys)).Length; i++)
            {
                Keys key = (Keys)((System.Collections.IList)Enum.GetValues(typeof(Keys)))[i];
                if (KeyboardHelper.IsKeyDown((WindowsInput.Native.VirtualKeyCode)key))
                {
                    keyDownList.Add(key.ToString());
                }
            }
            return true;
        }
    }
}
