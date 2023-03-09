using System;
using System.Windows.Media;
using System.Linq;
using Quicker.Public;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Quicker.Utilities;
using Quicker.View.Controls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quicker.Public.Extensions;
using System.Windows.Controls.Primitives;
using System.Text.RegularExpressions;
using Quicker.Public.Interfaces;

namespace WpfApp1.View.Menu
{
    /// <summary>
    /// OneContextMenu.xaml 的交互逻辑
    /// </summary>
    public partial class OneContextMenu : Window
    {
        public OneContextMenu()
        {
            InitializeComponent();
        }
        public static Window TheWindow;
        public static ContextMenu TheMenu;
        private static IDictionary<string, object> _dataContext;
        public static ICustomWindowContext WindowContext;
        public static void OnWindowCreated(Window win, IDictionary<string, object> dataContext, ICustomWindowContext winContext)
        {
            TheWindow = win;
            WindowContext = winContext;
            _dataContext = dataContext;
            var menu = win.FindName("MyContextMenu") as ContextMenu;
            TheMenu = menu; SetMenuStartUpLocation();
            var sourceData = dataContext["sourceData"] as string;
            var sourceDataType = dataContext["sourceDataType"] as string;
            switch (sourceDataType)
            {
                case "simple":
                    MenuHelper.AddItemsToMenu(menu, MenuDataItem.CreateListFromText(sourceData));
                    break;
                case "json":
                    MenuHelper.AddItemsToMenu(menu, JsonConvert.DeserializeObject<List<MenuDataItem>>(sourceData));
                    break;
                default:
                    throw new Exception("没有指定数据类型");
            }
            var menuLayout = dataContext["menuLayout"] as string;
            switch (menuLayout)
            {
                case "Horizontal":
                    menu.Style = win.FindResource("HorizontalContextMenuStyle") as Style;
                    foreach (var item in menu.Items)
                    {
                        if (item is MenuItem)
                        {
                            (item as MenuItem).Style = win.FindResource("HorizontalMenuItemStyle") as Style;
                        }
                    }
                    menu.MaxWidth = (double)dataContext["maxWidth"];
                    break;
                case "Vertical":
                default:
                    break;
            }
            var context = dataContext["context"] as IActionContext;
            if (context != null)
            {
                //AppHelper.ShowInformation("context");
            }
            menu.AddHandler(MenuItem.ClickEvent, new RoutedEventHandler(MenuClicked));
            //win.MouseDoubleClick += (s, e) => { win.Close(); };
            win.MouseLeftButtonDown += (s, e) => { win.Close(); };
            win.Loaded += TheWindow_Loaded;
            win.MouseRightButtonDown += (s, e) =>
            {
                TheMenu.HorizontalOffset = TheWindow.Left;
                TheMenu.VerticalOffset = TheWindow.Top;
            };
        }

        private static void SetMenuStartUpLocation()
        {
            switch ((string)_dataContext["startUpLocation"])
            {
                case "CenterScreen":
                    TheMenu.Placement = PlacementMode.Center;
                    var point = Quicker.Utilities.Win32.NativeMethods.GetMousePosition();
                    double dpiScale = SnipInsight.Util.DpiUtilities.GetDpiScaleByPoint(point);
                    var workingArea = System.Windows.Forms.Screen.FromPoint(point).WorkingArea;
                    TheMenu.HorizontalOffset = workingArea.Width * 0.5 * dpiScale;
                    TheMenu.VerticalOffset = workingArea.Height * 0.5 * dpiScale;
                    break;
                case "Mouse":
                default:
                    break;
            }
        }
        private static void TheWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var scale = (double)_dataContext["scale"];
            TheMenu.LayoutTransform = new ScaleTransform(scale, scale);
            TheMenu.IsOpen = true;
        }
        private static void MenuClicked(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is MenuItem)
            {
                try
                {
                    var menuitem = e.OriginalSource as MenuItem;
                    var itemData = menuitem.DataContext as MenuDataItem;
                    ReturnResult(JToken.FromObject(itemData).ToString());
                }
                catch
                {
                    AppHelper.ShowInformation("点击菜单出错了");
                    TheWindow.Close();
                }
            }
        }
        public static class MenuHelper
        {
            public static void AddItemsToMenu(MenuBase menu, List<MenuDataItem> menuDataItems)
            {
                AddMenuItemToItems(menu.Items, menuDataItems);
            }
            public static void AddMenuItemToItems(ItemCollection items, List<MenuDataItem> dataItems)
            {
                dataItems.ForEach(x =>
                {
                    if (x.Type == MenuDataItem.MenuType.Separator)
                    {
                        items.Add(new Separator());
                        return;
                    }
                    var menuItem1 = x.ToSimpleMenuItem();
                    items.Add(menuItem1);
                    if (x.Children != null)
                    {
                        AddMenuItemToItems(menuItem1.Items, x.Children);
                    }
                });
            }
        }
        public static void ReturnResult(string result)
        {
            _dataContext["result"] = result;
            TheWindow.Close();
        }
        public class MenuDataItem
        {
            public enum MenuType
            {
                None = 0,
                Simple = 1,
                Separator = 2
            }
            //C#模块用不了初始值
            public MenuDataItem() { Type = MenuType.Simple; }
            public MenuDataItem(string icon, string name, string des, string data) : this()
            {
                Icon = icon == null ? "" : icon;
                Name = name == null ? "" : name;
                Description = des == "" ? null : des;
                Data = data == null ? "" : data;
            }
            public MenuType Type { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public string Icon { get; set; }
            public string Data { get; set; }
            public List<MenuDataItem> Children { get; set; }
            public MenuItem ToSimpleMenuItem()
            {
                return new MenuItem()
                {
                    Header = Name,
                    ToolTip = string.IsNullOrEmpty(Description) ? null : Description,
                    Icon = new IconControl() { Icon = this.Icon },
                    DataContext = this
                };
            }
            public static MenuDataItem FromText(string text)
            {
                if (Regex.IsMatch(text.Trim(), @"^-{3,}$"))
                {
                    return new MenuDataItem() { Type = MenuType.Separator };
                }
                var opItem = AppHelper.ParseOperationItem(text, true, "|");
                return new MenuDataItem(opItem.Icon, opItem.Name, opItem.Description, opItem.Key);
            }
            public static List<MenuDataItem> CreateListFromText(string source)
            {
                var list = source.SplitToList().Select(x => x.Trim()).ToList();

                var root = new MenuDataItem()
                {
                    Children = new List<MenuDataItem>()
                };

                var currentItemList = new List<MenuDataItem>() { root }; //记录父级
                var childrenList = root.Children;
                var addToChildList = false;
                foreach (string text in list)
                {
                    if (text == "{")
                    {
                        var last = childrenList.Last();
                        last.Children = childrenList = new List<MenuDataItem>();
                        currentItemList.Add(last);
                        addToChildList = true;
                        continue;
                    }
                    else if (text == "}")
                    {
                        var last = currentItemList.Last();
                        currentItemList.Remove(last);
                        childrenList = currentItemList.Last().Children;
                        continue;
                    }
                    else
                    {
                        var item = MenuDataItem.FromText(text);
                        if (addToChildList)
                        {
                            childrenList.Add(item);
                        }
                        else
                        {
                            root.Children.Add(MenuDataItem.FromText(text));
                        }
                    }
                }
                return root.Children;
            }
        }
    }
}
