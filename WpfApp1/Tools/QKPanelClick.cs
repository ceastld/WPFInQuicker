using System.Collections.Generic;
using System.Linq;
using Quicker.Public.Extensions;
using Quicker.Domain;
using Quicker.View;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;

namespace WpfApp1.Tools
{
    /// <summary>
    /// 点击面板上面的锁定，盯住按钮
    /// </summary>
    class QKPanelClick
    {
        public Quicker.Public.Interfaces.IActionContext _context = null;
        public static void Exec(Quicker.Public.IStepContext context)
        {
            var handle = AppState.MainWinHandle;
            var window = HwndSource.FromHwnd(handle).RootVisual as PopupWindow;
            var btn_pin = window.FindName("BtnPin") as Button;
            var btn_lock = window.FindName("BtnToggleLock") as Button;
            btn_lock.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            btn_pin.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            window.Show();
        }
        void windowtest()
        {
            var window = new Window();
            window.Content = new Button() { Name = "testButton" };
            window.ShowDialog();
            var btn = window.FindName("testButton") as Button;
        }
        IEnumerable<string> GenerateTypeName()
        {
            var dict = new Dictionary<string, object>();
            //const string splitter = "|";
            var usings = "";
            var usingList = usings.SplitToList();
            return usingList.Select(x =>
            {
                if (x.StartsWith("using"))
                    x = x.Substring("using".Length);
                x = x.Trim(';', ' ');
                dict.Keys.ToList().ForEach(key =>
                {
                    if (x.Contains(key))
                        x = x + "| " + dict[key].ToString();
                });
                return x;
            });
        }
    }
}
