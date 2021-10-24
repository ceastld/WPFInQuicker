using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp1
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

        public static string test()
        {
            string text = "";
            text = text.Trim();
            int index = text.IndexOf("\r\n");
            var line = text.Substring(0, index);
            var leftover = text.Substring(index);
            return line.Substring(0, line.IndexOf("x:Class")) + leftover;

        }
    }
}
