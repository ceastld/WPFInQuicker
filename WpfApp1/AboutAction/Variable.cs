using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quicker.Domain.Actions.X;
using Quicker.Domain.Actions.X.BuiltinRunners.Misc;
using Quicker.Domain.Actions.X.Storage;
using Quicker.Public.Extensions;
using Quicker.Utilities;
using Quicker.View.X;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.AboutAction
{
    public class Variable : Test.ExpEnvironment
    {
        public static void 注册()
        {
            //Quicker.Domain.Actions.X.Storage.ActionVariable

            _eval.RegisterNamespace(Assembly.Load("Quicker.Common"), "Quicker.Domain.Actions.X");
            var temp_str = new string[]
            {
                "Quicker.Utilities.AppHelper, Quicker",
                "Quicker.View.X.ActionDesignerWindow, Quicker",
                "Quicker.Domain.Actions.X.VarType, Quicker.Common",
                "Quicker.Domain.Actions.X.Storage.ActionVariable, Quicker",
                "Quicker.Domain.Actions.X.BuiltinRunners.Misc.ActionStateWriter, Quicker",
            };
            _eval.RegisterType(temp_str.Select(x => Type.GetType(x)).ToArray());
            //Quicker.Domain.Actions.X.BuildinRunners
        }
        public static void GenerateVariable()
        {
            #region InQuicker
            Func<string, JObject> 生成变量 = (text) =>
            {
                text = text.Trim();
                string[] instructions = text.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                var type = VarType.Text; //默认的未Text
                string varName;
                int index = 0;
                if (instructions.Length == 1)
                {
                    varName = instructions[0];
                }
                else
                {
                    string strType = instructions[0];
                    index = 1;
                    switch (strType)
                    {
                        case "string": break;
                        case "int": type = VarType.Integer; break;
                        case "double": type = VarType.Number; break;
                        case "bool": type = VarType.Boolean; break;
                        case "list": type = VarType.List; break;
                        case "dict": type = VarType.Dict; break;
                        case "time": type = VarType.DateTime; break;
                        case "image": type = VarType.Image; break;
                        case "object":
                        case "any": type = VarType.Any; break;
                        default: index = 0; break;
                    }
                    varName = instructions[index];
                    index++; //索引增加, 方便后面使用
                }

                try { _eval.Execute("int " + varName); }
                catch
                {
                    var message = varName + " 不符合命名规范";
                    AppHelper.ShowWarning(message);
                    throw new Exception(message);
                }
                var temp1 = new ActionVariable()
                {
                    Key = varName,
                    Type = type
                };
                for (int i = index; i < instructions.Length; i++)
                {
                    switch (instructions[i])
                    {
                        case "-o":
                        case "-output": temp1.IsOutput = true; break;
                        case "-i":
                        case "-input": temp1.IsInput = true; break;
                        case "-s":
                        case "-state": temp1.SaveState = true; break;
                        default: break;
                    }
                }
                return JObject.FromObject(temp1);
            };

            List<string> list = (List<string>)_context.GetVarValue("list"); //
            JArray varList = new JArray();
            List<string> nameList = new List<string>();
            foreach (var x in list)
            {
                //var temp = variableTemp;
                //temp.Merge(生成变量(x)); // 防止直接引用
                if (string.IsNullOrWhiteSpace(x)) continue;
                var temp = 生成变量(x);
                varList.Add(temp);
                nameList.Add("{" + temp["Key"].ToString() + "}");
            }
            JObject template = JObject.Parse((string)_context.GetVarValue("template"));
            template["Variables"] = varList;
            template["Steps"][0]["InputParams"]["note"]["Value"] = "$$" + string.Join("", nameList);
            _context.SetVarValue("stepData", template.ToString());
            #endregion
        }

        public static List<string> 生成赋值语句()
        {
            var selectedVar = (_context.GetVarValue("variableList") as IEnumerable<object>).Cast<ActionVariable>();
            List<string> nameList = new List<string>();
            foreach (var item in selectedVar)
            {
                var key = item.Key;
                string type_str = "object";
                switch (item.Type)
                {
                    case VarType.Text: type_str = "string"; break;
                    case VarType.Number: type_str = "double"; break;
                    case VarType.Boolean: type_str = "bool"; break;
                    case VarType.Image: type_str = "image"; break;
                    case VarType.List: type_str = "List<string>"; break;
                    case VarType.DateTime: type_str = "DateTime"; break;
                    case VarType.Dict: type_str = "Dictionary<string,object>"; break;
                    case VarType.Integer: type_str = "int"; break;
                    case VarType.Table: type_str = "DataTable"; break;
                    case VarType.Any: type_str = "object"; break;
                    default: break;
                }
                var str = $"var {key} = ({type_str})_" + $"context.GetVarValue(\"{key}\");";
                nameList.Add(str);
            }
            return nameList;
        }

        public static string 查看状态变量()
        {
            #region 获取编辑窗口动作id
            var window = AppHelper.GetQuickerActiveWindow();
            if (window.GetType().Name != "ActionDesignerWindow")
            {
                throw new Exception("请在动作编辑窗口上使用");
            }
            var actionId = (window as ActionDesignerWindow).ResultActionItem.Id;
            #endregion

            #region aaa
            var selectedVar = (_context.GetVarValue("variableList") as IEnumerable<object>).Cast<ActionVariable>();
            var dataObj = new JObject();
            string stateKey;
            foreach (var item in selectedVar)
            {
                stateKey = "$var:" + item.Key;
                var result = JObject.FromObject(ActionStateWriter.ReadActionStateValue(actionId, stateKey));
                dataObj[item.Key] = (bool)result["Item1"] ? result["Item2"] : null;
            }
            return dataObj.ToJson(true);
            #endregion
        }

        static Func<string, bool> IsIdentifier = (text) =>
        {
            if (string.IsNullOrEmpty(text)) return false;
            if (!char.IsLetter(text[0]) && text[0] != '_')
                return false;
            for (int i = 1; i < text.Length; i++)
            {
                if (!char.IsLetterOrDigit(text[i]) && text[i] != '_')
                    return false;
            }
            return true;
        };
    }
}
