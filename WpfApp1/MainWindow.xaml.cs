using FontAwesome5.WPF;
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
            //SvgAwesome

        }

        private void btnTest_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            new string('f', 49);
            "fef".Substring(1);

        }
        public void listtest()
        {
            Action<List<string>, string> pad = (l, p) =>
             {

                 var indexList = l.Select(x => x.IndexOf(p)).ToList();
                 var max = indexList.Max();
                 if (max <= 0) return;
                 for (int i = 0; i < l.Count; i++)
                 {
                     var index = indexList[i];
                     if (index > 0)
                         l[i] = l[i].Insert(index, new string(' ', max - index));
                 }
             };

            var action = (Action<IntPtr>)(handle =>
            {
                var window = System.Windows.Interop.HwndSource.FromHwnd(handle).RootVisual as Window;
                window.Activate();
            });
            var keys = Enum.GetValues(typeof(System.Windows.Forms.Keys)).Cast<System.Windows.Forms.Keys>().Skip(1).ToArray();
            foreach (var key in Enum.GetValues(typeof(System.Windows.Forms.Keys)))
            {

            }
        }
    }
}
