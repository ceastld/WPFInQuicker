using Quicker.Public;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp1.View.MyTable
{
    /// <summary>
    /// DataGridTest.xaml 的交互逻辑
    /// </summary>
    public partial class DataGridTest : Window
    {
        public DataGridTest()
        {
            InitializeComponent();
        }

        public static void OnWindowCreated(Window win, IDictionary<string, object> dataContext, ICustomWindowContext winContext)
        {
            dGSimpleWrapper = new DGSimpleWrapper(win);
        }

        public static DGSimpleWrapper dGSimpleWrapper;

        public class DGSimpleWrapper
        {
            private DataGrid dgSimple;
            public DGSimpleWrapper(Window win)
            {
                dgSimple = win.FindName("dgSimple") as DataGrid;
                 
                dgSimple.ItemsSource = new[]
                {
                    new{ Id = 1, Name = "John Doe", Birthday = new DateTime(1971, 7, 23) },
                    new{ Id = 1, Name = "John Doe", Birthday = new DateTime(1971, 7, 23) },
                    new{ Id = 1, Name = "John Doe", Birthday = new DateTime(1971, 7, 23) },
                };
            }

        }

    }
}
