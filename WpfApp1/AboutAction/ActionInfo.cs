using Newtonsoft.Json;
using Quicker.Common;
using Quicker.Public.Interfaces;
using Quicker.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApp1.AboutAction
{
    public static class ActionInfo
    {
        private static IActionContext _context;

        public static ActionItem GetActionItemById()
        {
            string id = "";
            ActionItem action = Quicker.Domain.AppState.DataService.GetActionById(id).ToTuple().Item1;
            string data = action.ContextMenuData;
            return action;
        }
        //public static void UpdateIconInfo()
        //{
        //    var configDict = ActionPanelControl.AppState.PanelData.AllConfigs;
        //    Action<string, string, bool> ExecuteAction = (id, cmds, wait) => Quicker.Domain.AppState.AppServer.ExecuteActionByIdOrName(id, null, false, wait, true, cmds, Quicker.Domain.Actions.Runtime.ActionTrigger.FloatButton);
        //    foreach (var config in configDict.Values.ToList())
        //    {
        //        if (config == null || config.PanelItems == null) continue;
        //        foreach (var item in config.PanelItems.ToList())
        //        {
        //            //AppHelper.ShowInformation(JsonConvert.SerializeObject(item));
        //            if (item == null || string.IsNullOrEmpty(item.Id)) continue;
        //            var newItem = Quicker.Domain.AppState.DataService.GetActionByIdOrNameOrTemplateId(item.Id).ToTuple().Item1;
        //            if (newItem == null) continue;
        //            item.Icon = newItem.Icon;
        //            item.Title = newItem.Title;
        //        }
        //        ActionPanelControl.AppState.PanelData.UpdateConfig(config);

        //        var configAction = Quicker.Domain.AppState.DataService.GetActionById(config.ActionId).ToTuple().Item1;
        //        if (configAction == null) continue;
        //        ExecuteAction(config.ActionId, "同步", false);
        //        Quicker.Utilities.AppHelper.ShowInformation($"刷新成功\r\n在动作: {configAction.Title}\r\n 动作ID:{configAction.Id}");
        //    }
        //}
        //public static string Test()
        //{
        //    var configDict = ActionPanelControl.AppState.PanelData.AllConfigs;
        //    return JsonConvert.SerializeObject(configDict);
        //}
        public static void Test2()
        {
            var result = Quicker.Domain.AppState.DataService.GetActionById("acc71f46-9631-44d2-a889-2be482eb6416").ToTuple();
            AppHelper.ShowSuccess(JsonConvert.SerializeObject(result.Item1));
        }
        public static bool FindFirstAction()
        {
            string actionId = (string)_context.GetVarValue("actionId");
            var actions = Quicker.Domain.AppState.DataService.GetActionsByTemplateId(actionId);
            _context.SetVarValue("count", actions.Count);
            if (actions.Count > 0)
            {
                if (actions.Count > 1)
                {
                    AppHelper.ShowInformation($"你安装了多个动作 {string.Join(";", actions.Select(x => x.Title))}\r\n请找到多余的动作并删除");
                }
                _context.SetVarValue("actionId", actions[0].Id);
            }
            return true;
        }
        /// <summary>
        /// 生成动作写入剪贴板
        /// </summary>
        public static void CreatActionItem()
        {
            var ActionItem = new Quicker.Common.ActionItem()
            {
                ActionType = Quicker.Common.ActionType.XAction,
                Title = "",
                Icon = "",
                LastEditTimeUtc = DateTime.Now,
                EnableEvaluateVariable = false
            };
            Clipboard.SetData("quicker-action-item", ActionItem);
            JsonConvert.SerializeObject(Clipboard.GetData("quicker-action-item"));
            Clipboard.ContainsData("quicker-action-item");
            string.Join("\r\n", Enumerable.Range(5, 10).Select(x => x * 50).Select(x => x.ToString()).Select(x => $"{x}|{x}.0"));
        }

        public static void GetActionPageInfo()
        {

        }
    }
}
