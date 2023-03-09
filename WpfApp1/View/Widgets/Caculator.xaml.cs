using Quicker.Public;
using Quicker.Utilities;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace WpfApp1.View.Widgets
{
    /// <summary>
    /// Caculator.xaml 的交互逻辑
    /// </summary>
    public partial class Caculator : Window
    {
        public Caculator()
        {
            InitializeComponent();
        }
        public static void OnWindowCreated(Window win, IDictionary<string, object> dataContext, ICustomWindowContext winContext)
        {
            WindowWrapper = new CaculatorWindowWrapper(win, dataContext);
        }

        public static CaculatorWindowWrapper WindowWrapper;
        public class CaculatorWindowWrapper
        {
            private TextBox InputBox;
            private TextBox OutputBox;
            private UIElement ThePanel;

            public CaculatorWindowWrapper(Window win, IDictionary<string, object> dataContext)
            {
                InputBox = win.FindName("InputBox") as TextBox;
                OutputBox = win.FindName("OutputBox") as TextBox;
                //ThePanel = win.FindName("ThePanel") as UIElement;
                win.AddHandler(Button.ClickEvent, new RoutedEventHandler(ButtonClick));
            }

            private object TryExecute(string code)
            {
                try
                {
                    return Z.Expressions.Eval.Execute(code);
                }
                catch (Exception e)
                {
                    return e.Message;
                }
            }

            private void ButtonClick(object sender, RoutedEventArgs e)
            {
                var source = e.OriginalSource as Button;
                if (source == null) return;
                var tag = source.Tag.ToString();
                switch (tag)
                {
                    case "=":
                        OutputBox.Text = TryExecute(InputBox.Text).ToString();
                        break;
                    case "clear":
                        InputBox.Clear();
                        OutputBox.Clear();
                        break;
                    default:
                        InputBox.Text += tag;
                        break;
                }
            }
        }
    }
} 
