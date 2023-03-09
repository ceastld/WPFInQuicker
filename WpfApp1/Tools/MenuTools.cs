using Quicker.Public;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Quicker.Domain;
using System.Windows.Interop;
using System.Linq;


namespace WpfApp1.Tools
{
    internal class MenuTools
    {


        /// <summary>
        /// 显示工具菜单
        /// </summary>
        public class QuickerToolsMenu
        {
            public static void OnWindowCreated(Window win, IDictionary<string, object> dataContext, ICustomWindowContext winContext)
            {
                var contextMenu = win.FindName("MyContextMenu") as ContextMenu;
                contextMenu.AddHandler(MenuItem.ClickEvent, new RoutedEventHandler((s, e) => { win.Close(); }));
                var handle = AppState.MainWinHandle;
                var mainWindow = HwndSource.FromHwnd(handle).RootVisual as Quicker.View.PopupWindow;
                var mainContextMenu = mainWindow.FindName("MainContextMenu1") as ContextMenu;
                var toolsItem = mainContextMenu.Items.Cast<MenuItem>().First(x => (string)x.Header == "工具");
                var allMenuItems = toolsItem.Items.Cast<MenuItem>().ToList();
                allMenuItems.ForEach(item =>
                {
                    toolsItem.Items.Remove(item);
                    contextMenu.Items.Add(item);
                });
                win.Loaded += (s, e) =>
                {
                    contextMenu.IsOpen = true;
                };
                win.Closed += (s, e) =>
                {
                    allMenuItems.ForEach(item =>
                    {
                        contextMenu.Items.Remove(item);
                        toolsItem.Items.Add(item);
                    });
                };
                win.MouseLeftButtonDown += (s, e) => { win.Close(); };
            }
        }
        public class Quicker托盘菜单
        {
            public static void OnWindowCreated(Window win, IDictionary<string, object> dataContext, ICustomWindowContext winContext)
            {
                var handle = AppState.MainWinHandle;
                var mainWindow = HwndSource.FromHwnd(handle).RootVisual as Quicker.View.PopupWindow;
                var menu = mainWindow.FindResource("NotifierContextMenu") as ContextMenu;
                win.ContextMenu = menu;
                win.Loaded += (s, e) => { menu.IsOpen = true; };
                win.Closed += (s, e) => { win.ContextMenu = null; };
                win.MouseLeftButtonDown += (s, e) => { win.Close(); };
            }
        }
    }
}
