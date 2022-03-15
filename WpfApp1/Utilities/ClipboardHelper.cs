using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows;

namespace WpfApp1.Utilities
{
    internal class ClipboardHelper : Test.ExpEnvironment
    {
        public void WriteToClipboard()
        {
            #region 赋值
            string text = (string)_context.GetVarValue("text");
            Image image = (Image)_context.GetVarValue("image");
            var files = (_context.GetVarValue("files") as List<string>).ToArray();
            #endregion
            var dataobj = new DataObject();
            if (!string.IsNullOrEmpty(text)) dataobj.SetText(text);
            //if (image != null) dataobj.SetImage(image);

        }
    }
}
