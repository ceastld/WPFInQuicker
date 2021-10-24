using Quicker.Domain;
using Quicker.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Quicker.Utilities._3rd;
using Quicker.View;
using Quicker.View.Hotkeys;
using Newtonsoft.Json;
using Z.Expressions;
using Quicker.Public.Interfaces;
//using Newtonsoft.Json.Linq;
using Quicker.Public;

namespace WpfApp1.Tools
{
    /// <summary>
    /// KeyInputWindow.xaml 的交互逻辑
    /// </summary>
    public partial class KeyInputWindow : HandyControl.Controls.Window
    {
        public KeyInputWindow()
        {
            InitializeComponent();
        }
        public static void GetLastForegroundWindow()
        {
            var handle = AppState.AppServer.GetLastForegroundWindow();
            AppHelper.SetForegroundWindow(handle);
        }
        public static EvalContext _eval;
        public static IActionContext _context;
        public static void Registe()
        {
            string[] list = new string[]
            {
                "Quicker.View.Hotkeys.Hotkey, Quicker",
                "Quicker.Utilities._3rd.SmartCollection`1, Quicker",
                "Quicker.Utilities.AppHelper, Quicker",
                "Quicker.View.OperationItem, Quicker"
            };
            foreach (var item in list)
            {
                _eval.RegisterType(Type.GetType(item));
            }
            _eval.RegisterType(Type.GetType("Quicker.View.Hotkeys.Hotkey, Quicker"));
        }
        public static SmartCollection<OperationItem> ReadState()
        {
            const string str_h = "historyList";
            string history = _context.ReadState(str_h, "[]");
            if (history == null || history == "null") history = "[]";
            try
            {
                var list = JsonConvert.DeserializeObject<SmartCollection<OperationItem>>(history);
                return list == null ? new SmartCollection<OperationItem>() : list;
            }
            catch
            {
                return new SmartCollection<OperationItem>();
            }
        }
        public static void CallSubprogram()
        {
            SmartCollection<OperationItem> historyList = new SmartCollection<OperationItem>();
            Hotkey hotkey = (Hotkey)_context.GetVarValue("hotkey");
            if (historyList.Any(x => x.Name == hotkey.ToString()))
                return;
            historyList.Insert(0, new OperationItem()
            {
                Name = hotkey.ToString(),
                OriginText = hotkey.ToData()
            });
            if (historyList.Count > 5)
            {
                for (int i = 5; i < historyList.Count; i++)
                {
                    historyList.RemoveAt(5);
                }
            }
            for (int i = 0; i < historyList.Count; i++)
            {
                historyList[i].Key = $"operation=sp&spname=sp{i}";
            }
            _context.SetVarValue("historyList", historyList);
        }
        public static void SetKeyByHistory()
        {
            int index = 0;
            SmartCollection<OperationItem> historyList = JsonConvert.DeserializeObject<SmartCollection<OperationItem>>((string)_context.ReadState("historyList", "[]"));
            AppHelper.ShowInformation(JsonConvert.SerializeObject(historyList));
            OperationItem item = historyList[index];
            AppHelper.ShowInformation(item.OriginText);
            Hotkey hotkey = JsonConvert.DeserializeObject<Hotkey>(item.OriginText);
            try
            {
                AppHelper.ShowInformation(hotkey.ToString());
                _context.SetVarValue("hotkey", hotkey);
                return;
            }
            catch
            {
                return;
            }
        }
    }
    public static class AssistCSCode
    {
        public static void OnWindowCreated(Window win,
                                           IDictionary<string, object> dataContext,
                                           ICustomWindowContext winContext)
        {

        }

        public static void OnWindowLoaded(Window win, IDictionary<string, object> dataContext,
            ICustomWindowContext winContext)
        {
            var hotkeyEditor = win.FindName("hotkeyEditor") as HotkeyEditorControl;
            var button = win.FindName("SendKeyBtn") as Button;
            hotkeyEditor.BeginInput();
            win.Activated += (s, e) => { hotkeyEditor.BeginInput(); };
            win.KeyDown += (s, e) =>
            {
                if (e.Key == Key.Enter)
                {
                    button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                }
            };
        }

        public static bool OnButtonClicked(string controlName, object controlTag, Window win, IDictionary<string, object> dataContext,
            ICustomWindowContext winContext)
        {
            return true;
        }
    }
}
