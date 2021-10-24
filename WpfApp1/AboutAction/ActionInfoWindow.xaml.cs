using Quicker.Public;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp1.AboutAction
{
    public partial class ActionInfoWindow : HandyControl.Controls.Window
    {
        public ActionInfoWindow()
        {
            InitializeComponent();
        }
        public static void OnWindowCreated(Window win, IDictionary<string, object> dataContext, ICustomWindowContext winContext)
        {
            var paramGird = win.FindName("ParamGrid") as Grid;
            paramGird.DragOver += ParamGird_DragOver;
        }

        private static void ParamGird_DragOver(object sender, DragEventArgs e)
        {
            
        }
    }
}
