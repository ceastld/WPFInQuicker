
using MdXaml;
using Quicker.Public;
using System.Diagnostics;
using System.Windows.Input;
using System.Collections.Generic;
using System.Windows;
using ICSharpCode.AvalonEdit;

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
            win.CommandBindings.Add(new CommandBinding(
               NavigationCommands.GoToPage,
               (sender, e) =>
               {
                   var proc = new Process();
                   proc.StartInfo.UseShellExecute = true;
                   proc.StartInfo.FileName = (string)e.Parameter;
                   proc.Start();
               }));
            var viewer = win.FindName("mdViewer") as MarkdownScrollViewer;
            var textEditor = win.FindName("TextEditor") as TextEditor;
            textEditor.TextChanged += (s, e) =>
            {
                dataContext["text"] = textEditor.Text;
                viewer.Markdown = textEditor.Text;
            };
            textEditor.Text = (string)dataContext["text"];
        }
        public class MdViewerWrapper
        {
            public MarkdownScrollViewer MdViewer;
            public MdViewerWrapper(MarkdownScrollViewer mdViewer)
            {
                MdViewer = mdViewer;
            }
        }
    }
}
