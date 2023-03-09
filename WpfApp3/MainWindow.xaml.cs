using PInvoke;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
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
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ThePopUp.IsOpen = true;
            IntPtr handle = ((HwndSource)PresentationSource.FromVisual(ThePopUp.Child)).Handle;
            Trace.WriteLine(handle);
            MoveWindow(handle, 100, 100);
        }

        public static void MoveWindow(IntPtr handle, int x, int y) => User32.SetWindowPos(handle, IntPtr.Zero, x, y, 0, 0, User32.SetWindowPosFlags.SWP_DRAWFRAME | User32.SetWindowPosFlags.SWP_NOACTIVATE | User32.SetWindowPosFlags.SWP_NOSIZE);

    }
}
