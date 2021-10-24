using Newtonsoft.Json.Linq;
using Quicker.Public.Extensions;
using Quicker.Public.Interfaces;
using Quicker.Utilities.Ext;
using Quicker.Utilities.Pinyin;
using Quicker.View;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WpfApp1.Search
{
    public class TabSearch
    {
        private IActionContext _context;
        public List<string> TransformSearchResult()
        {
            const string SEARCHRESULT = "searchResult";
            JObject searchResult = _context.ReadCache<JObject>("all_tabs", null);
            if (searchResult == null)
            {
                searchResult = (JObject)_context.RunSp("获取所有标签", null)[SEARCHRESULT];
                _context.WriteCache(SEARCHRESULT, searchResult, 10);
            }

            var resultList = new List<string>();

            Func<JObject, string, string> get_search_string =
                (jo, type) => $"{ jo["title"] }(【{type}】{jo["url"].ToString().UrlDecode()})|operation=run&data={jo["url"].ToString().UrlEncode()}";

            string search_text = (string)_context.GetVarValue("text");

            foreach (var tab in (JArray)searchResult["tabs"])
            {
                var url = tab["url"].ToString();
                var title = tab["title"].ToString().Replace("(", "（").Replace(")", "）").Replace("|", "");
                if (url.Contains(search_text)
                    || title.Contains(search_text)
                    || PinyinHelper.IsPinyinMatch(PinyinHelper.GetPinYinMatchString(title), search_text, false, false)
                    )
                {
                    string favicon = "";
                    if ((tab as JObject).TryGetValue("favIconUrl", out JToken fjt))
                    {
                        favicon = fjt.ToString();
                        favicon = favicon.StartsWith("http") ? $"[url:{favicon}]" : "";
                    }
                    var data = $"activate:{tab["id"]}".UrlEncode();
                    resultList.Add($"{favicon}{title}({url.UrlDecode()})|operation=callback&data={data}");
                }
            }
            return resultList;


            foreach (var bookmark in (JArray)searchResult["bookmarks"])
            {
                if ((bookmark as JObject).ContainsKey("url"))
                {
                    resultList.Add(get_search_string((JObject)bookmark, "书签"));
                }
                else
                {
                    resultList.Add($"{ bookmark["title"] }(【书签夹】)|operation=run&data={("chrome.exe chrome://bookmarks/?id=" + bookmark["id"]).UrlEncode()}");
                }
            }
            foreach (var history in (JArray)searchResult["historyItems"])
            {
                resultList.Add(get_search_string((JObject)history, "历史"));
            }
        }

        public class SearchResult
        {
            public string icon { get; set; }
            public string title { get; set; }
            public string tips { get; set; }
            /// <summary>
            /// run:直接运行 
            /// paste:粘贴
            /// copy:复制到剪贴板
            /// text:???
            /// sendkey:发送按键
            /// callback:返回,调用动作
            /// </summary>
            public string op { get; set; }
            public string data { get; set; }
            public override string ToString()
            {
                var visualIcon = string.IsNullOrEmpty(icon) ? string.Empty : $"[{icon}]";
                var visualTips = tips.Replace("(", "（").Replace(")", "）").Replace("|", "");
                return $"[{visualIcon}]{title}({visualTips})|operation={op}&data={data.UrlEncode()}";
            }
        }
        public List<string> CreateSearchText()
        {
            const string ORIGINAL_DATA = "originalData";
            var resultList = new List<string>();
            JArray originalData = _context.ReadCache<JArray>(ORIGINAL_DATA, null);
            if (originalData == null)
            {
                string originalData_str = (string)_context.GetVarValue("originalData");
                originalData = JArray.Parse(originalData_str);
                _context.WriteCache(ORIGINAL_DATA, originalData, 10);
            }
            Func<JObject, string> GetSearchText = jo =>
             {
                 string icon = (string)jo["icon"];
                 string title = (string)jo["title"];
                 string tips = (string)jo["tips"];
                 string op = (string)jo["operation"];
                 string data = (string)jo["data"];
                 var visualIcon = string.IsNullOrEmpty(icon) ? string.Empty : $"[url:{icon}]";
                 var visualTips = tips.ToString().Replace("(", "（").Replace(")", "）").Replace("|", "");
                 return $"{visualIcon}{title}({visualTips})|operation={op}&data={data.ToString().UrlEncode()}";
             };
            string search_text = (string)_context.GetVarValue("text");
            foreach (var item in originalData)
            {
                var title = item["title"].ToString();
                var url = item["tips"].ToString();
                if (url.Contains(search_text)
                    || title.Contains(search_text)
                    || PinyinHelper.IsPinyinMatch(PinyinHelper.GetPinYinMatchString(title), search_text, false, false))
                {
                    resultList.Add(GetSearchText((JObject)item));
                }
            }
            return resultList;
        }

        /// <summary>
        /// 对象数据,转换成搜索框中可显示的数据...
        /// </summary>
        /// <param name="originalData"></param>
        /// <param name="filterText"></param>
        /// <returns></returns>
        public List<string> Object2SearchItem()
        {
            var resultList = new List<string>();
            #region 数据初始化判断
            JArray originalData;
            var obj = _context.GetVarValue("originalData");
            if (obj is string)
            {
                originalData = JArray.Parse((string)obj);
            }
            else if (obj is JArray)
            {
                originalData = (JArray)obj;
            }
            else
            {
                return resultList;
            }
            #endregion

            Func<JObject, string> GetSearchText = jo =>
            {
                string icon = (string)jo["icon"];
                string title = (string)jo["title"];
                string tips = (string)jo["tips"];
                string op = (string)jo["operation"];
                string data = (string)jo["data"];
                var visualIcon = string.IsNullOrEmpty(icon) ? string.Empty :
                    icon.StartsWith("http") ? $"[url:{icon}]" :
                    icon.StartsWith("fa:") ? $"[{icon}]" : $"[icon:{icon}]";
                var visualTips = tips.ToString().Replace("(", "（").Replace(")", "）").Replace("|", "");
                return $"{visualIcon}{title}({visualTips})|operation={op}&data={data.ToString().UrlEncode()}";
            };
            string filter_text = (string)_context.GetVarValue("filterText");
            Func<string, string, bool> str_contains = (s, t) => s.IndexOf(t, StringComparison.OrdinalIgnoreCase) != -1;
            foreach (var item in originalData)
            {
                var title = item["title"].ToString();
                var tips = item["tips"].ToString();
                if (str_contains(title, filter_text)
                    || str_contains(tips, filter_text)
                    || PinyinHelper.IsPinyinMatch(PinyinHelper.GetPinYinMatchString(title), filter_text, false, false))
                {
                    resultList.Add(GetSearchText((JObject)item));
                }
            }
            return resultList;
        }

        public JArray GetEnvFolder()
        {
            const string SYS_SPECIALFOLDERS = "system_specialFolders";
            var cachearray = _context.ReadCache<JArray>(SYS_SPECIALFOLDERS, null);
            if (cachearray == null)
            {
                Func<JArray> getEnvFolders = () =>
                {
                    var type = typeof(Environment.SpecialFolder);
                    var array = new JArray();
                    var collection = Enum.GetValues(type).Cast<Environment.SpecialFolder>()
                        .Select(x => Environment.GetFolderPath(x)).Distinct().OrderBy(x => x.Length);
                    foreach (var path in collection.OrderBy(x => x.Length).ToList())
                    {
                        var item = new JObject();
                        try
                        {
                            var name = Path.GetFileName(path);
                            if (string.IsNullOrEmpty(name)) continue;
                            item["title"] = name;
                            item["icon"] = item["tips"] = item["data"] = path;
                            item["operation"] = "run";
                            array.Add(item);
                        }
                        catch
                        {

                        }
                    }
                    return array;
                };
                var result = getEnvFolders();
                _context.WriteCache(SYS_SPECIALFOLDERS, result, 10);
                return result;
            }
            {
                return cachearray;
            }
        }

        public void Test()
        {
            var text = "efef$444$df$222$fesf$111$fef";
            var partten = @"\$[^\$]\$";
            var index = 0;
            Regex.Replace(text, partten, match => index++.ToString());
        }
    }
}
