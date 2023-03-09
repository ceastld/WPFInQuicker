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

namespace WpfApp1.View.Widgets
{
    /// <summary>
    /// FullScreenWindow.xaml 的交互逻辑
    /// </summary>
    public partial class FullScreenWindow : Window
    {
        public FullScreenWindow()
        {
            InitializeComponent();
        }
        private void Test()
        {
            var items = Quicker.Domain.AppState.DataService.GetAllActionItems();

            items.Where(x => x.TemplateId == null)
                 .Where(x => (int)x.ActionType == 24)
                 .Count(); //自己写的组合动作

            items.Where(x => x.TemplateId != null)
                 .GroupBy(x => x.TemplateId)
                 .OrderByDescending(g => g.Count())
                 .Where(g => g.Count() > 1)
                 .Select(g => $"安装 {g.First().Title} {g.Count()} 个");
        }
    }
}
