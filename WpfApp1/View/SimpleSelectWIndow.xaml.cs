using Newtonsoft.Json.Linq;
using Quicker.Public.Entities;
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

namespace WpfApp1.View
{
    /// <summary>
    /// SimpleSelectWIndow.xaml 的交互逻辑
    /// </summary>
    public partial class SimpleSelectWIndow : Window
    {
        public SimpleSelectWIndow()
        {
            InitializeComponent();
        }
        public void QuickerItem()
        {
            new CommonOperationItem();
            var items = CommonOperationItem.ParseLines("fe", false, true);

            JToken x = new JObject();
            var item = new CommonOperationItem
            {
                Title = (string)x["title"],
                Description = (string)x["description"],
                Data = (string)x["id"]
            };
            string icon = (string)x["favorIconUrl"];
            if (icon.StartsWith("http"))
            {
                item.Icon = icon;
            }
        }

    }
}
