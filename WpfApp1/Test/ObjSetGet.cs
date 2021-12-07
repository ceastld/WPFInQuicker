using Newtonsoft.Json.Linq;
using Quicker.Public.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Test
{
    internal class ObjSetGet : ExpEnvironment
    {
        public void Get()
        {
            object obj = _context.GetVarValue("obj");
            if (obj == null)
            {
                throw new Exception("对象不能为null");
            }
            var mainContext = _context.GetVarValue("main_context") as Quicker.Public.Interfaces.IActionContext;
            var keyList = _context.GetVarValue("keyList") as List<string>;
            foreach (var key in keyList)
            {
                var property = obj.GetType().GetProperty(key);
                if (property != null)
                {
                    var value = property.GetValue(obj);
                    if (value != null)
                    {
                        mainContext.SetVarValue(key, value);
                    }
                }
            }
        }
        public void Set()
        {
            object obj = _context.GetVarValue("obj");
            var mainContext = _context.GetVarValue("main_context") as Quicker.Public.Interfaces.IActionContext;
            var keyList = _context.GetVarValue("keyList") as List<string>;
            if (obj == null)
            {
                var jobject = new JObject();
                keyList.ForEach(key =>
                {
                    jobject[key] = JToken.FromObject(mainContext.GetVarValue(key));
                });
                _context.SetVarValue("obj", jobject);
                return;
            }
            foreach (var key in keyList)
            {
                var prop = obj.GetType().GetProperty(key);
                if (prop != null)
                {
                    var value = mainContext.GetVarValue(key);
                    if (value != null)
                    {
                        prop.SetValue(obj, value);
                    }
                }
            }
        }
    }
}
