using Newtonsoft.Json;
using Quicker.Domain;
using Quicker.Domain.Actions.X.Storage;
using Quicker.Public;
using Quicker.Utilities;
using Quicker.View.X;
using Quicker.View.X.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp1.Tools
{
    /// <summary>
    /// todo : 
    ///     如何判断当前是否处于子程序里面
    ///     如果是子程序里面就要复制子程序里面的变量
    /// </summary>
    internal class GetVar
    {
        public static void Exec(IStepContext context)
        {
            //改成动作里面的 window 对象
            var window = context.GetVarValue("window");
            var variables = GetVarWrapper.GetSelectedVariable(window);
            if (variables != null)
            {
                GetVarWrapper.VarToClip(variables);
            }
        }
        public class GetVarWrapper
        {
            public static object GetNPField(object sender, string name)
            {
                var tp = sender.GetType().GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (tp == null) return null;
                return tp.GetValue(sender);
            }
            public static List<ActionVariable> GetItemByEditor(object editor)
            {
                var listbox = GetNPField(editor, "LbVariables") as ListBox;
                var items = listbox.SelectedItems;
                if (items == null) return null;
                return items.Cast<ActionVariable>().ToList();
            }
            public static IList<ActionVariable> GetSelectedVariable(object window)
            {
                var Designer = window as ActionDesignerWindow;
                if (Designer == null) return null;

                var SPEditor = GetNPField(Designer, "SubProgramEditor") as SubProgramEditor;
                var VList = GetNPField(Designer, "VariableListControl") as VariableListControl;
                var items = GetItemByEditor(VList);
                if (items == null)
                {
                    items = GetItemByEditor(SPEditor);
                }
                return items;
            }


            public static void VarToClip(IList<ActionVariable> variables)
            {
                var template = @"{""Variables"":[],""Steps"":[{""StepRunnerKey"":""sys:comment"",""InputParams"":{""note"":{""VarKey"":null,""Value"":""""}},""OutputParams"":{},""IfSteps"":null,""ElseSteps"":null,""Note"":"""",""Disabled"":false,""Collapsed"":false}],""SubPrograms"":[]}";
                ActionStepsDto stepsDto = JsonConvert.DeserializeObject<ActionStepsDto>(template);
                var ip = stepsDto.Steps[0].InputParams;
                string note = "$$" + string.Join("", variables.Select(x => "{" + x.Key + "}"));
                ip.Clear(); ip.Add("note", new ActionStepParam()
                {
                    VarKey = "",
                    Value = note
                });
                stepsDto.Variables = variables;
                DataObject data = new DataObject();
                data.SetData(DataFormats.Text, string.Join("\r\n", variables.Select(x => x.Key)));
                data.SetData("quicker-action-steps", JsonConvert.SerializeObject(stepsDto));
                Clipboard.SetDataObject(data, true);
                AppHelper.ShowSuccess("步骤已写入剪贴板");
            }
            public void TestCopy()
            {
                DataObject data = new DataObject();
                data.SetData(DataFormats.Text, "fefe");
                data.SetData("quicker-action-steps", "{}");
                Clipboard.SetDataObject(data, false);
            }
        }
    }
}
