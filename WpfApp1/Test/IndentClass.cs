using Quicker.Public.Entities;
using Quicker.Public.Extensions;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WpfApp1.Test
{
    /// <summary>
    /// 格式化代码
    /// </summary>
    internal class IndentClass : ExpEnvironment
    {
        public IEnumerable<string> addd()
        {
            var itemList = new List<CommonOperationItem>();
            var resultList = new HashSet<CommonOperationItem>();
            Action<CommonOperationItem> doone = null;
            doone = x =>
            {
                if (x.Children == null) return;
                if (!resultList.Add(x)) return;
                foreach (var item in x.Children)
                {
                    doone(item);
                }
            };
            itemList.ForEach(doone);
            var r1 = new List<string>();
            foreach (var item in resultList)
            {
                r1.Add(item.OriginText);
                foreach (var ci in item.Children)
                {
                    r1.Add("    " + ci.OriginText.Trim());
                }
            }
            return r1;
        }
        public static IList<CommonOperationItem> tab缩进解析toCommonOperationItem()
        {
            string data = "fsefes\r\nfsefse\r\nsefesf";
            var code = data.SplitToList();
            if (code.Length == 0) return new List<CommonOperationItem>();

            #region 缩进判断
            string indent = (string)_context.GetVarValue("indent");
            if (string.IsNullOrEmpty(indent))
            {
                indent = Regex.Match(string.Join("\r\n", code), @"^\s+", RegexOptions.Multiline).Value;
                if (string.IsNullOrEmpty(indent))
                {
                    indent = "\t";
                }
            }
            #endregion

            #region level func
            Func<string, int> gl = str =>
            {
                int i = 0;
                while (i < str.Length && str[i] == indent[i % indent.Length])
                {
                    i++;
                }
                return i / indent.Length;
            };
            #endregion

            #region main
            var root = new CommonOperationItem() //根节点
            {
                Title = "",
                Children = new List<CommonOperationItem>()
            };
            var parentList = new List<CommonOperationItem>() { root };//路径

            int lastLevel = gl(code[0]);

            List<string> pathList = new List<string>();

            foreach (string line in code)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                var level = gl(line);
                var item = CommonOperationItem.ParseLine(line.Trim(), true);
                //同级
                if (level == lastLevel)
                {
                    parentList.Last().Children.Add(item);
                }
                //父级兄弟
                else if (level < lastLevel)
                {
                    if (parentList.Count > 0)
                    {
                        var index = Math.Min(parentList.Count - 1, level);
                        parentList = parentList.Take(index + 1).ToList();
                    }
                    parentList.Last().Children.Add(item);
                }
                //子级
                else if (level > lastLevel)
                {
                    var parent = parentList.Last().Children.Last();
                    parent.Children = new List<CommonOperationItem>() { item };
                    parentList.Add(parent);
                }
                lastLevel = level;
                try
                {
                    var path = Path.Combine(parentList.Concat(new[] { item }).Select(x => x.Data ?? x.Title).ToArray());
                    item.ExtraData = new Dictionary<string, object>();
                    item.ExtraData.Add("path", path);
                    pathList.Add(path);
                }
                catch { pathList.Add("路径中含有非法字符"); }
            }
            _context.SetVarValue("pathList", pathList);
            return root.Children;
            #endregion
        }

        public List<string> 深度优先遍历()
        {
            var itemList = new List<CommonOperationItem>();
            var resultList = new List<string>();
            Action<CommonOperationItem> add = item =>
            {
                resultList.Add(item.OriginText);
                foreach (var ci in item.Children)
                {
                    resultList.Add("    " + ci.OriginText.Trim());
                }
            };
            Action<IList<CommonOperationItem>> dol = null;
            dol = items =>
             {
                 foreach (var item in items)
                 {
                     if (item.Children != null)
                     {
                         add(item);
                         dol(item.Children);
                     }
                 }
             };
            dol(itemList);
            return resultList;
        }
        public List<string> 广度优先遍历()
        {
            var itemList = new List<CommonOperationItem>();
            var q = new Queue<CommonOperationItem>(itemList);
            var resultList = new List<string>();
            Action<IList<CommonOperationItem>> eql = items =>
            {
                foreach (var item in items)
                {
                    if (item.Children == null) continue;
                    q.Enqueue(item);
                }
            };
            Action<CommonOperationItem> add = item =>
            {
                resultList.Add(item.OriginText);
                foreach (var ci in item.Children)
                {
                    resultList.Add("    " + ci.OriginText.Trim());
                }
            };
            while (q.Count > 0)
            {
                var first = q.Dequeue();
                add(first);
                if (first.Children == null) continue;
                eql(first.Children);
            }
            return resultList;
        }

        public void 查看源代码()
        {
            //new Exception("fsef").ToString();

        }
    }
}
