using Quicker.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Z.Expressions;

namespace WpfApp1.AboutAction
{
    class QuickerActionStep : Test.ExpEnvironment
    {
        public static void regist()
        {
            _eval.RegisterGlobalVariable("isSuccess", false);
            _eval.RegisterType(Type.GetType("Quicker.Utilities.AppHelper, Quicker"));
            _eval.RegisterType(Type.GetType("System.Windows.Forms.Clipboard, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"));
        }
        public static string QucikerSteps()
        {
            const string cbType = "quicker-action-steps";

            #region 赋值
            string get_or_set = "";
            string data = "";
            bool isSuccess = false;
            #endregion

            switch (get_or_set)
            {
                case "1":
                case "获取":
                    if (isSuccess = Clipboard.ContainsData(cbType))
                    {
                        data = (string)Clipboard.GetData(cbType);
                        AppHelper.ShowSuccess("获取成功");
                    }
                    else
                    {
                        AppHelper.ShowWarning("获取失败,剪贴板中可能没有quciker步骤");
                    }
                    break;
                case "2":
                case "写入":
                    try
                    {
                        Clipboard.SetData(cbType, data);
                        isSuccess = true;
                        AppHelper.ShowSuccess("写入成功");
                    }
                    catch
                    {
                        AppHelper.ShowWarning("写入失败");
                    }
                    break;
            }
            return data;
        }

    }
}
