using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quicker.Public;
using Quicker.Public.Extensions;
using Quicker.Public.Interfaces;
using Quicker.Utilities;
using Quicker.View.Controls;
using System.Globalization;

namespace WpfApp1.View.Widgets
{
    /// <summary>
    /// TimeWindow.xaml 的交互逻辑
    /// 显示时间控件
    /// </summary>
    public partial class TimeWindow : FadeOutWindow
    {
        public TimeWindow()
        {
            InitializeComponent();
        }

        public static void OnWindowCreated(Window win, IDictionary<string, object> dataContext, ICustomWindowContext winContext)
        {
            WrapperSet.Add(new TimeWindowWrapper(win, dataContext));
        }
        public static HashSet<TimeWindowWrapper> WrapperSet = new HashSet<TimeWindowWrapper>();
        public static class ChineseDateTime
        {

            private static readonly string[] _chineseMonth =
            {
                "正", "二", "三", "四", "五", "六", "七", "八", "九", "十", "冬", "腊"
            };
            private static readonly string[] _chineseDay =
            {
                "初一", "初二", "初三", "初四", "初五", "初六", "初七", "初八", "初九", "初十",
                "十一", "十二", "十三", "十四", "十五", "十六", "十七", "十八", "十九", "二十",
                "廿一", "廿二", "廿三", "廿四", "廿五", "廿六", "廿七", "廿八", "廿九", "三十"
            };
            private static readonly ChineseLunisolarCalendar chineseDate = new ChineseLunisolarCalendar();
            public static string GetDateString(DateTime dateTime)
            {
                var month = chineseDate.GetMonth(dateTime);
                var day = chineseDate.GetDayOfMonth(dateTime);
                return _chineseMonth[month - 1] + "月" + _chineseDay[day - 1];
            }
        }

        #region for config
        /// <summary>
        /// 一定要小写，quicker里面也是小写的
        /// </summary>
        public class TimerConfig
        {
            public bool useLunar { get; set; }
        }
        #endregion

        public class TimeWindowWrapper
        {
            public Window TheWindow;
            public IDictionary<string, object> DataContext;
            public TextBlock ClockBlock;
            public TextBlock DayBlock;
            public TimerConfig Config { get; set; }
            public TimeWindowWrapper(Window win, IDictionary<string, object> dataContext)
            {
                TheWindow = win;
                DataContext = dataContext;
                ClockBlock = win.FindName("ClockBlock") as TextBlock;
                DayBlock = win.FindName("DayBlock") as TextBlock;
                win.MouseWheel += Win_MouseWheel;
                string type = (string)dataContext["timerType"];
                var jobj = JObject.FromObject(dataContext["duration"]);
                var config = JObject.FromObject(dataContext["config"]).ToObject<TimerConfig>();

                //coalesce表达式需要C#6.0
                Config = config == null ? new TimerConfig() : config;

                //本地函数需要C#7.0
                Func<object, int> toint = o =>
                 {
                     try { return Convert.ToInt32(o); } catch { return 0; }
                 };
                var times = new TimeSpan(toint(jobj["hours"]),
                                         toint(jobj["minutes"]),
                                         toint(jobj["seconds"]));
                InitTimer(type, times);
                TheWindow.Closed += TheWindow_Closed;
            }

            private void TheWindow_Closed(object sender, EventArgs e)
            {
                Timer.Stop();
                WrapperSet.Remove(this);
            }

            public DispatcherTimer Timer = new DispatcherTimer();

            public void InitTimer(string type, TimeSpan time = default(TimeSpan))
            {
                switch (type)
                {
                    case "timer_now":
                        CreateTimerNow();
                        break;
                    case "timer_up":
                        CreateTimerUp(TimeSpan.FromHours(2));
                        break;
                    case "timer_up_short":
                        CreateTimerUp(TimeSpan.Zero);
                        break;
                    case "timer_down":
                        var timeOut = DataContext["timeOutHandler"];
                        var action = timeOut is Action ? (Action)timeOut : null;
                        CreateTimerDown(time, action);
                        break;
                    default:
                        AppHelper.ShowWarning("不支持的时间类型:" + type);
                        return;
                }
            }

            #region 创建计时器

            /// <summary>
            /// 显示倒计时
            /// </summary>
            private void CreateTimerDown(TimeSpan duration, Action timeOut)
            {
                DayBlock.Visibility = Visibility.Collapsed;
                var time_end = DateTime.Now + duration;
                CreateTimer(100, (s, e) =>
                {
                    var t = time_end - DateTime.Now;
                    if (t <= TimeSpan.Zero)
                    {
                        Timer.Stop();
                        TheWindow.Close();
                        if (timeOut != null)
                        {
                            Application.Current.Dispatcher.InvokeAsync(timeOut);
                        }
                        else
                        {
                            var tooltip = (string)DataContext["tooltip"];
                            if (string.IsNullOrEmpty(tooltip))
                            {
                                tooltip = "倒计时结束";
                            }
                            AppHelper.ShowInformation(tooltip);
                        }
                    }
                    ClockBlock.Text = FormatTime(t);
                });
            }

            /// <summary>
            /// 显示正计时
            /// </summary>
            private void CreateTimerUp(TimeSpan maxTime)
            {
                DayBlock.Visibility = Visibility.Collapsed;
                var time_start = DateTime.Now;
                if (maxTime.Hours == 0)
                {
                    CreateTimer(10, (s, e) =>
                    {
                        var t = DateTime.Now - time_start;

                        ClockBlock.Text = string.Format("{0:00}:{1:00}.{2:00}", t.Minutes, t.Seconds, t.Milliseconds / 10);
                    });
                }
                else
                {
                    CreateTimer(100, (s, e) =>
                    {
                        var t = DateTime.Now - time_start;
                        ClockBlock.Text = FormatTime(t);
                    });
                }
            }
            public string FormatTime(TimeSpan t)
            {
                return string.Format("{0:00}:{1:00}:{2:00}", t.Hours, t.Minutes, t.Seconds);
            }
            /// <summary>
            /// 生成显示当前时间的timer
            /// </summary>
            private void CreateTimerNow()
            {
                DayBlock.Visibility = Visibility.Visible;
                CreateTimer(100, (s, e) =>
                {
                    var now = DateTime.Now;
                    if (now.Millisecond < 200)
                    {
                        ClockBlock.Text = now.ToString("HH:mm:ss");
                        UpdateDay();
                    }
                });
            }
            /// <summary>
            /// 创建 timer
            /// </summary>
            /// <param name="ms">毫秒</param>
            /// <param name="eh">事件处理</param>
            private void CreateTimer(int ms, EventHandler eh)
            {
                Timer.Interval = TimeSpan.FromMilliseconds(ms);
                eh.Invoke(null, null);
                Timer.Tick += eh;
                Timer.Start();
            }
            /// <summary>
            /// 跟新控件中日期部分
            /// </summary>
            private void UpdateDay()
            {
                var timeNow = DateTime.Now.ToString("yyyy/MM/dd ddd");
                if (Config.useLunar)
                {
                    var lunarNow = ChineseDateTime.GetDateString(DateTime.Now);
                    DayBlock.Text = timeNow + "\r\n" + lunarNow;
                }
                else
                {
                    DayBlock.Text = timeNow;
                }
            }
            #endregion 

            #region 调整字体大小
            private void Win_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
            {
                //AppHelper.ShowInformation(e.Delta.ToString());
                if (e.Delta != 0)
                {
                    AdjustBlockSize(e.Delta / 60);
                }
            }
            public void AdjustBlockSize(int delta)
            {
                ClockBlock.FontSize = Math.Max(10, ClockBlock.FontSize + delta);
            }
            #endregion 
        }

    }
}
