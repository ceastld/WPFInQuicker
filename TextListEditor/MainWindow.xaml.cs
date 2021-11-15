using Quicker.Utilities._3rd;
using Quicker.Utilities.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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

namespace TextListEditor
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ViewModel.Load();
            this.DataContext = ViewModel;
            ViewModel.AddBlock("fsefefe");
            ViewModel.AddBlock("efefe\rfsefs");
        }
        public MainViewModel ViewModel { get; set; } = new MainViewModel();

        private void TheTextEditor_GotFocus(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is DependencyObject dp && UIHelper.FindParent<ListBox>(dp) is ListBox listBox)
            {
                listBox.SelectedItem = null;
            }
        }

        private void TextList_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var mdf = e.KeyboardDevice.Modifiers;
            if (mdf == ModifierKeys.None && e.Key == Key.Back)
            {
                
            }
            if (e.KeyboardDevice.Modifiers == ModifierKeys.Shift)
            {
                if(e.Key == Key.Return)
                {

                    e.Handled = true;
                }
            }
        }
    }
}
