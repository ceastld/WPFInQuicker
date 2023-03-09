using Quicker.Modules.TextTools.Tools;
using Quicker.Public.Entities;
using Quicker.Utilities._3rd;
using Quicker.View;
using Quicker.View.Hotkeys;
using Quicker.View.X;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfApp1.Tools
{
    /// <summary>
    /// NumpadWindow.xaml 的交互逻辑
    /// </summary>
    public partial class NumpadWindow : Window
    {
        public NumpadWindow()
        {
            InitializeComponent();
        }
        public void SendKey()
        {
            var coll = new SmartCollection<OperationItem>();
            coll.Reset(coll);
            var key = new Quicker.View.Hotkeys.Hotkey("");
            return;
        }
        public void Getkeycode(Hotkey hotkey, string keyCode)
        {
            var win = new KeysInputWindow(KeysInputMode.CombinedKey);
            win.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            win.ShowDialog();
            win.Topmost = true;
            hotkey = win.SelectedHotkey;
            keyCode = hotkey.ToString();
        }

        public List<string> GenerateList(List<string> list)
        {
            list = list.Where(x => !string.IsNullOrWhiteSpace(x))
                       .Select(x =>
                       {
                           var si = x.IndexOf('|');
                           string name = "";
                           if (si != -1)
                           {
                               name = x.Substring(0, si);
                               x = x.Substring(si + 1);
                           }
                           var index = x.IndexOf(':');
                           if (index != -1)
                           {
                               var pre = x.Substring(0, index);
                               var post = x.Substring(index + 1);
                               if (string.IsNullOrEmpty(name)) name = post;
                               if (Regex.IsMatch(pre, "^sendkeys|copy|paste|action|open"))
                               {
                                   return $"{name}|operation={pre}&data={post.UrlEncode()}";
                               }
                           }
                           if (string.IsNullOrEmpty(name)) name = x;
                           return $"{name}|operation=paste&data={x.UrlEncode()}";
                       })
                       .ToList();
            list.Insert(0, "编辑|operation=sp&spname=编辑数据");
            return list;
        }

        public List<int> GenerateNum()
        {
            Func<List<int>> ge = () =>
            {
                var _nums = new List<int>();
                var random = new Random();
                for (int i = 0; i < 9; i++)
                {
                    _nums.Add(random.Next(10, 21)); //10 - 20
                }
                return _nums;
            };
            int j = 0;
            while (j++ < 100)
            {
                var nums = ge();
                var num_end = 150 - ge().Sum();
                if (10 <= num_end && num_end <= 20)
                {
                    nums.Add(num_end);
                    return nums;
                }
            }
            return new List<int>();
        }

        public List<int> RandomGe2()
        {
            var random = new Random();
            Func<double, double, double, int> nd = (_min, _max, _center) =>
            {
                if (_max <= _min) return (int)_min;
                if (_center <= _min) return (int)_min;
                if (_center > _max) return (int)_max;

                if (_center < (_min + _max) / 2)
                {
                    _max = (2 * _center - _min);
                }
                else if (_center > (_min + _max) / 2)
                {
                    _min = (2 * _center - _max);
                }
                return (int)(random.NextDouble() * (_max + 1 - _min) + _min);
            };
            //Enumerable.Range(0, 20).Select(x => nd(10, 20, 15));
            int min = 10;
            int max = 20;
            int count = 10;
            int sum = 150;
            //生成 n-1个符合要求的随机数
            Func<List<int>> ge = () =>
            {
                var _nums = new List<int>();
                for (int i = 0; i < count - 1; i++)
                {
                    var center = ((double)(sum - _nums.Sum())) / (count - i);
                    _nums.Add(nd(min, max, center));
                }
                return _nums;
            };
            int j = 0;
            //超过100次则返回空值
            while (j++ < 100)
            {
                var nums = ge();
                var num_end = sum - nums.Sum();
                if (min <= num_end && num_end <= max)
                {
                    nums.Add(num_end);
                    return nums;
                }
            }
            return new List<int>();
        }
        public int RandomDouble(double min, double max, double center)
        {

            if (max <= min) return (int)min;
            if (center <= min) return (int)min;
            if (center > max) return (int)max;

            max++;
            if (center <= (min + max) / 2)
            {
                max = (2 * center - min);
            }
            else
            {
                min = (2 * center - max);
            }
            return (int)(new Random().NextDouble() * (max - min) + min);
        }


    }
}
