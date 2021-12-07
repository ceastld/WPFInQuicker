using Quicker.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Quicker.Common.Vm.Account;
using Quicker.Public;

namespace WpfApp1.Test
{
    internal class GenurateReport : ExpEnvironment
    {
        public static void Exec(IStepContext context)
        {
            context.SetVarValue("dict", Generate());
        }
        public static Dictionary<string, object> Generate()
        {
            var items = AppState.DataService.GetAllActionItems();
            var dict = new Dictionary<string, object>();
            dict["action_count"] = items.Count();
            dict["userAction_count"] = items.Count(x => string.IsNullOrEmpty(x.TemplateId));
            dict["globalAction_count"] = items.Count(x => !string.IsNullOrEmpty(x.TemplateId));
            dict["globalSp_count"] = AppState.DataService.GlobalSubPrograms.Count();
            dict["userName"] = AppState.DataService.UserName;
            dict["userId"] = AppState.DataService.UserId;
            var userInfo = AppState.DataService
                .GetType()
                .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                .First(x => x.ReturnType == typeof(UserInfo))
                .Invoke(AppState.DataService, null) as UserInfo;
            dict["userNickName"] = userInfo.NickName;
            //userInfo.MemberLevel = MemberLevel.Free;
            return dict;
        }
    }
}
