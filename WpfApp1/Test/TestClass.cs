using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quicker.Common;
using Quicker.Domain.Actions.X;
using Quicker.Domain.Actions.X.BuiltinRunners.Misc;
using Quicker.Domain.Actions.X.Storage;
using Quicker.Public;
using Quicker.Public.Entities;
using Quicker.Public.Extensions;
using Quicker.Public.Interfaces;
using Quicker.Utilities;
using Quicker.Utilities._3rd;
using Quicker.Utilities.UI.Behaviors;
using Quicker.View;

namespace WpfApp1.Test
{
    public static class Expression
    {
        private static readonly string ZExpretionPath = @"C:\Program Files\Quicker\Z.Expressions.Eval.dll";
        public static void test()
        {
            var context = new Z.Expressions.EvalContext();
            context.Execute("");
            Z.Expressions.Eval.Execute("1+1");
        }
        public static object RunExpression(string exip)
        {
            var assembly = Assembly.LoadFrom(ZExpretionPath);
            var type_ec = Type.GetType("Z.Expressions.EvalContext, Z.Expressions.Eval");
            var _context = assembly.CreateInstance("Z.Expressions.EvalContext, Z.Expressions.Eval");
            return null;
        }
        public static Type GetEval()
        {
            Assembly assembly = Assembly.LoadFrom(ZExpretionPath);
            return assembly.GetType("Z.Expressions.Eval");
        }
        public static object RunExpressionStatic<TResult>(string exp)
        {
            var Eval = GetEval();
            //MethodInfo method = Eval.GetMethod("Execute", new[] { typeof(string) });
            MethodInfo method = Eval.GetMethods().First(x => x.IsGenericMethod && x.Name == "Execute");
            MethodInfo method1 = method.MakeGenericMethod(typeof(TResult));
            return method1.Invoke(null, new object[] { exp });
        }
    }
    public class ActionEditor
    {
        public static string GetMethodName()
        {
            var type = typeof(Quicker.View.X.ActionDesignerWindow);
            List<string> methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Select(x => x.Name).ToList();
            var index = methods.IndexOf("CheckIfCanSave");
            var myMethod = methods.Skip(index).ToArray();
            return myMethod[6];
        }
    }
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
            IList<CommonOperationItem> list2 = new List<CommonOperationItem>();
            if (list is string)
            {
                list2 = CommonOperationItem.ParseLines((string)list, true, true);
            }
            else if (list is IEnumerable<string>)
            {
                list2 = CommonOperationItem.ParseLines((list as IList<string>).JoinToString("\r\n"), true, true);
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
        public List<string> 判断大小括号()
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
        public static void 设置是否可以触发Quicker()
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
        public static string 生成随机字符串()
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

        public static SmartCollection<CommonOperationItem> 列表_构造可视化对象2()
        {
            var list = _context.GetVarValue("list");
            var smartList = new SmartCollection<CommonOperationItem>((IEnumerable<CommonOperationItem>)list);
            foreach (var item in smartList)
            {
                if (item.Children != null && !(item.Children is SmartCollection<CommonOperationItem>))
                {
                    item.Children = new SmartCollection<CommonOperationItem>(item.Children);
                }
            }
            return smartList;
        }

        public void tesss()
        {
            string[] code = new[] { "fsef", "fse", "fsefes" };
            foreach (var line in code)
            {
                var aaaa = new List<string>() { line };
            }
            "fe".ToJson(true);

        }
        public void 词典_批量取值与赋值pro()
        {
            #region for valuation
            var main_context = (Quicker.Public.Interfaces.IActionContext)_context.GetVarValue("main_context");
            var dict = (Dictionary<string, object>)_context.GetVarValue("dict");
            var operationtype = (string)_context.GetVarValue("操作类型");
            var mapList = (List<string>)_context.GetVarValue("mapList");
            #endregion

            Action<string, string> domapValue;
            switch (operationtype)
            {
                case "get": //词典取值到变量
                    domapValue = (dkey, vkey) =>
                     {
                         if (dict.ContainsKey(dkey))
                         {
                             main_context.SetVarValue(vkey, dict[dkey]);
                         }
                     };
                    break;
                case "set": //变量赋值给词典
                    domapValue = (dkey, vkey) =>
                     {
                         var value = main_context.TryGetValue(vkey, null);
                         if (value != null)
                         {
                             dict[dkey] = value;
                         }
                     };
                    break;
                default: //其余情况直接返回
                    return;
            };
            string dk;
            string vk;
            if (mapList.Count == 0) mapList = dict.Keys.ToList();
            foreach (var mapstr in mapList)
            {
                if (mapstr.Contains(':'))
                {
                    var sp = mapstr.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                    dk = sp[0];
                    vk = sp[1].Trim('{', '}');
                }
                else
                {
                    dk = vk = mapstr.Trim('{', '}');
                }
                domapValue(dk, vk);
            }
        }

        public object 全局缓存()
        {
            string id = (string)_context.GetVarValue("id");
            if (string.IsNullOrEmpty(id)) id = "dddf679a31b44c9d81b9521357a17c47";//随机生成的guid，不要改
            string key = (string)_context.GetVarValue("key");
            object dValue = _context.GetVarValue("defaultValue");
            string op = (string)_context.GetVarValue("operation");
            var dc = Z.Expressions.EvalManager.DefaultContext;
            var dcdict = dc.AliasGlobalVariables;
            var dict = (Dictionary<string, object>)dcdict.GetOrAdd(id, new Dictionary<string, object>());
            switch (op)
            {
                case "read":
                    return dict.GetValueOrDefault(key, dValue);
                case "remove":
                    dict.Remove(key); break;
                case "write":
                    dict[key] = _context.GetVarValue("value"); break;
                case "delete":
                    dc.UnregisterGlobalVariable(id); break;
                default:
                    break;
            }
            return null;
        }

        public object 全局状态存取()
        {
            string id = (string)_context.GetVarValue("actionId");
            if (string.IsNullOrEmpty(id)) id = _context.ActionId;
            //"dddf679a31b44c9d81b9521357a17c47";//随机生成的guid，不要改
            string key = (string)_context.GetVarValue("key");
            object dValue = _context.GetVarValue("defaultValue");
            string op = (string)_context.GetVarValue("operation");
            switch (op)
            {
                case "read":
                    var res = JObject.FromObject(ActionStateWriter.ReadActionStateValue(id, key));
                    return (bool)res["Item1"] ? res["Item2"] : dValue;
                case "write":
                    var obj = _context.GetVarValue("value");
                    var value = (obj is string) ? (string)obj : obj.ToJson();
                    ActionStateWriter.WriteActionState(id, key, value);
                    break;
                case "remove": ActionStateWriter.WriteActionState(id, key, "*NULL*"); break;
                case "delete": ActionStateWriter.DeleteStateFile(id); break;
                default: //继续执行后续步骤
                    _context.SetVarValue("keepon", true);
                    break;
            }
            return null;
        }
        public List<CommonOperationItem> 文件遍历()
        {
            List<string> pathList = new List<string>();
            Func<string, CommonOperationItem> one_file = file => new CommonOperationItem
            {
                Icon = "icon:" + file,
                Title = Path.GetFileName(file),
                Data = file,
                Description = file
            };
            Func<string, CommonOperationItem> one_path = null;
            one_path = path =>
            {
                var item = one_file(path);
                if (Directory.Exists(path))
                    item.Children = Directory.GetDirectories(path).Concat(Directory.GetFiles(path)).Select(one_path).ToList();
                return item;
            };
            return pathList.Select(x => one_path(x)).ToList();
        }
        public int 阶乘1()
        {
            Func<int, int> fact = null;
            fact = n =>
             {
                 if (n <= 1) return 1;
                 return n * fact(n - 1);
             };
            return fact(10);
        }
        private void testaa()
        {
            var test = new[] { 1, 2, 3 }.Select(x => new JObject());
            test.ToList().ForEach(x => { x["a"] = ""; });
            /// 返回的test中的所有JObject仍然没有键a
        }
        public string ComputeHash(string path, System.Security.Cryptography.HashAlgorithm algorithm)
        {
            using (var file = File.OpenRead(path))
            {
                var hash_byte = algorithm.ComputeHash(file);
                return BitConverter.ToString(hash_byte).Replace("-", "");
            }
        }
        public string SingleComputeHash(string path, string type)
        {
            string hash = "";
            switch (type)
            {
                case "md5": hash = ComputeHash(path, new MD5CryptoServiceProvider()); break;
                case "sha1": hash = ComputeHash(path, new SHA1CryptoServiceProvider()); break;
                case "sha256": hash = ComputeHash(path, new SHA256Managed()); break;
                default:
                    break;
            }
            return hash;
        }
        //表达式里面不支持 out，不支持元组
        public object MultiComputeHash(string path, bool hash_md5, bool hash_sha1, bool hash_sha256)
        {
            Func<string, HashAlgorithm, string> ComputeHash = (p, algorithm) =>
             {
                 using (var file = File.OpenRead(p))
                 {
                     var hash_byte = algorithm.ComputeHash(file);
                     return BitConverter.ToString(hash_byte).Replace("-", "");
                 }
             };
            string md5 = "";
            string sha1 = "";
            string sha256 = "";
            if (hash_md5) md5 = ComputeHash(path, new MD5CryptoServiceProvider());
            if (hash_sha1) sha1 = ComputeHash(path, new SHA1CryptoServiceProvider());
            if (hash_sha256) sha256 = ComputeHash(path, new SHA256Managed());
            return new { md5, sha1, sha256 };
        }
        public void alfej()
        {
            new Random().Next();
            _eval.RegisterType(Type.GetType("Quicker.Domain.Services.RunningActionMgr, Quicker"));
            new Quicker.Domain.Services.RunningActionMgr().GetAllRunningActions();
            _eval.RegisterType(Type.GetType("Quicker.Domain.AppState, Quicker"));
            Quicker.Domain.AppState.AppServer.GetRunningActionsInfo();
        }

        public void MultiGetSetVaiable(IActionContext context, Dictionary<string, object> dict, string op, List<string> mapList)
        {
            Action<string, string> domapValue;
            switch (op)
            {
                case "get": //词典取值到变量
                    domapValue = (dkey, vkey) =>
                    {
                        if (dict.ContainsKey(dkey))
                        {
                            context.SetVarValue(vkey, dict[dkey]);
                        }
                    };
                    break;
                case "set": //变量赋值给词典
                    domapValue = (dkey, vkey) =>
                    {
                        var value = context.TryGetValue(vkey, null);
                        if (value != null)
                        {
                            dict[dkey] = value;
                        }
                    };
                    break;
                default: //其余情况直接返回
                    return;
            };
            string dk;
            string vk;
            if (mapList.Count == 0) mapList = dict.Keys.ToList();
            foreach (var mapstr in mapList)
            {
                if (mapstr.Contains(':'))
                {
                    var sp = mapstr.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                    dk = sp[0];
                    vk = sp[1].Trim('{', '}');
                }
                else
                {
                    dk = vk = mapstr.Trim('{', '}');
                }
                domapValue(dk, vk);
            }
        }

        public object ActionInstanceGlobalVar()
        {
            var id = _context.ActionId;
            var root = _context.GetRootContext();

            string key = (string)_context.GetVarValue("key");
            object dValue = _context.GetVarValue("defaultValue");
            string op = (string)_context.GetVarValue("operation");

            var dict = (Dictionary<string, object>)root.TryGetValue(id, null);
            if (dict == null)
            {
                dict = new Dictionary<string, object>();
                root.SetVarValue(id, dict);
            }

            switch (op)
            {
                case "read": return dict.GetValueOrDefault(key, dValue);
                case "write": dict[key] = _context.GetVarValue("value"); break;
                case "multi_get":
                case "multi_set":
                    var context = (IActionContext)_context.GetVarValue("main_context");
                    var mapList = (List<string>)_context.GetVarValue("mapList");
                    MultiGetSetVaiable(context, dict, op.Split('_')[1], mapList);
                    break;
                case "remove": dict.Remove(key); break;
                case "clear": dict.Clear(); break;
                default:
                    break;
            }
            return null;
        }
        public void aaa()
        {
            LinkedList<string> list = new LinkedList<string>();
            list.FirstOrDefault(x => x == "fse");
            list.IndexOf(x => x == "fsefe");
            list.Remove("fsef");
        }

        public List<string> getActionIcons(IList<string> actionIds)
        {
            var result = new List<string>();
            foreach (var id in actionIds)
            {
                try
                {
                    var item = Quicker.Domain.AppState.DataService.GetActionById(id).Item1;
                    result.Add(item.Icon);
                }
                catch
                {
                    result.Add("");
                }
            }
            return result;
        }
    }
}

namespace 获取文件夹路径aaa
{
    using SHDocVw;
    using Quicker.Utilities.Win32;
    public class aaa
    {
        public static string 获取文件夹路径()
        {
            var handle = NativeMethods.GetForegroundWindow();
            ShellWindows shellWindows = new ShellWindows();
            foreach (InternetExplorer ie in shellWindows)
            {
                var filename = Path.GetFileNameWithoutExtension(ie.FullName).ToLower();
                if (filename.Equals("explorer") && (IntPtr)ie.HWND == handle)
                {
                    try
                    {
                        return new Uri(ie.LocationURL).LocalPath;
                    }
                    catch
                    {
                        return "";
                    }
                }
            }
            return "";
        }
    }
}

namespace WpfApp1.Test1
{
    using WpfApp1.Test;
    using Growl = HandyControl.Controls.Growl;
    using HandyControl.Data;
    using GrowlWindow = HandyControl.Controls.GrowlWindow;
    public class TestClass1 : ExpEnvironment
    {
        public void Fixup()
        {
            var panel = Growl.GrowlPanel;
            if (panel != null)
            {
                try
                {
                    var win = System.Windows.Window.GetWindow(panel);
                    if (!win.IsVisible)
                    {
                        Growl.GrowlPanel = null;
                    }
                    return;
                }
                catch
                {
                    Growl.GrowlPanel = null;
                    return;
                }
            }
        }

        /// <summary>
        /// 注册类型
        /// HandyControl.Controls.Growl, HandyControl
        /// HandyControl.Data.GrowlInfo, HandyControl
        /// HandyControl.Data.InfoType, HandyControl
        /// Quicker.Utilities.AppHelper, Quicker
        /// System.Windows.Window, PresentationFramework
        /// </summary>
        /// <param name="type">
        /// 成功|success
        /// 信息|info
        /// 警告|warning
        /// 错误|error
        /// 崩溃|fatal
        /// 询问|ask
        /// 清空|clear
        /// </param>
        /// <param name="wait">
        /// wait = 0，则 stay 一直停留
        /// </param>
        public void 提示消息(string type, string message, int wait, bool global, string cmd, bool showDateTime)
        {
            Fixup();

            if (type.Equals("clear", StringComparison.OrdinalIgnoreCase))
            {
                Growl.ClearGlobal();
                Growl.Clear();
                return;
            }

            var infoType = (InfoType)Enum.Parse(typeof(InfoType), type, true);
            var methodName = infoType.ToString();
            if (global) methodName += "Global";

            var method = typeof(Growl).GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
                        .Where(x => x.Name == methodName)
                        .Where(x => x.GetParameters().FirstOrDefault()?.ParameterType == typeof(GrowlInfo))
                        .FirstOrDefault();

            var info = new GrowlInfo()
            {
                WaitTime = wait,
                Message = message,
                StaysOpen = wait == 0,
                IsCustom = true,
                ShowDateTime = showDateTime,
            };

            if (infoType == InfoType.Ask)
            {
                info.ActionBeforeClose = result =>
                {
                    if (result)
                    {
                        AppHelper.TryOpenUrlOrFile(cmd);
                    }
                    return true;
                };
            }

            method?.Invoke(null, new object[] { info });
        }
        public void test(int a)
        {
            //Growl.Ask
            //FontAwesome5.WPF.SvgAwesome

            //Growl.GrowlPanel.Children
        }

        public int GetLines(string data)
        {
            const string key = "\"StepRunnerKey\"";
            return (data.Length - data.Replace(key, "").Length) / key.Length;
        }
        public string tj1()
        {
            var items = Quicker.Domain.AppState.DataService.GetAllActionItems()
                .Where(x => (int)x.ActionType == 24)
                .Where(x => x.TemplateId == null)
                .Select(item =>
                {
                    return new
                    {
                        item,
                        count = GetLines(item.Data)
                    };
                })
                .OrderByDescending(x => x.count)
                .ToList();
            var sb = new StringBuilder();
            sb.AppendLine($"组合动作，共计 {items.Sum(x => x.count)} 行");
            foreach (var item in items)
            {
                sb.AppendLine($"{item.item.Title}\t{item.count} 行");
            }
            return sb.ToString();
        }

        public string tj2()
        {
            var items = Quicker.Domain.AppState.DataService.GlobalSubPrograms
                    .Select(item =>
                    {
                        return new
                        {
                            item,
                            count = GetLines(item.ToJson())
                        };
                    })
                    .OrderByDescending(x => x.count)
                    .ToList();
            var sb = new StringBuilder();
            sb.AppendLine($"公共子程序，共计 {items.Sum(x => x.count)} 行");
            foreach (var item in items)
            {
                sb.AppendLine($"{item.item.Name}\t{item.count} 行");
            }
            return sb.ToString();
        }
        //public class GrowlPlus
    }
}
