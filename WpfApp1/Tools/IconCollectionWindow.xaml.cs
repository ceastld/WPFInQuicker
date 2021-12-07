using Newtonsoft.Json;
using Quicker.Common;
using Quicker.Domain;
using Quicker.Public;
using Quicker.Public.Interfaces;
using Quicker.Utilities;
using Quicker.Utilities._3rd;
using Quicker.Utilities.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfApp1.Tools
{
    /// <summary>
    /// IconCollectionWindow.xaml 的交互逻辑
    /// </summary>
    public partial class IconCollectionWindow : Window
    {
        public IconCollectionWindow()
        {
            InitializeComponent();
            wrapper = new IconWindowWrapper(this, new Dictionary<string, object>());
        }
        public static void OnWindowCreated(Window win, IDictionary<string, object> dataContext, ICustomWindowContext winContext)
        {
            wrapper = new IconWindowWrapper(win, dataContext);
        }
        public static IconWindowWrapper wrapper;
        public static IActionContext _context;
        public class IconWindowWrapper
        {
            private const string Type_ActionIcon = "ActionIcon";
            private const string Type_FavoriteIcon = "FavoriteIcon";
            private const string TYPE_INTERNALICON = "InternalIcon";
            private Window TheWindow;

            public ContextMenu ContextMenu;

            private IDictionary<string, object> dataContext;
            private ListBox TheButtonList;
            private IActionContext context;
            private HandyControl.Controls.ComboBox IconLibraryBox;
            public HandyControl.Controls.SearchBar TheSearchBox;
            private HandyControl.Controls.ComboBox FilterTypeBox;
            private HandyControl.Controls.CheckComboBox FilterConditionBox;
            private Button DataAsyncButton;

            public IconWindowWrapper(Window win, IDictionary<string, object> dataContext)
            {
                this.TheWindow = win;
                this.ContextMenu = win.FindName("TheListBoxContextMenu") as ContextMenu;
                this.dataContext = dataContext;
                this.TheButtonList = win.FindName("TheButtonList") as ListBox;
                object obj;
                dataContext.TryGetValue("context", out obj);
                this.context = obj as IActionContext;
                _context = context;
                this.IconLibraryBox = win.FindName("IconLibraryBox") as HandyControl.Controls.ComboBox;
                this.TheSearchBox = win.FindName("TheSearchBox") as HandyControl.Controls.SearchBar;
                this.FilterTypeBox = win.FindName("FilterTypeBox") as HandyControl.Controls.ComboBox;
                this.FilterConditionBox = win.FindName("FilterConditionBox") as HandyControl.Controls.CheckComboBox;
                this.IconLibraryBox.SelectionChanged += IconLibraryBox_SelectionChanged;
                this.FilterConditionBox.SelectionChanged += FilterConditionBox_SelectionChanged;
                this.FilterTypeBox.SelectionChanged += FilterTypeBox_SelectionChanged;
                this.TheSearchBox.SearchStarted += TheSearchBox_SearchStarted;
                this.DataAsyncButton = win.FindName("DataAsyncButton") as Button;
                DataAsyncButton.Click += (s, e) => { IconDataWrapper.UploadData(true); };
                TheButtonList.AddHandler(Button.ClickEvent, new RoutedEventHandler(this.ButtonClicked));
                this.ContextMenu.AddHandler(MenuItem.ClickEvent, new RoutedEventHandler(this.MenuClicked));

                TheWindow.Loaded += (s, e) =>
                {
                    InitIconList(IconDataWrapper.GetFavoriteIcon());
                    IconDataWrapper.MergeData(false);
                };
                TheWindow.Closing += (s, e) =>
                {
                    IconDataWrapper.SaveIcon();
                    IconDataWrapper.UploadData();
                };
            }
            private ListCollectionView collectionView;
            private void InitIconList(IList list)
            {
                collectionView = new ListCollectionView(list);
                collectionView.Filter = new Predicate<object>(this.Filter);
                TheButtonList.ItemsSource = collectionView;
                TheButtonList.Refresh();
            }
            private void RefreshList()
            {
                if (collectionView == null || TheButtonList == null) return;
                collectionView.Refresh();
            }
            private T GetObjectProperty<T>(object obj, string propertyName) where T : class
            {
                return obj.GetType().GetProperty(propertyName).GetValue(obj) as T;
            }
            private bool Filter(object obj)
            {
                if (obj == null) return false;
                var title = GetObjectProperty<string>(obj, "Title");
                var icon = GetObjectProperty<string>(obj, "Icon");
                var text = TheSearchBox.Text;
                if (!ShowAllIcon)
                {
                    if (icon.StartsWith("http"))
                    {
                        if (icon.EndsWith("svg"))
                        {
                            if (!ShowSvgIcon)
                            {
                                return false;
                            }
                        }
                        else
                        {
                            if (!ShowNormalIcon)
                            {
                                return false;
                            }
                        }
                    }
                    else if (icon.StartsWith("fa") && !ShowInternalSvgIcon)
                    {
                        return false;
                    }
                }
                if (string.IsNullOrEmpty(text)) return true;
                return icon.Contains(text) || title.Contains(text);
            }
            private void TheSearchBox_SearchStarted(object sender, HandyControl.Data.FunctionEventArgs<string> e)
            {
                RefreshList();
            }

            private bool ShowAllIcon = true;

            private void FilterTypeBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
            {
                var tag = (e.AddedItems[0] as FrameworkElement).Tag as string;
                switch (tag)
                {
                    case "All":
                        ShowAllIcon = true;
                        break;
                    case "Any":
                        ShowAllIcon = false;
                        break;
                }
                RefreshList();
            }

            private bool ShowNormalIcon = false;
            private bool ShowSvgIcon = false;
            private bool ShowInternalSvgIcon = false;
            private void FilterConditionBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
            {
                if (e.AddedItems.Count > 0)
                {
                    var tag = (e.AddedItems[0] as FrameworkElement).Tag as string;
                    ChangeCondition(tag, true);
                }
                else if (e.RemovedItems.Count > 0)
                {
                    var tag = (e.RemovedItems[0] as FrameworkElement).Tag as string;
                    ChangeCondition(tag, false);
                }
            }
            private void ChangeCondition(string type, bool value)
            {
                switch (type)
                {
                    case "normal":
                        ShowNormalIcon = value;
                        break;
                    case "svg":
                        ShowSvgIcon = value;
                        break;
                    case "svg_internal":
                        ShowInternalSvgIcon = value;
                        break;
                }
                RefreshList();
            }
            private string IconTagNow = "";

            /// <summary>
            /// 现在处于收藏模式
            /// </summary>
            private bool IsFavoriteNow { get; set; }

            private void IconLibraryBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
            {
                IconDataWrapper.SaveIcon();
                var tag = (e.AddedItems[0] as FrameworkElement).Tag as string;
                IconTagNow = tag;
                if (tag == Type_ActionIcon)
                {
                    IsFavoriteNow = false;
                    InitIconList(IconDataWrapper.GetActionIcon());
                }
                else if (tag == Type_FavoriteIcon)
                {
                    IsFavoriteNow = true;
                    var data = IconDataWrapper.GetFavoriteIcon();
                    InitIconList(data);
                }
            }

            private void MenuClicked(object sender, RoutedEventArgs e)
            {
                if (e.OriginalSource is MenuItem)
                {
                    var menuItem = (MenuItem)e.OriginalSource;
                    switch (menuItem.Tag as string)
                    {
                        case "Collect":
                            this.CollectIcon();
                            break;
                        case "Delete":
                            this.DeleteIcon();
                            break;
                        case "Add":
                            IconDataWrapper.AddIcon(index: TheButtonList.SelectedIndex);
                            break;
                        case "Edit":
                            this.EditIcon();
                            break;
                    }
                    TheButtonList.SelectedItem = null;
                    e.Handled = true;
                }
            }

            private void EditIcon()
            {
                var item = TheButtonList.SelectedItem as IconItem;
                IconDataWrapper.EditIcon(item);
            }

            private void CollectIcon()
            {
                var item = TheButtonList.SelectedItem as IconItem;
                IconDataWrapper.CollectIcon(item);
            }

            private void DeleteIcon()
            {
                var items = TheButtonList.SelectedItems.Cast<IconItem>().ToList();
                IconDataWrapper.RemoveIcons(items);
            }
            private void ButtonClicked(object sender, RoutedEventArgs e)
            {
                var button = e.OriginalSource as Button;
                if (button == null)
                {
                    TheButtonList.SelectedItem = null;
                    return;
                }
                if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                {
                    var listboxItem = UIHelper.FindParent<ListBoxItem>(button);
                    if (listboxItem != null)
                    {
                        listboxItem.IsSelected = true;
                        return;
                    }
                }
                TheButtonList.SelectedItem = null;
                var item = button.DataContext as IconItem;
                if (item != null)
                {
                    Clipboard.SetText(item.Icon);
                    AppHelper.ShowSuccess(item.Icon);
                }
            }

        }
        public static class IconDataWrapper
        {
            public class IconComparer : IEqualityComparer<ActionItem>, IEqualityComparer<IconItem>
            {
                public bool Equals(ActionItem x, ActionItem y)
                {
                    return string.Compare(x.Icon, y.Icon) == 0;
                }

                public bool Equals(IconItem x, IconItem y)
                {
                    return string.Compare(x.Icon, y.Icon) == 0;
                }

                public int GetHashCode(ActionItem obj)
                {
                    return base.GetHashCode();
                }

                public int GetHashCode(IconItem obj)
                {
                    return base.GetHashCode();
                }
            }
            public static List<IconItem> GetActionIcon()
            {
                if (AppState.DataService != null)
                {
                    return AppState.DataService.GetAllActionItems().Distinct(new IconComparer())
                        .Where(x => !string.IsNullOrWhiteSpace(x.Icon))
                        .Select(x => new IconItem() { Title = x.Title, Icon = x.Icon })
                        .ToList();
                }
                else
                {
                    return new List<IconItem>()
                            {
                                new IconItem() {Title ="aa", Icon="https://files.getquicker.net/_icons/91943F4CEF747C5A9CA96454E7A45173D10F1363.png"},
                                new IconItem() {Title ="aa", Icon="https://files.getquicker.net/_icons/91943F4CEF747C5A9CA96454E7A45173D10F1363.png"},
                                new IconItem() {Title ="aa", Icon="https://files.getquicker.net/_icons/91943F4CEF747C5A9CA96454E7A45173D10F1363.png"},
                                new IconItem() {Title ="aa", Icon="https://files.getquicker.net/_icons/91943F4CEF747C5A9CA96454E7A45173D10F1363.png"},
                            };
                }
            }
            private static List<IconItem> InternalIconCache;
            public static List<IconItem> GetAllInternalIcon()
            {
                if (InternalIconCache != null) return InternalIconCache;
                var names = Enum.GetNames(typeof(FontAwesome5.EFontAwesomeIcon));
                InternalIconCache = new List<IconItem>();
                for (int i = 1; i < names.Length; i++)
                {
                    InternalIconCache.Add(new IconItem()
                    {
                        Icon = "fa:" + names[i],
                        Title = names[i]
                    });
                }
                return InternalIconCache;

            }
            public static SmartCollection<IconItem> FavoriteIconList = new SmartCollection<IconItem>();
            public static void RemoveIcons(IList<IconItem> items)
            {
                foreach (var item in items)
                {
                    RemoveIcon(item);
                }
            }
            public static void RemoveIcon(IconItem item)
            {
                FavoriteIconList.Remove(item);
            }
            public static void CollectIcon(IconItem item, bool showmessage = true)
            {
                var ex = FavoriteIconList.FirstOrDefault(x => x.Icon == item.Icon);
                if (ex != null)
                {
                    var result = MessageBox.Show("此图标已经收藏过,是否要重复收藏", "确认", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                    if (result != MessageBoxResult.OK) return;
                }
                FavoriteIconList.Insert(0, item);
                if (showmessage)
                {
                    try
                    {
                        AppHelper.ShowSuccess("图标 " + item.Title + " 已收藏");
                    }
                    catch { }
                }
            }
            public static void AddIcon(IconItem item = null, int index = 0)
            {
                if (item == null) item = new IconItem();
                EditIcon(item);
                FavoriteIconList.Insert(index, item);
            }
            public static async void EditIcon(IconItem item)
            {
                await RunSpAsync("编辑图标", new { item });
            }
            private static bool isFavoriteInited = false;
            public static SmartCollection<IconItem> GetFavoriteIcon()
            {
                if (FavoriteIconList == null || FavoriteIconList.Count == 0)
                {
                    if (FavoriteIconList.Count == 0 && isFavoriteInited)
                    {

                    }
                    else
                    {
                        var iconList_str = _context == null ? "[]" : _context.ReadState("iconList", "[]");
                        FavoriteIconList = JsonConvert.DeserializeObject<SmartCollection<IconItem>>(iconList_str);
                        isFavoriteInited = true;
                    }
                }
                return FavoriteIconList;
            }

            internal static void SaveIcon()
            {
                if (_context == null) return;
                _context.WriteState("iconList", JsonConvert.SerializeObject(FavoriteIconList));
            }
            private static readonly string TemplateId = "17b78d43-8bc7-4200-a090-08d9ac50b9f2";
            private static readonly string SPNAME = "@@7b3cbe65-c565-41d2-86f8-08d96e79e5d4@0@程序集交互_云状态";
            public static async void MergeData(bool showmessage)
            {
                var result = await RunSpAsync(SPNAME, new { operation = "downLoad", key = "Ceastld " + TemplateId });
                if (result == null) return;
                object obj;
                result.TryGetValue("value", out obj);
                if (obj != null)
                {
                    var list = IconItem.GetIconItems((string)obj);
                    if (list != null && list.Count > 0)
                    {
                        var ex = list.Except(FavoriteIconList, new IconComparer()).ToList();
                        //差集为空
                        if (ex.Count() == 0)
                        {
                            if (list.Count == FavoriteIconList.Count)
                            {
                                if (showmessage)
                                {
                                    AppHelper.ShowSuccess("数据已经最新");
                                }
                                return;
                            }
                        }
                        else
                        {
                            ex.ForEach(item => FavoriteIconList.Insert(0, item));
                        }
                    }
                }
                UploadData();
            }
            public static async void UploadData(bool showmessage = false)
            {

                var value = IconItem.GetString(FavoriteIconList);
                if (string.IsNullOrEmpty(value)) return;
                await RunSpAsync(SPNAME, new { operation = "upLoad", key = "Ceastld " + TemplateId, value });
                if (showmessage) AppHelper.ShowSuccess("数据已上传,每次打开动作时会自动同步");
            }
        }
        public static async Task<IDictionary<string, object>> RunSpAsync(string name, object inputParams = null, bool showerror = false)
        {
            if (_context == null) return null;
            try
            {
                return await _context.RunSpAsync(name, inputParams);
            }
            catch (Exception error)
            {
                if (showerror)
                {
                    AppHelper.ShowWarning(error.Message);
                }
                return null;
            }
        }
        public class IconItem : INotifyPropertyChanged
        {
            public void Changed()
            {
                OnPropertyChanged("IconItem");
            }
            #region For INotifyPropertyChanged
            public event PropertyChangedEventHandler PropertyChanged;
            protected void OnPropertyChanged([CallerMemberName] string name = null)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs(name));
                }
            }

            protected void OnPropertyChanged(params string[] vs)
            {
                foreach (var s in vs)
                {
                    OnPropertyChanged(s);
                }
            }
            #endregion
            private string icon = "";
            public string Icon
            {
                get { return icon; }
                set { icon = value; OnPropertyChanged("Icon"); }
            }

            private string title = "";

            public string Title
            {
                get { return title; }
                set { title = value; OnPropertyChanged("Title"); }
            }
            public static string GetString(object obj)
            {
                return JsonConvert.SerializeObject(obj);
            }
            public static List<IconItem> GetIconItems(string data)
            {
                try
                {
                    return JsonConvert.DeserializeObject<List<IconItem>>(data);
                }
                catch
                {
                    return new List<IconItem>();
                }
            }
        }
    }
}
