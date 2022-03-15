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

namespace WpfApp2
{
    /// <summary>
    /// Window1.xaml 的交互逻辑
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            IsUsingCustomTextTool = true;
        }


        public bool IsUsingCustomTextTool
        {
            get { return (bool)GetValue(IsUsingCustomTextToolProperty); }
            set { SetValue(IsUsingCustomTextToolProperty, value); }
        }
        public static readonly DependencyProperty IsUsingCustomTextToolProperty = DependencyProperty.Register("IsUsingCustomTextTool", typeof(bool), typeof(Window1), new PropertyMetadata(false));
    }
}
