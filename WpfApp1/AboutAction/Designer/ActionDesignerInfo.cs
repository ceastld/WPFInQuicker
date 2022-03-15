using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using Z.Expressions;
using Quicker.View.X.Controls;
using System.Reflection;
using System.Linq;

namespace WpfApp1.AboutAction.Designer
{
    public class ActionDesignerInfo
    {
        static class WinOp
        {
            [DllImport("User32.dll")]
            public static extern IntPtr GetForegroundWindow();     //获取活动窗口句柄

            public static IntPtr GetHandle(Window window)
            {
                return new WindowInteropHelper(window).Handle;
            }
            public static WType GetWindow<WType>() where WType : class
            {
                return HwndSource.FromHwnd(GetForegroundWindow()).RootVisual as WType;
            }
        }
        public static void Exec(Quicker.Public.IStepContext context)
        {
            string type = (string)context.GetVarValue("opType");
            string methodName = GetUpdataUIMethodName();
            var win = WinOp.GetWindow<Quicker.View.X.ActionDesignerWindow>();
            Application.Current.Dispatcher.Invoke(() =>
            {
                switch (type)
                {
                    case "1":
                        context.SetVarValue("output", JsonConvert.SerializeObject(win.Action));
                        break;
                    case "2":
                        string str_action = (string)context.GetVarValue("input");
                        win.Action = JsonConvert.DeserializeObject<Quicker.Domain.Actions.X.XAction>(str_action);
                        CallNonPublicMethod(win, methodName, null);
                        break;
                    case "3":
                        if (Clipboard.ContainsData("quicker-action-item"))
                        {
                            var actionItem = Clipboard.GetData("quicker-action-item") as Quicker.Common.ActionItem;
                            win.Action = JsonConvert.DeserializeObject<Quicker.Domain.Actions.X.XAction>(actionItem.Data);
                            CallNonPublicMethod(win, methodName, null);
                        }
                        break;
                }
            });

        }
        public static object CallNonPublicMethod(object instance, string methodName, object[] param)
        {
            Type type = instance.GetType();
            MethodInfo method = type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
            object result;
            try
            {
                result = method.Invoke(instance, param);
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
            return result;
        }
        public static string GetUpdataUIMethodName()
        {
            var type = typeof(Quicker.View.X.ActionDesignerWindow);
            List<string> methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Select(x => x.Name).ToList();
            var index = methods.IndexOf("CheckIfCanSave");
            var myMethod = methods.Skip(index).ToArray();
            return myMethod[6];
        }
    }
}
