using Quicker.Public;
using Quicker.Public.Entities;
using Quicker.View.Controls;
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

namespace WpfApp1.View.Menu
{
    /// <summary>
    /// DictListMenu.xaml 的交互逻辑
    /// </summary>
    public partial class DictListMenu : Window
    {
        public DictListMenu()
        {
            InitializeComponent();
        }
        public static void OnWindowCreated(Window win, IDictionary<string, object> dataContext, ICustomWindowContext winContext)
        {

        }

        public class MenuHelper
        {
            public static MenuItem CreateMenuFromCpItem(CommonOperationItem item)
            {
                var mi = new MenuItem()
                {
                    Header = item.Title,
                    ToolTip = item.Description,
                    DataContext = item,
                };
                if (!string.IsNullOrEmpty(item.Icon))
                    mi.Icon = new IconControl() { Icon = item.Icon };
                return mi;
            }
        }
    }
}
