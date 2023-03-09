
using System.IO;

public static class Call
{
    public static void test()
    {
        var r = new Random();
        for (int i = 0; i < 1000; i++)
        {
            CallMethod(() => Console.WriteLine(i));
            Thread.Sleep(r.Next(0, 100));
        }
    }
    public static Queue<DateTime> CallTimeQ = new Queue<DateTime>();
    public static void CallMethod(Action method)
    {
        if (CallTimeQ.Count < 10)
        {
            CallTimeQ.Enqueue(DateTime.Now);
            method.Invoke();
        }
        else //去除多余的
        {
            var time = DateTime.Now - TimeSpan.FromSeconds(3);
            while (CallTimeQ.Any() && CallTimeQ.Peek() < time)
            {
                CallTimeQ.Dequeue();
            }
        }
    }
    public static string post(string path, string p)
    {
        var dir = Path.GetDirectoryName(path);
        var name = Path.GetFileNameWithoutExtension(path);
        var ext = Path.GetExtension(path);
        return Path.Combine(dir, $"{name}p{ext}");
    }
}
