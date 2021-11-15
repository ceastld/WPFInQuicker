using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.Expressions;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var context = new EvalContext();
            //context.Execute(@"'aaa''.Intersect("a");")
            Console.WriteLine("fef".Intersect("f").ToList());
            levenshtein("https://getquicker.net", "tps://net");
            Console.Read();
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
