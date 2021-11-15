using Newtonsoft.Json.Linq;
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
    /// SiteBindingWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SiteBindingWindow : HandyControl.Controls.Window
    {
        public SiteBindingWindow()
        {
            InitializeComponent();
        }
    }
    public class BackCode : Test.ExpEnvironment
    {
        public IEnumerable<char> IntersectTest()
        {
            var charList1 = "ffefse".ToList();
            var charList2 = "fsekljfe".ToList();
            return charList1.Intersect(charList2);
        }
        public void SearchPreview()
        {
            //_context.GetVarValue("title")
            List<string> dlist = (List<string>)_context.GetVarValue("list");
            var tabs = _context.GetVarValue("tabs") as JArray;
            Func<string, string, double> levenString = (str1, str2) =>
            {
                if (str1.Contains(str2)) return 0.5;
                //计算两个字符串的长度。 
                int len1 = str1.Length; int len2 = str2.Length;
                //建立上面说的数组，比字符长度大一个空间 
                var dif = new int[len1 + 1, len2 + 1];
                //赋初值，步骤B。 
                for (int a = 0; a <= len1; a++) { dif[a, 0] = a; }
                for (int a = 0; a <= len2; a++) { dif[0, a] = a; }
                //计算两个字符是否一样，计算左上的值 
                int temp;
                for (int i = 1; i <= len1; i++)
                {
                    for (int j = 1; j <= len2; j++)
                    {
                        if (str1[i - 1] == str2[j - 1])
                        {
                            temp = 0;
                        }
                        else
                        {
                            temp = 1;
                        }
                        //取三个值中最小的 
                        dif[i, j] = Math.Min(Math.Min(dif[i - 1, j - 1] + temp, dif[i, j - 1] + 1), dif[i - 1, j] + 1);
                    }
                }
                return 1 - (double)dif[len1, len2] / Math.Max(str1.Length, str2.Length);
            };
            tabs.Where(x => dlist.Any(item =>
                {
                    if (string.IsNullOrWhiteSpace(item)) return false;
                    var index = item.LastIndexOf('|');
                    var title = x["title"].ToString();
                    var url = x["url"].ToString();
                    var s_title = "";
                    var s_url = "";
                    if (index != -1)
                    {
                        s_title = item.Substring(0, index);
                        s_url = item.Substring(index + 1);
                    }
                    else
                    {
                        s_url = item;
                    }
                    return levenString(title, s_title) + levenString(url, s_url) >= 1;
                })).ToList();
        }
        public static double CompareString(string str1, string str2)
        {
            Func<string, List<string>> toStringList = str =>
            {
                return str.Select(x => x.ToString()).ToList();
            };
            Func<string, string, double> func = (source, str) =>
              {
                  if (source.Contains(str)) return str.Length * 1.0 / source.Length;
                  var count = toStringList(source).Intersect(toStringList(str)).Count();
                  return count * 1.0 / Math.Min(str.Length, source.Length);
              };
            return 0;
        }
        public static float levenshtein(string str1, string str2)
        {
            //计算两个字符串的长度。 
            int len1 = str1.Length;
            int len2 = str2.Length;
            //建立上面说的数组，比字符长度大一个空间 
            int[,] dif = new int[len1 + 1, len2 + 1];
            //赋初值，步骤B。 
            for (int a = 0; a <= len1; a++)
            {
                dif[a, 0] = a;
            }
            for (int a = 0; a <= len2; a++)
            {
                dif[0, a] = a;
            }
            //计算两个字符是否一样，计算左上的值 
            int temp;
            for (int i = 1; i <= len1; i++)
            {
                for (int j = 1; j <= len2; j++)
                {
                    if (str1[i - 1] == str2[j - 1])
                    {
                        temp = 0;
                    }
                    else
                    {
                        temp = 1;
                    }
                    //取三个值中最小的 
                    dif[i, j] = Math.Min(Math.Min(dif[i - 1, j - 1] + temp, dif[i, j - 1] + 1), dif[i - 1, j] + 1);
                }
            }
            Console.WriteLine("字符串\"" + str1 + "\"与\"" + str2 + "\"的比较");

            //取数组右下角的值，同样不同位置代表不同字符串的比较 
            Console.WriteLine("差异步骤：" + dif[len1, len2]);
            //计算相似度 
            float similarity = 1 - (float)dif[len1, len2] / Math.Max(str1.Length, str2.Length);
            Console.WriteLine("相似度：" + similarity);
            return similarity;
        }

    }
}
