using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json.Linq;
using Quicker.Public.Entities;
using Quicker.Public.Interfaces;

namespace WpfApp1.Test.Test
{
    public class TestClass
    {
        public static IActionContext _context;
        public static bool GetDataFromClipboard()
        {
            string cbType;
            if (Clipboard.ContainsText())
            {
                cbType = "text";
                _context.SetVarValue("data", Clipboard.GetText());
            }
            else if (Clipboard.ContainsFileDropList())
            {
                cbType = "file";
                _context.SetVarValue("data", Clipboard.GetFileDropList().Cast<string>());
            }
            else if (Clipboard.ContainsData("quicker-action-steps"))
            {
                cbType = "actionSteps";

                _context.SetVarValue("data", (string)Clipboard.GetData("quicker-action-steps"));
            }
            else if (Clipboard.ContainsData("quicker-action-item"))
            {
                cbType = "actionItem";
                _context.SetVarValue("data", JObject.FromObject(Clipboard.GetData("quicker-action-item")).ToString());
            }
            else
            {
                return false;
            }
            _context.SetVarValue("cbType", cbType);
            return true;
        }
        public static dynamic 随机列表()
        {
            List<int> list1 = Enumerable.Range(1, 34).ToList();
            List<int> list2 = new List<int>();
            Random random = new Random();
            for (int i = 0; i < 6; i++)
            {
                int index = random.Next(list1.Count);
                list2.Add(list1[index]);
                list1.RemoveAt(index);
            }
            list2.Add(random.Next(1, 17));
            return string.Join(" ", list2.Select(x => x.ToString().PadLeft(2, '0')));
        }
        public static void 阶乘()
        {
            Func<int, int> fact = null;
            fact = num =>
            {
                if (num < 0) return 0;
                if (num <= 1) return 1;
                else return fact(num - 1);
            };
            fact(1);
        }
        public IList<CommonOperationItem> Parse()
        {
            object temp = null;
            var list = _context.GetVarValue("list");
            if (list is string)
            {
                temp = (string)list;
            }
            else if (list is IEnumerable<string>)
            {
                temp = string.Join("\r\n", (IEnumerable<string>)list);
            }
            return CommonOperationItem.ParseLines((string)temp, true, true);
        }
    }
}
