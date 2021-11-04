using Quicker.Public;
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
using System.Windows.Shapes;

namespace WpfApp1.MarkDown
{
    /// <summary>
    /// MarkDownHintWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MarkDownHintWindow : Window
    {
        public MarkDownHintWindow()
        {
            InitializeComponent();
        }

        public IDictionary<string, object> dataContext = new Dictionary<string, object>()
        {
            ["width"] = 200,
            ["height"] = 200,
        };

        public static void OnWindowCreated(Window win, IDictionary<string, object> dataContext, ICustomWindowContext winContext)
        {

        }
    }
}
