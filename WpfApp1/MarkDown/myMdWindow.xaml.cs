
using MdXaml;
using Quicker.Public;
using System.Diagnostics;
using System.Windows.Input;
using System.Collections.Generic;
using System.Windows;
using ICSharpCode.AvalonEdit;
using System.Windows.Controls;
using System.Windows.Threading;
using Quicker.Utilities;
using System;

namespace WpfApp1.MarkDown
{
    /// <summary>
    /// myMdWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MyMdWindow : Window
    {
        public MyMdWindow()
        {
            InitializeComponent();
        }
        public static void OnWindowCreated(Window win, IDictionary<string, object> dataContext, ICustomWindowContext winContext)
        {
            MdWrapp = new MdViewerWrapper(win, dataContext);
        }
        public static MdViewerWrapper MdWrapp;
        public class DelayTriggerTimer
        {
            public DispatcherTimer Timer;
            public DelayTriggerTimer(int ms)
            {
                Timer = new DispatcherTimer()
                {
                    Interval = TimeSpan.FromMilliseconds(ms),
                };
                Timer.Tick += Timer_Tick;
                Timer.Start();
            }
            public Action DoAction = null;
            private void Timer_Tick(object sender, EventArgs e)
            {
                if (DoAction == null) return;
                DoAction.Invoke();
                DoAction = null;
            }
        }
        public class MdViewerWrapper
        {
            public MarkdownScrollViewer MdViewer;
            public TextEditor TextEditor;
            public DelayTriggerTimer DelayTimer;
            private readonly IDictionary<string, object> DataContext;
            private readonly Window TheWindow;
            public MdViewerWrapper(Window win, IDictionary<string, object> dataContext)
            {
                TheWindow = win;
                DataContext = dataContext;
                DelayTimer = new DelayTriggerTimer(200);
                MdViewer = TheWindow.FindName("mdViewer") as MarkdownScrollViewer;
                TextEditor = TheWindow.FindName("TextEditor") as TextEditor;
                TextEditor.TextChanged += TextEditor_TextChanged;
                TextEditor.Text = (string)dataContext["text"];
                (TheWindow.FindName("TheCopyButton") as Button).Click += CopyButton_Click;
            }
            private void TextEditor_TextChanged(object sender, EventArgs e)
            {
                DelayTimer.DoAction = () =>
                {
                    DataContext["text"] = TextEditor.Text;
                    MdViewer.Markdown = TextEditor.Text;
                };
            }
            private void CopyButton_Click(object sender, RoutedEventArgs e)
            {
                Clipboard.SetText(TextEditor.Text);
                AppHelper.ShowSuccess("文本已写入剪贴板");
            }
        }
    }
}
