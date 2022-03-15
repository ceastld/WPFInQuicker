using Newtonsoft.Json;
using Quicker.Public;
using Quicker.Public.Extensions;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp1.Customization
{
    /// <summary>
    /// BoxSizeCompute.xaml 的交互逻辑
    /// </summary>
    public partial class BoxSizeCompute : Window
    {
        public BoxSizeCompute()
        {
            InitializeComponent();
        }

        public static void OnWindowCreated(Window win, IDictionary<string, object> dataContext, ICustomWindowContext winContext)
        {
            BoxWrapper = new BoxComputeWrapper(win, dataContext);
        }
        public static BoxComputeWrapper BoxWrapper;

        public class BoxComputeWrapper
        {
            private IDictionary<string, object> DataContext;
            private Window TheWindow;
            private TextBlock OutputBox;

            public BoxComputeWrapper(Window win, IDictionary<string, object> dataContext)
            {
                this.DataContext = dataContext;
                this.TheWindow = win;
                this.OutputBox = win.FindName("OutputBox") as TextBlock;
                (win.FindName("ComputeButton") as Button).Click += ComputeButton_Click;
            }
            private void ComputeButton_Click(object sender, RoutedEventArgs e)
            {
                var manager = JsonConvert.DeserializeObject<BoxManager>(DataContext.ToJson(false));
                OutputBox.Text = manager.GetOutput();
            }
            public class BoxManager
            {
                public Box bigBox { get; set; }
                public Box smallBox { get; set; }
                public BoxManager()
                {

                }
                public string GetOutput()
                {
                    StringBuilder output = new StringBuilder();

                    int lc = (int)(bigBox.length / smallBox.length);
                    int wc = (int)(bigBox.width / smallBox.width);
                    int hc = (int)(bigBox.height / smallBox.height);

                    var lr = bigBox.length % smallBox.length;
                    var wr = bigBox.width % smallBox.width;
                    var hr = bigBox.height % smallBox.height;

                    int count = lc * wc * hc;
                    output.Append("小箱子个数\t" + count + "\r\n");
                    output.Append("箱子总重量\t" + count * smallBox.weight + "\r\n");
                    output.Append("剩余长度\t" + lr + "\r\n");
                    output.Append("剩余宽度\t" + wr + "\r\n");
                    output.Append("剩余高度\t" + hr + "\r\n");
                    return output.ToString();
                }
            }
            public class Box
            {
                public double length { get; set; }
                public double width { get; set; }
                public double height { get; set; }
                public double weight { get; set; }
                public Box()
                {

                }
            }
        }

    }
}
