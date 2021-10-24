using Quicker.Domain.Actions.X;
using Quicker.Domain.Actions.X.Storage;
using Quicker.Public.Interfaces;
using Quicker.Utilities;
using Quicker.Utilities._3rd;
using Quicker.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfApp1.ListBoxTest
{
    /// <summary>
    /// listbox1.xaml 的交互逻辑
    /// </summary>
    public partial class listbox1 : Window
    {
        public listbox1()
        {
            InitializeComponent();
        }
        public static IActionContext _context;

        public static SmartCollection<string> getitems()
        {
            var list = new List<string>();
            return new SmartCollection<string>(list);
        }
        public static Quicker.View.OperationItem get__()
        {
            string a = "";
            return AppHelper.ParseOperationItem(a, true, "|");
        }
        public string listboxstr()
        {
            int count = 3;
            int col = 2;
            string defstr = "";
            for (int i = 0; i < count; i++)
            {
                defstr += $"<ListBox ItemsSource=\"{{Binding [itemList{i}]}}\" Grid.Column=\"{i % col}\" Grid.Row=\"{i / col}\"></ListBox>";
            }
            Func<List<string>, SmartCollection<OperationItem>> aaa = list => new SmartCollection<OperationItem>(list.Select(x => AppHelper.ParseOperationItem(x, true, "|")));
            Func<string,string> bb = a => a + a;
            return "";
        }
        public void 构造可视化列表()
        {
            int count = 0;
            Func<List<string>, List<string>> aa = list => list;
            for (int i = 0; i < count; i++)
            {
                
            }
        }
        public void colrowcount()
        {
            int count = 10;
            int col, row;
            switch (count)
            {
                case 2: col = 2; row = 1;break;
                case 3: col = 3; row = 1;break;
                case 4: col = 2; row = 2;break;
                case 5:
                case 6: col = 3; row = 2;break;
                case 7:
                case 8: col = 4; row = 2;break;
                case 9: col = 3; row = 3;break;
                default: col = row = 1;break;
            }
            _context.SetVarValue("row", row);
            _context.SetVarValue("col", col);
        }
        public string aaaa()
        {
            #region assignment area
            int col0 = 4;
            int row0 = 3;
            string coldefstr = "";
            string rowdefstr = "";
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

            return colstr + rowstr;
            #endregion
        }
    }
}
