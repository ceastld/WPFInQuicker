using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WpfApp1.View.Icon
{
    internal class ImageHelper : Test.ExpEnvironment
    {
        public void convertimg()
        {
            var url = "";
            var imageMatch = "(png|gif|jpg|jpeg|svg)";
            //base64
            string iconPath = "";
            if (url.StartsWith("data:image/"))
            {
                var name = Guid.NewGuid().ToString() + "." + Regex.Match(url, @"data:image/(\w+),").Groups[1].Value;
            }
        }
    }
}
