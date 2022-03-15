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

namespace WpfApp3
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            InitData();
        }
        public void InitData()
        {
            dgSimple.ItemsSource = new[]
            {
                new{ Id = 1, Name = "John Doe", Birthday = new DateTime(1971, 7, 23) },
                new{ Id = 2, Name = "bbboe", Birthday = new DateTime(1971, 5, 23) },
                new{ Id = 3, Name = "aaa", Birthday = new DateTime(1971, 6, 23) },
            };
        }
    }
}
