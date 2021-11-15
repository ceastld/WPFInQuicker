using Quicker.Public;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace WpfApp1.View
{
    /// <summary>
    /// ClockWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ClockWindow : Window
    {
        public ClockWindow()
        {
            InitializeComponent();
        }
        //public static ClockWindowWrapper wrapper;
        public static void OnWindowCreated(Window win, IDictionary<string, object> dataContext, ICustomWindowContext winContext)
        {
            var clockBlock = win.FindName("ClockBlock") as TextBlock;
            var wrapper = new ClockWindowWrapper(clockBlock);
            win.MouseDoubleClick += (s, e) =>
            {
                win.Close();
            };
            win.MouseWheel += (s, e) =>
            {
                if (e.Delta > 0)
                {
                    wrapper.FontSizePlus();
                }
                else
                {
                    wrapper.FontSizeMinus();
                }
            };
        }

        public class ClockWindowWrapper
        {
            public DispatcherTimer Timer = new DispatcherTimer();
            private TextBlock ClockBlock;
            public ClockWindowWrapper(TextBlock block)
            {
                Timer.Interval = TimeSpan.FromSeconds(1);
                Timer.Tick += Timer_Tick;
                this.ClockBlock = block;
                Timer.Start();
            }

            private void Timer_Tick(object sender, EventArgs e)
            {
                ClockBlock.Text = DateTime.Now.ToString("HH:mm:ss");
            }
            public void FontSizePlus()
            {
                ClockBlock.FontSize += 2;
            }

            public void FontSizeMinus()
            {
                if (ClockBlock.FontSize > 10)
                    ClockBlock.FontSize -= 2;
            }
        }
    }
    public class ForTest
    {
        public void aaa()
        {
            DateTime.Now.ToString("yyyy/MM/dd,ddd"); //年/月/日,周几
        }
    }
}
