using Newtonsoft.Json.Linq;
using Quicker.Public.Entities;
using Quicker.Public.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Z.Expressions;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            HttpRequestAsync();
            Console.Read();
        }
        public static HttpClient Client = new HttpClient();
        public static async void HttpRequestAsync()
        {
            string responseBody = await Client.GetStringAsync("https://getquicker.net/User/Actions/113342-Ceastld");
            Console.WriteLine(responseBody);
            //var content = new StringContent();
            //content.Headers.
            //Client.PostAsync("https://api.getquicker.net/api/SharedAction/Vote",)
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

        public static IList<CommonOperationItem> tab缩进解析()
        {
            string data = @"
- sefk

  - fskel

  - fsef

    - fsek

  - fse
";
            var code = data.SplitToList();
            if (code.Length == 0) return new List<CommonOperationItem>();

            #region 缩进判断
            string indent = "";
            if (string.IsNullOrEmpty(indent))
            {
                indent = Regex.Match(string.Join("\r\n", code), @"^\s+", RegexOptions.Multiline).Value;
                if (string.IsNullOrEmpty(indent))
                {
                    indent = "\t";
                }
            }
            #endregion

            #region level func
            Func<string, int> gl = str =>
            {
                int i = 0;
                while (i < str.Length && str[i] == indent[i % indent.Length])
                {
                    i++;
                }
                return i / indent.Length;
            };
            #endregion

            #region main
            var root = new CommonOperationItem() //根节点
            {
                Title = "",
                Children = new List<CommonOperationItem>()
            };
            var parentList = new List<CommonOperationItem>() { root };//路径

            int lastLevel = gl(code[0]);

            List<string> pathList = new List<string>();

            foreach (string line in code)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                var level = gl(line);
                var item = CommonOperationItem.ParseLine(line.Trim(), true);
                //同级
                if (level == lastLevel)
                {
                    parentList.Last().Children.Add(item);
                }
                //父级兄弟
                else if (level < lastLevel)
                {
                    if (parentList.Count > 0)
                    {
                        var index = Math.Min(parentList.Count - 1, level);
                        parentList = parentList.Take(index + 1).ToList();
                    }
                    parentList.Last().Children.Add(item);
                }
                //子级
                else if (level > lastLevel)
                {
                    var parent = parentList.Last().Children.Last();
                    parent.Children = new List<CommonOperationItem>() { item };
                    parentList.Add(parent);
                }
                lastLevel = level;
                pathList.Add(string.Join(@"\", parentList.Select(x => x.Title)) + "\\" + item.Title);
            }
            //_context.SetVarValue("pathList", pathList);
            return root.Children;
            #endregion
        }

    }
}
