using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using System.Xml;

namespace WpfApp1.Utilities
{
    internal class XamlClass
    {
        public void LoadXamlToAppxaml()
        {
            string resource = "";
            var strR = new System.IO.StringReader(resource);
            var xmlR = XmlReader.Create(strR);
            var resourceDictionary = XamlReader.Load(xmlR) as ResourceDictionary;
            Application.Current.Resources.MergedDictionaries.Add(resourceDictionary);

        }
        public static T CreateXamlObject<T>(string str1) where T : class
        {
            StringReader str = new StringReader(str1);
            XmlReader xmlReader = XmlReader.Create(str);
            return XamlReader.Load(xmlReader) as T;
        }
    }
}
