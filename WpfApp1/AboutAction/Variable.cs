using Newtonsoft.Json.Linq;
using Quicker.Domain.Actions.X;
using Quicker.Domain.Actions.X.Storage;
using Quicker.Utilities;
using System;
using System.Collections.Generic;
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
            _eval.RegisterNamespace(Assembly.Load("Quicker"), "Quicker.Domain.Actions.X");
            _eval.RegisterType(Type.GetType("Quicker.Utilities.AppHelper, Quicker"));
            _eval.RegisterType(Type.GetType("Quicker.Domain.Actions.X.VarType, Quicker"));
            _eval.RegisterType(Type.GetType("Quicker.Domain.Actions.X.Storage.ActionVariable, Quicker"));
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
            template["Steps"][0]["Note"] = "$$" + string.Join("", nameList);
            _context.SetVarValue("stepData", template.ToString());
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
