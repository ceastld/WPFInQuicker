using Newtonsoft.Json;
using Quicker.Public;
using Quicker.Public.Entities;
using Quicker.Public.Extensions;
using Quicker.Public.Interfaces;
using Quicker.Utilities;
using Quicker.View.Controls;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace WpfApp1.View.MyMenu
{
    /// <summary>
    /// FileMenuWindow.xaml 的交互逻辑
    /// </summary>
    public partial class FileMenuWindow : Window
    {
        public static void OnWindowCreated(Window win, IDictionary<string, object> dataContext, ICustomWindowContext winContext)
        {
            //var cm = new ContextMenu();
            var cm = win.FindName("TheWindowContextMenu") as ContextMenu;
            //win.ContextMenu = cm;

            TheWindow = win;
            var files = dataContext["files"] as IEnumerable<string>;
            cm.StaysOpen = true;
            cm.Closed += (s, e) => win.Close();
            //win.LostFocus += (s, e) => win.Close();
            win.Loaded += (s, e) =>
            {
                cm.IsOpen = true;
                if (cm.Items.Count == 1)
                {
                    var mw = (cm.Items[0] as MenuItem).DataContext as FileMenuItemWrapper;
                    mw.CreateChildItems(mw.FilePath);
                    (cm.Items[0] as MenuItem).IsSubmenuOpen = true;
                }
            };
            MenuHelper.ParseDataContext(dataContext);

            #region add style
            var style = new Style
            {
                TargetType = typeof(IconControl)
            };
            var pw = MenuHelper.pw;
            style.Setters.Add(new Setter(Control.HeightProperty, pw.IconSize));
            style.Setters.Add(new Setter(Control.WidthProperty, pw.IconSize));
            cm.Resources.Add(typeof(IconControl), style);
            var menuStyle = cm.FindResource("MyMenuItemStyle") as Style;
            menuStyle.Setters.Add(new Setter(Control.FontSizeProperty, pw.menuFontSize));
            #endregion

            MenuHelper.AddMenuItem(cm, files);
        }
        public static Window TheWindow;
        public static class MenuHelper
        {
            public class ParamIn
            {
                public bool handled = false;
                public string fileSearchParttern = "*";
                public bool showHiddenFiles = false;
                public bool showExtension = true;
                public int limitCount = 30;
                public FileShowType showType;
                public enum FileShowType
                {
                    dir,
                    file,
                    dir_file,
                }

                public ClickAction clickAction;
                public enum ClickAction
                {
                    open,
                    callback,
                }
                public bool splitDirAndFile = false;
                public SortByType sortType;
                public enum SortByType
                {
                    NA = 0,
                    name,
                    createTime,
                    accessTime,
                    editTime,
                }
                public List<string> rightMenu;
                public bool quickLoadSubmenu = false;
                public double menuFontSize = 12;
                //[Obsolete("menuitem icon width 写死了，设置这个没用")]
                public double IconSize
                {
                    get { return Math.Max(20, menuFontSize * 1.3); }
                    set { }
                }
                public double MenuFontSize
                {
                    get
                    {
                        return Math.Max(10, menuFontSize);
                    }
                    set { }
                }
                public FileAttributes FileFilter;
                public void Init()
                {
                    FileFilter |= FileAttributes.System;
                    if (!pw.showHiddenFiles)
                        FileFilter |= FileAttributes.Hidden;
                }
            }
            public static ParamIn pw = new ParamIn(); //参数包装器

            #region for file
            public static IEnumerable<string> FileOrderByType(IEnumerable<string> files)
            {
                switch (pw.sortType)
                {
                    case ParamIn.SortByType.createTime:
                        return files.OrderByDescending(x => new FileInfo(x).CreationTime);
                    case ParamIn.SortByType.accessTime:
                        return files.OrderByDescending(x => new FileInfo(x).LastAccessTime);
                    case ParamIn.SortByType.editTime:
                        return files.OrderByDescending(x => new FileInfo(x).LastWriteTime);
                    case ParamIn.SortByType.NA:
                    case ParamIn.SortByType.name:
                    default:
                        return files;
                }
            }
            public static IEnumerable<string> GetEntries(string path)
            {
                var fsp = pw.fileSearchParttern;
                //new FileInfo(path).Attributes
                IEnumerable<string> entries;
                switch (pw.showType)
                {
                    case ParamIn.FileShowType.dir:
                        entries = Directory.EnumerateDirectories(path, fsp); break;
                    case ParamIn.FileShowType.file:
                        entries = Directory.EnumerateFiles(path, fsp); break;
                    case ParamIn.FileShowType.dir_file:
                        if (pw.splitDirAndFile)
                        {
                            var ed = FileOrderByType(Directory.EnumerateDirectories(path, fsp));
                            var ef = FileOrderByType(Directory.EnumerateFiles(path, fsp));
                            entries = ed.Concat(ef);
                        }
                        else
                        {
                            entries = FileOrderByType(Directory.EnumerateFileSystemEntries(path, fsp));
                        }
                        break;
                    default:
                        return Array.Empty<string>();
                }
                entries = entries.Where(FilterByAttr);
                return entries.ToList();
            }
            public static bool FilterByAttr(string path)
            {
                try
                {
                    return (File.GetAttributes(path) & pw.FileFilter) == 0;
                }
                catch { return false; }
            }
            public static bool IsHiddenFile(string path)
            {
                try
                {
                    return (File.GetAttributes(path) & FileAttributes.Hidden) == FileAttributes.Hidden;
                }
                catch { return true; }
            }
            public static bool IsDirectoryEmpty(string path)
            {
                try
                {
                    return !Directory.EnumerateFileSystemEntries(path).Any();
                }
                catch { return true; }
            }
            public static bool IsDrive(string path, out string name)
            {
                name = "";
                if (Path.GetDirectoryName(path) == null)
                {
                    var di = new DriveInfo(path.Trim('\\'));
                    name = di.VolumeLabel;
                    if (string.IsNullOrEmpty(name))
                        name = "本地磁盘";
                    name += "(" + path.Trim('\\') + ")";
                    return true;
                }
                return false;
            }
            public static string ExtraFileName(string path)
            {
                string name;
                if (IsDrive(path, out name))
                {
                    return name;
                }
                if (pw.showExtension || Directory.Exists(path))
                {
                    return Path.GetFileName(path);
                }
                else
                {
                    var name_noext = Path.GetFileNameWithoutExtension(path);
                    return string.IsNullOrEmpty(name_noext) ? Path.GetFileName(path) : name_noext;
                }
            }
            public static void LocateFile(string path)
            {
                try { SetForegroundWindow(fgw); } catch { }
                ac.RunSpAsync("定位文件", new { path });
            }
            #endregion

            public static void AddMenuItem(ItemsControl itemsControl, IEnumerable<string> files)
            {
                pw.handled = false;
                //to do：some costom menu
                var items = itemsControl.Items;
                foreach (var file in files)
                {
                    if (Regex.IsMatch(file, @"^--[-\s]+$"))
                    {
                        items.Add(new Separator());
                    }
                    else
                    {
                        itemsControl.Items.Add(new FileMenuItemWrapper(file).MyMenuItem);
                    }
                }
            }
            public static void DoClickAction(string filePath)
            {
                ow.selectedPath = filePath;
                switch (pw.clickAction)
                {
                    case ParamIn.ClickAction.open:
                        System.Diagnostics.Process.Start(filePath);
                        break;
                    case ParamIn.ClickAction.callback:
                        break;
                    default:
                        break;
                }
                TheWindow.Close();
            }
            public static IDictionary<string, object> dc;
            public static ParamOutPut ow;
            public class ParamOutPut
            {
                public string selectedPath;
            }
            public static IActionContext ac;
            public static string ToJson(object obj, bool indent = false, bool ignoreNull = false)
            {
                if (obj == null) return "";
                Formatting formatting = indent ? Formatting.Indented : Formatting.None;
                JsonSerializerSettings settings = ignoreNull ? new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore, //忽略嵌套
                } : null;
                return JsonConvert.SerializeObject(obj, formatting, settings);
            }
            internal static void ParseDataContext(IDictionary<string, object> dataContext)
            {
                dc = dataContext;
                pw = ToJson(dataContext, false, true).JsonToObject<ParamIn>();

                ow = new ParamOutPut();
                dataContext["result"] = ow;

                ac = dataContext["context"] as IActionContext;
                fgw = GetForegroundWindow();
                pw.Init();
            }

            private static IntPtr fgw;

            #region native method
            [DllImport("User32.dll")]
            public static extern IntPtr GetForegroundWindow();     //获取活动窗口句柄
            [DllImport("user32")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool SetForegroundWindow(IntPtr hWnd);
            #endregion

            #region for create menu
            public static IconControl GenurateFileIcon(string path)
            {
                var iconControl = new IconControl() { Icon = "icon:" + path };
                if (IsHiddenFile(path)) iconControl.Opacity = 0.5; //隐藏文件图标透明
                return iconControl;
            }
            public static MenuItem CreateMenuItem(string icon, string title, string des, object dc = null, Action<string> click = null, IEnumerable<Control> controls = null, bool stay = false)
            {
                var menu = new MenuItem
                {
                    Icon = new IconControl() { Icon = icon },//记得自己加前缀
                    Header = title,
                    ToolTip = des,
                    StaysOpenOnClick = stay,
                };
                if (dc != null) menu.DataContext = dc;
                if (click != null) menu.Click += (s, e) => { e.Handled = true; click.Invoke(GetMenuItemPath(e)); };
                if (controls != null) AddRange(menu, controls);
                return menu;
            }
            private static IEnumerable<Control> GetCopyNameMenus()
            {
                yield return CreateMenuItem("", "复制带引号路径,例如:\"abc\"", "", null, p => Clipboard.SetText(p.ToJson()));
                yield return CreateMenuItem("", "复制文件名", "", null, p => Clipboard.SetText(Path.GetFileName(p)));
                yield return CreateMenuItem("", "复制文件名无扩展名", "", null, p => Clipboard.SetText(Path.GetFileNameWithoutExtension(p)));
                yield return CreateMenuItem("", "复制所在文件夹路径", "", null, p => Clipboard.SetText(Path.GetDirectoryName(p)));
            }
            public static void AddRange(ItemsControl itemsControl, IEnumerable<Control> controls)
            {
                foreach (var control in controls)
                {
                    itemsControl.Items.Add(control);
                }
            }
            private static MenuItem CreateMenuItem1(string icon, string title, string des, RoutedEventHandler handler = null, bool stay = false)
            {
                var menu = new MenuItem
                {
                    Header = title,
                    ToolTip = des,
                    StaysOpenOnClick = stay,
                };
                if (!string.IsNullOrEmpty(icon)) menu.Icon = new IconControl() { Icon = icon };
                if (handler != null)
                {
                    menu.Click += (s, e) => e.Handled = true;
                    menu.Click += handler;
                }

                return menu;
            }
            public static IEnumerable<Control> CreateSortTypeMenu()
            {
                yield return CreateMenuItem1("", "按名称排序", "", (s, e) => { pw.sortType = ParamIn.SortByType.name; }, true);
                yield return CreateMenuItem1("", "按创建时间排序", "", (s, e) => { pw.sortType = ParamIn.SortByType.createTime; }, true);
                yield return CreateMenuItem1("", "按查看时间排序", "", (s, e) => { pw.sortType = ParamIn.SortByType.accessTime; }, true);
                yield return CreateMenuItem1("", "按编辑时间排序", "", (s, e) => { pw.sortType = ParamIn.SortByType.editTime; }, true);
            }
            public static IEnumerable<Control> CreateRightClickMenu()
            {
                yield return CreateMenuItem("", "定位文件", "在另存为窗口中定位，否则在资源管理器中定位", null, p => LocateFile(p));
                yield return CreateMenuItem("", "复制文件", "", null, p => Clipboard.SetFileDropList(new StringCollection { p }));
                yield return CreateMenuItem("", "复制完整路径", "", null, p => Clipboard.SetText(p));
                yield return CreateMenuItem("", "属性(_R)", "", null, p => Shell.ShowFileProperties(p), null, true);
                //yield return CreateMenuItem("", "排序方式", "", null, null, CreateSortTypeMenu(), true);
                yield return CreateMenuItem("", "更多复制", "", null, null, GetCopyNameMenus());
            }
            public static string GetMenuItemPath(RoutedEventArgs e)
            {
                return GetMenuItemWrapper(e).FilePath;
            }
            private static FileMenuItemWrapper GetMenuItemWrapper(RoutedEventArgs e)
            {
                return (e.OriginalSource as MenuItem).DataContext as FileMenuItemWrapper;
            }
            #endregion
            public class Shell
            {
                [DllImport("shell32.dll", CharSet = CharSet.Auto)]
                static extern bool ShellExecuteEx(ref SHELLEXECUTEINFO lpExecInfo);

                [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
                public struct SHELLEXECUTEINFO
                {
                    public int cbSize;
                    public uint fMask;
                    public IntPtr hwnd;
                    [MarshalAs(UnmanagedType.LPTStr)]
                    public string lpVerb;
                    [MarshalAs(UnmanagedType.LPTStr)]
                    public string lpFile;
                    [MarshalAs(UnmanagedType.LPTStr)]
                    public string lpParameters;
                    [MarshalAs(UnmanagedType.LPTStr)]
                    public string lpDirectory;
                    public int nShow;
                    public IntPtr hInstApp;
                    public IntPtr lpIDList;
                    [MarshalAs(UnmanagedType.LPTStr)]
                    public string lpClass;
                    public IntPtr hkeyClass;
                    public uint dwHotKey;
                    public IntPtr hIcon;
                    public IntPtr hProcess;
                }

                private const int SW_SHOW = 5;
                private const uint SEE_MASK_INVOKEIDLIST = 12;
                public static bool ShowFileProperties(string Filename)
                {
                    SHELLEXECUTEINFO info = new SHELLEXECUTEINFO();
                    info.cbSize = Marshal.SizeOf(info);
                    info.lpVerb = "properties";
                    info.lpFile = Filename;
                    info.nShow = SW_SHOW;
                    info.fMask = SEE_MASK_INVOKEIDLIST;
                    return ShellExecuteEx(ref info);
                }

            }

        }
        public class FileMenuItemWrapper
        {
            private bool isChildrenInited = false;
            private bool isRightMenuOpen = false;
            public string FilePath;
            public MenuItem MyMenuItem;
            public FileMenuItemWrapper(string path)
            {
                FilePath = path;
                MyMenuItem = new MenuItem
                {
                    Icon = MenuHelper.GenurateFileIcon(path),
                    Header = MenuHelper.ExtraFileName(path),
                    ToolTip = path,
                    DataContext = this,
                };

                if (Directory.Exists(path))
                {
                    MyMenuItem.Items.Add(EmptyMenu());
                }
                MyMenuItem.Click += FileMenuItem_Click;

                MyMenuItem.MouseDoubleClick += (s, e) =>
                {
                    if (MenuHelper.pw.handled) return;
                    MenuHelper.pw.handled = true;
                    Open();
                };

                MyMenuItem.MouseEnter += (s, e) =>
                {
                    if (Directory.Exists(path))
                    {
                        //文件夹是空的，快速预览
                        if (MenuHelper.IsDirectoryEmpty(path))
                        {
                            CreateChildItems(path);
                        }
                        else
                        {
                            if (MenuHelper.pw.quickLoadSubmenu)
                                MyMenuItem.IsSubmenuOpen = true;
                        }
                    }
                }; //鼠标进入后打开菜单
                MyMenuItem.SubmenuOpened += (s, e) =>
                {
                    if (!isRightMenuOpen) CreateChildItems(path);
                };
                //MyMenuItem.PreviewMouseRightButtonDown += (s, e) => e.Handled = true;
                MyMenuItem.PreviewMouseRightButtonUp += (s, e) =>
                {
                    if ((e.Source as MenuItem) == MyMenuItem)
                    {
                        e.Handled = true;
                        if (isRightMenuOpen) return;
                        isRightMenuOpen = true;
                        ShowRightClickMenu();
                    }
                };
                MyMenuItem.PreviewMouseLeftButtonUp += (s, e) =>
                {
                    if ((e.Source as MenuItem) == MyMenuItem && isRightMenuOpen)
                    {
                        e.Handled = true;
                        isRightMenuOpen = false;
                        isChildrenInited = false;
                        CreateChildItems(path);
                    }
                };

                //MyMenuItem.DragLeave += (s, e) => AppHelper.ShowSuccess("drag");
                //MyMenuItem.SubmenuClosed += (s, e) =>
                //{
                //    MenuHelper.pw.isRightMenuOpen = false;
                //    isChildrenInited = false;
                //};
            }

            private void FileMenuItem_Click(object sender, RoutedEventArgs e)
            {
                e.Handled = true;
                Open();
            }
            private void Open()
            {
                MenuHelper.DoClickAction(FilePath);
            }
            private MenuItem EmptyMenu()
            {
                return new MenuItem() { Header = "空空如也" };
            }
            private void TryAddItem(string path)
            {
                try { MyMenuItem.Items.Add(new FileMenuItemWrapper(path).MyMenuItem); } catch { }
            }
            private void TryInsetItem(int index, string path)
            {
                try { MyMenuItem.Items.Insert(index, new FileMenuItemWrapper(path).MyMenuItem); } catch { }
            }
            public void CreateChildItems(string path)
            {
                if (isChildrenInited) return;
                isChildrenInited = true;
                MyMenuItem.Items.Clear();

                if (Directory.Exists(path) && !MenuHelper.IsDirectoryEmpty(path))
                {
                    var entries = MenuHelper.GetEntries(path);
                    //var enumerator = entries.GetEnumerator();
                    var now = 0;
                    var limit = MenuHelper.pw.limitCount;
                    var enumerator = entries.GetEnumerator();
                    {
                        for (int i = 0; i < limit && enumerator.MoveNext(); i++)
                        {
                            TryAddItem(enumerator.Current);
                            now++;
                        }
                        bool canAdd;
                        if (canAdd = enumerator.MoveNext()) //先做一个 move next ，后面只能 do
                        {
                            var moremenu = new MenuItem()
                            {
                                Header = "加载更多",
                                StaysOpenOnClick = true,
                            };
                            MyMenuItem.Items.Add(moremenu);
                            Action load = () =>
                            {
                                for (int i = 0; i < limit; i++)
                                {
                                    if (!canAdd) break;
                                    TryInsetItem(now, enumerator.Current);
                                    canAdd = enumerator.MoveNext();
                                    now++;
                                }
                                if (!canAdd)
                                {
                                    MyMenuItem.Items.Remove(moremenu);
                                    enumerator.Dispose();
                                }
                            };
                            if (MenuHelper.pw.quickLoadSubmenu)
                            {
                                moremenu.MouseEnter += (s, e) =>
                                  {
                                      e.Handled = true; load();
                                  };
                            }
                            else
                            {
                                moremenu.Click += (s, e) =>
                                {
                                    e.Handled = true; load();
                                };
                            }
                        }
                    }
                }
            }
            private void ShowRightClickMenu()
            {
                var mi = MyMenuItem;
                mi.Items.Clear();
                foreach (var item in MenuHelper.CreateRightClickMenu())
                {
                    mi.Items.Add(item);
                }
                mi.IsSubmenuOpen = true;
                //测试结果, isChildrenInited = false; 这一句,导致菜单显示过后立即消失
            }
        }

        #region 无关
        public FileMenuWindow()
        {
            InitializeComponent();
        }
        public class MyMenuItem : MenuItem
        {
            protected override void OnMouseEnter(MouseEventArgs e)
            {
                base.OnMouseEnter(e);
            }
        }
        #endregion
    }
}
