using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quicker.Public;
using Quicker.Public.Entities;
using Quicker.Public.Extensions;
using Quicker.Public.Interfaces;
using Quicker.Utilities;
using Quicker.Utilities._3rd;
using Quicker.Utilities.UI.Behaviors;
using Quicker.View;

namespace WpfApp1.Test.Test
{
    public class TestClass : ExpEnvironment
    {
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
            object list = _context.GetVarValue("list");
            if (list is string)
            {
                return new SmartCollection<CommonOperationItem>(CommonOperationItem.ParseLines((string)list, true, true));
            }
            else if (list is IEnumerable<string>)
            {
                return new SmartCollection<CommonOperationItem>(CommonOperationItem.ParseLines(((IList<string>)list).MergeToString("\r\n"), true, true));
            }
            else if (list is IEnumerable<CommonOperationItem>)
            {
                return new SmartCollection<CommonOperationItem>((List<CommonOperationItem>)list);
            }
            return new SmartCollection<CommonOperationItem>();
        }
        public IList<CommonOperationItem> Parse1()
        {
            return new List<CommonOperationItem>().Select(x =>
            {
                var index = x.OriginText.LastIndexOf('|');
                if (index >= 0)
                {
                    x.Data = x.OriginText.Substring(index + 1);
                }
                if (x.Operation == null && !x.Data.StartsWith("close:"))
                {
                    x.Data = "close:" + x.Data;
                }
                return x;
            }).ToList();
        }
        public string WriteClipboard()
        {
            object obj = _context.GetVarValue("obj");
            return obj is string ? (string)obj : JsonConvert.SerializeObject(obj);
        }
        public void aaaa()
        {
            //Quicker.Domain.AppState.CurrentProcessName;
            Quicker.Domain.AppState.AppServer.GetLastForegroundWindow();

        }
        public StringBuilder xaml代码处理()
        {
            string text = "";
            StringBuilder result = new StringBuilder();
            bool cantrim = false;
            foreach (var line in text.SplitToList())
            {
                string temp = line.Trim();
                if (temp.EndsWith(">"))
                {
                    result.Append(temp);
                    cantrim = false;
                    continue;
                }
                else if (temp.StartsWith("<"))
                {
                    result.Append(temp);
                    cantrim = true;
                    continue;
                }
                if (cantrim) result.Append(" " + temp);
                else result.Append("\r\n" + line);
            }
            return result;
        }
        public void 编辑界面()
        {
            var id = new ActionEditorWindow(null).EditingActionItem.Id;

        }
        public bool 使用说明()
        {
            #region input
            string instruction = "";
            #endregion
            //没有输入版本,直接开始提示

            //判断提示是否有改变,没有改变则不提示
            return _context.ReadState("action_instruction", "") == instruction;

            //版本不是最新,开始提示
            //var last = Convert.ToDouble(_context.ReadState("action_version", "0"));
            //if (last < Convert.ToDouble(version)) return false;

        }
        public List<string> test()
        {
            var code = new List<string>();
            var index = code.IndexOf(x => x.Trim() == "{");
            if (index != -1)
            {
                var index2 = code.LastIndexOf(x => x.Trim() == "}");
                if (index2 != -1)
                {
                    code = code.Take(index2 - 1).Skip(index).ToList();
                }
            }
            return code;
        }
        public static void 图标收藏判断()
        {
            string icon = (string)_context.GetVarValue("icon");
            if (icon.StartsWith("fa:"))
            {
                var re = new Regex(@"(fa:\w+):(([#\w]+))");
                if (re.IsMatch(icon))
                {
                    var groups = re.Match(icon).Groups;
                    _context.SetVarValue("icon", groups[1].Value);
                    _context.SetVarValue("color", groups[2].Value);
                }
                _context.SetVarValue("type", "internal");
            }
            else
            {
                _context.SetVarValue("type", "normal");
            }
        }
        public static void 图标转换()
        {
            var fi = "fab";
            switch (fi)
            {
                case "fal": fi = "fa:Light_"; break;
                case "far": fi = "fa:Regular_"; break;
                case "fas": fi = "fa:Solid_"; break;
                case "fab": fi = "fa:Brands_"; break;
            }
        }
        private static void 设置是否可以触发Quicker()
        {
            var value = true;
            IntPtr handle = (IntPtr)0;
            if (value)
            {
                RegisterHWndBehavior.HWnds[handle] = 0;
            }
            else
            {
                RegisterHWndBehavior.HWnds.TryRemove(handle, out int a);
            }
        }
        public static string GenerateRandomString()
        {
            Func<string, object> gvv = _context.GetVarValue;
            Action<string, object> svv = _context.SetVarValue;

            string chars = (string)gvv("source");
            int length = (int)gvv("length");

            var stringChars = new char[length];
            var random = new Random();
            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }
            return new string(stringChars);
        }
        public static void Exec(IStepContext context)
        {
            
        }
    }
}
