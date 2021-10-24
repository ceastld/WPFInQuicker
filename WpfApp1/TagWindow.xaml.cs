using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Quicker.Public.Interfaces;
using Quicker.Utilities;
using Quicker.Utilities.UI;

namespace WpfApp1
{
    /// <summary>
    /// TagWindow.xaml 的交互逻辑
    /// </summary>
    public partial class TagWindow : Window
    {
        public TagWindow()
        {
            InitializeComponent();
        }
        public static string 生成参考线表格代码()
        {
            #region assignment area
            int col0 = 4;
            int row0 = 3;
            string coldefstr = "";
            string rowdefstr = "";
            List<string> textlist = new List<string>() { "1", "1", "1", "1", "1", "1", "1", "1", "1", "1", "1", "1" };
            #endregion
            #region code area

            Func<string, string, string> addingTwoEnd = (adding, str) =>
            {
                return $"<{adding}>{str}</{adding}>";
            };

            Func<string, int, int[]> getthickdef = (thickdefstr, length) =>
            {
                int[] def = new int[length];
                int[] lengthdef = thickdefstr.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x =>
                {
                    try
                    {
                        return Convert.ToInt32(x.Trim());
                    }
                    catch
                    {
                        return 1;
                    }
                })
                .ToArray();
                for (int i = 0; i < length; i++)
                {
                    try
                    {
                        def[i] = lengthdef[i] > 0 ? lengthdef[i] : 1;
                    }
                    catch
                    {
                        def[i] = 1;
                    }
                }
                return def;
            };

            int[] coldef = getthickdef(coldefstr, col0);
            int[] rowdef = getthickdef(rowdefstr, row0);

            #region rowdef
            string rowstr = "";
            for (int j = 0; j < row0; j++)
            {
                rowstr += $"<RowDefinition Height=\"{rowdef[j]}*\"/>";
            }
            rowstr = addingTwoEnd("Grid.RowDefinitions", rowstr);
            #endregion

            #region coldef
            string colstr = "";
            for (int i = 0; i < col0; i++)
            {
                colstr += $"<ColumnDefinition Width=\"{coldef[i]}*\"/>";
            }
            colstr = addingTwoEnd("Grid.ColumnDefinitions", colstr);
            #endregion

            string defstr = colstr + rowstr;

            #region colsplitter
            for (int i = 0; i < col0 - 1; i++)
            {
                defstr += $"<GridSplitter Grid.Column=\"{i}\" Grid.RowSpan=\"{row0}\" Style=\"{{StaticResource VerticalSplitter}}\" ></GridSplitter>";
            }
            #endregion

            #region rowsplitter
            for (int j = 0; j < row0 - 1; j++)
            {
                defstr += $"<GridSplitter Style=\"{{StaticResource HorizontalSplitter}}\" Grid.Row=\"{j}\" Grid.ColumnSpan=\"{col0}\"></GridSplitter>";
            }
            #endregion

            #region textblock
            for (int i = 0; i < row0; i++)
            {
                for (int j = 0; j < col0; j++)
                {
                    string text = textlist[i * col0 + j];
                    defstr += $"<TextBlock Style=\"{{StaticResource TipTextStyle}}\" Grid.Row=\"{i}\" Grid.Column=\"{j}\" Text=\"{text}\"></TextBlock>";
                }
            }
            #endregion

            return addingTwoEnd("Grid", defstr);

            #endregion
        }
        public static IActionContext _context;
        public static void 构造行和列定义()
        {
            #region assignment area
            int col = 4;
            int row = 3;
            string coldefstr = "";
            string rowdefstr = "";
            #endregion
            Func<string, int, int[]> getdef = (defstr, length) =>
            {
                int[] def = new int[length];
                int[] lengthdef = defstr.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x =>
                {
                    try
                    {
                        return Convert.ToInt32(x.Trim());
                    }
                    catch
                    {
                        return 1;
                    }
                })
                .ToArray();
                for (int i = 0; i < length; i++)
                {
                    try
                    {
                        def[i] = lengthdef[i] > 0 ? lengthdef[i] : 1;
                    }
                    catch
                    {
                        def[i] = 1;
                    }
                }
                return def;
            };
            int[] coldef = getdef(coldefstr, col);
            int[] rowdef = getdef(rowdefstr, row);
            _context.SetVarValue("coldeflist", coldef);
            _context.SetVarValue("rowdeflist", rowdef);
        }
        public static List<string> 构造textblock定义()
        {
            int length = 0;
            string tip = "";
            var textlist = Regex.Split(tip, @"\r\n?|\n").Select(x => HttpUtility.HtmlEncode(x.Trim())).ToList();
            int addcount = length - textlist.Count;
            if (addcount > 0)
            {
                for (int i = 0; i < addcount; i++)
                {
                    textlist.Add("");
                }
            }
            return textlist;
        }
        public static string 构造Button()
        {
            List<string> itemList = new List<string>();
            string output = "";
            int col = 0;
            int index = 0;
            string opItemValue = "";
            for (int i = 0; i < itemList.Count; i++)
            {
                string x = itemList[i];
                if (string.IsNullOrWhiteSpace(x)) continue;
                index = x.IndexOf('|');
                if (index > -1)
                {
                    opItemValue = x.Substring(index + 1);
                    x = x.Substring(0, index);
                }
                else
                {
                    opItemValue = "";
                    x = "";
                }
                var result = UIHelper.ExtractIconAndTitle(x).ToTuple();
                output += $"<qk:ActionButton Label=\"{HttpUtility.HtmlEncode(result.Item2)}\" Icon=\"{HttpUtility.HtmlEncode(result.Item1)}\" qk:Att.Action=\"close:{HttpUtility.HtmlEncode(opItemValue)}\" Grid.Column=\"{i%col}\" Grid.Row=\"{i/col}\"></qk:ActionButton>";
            }
            return output;
        }
        public static void 屏幕范围测试()
        {
            _context.SetVarValue("width", SystemParameters.PrimaryScreenWidth);
            _context.SetVarValue("height", SystemParameters.PrimaryScreenHeight);
            Application.Current.Dispatcher.Invoke(() =>
            {
                //函数体
            });
            AppHelper.RunOnUiThread(true, () => { });
        }
    }
}
