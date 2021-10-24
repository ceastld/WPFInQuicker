using Newtonsoft.Json;
using Quicker.Public;
using Quicker.Utilities;
using Quicker.Utilities.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace WpfApp1.ListBoxTest
{
    public class MultiColumnList
    {
        //private static Window TheWindow;
        static IDictionary<string, object> _dataContext;
        public static void OnWindowCreated(Window win, IDictionary<string, object> dataContext, ICustomWindowContext winContext)
        {
            _dataContext = dataContext;
            win.AddHandler(Selector.SelectionChangedEvent, new SelectionChangedEventHandler(ListBoxSelectionChanged));
            win.AddHandler(UIElement.MouseLeftButtonUpEvent, new MouseButtonEventHandler(MouseLeftButtonUp));
        }
        private static void MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var lb_item = UIHelper.FindParent<ListBoxItem>((DependencyObject)e.OriginalSource);
            if (lb_item == null)
            {
                return;
            }
            else
            {
                string result = JsonConvert.SerializeObject(lb_item.DataContext);
                //AppHelper.ShowInformation(result);
                var win = UIHelper.FindParent<Window>((DependencyObject)e.OriginalSource);
                if (win != null)
                {
                    var res = lb_item.DataContext as Quicker.View.OperationItem;
                    if (res == null)
                    {
                        _dataContext["result"] = lb_item.DataContext as string;
                    }
                    else
                    {
                        _dataContext["result"] = res.OriginText;
                    }
                    win.Close();
                }
            }
        }
        static object _currentSelectedList;
        private static void ListBoxSelectionChanged(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource != _currentSelectedList)
            {
                if (_currentSelectedList is ListBox)
                {
                    (_currentSelectedList as ListBox).UnselectAll();
                }
                _currentSelectedList = e.OriginalSource;
            }
        }



    }
}
