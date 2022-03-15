using System.Text.RegularExpressions;

namespace WpfApp1.Test.Test
{
    /// <summary>
    /// 内置矢量图代码转换
    /// </summary>
    public class IconClass : ExpEnvironment
    {
        public static void 图标收藏判断()
        {
            string icon = (string)_context.GetVarValue("icon");
            if (icon.StartsWith("fa:"))
            {
                var re = new Regex(@"(fa:\w+):(([#\w]+))");
                if (re.IsMatch(icon))
                {
                    var groups = re.Match(icon).Groups;
                    _context.SetVarValue("icon", groups[1].Value);
                    _context.SetVarValue("color", groups[2].Value);
                }
                _context.SetVarValue("type", "internal");
            }
            else
            {
                _context.SetVarValue("type", "normal");
            }
        }
        public static void 图标转换()
        {
            var fi = "fab";
            switch (fi)
            {
                case "fal": fi = "fa:Light_"; break;
                case "far": fi = "fa:Regular_"; break;
                case "fas": fi = "fa:Solid_"; break;
                case "fab": fi = "fa:Brands_"; break;
            }
        }
    }
}