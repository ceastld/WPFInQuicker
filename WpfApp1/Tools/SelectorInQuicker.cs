using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Reflection;
using Newtonsoft.Json;
using Quicker.View.X.Controls;
using System.Windows.Controls;
using Quicker.Domain.Actions.X.Storage;
using Quicker.Utilities._3rd;
using Quicker.Domain;
using Quicker.Utilities;
using Newtonsoft.Json.Linq;
using System.Windows.Media;
using FontAwesome5.WPF;
using Quicker.View.X;
using System.Windows.Input;
using Quicker.Public;

namespace WpfApp1.Tools
{
    /// <summary>
    /// 获取选择变量？
    /// </summary>
    public static class SelectorInQuicker
    {
        static IStepContext Context;
        public static void Exec(IStepContext context)
        {
            Context = context;
            string template = (string)context.GetVarValue("template");
            var adder = new VariableListAdder() { Template = template };
            adder.DoSet();
        }
        public static class WinOp
        {

            [DllImport("User32.dll")]
            public static extern IntPtr GetForegroundWindow();     //获取活动窗口句柄
            public static IntPtr GetHandle(Window window)
            {
                return new WindowInteropHelper(window).Handle;
            }
            public static WType GetWindow<WType>() where WType : class
            {
                return HwndSource.FromHwnd(GetForegroundWindow()).RootVisual as WType;
            }
        }
        public static class ReflectionHelper
        {
            public static T GetField<T>(object sender, string name) where T : class
            {
                var tp = sender.GetType().GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                return tp.GetValue(sender) as T;
            }
        }
        public class VariableListAdder
        {
            public ActionDesignerWindow Desiger;
            public SubProgramEditor SPEditor;
            public VariableListControl VList;
            public VariableListAdder()
            {
                Desiger = WinOp.GetWindow<ActionDesignerWindow>();
                SPEditor = ReflectionHelper.GetField<SubProgramEditor>(Desiger, "SubProgramEditor");
                VList = ReflectionHelper.GetField<VariableListControl>(Desiger, "VariableListControl");
            }
            public void DoSet()
            {
                SetToMainEditor();
                SetToSubEditor();
                AppHelper.ShowSuccess("右键菜单 & 按钮已添加,\r\n快捷键ctrl+c也可以复制变量");
            }
            public string Template = "[]";
            public void SetToMainEditor()
            {
                var listBox = ReflectionHelper.GetField<ListBox>(VList, "LbVariables");
                var button = VList.FindName("BtnNewVar") as Button;
                var panel = button.Parent as StackPanel;
                new ButtonAndMenu(listBox, panel, Template).DoAdd();

            }
            public void SetToSubEditor()
            {
                var listBox = ReflectionHelper.GetField<ListBox>(SPEditor, "LbVariables");
                var button = SPEditor.FindName("BtnNewVar") as Button;
                var panel = button.Parent as StackPanel;
                new ButtonAndMenu(listBox, panel, Template).DoAdd();
            }
        }
        public class ButtonAndMenu
        {
            ListBox listBox;
            StackPanel panel;
            public ButtonAndMenu(ListBox l, StackPanel p, string temp)
            {
                listBox = l;
                panel = p;
                Template = temp;
            }
            /// <summary>
            /// 添加对应的按钮
            /// </summary>
            public void DoAdd()
            {
                SetMenuToListBox();
                InitCommandBindings();
                AddButton();
            }
            public void InitCommandBindings()
            {
                listBox.CommandBindings.AddKeyGesture(new KeyGesture(Key.C, ModifierKeys.Control), MenuItem_Click);
            }
            public void SetMenuToListBox()
            {
                listBox.SelectionMode = SelectionMode.Extended;
                MenuItem menu = new MenuItem()
                {
                    Header = "复制选中变量(_C)",
                };
                menu.Click += MenuItem_Click;
                var contextMenu = new ContextMenu();
                contextMenu.Items.Add(menu);
                listBox.ContextMenu = contextMenu;
            }
            public void AddButton()
            {
                var button = new Button()
                {
                    ToolTip = "复制变量",
                };
                var icon = new SvgAwesome
                {
                    Width = 16.0,
                    Height = 16.0,
                    Foreground = Brushes.Blue,
                    Icon = FontAwesome5.EFontAwesomeIcon.Light_Copy
                };
                button.Content = icon;
                button.Click += MenuItem_Click;
                panel.Children.Add(button);
            }

            public string Template = "[]";
            private void MenuItem_Click(object sender, RoutedEventArgs e)
            {
                if (listBox.SelectedItem == null) return;
                var items = listBox.SelectedItems.OfType<ActionVariable>().ToList();
                ActionStepsDto stepsDto = JsonConvert.DeserializeObject<ActionStepsDto>(Template);
                var ip = stepsDto.Steps[0].InputParams;
                string note = "$$" + string.Join("", items.Select(x => "{" + x.Key + "}"));
                ip.Clear(); ip.Add("note", new ActionStepParam()
                {
                    VarKey = "",
                    Value = note
                });
                stepsDto.Variables = items;
                Clipboard.SetData("quicker-action-steps", JsonConvert.SerializeObject(stepsDto));
                AppHelper.ShowSuccess("步骤已写入剪贴板");
            }

        }

        #region 弃用
        public static ListBox GetVariableList()
        {
            var win = WinOp.GetWindow<ActionDesignerWindow>();
            if (win.IsEditingSubProgram)
            {
                var spEditor = ReflectionHelper.GetField<SubProgramEditor>(win, "SubProgramEditor");
                var listBox = ReflectionHelper.GetField<ListBox>(spEditor, "LbVariables");
                return listBox;
            }
            else
            {
                var varList = ReflectionHelper.GetField<VariableListControl>(win, "VariableListControl");
                return ReflectionHelper.GetField<ListBox>(varList, "LbVariables");
            }
        }
        public static StackPanel GetStackPanel()
        {
            var win = WinOp.GetWindow<Quicker.View.X.ActionDesignerWindow>();
            if (win.IsEditingSubProgram)
            {
                var spEditor = ReflectionHelper.GetField<SubProgramEditor>(win, "SubProgramEditor");
                var button = spEditor.FindName("BtnNewVar") as Button;
                return button.Parent as StackPanel;
            }
            else
            {
                var varList = ReflectionHelper.GetField<VariableListControl>(win, "VariableListControl");
                var button = varList.FindName("BtnNewVar") as Button;
                return button.Parent as StackPanel;
            }
        }
        public static List<Quicker.Domain.Actions.X.Storage.ActionVariable> test()
        {
            var win = WinOp.GetWindow<Quicker.View.X.ActionDesignerWindow>();
            if (win.IsEditingSubProgram)
            {
                var spEditor = ReflectionHelper.GetField<Quicker.View.X.SubProgramEditor>(win, "SubProgramEditor");
                return spEditor.VariableList.ToList();
            }
            else
            {
                return win.VariableList.ToList();
            }
        }
        #endregion
    }
    public class FindTest
    {
        public ListBox LbVariables;
        private T GetVisualChild<T>(DependencyObject parent) where T : Visual
        {
            T child = default(T);
            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                Visual v = (Visual)VisualTreeHelper.GetChild(parent, i);
                child = v as T;
                if (child == null)
                {
                    child = GetVisualChild<T>(v);
                }
                if (child != null)
                {
                    break;
                }
            }
            return child;
        }
        private childItem FindVisualChild<childItem>(DependencyObject obj) where childItem : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is childItem)
                    return (childItem)child;
                else
                {
                    childItem childOfChild = FindVisualChild<childItem>(child);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
        }
        public MenuItem FindMenuItem()
        {
            ListBoxItem myListBoxItem = (ListBoxItem)(LbVariables.ItemContainerGenerator.ContainerFromIndex(0));
            //LbVariables.ItemTemplate.FindName("MenuItemHightlightVar", myListBoxItem);
            ContentPresenter myContentPresenter = FindVisualChild<ContentPresenter>(myListBoxItem);
            DataTemplate myDataTemplate = myContentPresenter.ContentTemplate;
            MenuItem target = (MenuItem)myDataTemplate.FindName("MenuItemHightlightVar", myContentPresenter);
            AppHelper.ShowSuccess(JsonConvert.SerializeObject(LbVariables.ItemTemplate));
            return target;
        }
        public ContextMenu FindContextMenu()
        {
            var dataT = LbVariables.ItemTemplate.LoadContent();
            AppHelper.ShowSuccess(JsonConvert.SerializeObject(dataT));
            //LbVariables.ApplyTemplate();
            //var h_menuitem = LbVariables.ItemTemplate.FindName("MenuItemHighlightVar", (LbVariables.Items[0] as ListBoxItem)) as MenuItem;
            var h_menuitem = FindMenuItem();
            //AppHelper.ShowSuccess(JsonConvert.SerializeObject(h_menuitem));
            if (h_menuitem == null)
                AppHelper.ShowError("找不到菜单项");
            var contextMenu = h_menuitem.Parent as ContextMenu;
            return contextMenu;

        }
    }
}
