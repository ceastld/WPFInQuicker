using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;

namespace WpfApp2
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string resource = TheTextBox.Text;
            var strR = new System.IO.StringReader(resource);
            var xmlR = XmlReader.Create(strR);
            var resourceDictionary = XamlReader.Load(xmlR) as ResourceDictionary;

            Application.Current.Resources.MergedDictionaries.Add(resourceDictionary);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            dynamic a = new JObject();
            a.Add("aa", "bbb");
            TheTextBox.Text = a.aa;
            dynamic b = new
            {
                aa = "fef",
                bb = "fesfe",
                dc = "fefe"
            };
            var c = b.aa;
            var d = b.bb;

        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MessageHelper.ShowMessage(e.RemovedItems.Count);
            MessageHelper.ShowMessage(e.AddedItems[0].ToString());
        }

        private void CheckComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MessageHelper.ShowMessage(e.OriginalSource);
        }
    }
}
