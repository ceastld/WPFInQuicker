using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using SHDocVw;
using System.Diagnostics.CodeAnalysis;
using System.Xml;
using System.Text.RegularExpressions;
using naa;

//Console.WriteLine(val.Attributes["filepath"].Value);
//Console.WriteLine(val?.InnerText ?? "NULL");


var dict = new Dictionary<int, object>()
        {
            {1, 20 },
            {3, 122},
            {5, 322 }
        };
Console.WriteLine(new sdkfds().test(dict).ToJson(true));

class sdkfds
{
    public Dictionary<int, object>? test(Dictionary<int, object> dict)
    {
        var keys = dict.Keys;
        if (keys.Count == 0) return null;
        int k_max = keys.Max();
        int k_min = keys.Min();
        int i = k_min;
        int j = k_min;
        return Enumerable.Range(k_min, k_max - k_min + 1).ToDictionary(x => x, x =>
            {
                if (dict.ContainsKey(i++))
                {
                    j = i;
                }
                return dict[j];
            });
    }

    unsafe float test(float number)
    {
        long i;
        float x2, y;
        const float threehalfs = 1.5F;

        x2 = number * 0.5F;
        y = number;
        i = *(long*)&y;                       // evil floating point bit level hacking（对浮点数的邪恶位元hack）
        i = 0x5f3759df - (i >> 1);               // what the fuck?（这他妈的是怎么回事？）
        y = *(float*)&i;
        y = y * (threehalfs - (x2 * y * y));   // 1st iteration （第一次迭代）
                                               //y = y * (threehalfs - (x2 * y * y));   

        return y;
    }
}

public class A : IEqualityComparer<A>, IEquatable<A>
{
    public A(int id)
    {
        Id = id;
    }
    public int Id { get; set; }

    public bool V_E(A other) => Id == other.Id;

    public bool Equals(A? x, A? y) => x is null ? y is null : x.Equals(y);

    public int GetHashCode([DisallowNull] A obj) => obj.Id % 2;

    public bool Equals(A? other) => other is not null && (Id - other.Id) % 2 == 0;
}

public static class Explorer
{
    [DllImport("User32.dll")]
    public static extern IntPtr GetForegroundWindow();     //获取活动窗口句柄

    public static void test()
    {
        var windows = new ShellWindows();

        windows.WindowRegistered += delegate
        {
            Console.WriteLine("窗口打开");
        };

        windows.WindowRevoked += delegate
        {
            Console.WriteLine("窗口关闭");
        };
    }

    public static string GetCurrentPath()
    {
        var handle = GetForegroundWindow();
        var ie = GetAllExplorer().FirstOrDefault(x => (IntPtr)x.HWND == handle);
        try
        {
            return new Uri(ie.LocationURL).LocalPath;
        }
        catch
        {
            return "";
        }
    }

    public static IEnumerable<InternetExplorer> GetAllExplorer()
    {
        ShellWindows shellWindows = new ShellWindows();
        foreach (InternetExplorer ie in shellWindows)
        {
            var filename = Path.GetFileNameWithoutExtension(ie.FullName).ToLower();
            if (filename.Equals("explorer"))
            {
                yield return ie;
            }
        }
    }
}

public class Test
{
    public void aaaaa()
    {
        var file = new FileInfo(@"E:\OneDrive\桌面\测试\bjsKA8vDqCYgWMtMZxNB9J");
        using (var fs = File.OpenRead(file.FullName))
        {
            fs.Position = 100;
            byte[] b = new byte[1024];
            var temp = new UTF8Encoding(true);
            while (fs.Read(b, 0, b.Length) > 0)
            {
                Console.WriteLine(temp.GetString(b, 0, 10));
            }
        }
    }

    internal static void test11()
    {
        for (int i = 0; i < 1000; i++)
        {
            var str = Test.GetToNowTimeString(DateTime.Now - TimeSpan.FromSeconds(300 * i));
            Console.WriteLine(str);
        }
    }
    internal static string GetToNowTimeString(DateTime time)
    {
        var now = DateTime.Now;
        var sub = now - time;
        if (sub.TotalDays > 30)
            return sub.Days + "天前";
        else if (sub.TotalHours > 24)
            return $"{sub.Days}天{sub.Hours}小时前";
        else if (sub.TotalMinutes > 60)
            return $"{sub.Hours}小时{sub.Minutes}分钟前";
        else if (sub.TotalSeconds > 60)
            return $"{sub.Minutes}分钟前";
        else if (sub.TotalMilliseconds > 1000)
            return $"{sub.Seconds}秒前";
        else
            return "刚刚";
    }
    public static void 灵运文件夹下一层解散()
    {
        var dir = @"a\b\c";

        if (!Directory.Exists(dir)) return;

        var targetDir = Path.GetDirectoryName(dir); //a\b

        if (!Directory.Exists(targetDir)) return;

        var newDir = Path.Combine(targetDir, Guid.NewGuid().ToString());

        Directory.Move(dir, newDir); //先重命名一下

        foreach (var file in Directory.EnumerateFiles(newDir))
        {
            var name = Path.GetFileName(file);
            var targetPath = Path.Combine(targetDir, name);
            File.Move(file, targetPath);
        }
        foreach (var dirPath in Directory.EnumerateDirectories(newDir))
        {
            var name = Path.GetFileName(dirPath);
            var targetPath = Path.Combine(targetDir, name);
            Directory.Move(dirPath, targetPath);
        }

        Directory.Delete(newDir);
    }

    public static void aa()
    {
        var arr1 = new[] { 1, 2, 3, 4, 5, 3, 4, 5, 6, 7, 9, 9, 8, 9, 2, 3, 4 };
        var arr2 = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 1, 2, 3 };
        var len1 = arr1.Length;
        var len2 = arr2.Length;
        var dp = new int[len1 + 1, len2 + 1];//初始值为 0,不需要再次赋值

        for (int i = 1; i <= len1; i++)
        {
            for (int j = 1; j <= len2; j++)
            {
                if (arr1[i - 1] == arr2[j - 1])
                {
                    dp[i, j] = dp[i - 1, j - 1] + 1;
                }
                else
                {
                    dp[i, j] = Math.Max(dp[i - 1, j], dp[i, j - 1]);
                }
            }
        }

        Console.WriteLine(dp[len1, len2]);

        //回溯，输出数组
        void pre_print(int i, int j)
        {

            if (i > 0 && dp[i, j] == dp[i - 1, j])
            {
                pre_print(i - 1, j);
                Console.WriteLine(" - " + arr1[i - 1]);
            }
            else if (j > 0 && dp[i, j] == dp[i, j - 1])
            {
                pre_print(i, j - 1);
                Console.WriteLine(" + " + arr2[j - 1]);
            }
            else if (i > 0 && j > 0)
            {
                pre_print(i - 1, j - 1);
                Console.WriteLine("   " + arr1[i - 1]);
            }
        }

        pre_print(len1, len2);
    }
}

namespace naa
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    public static class JsonExtensions
    {
        public static string ToJson(this object obj, bool indent = false, bool ignoreNull = false)
        {
            if (obj == null) return "";
            Formatting formatting = indent ? Formatting.Indented : Formatting.None;
            JsonSerializerSettings? settings = ignoreNull ? new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore, //忽略嵌套
            } : null;
            return JsonConvert.SerializeObject(obj, formatting, settings);
        }

        public static void Populate<T>(this JToken value, T target) where T : class
        {
            using var sr = value.CreateReader();
            JsonSerializer.CreateDefault().Populate(sr, target); // Uses the system default JsonSerializerSettings
        }
    }
}
