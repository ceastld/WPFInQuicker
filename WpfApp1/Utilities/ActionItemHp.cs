using Newtonsoft.Json;
using Quicker.Common;
using Quicker.Public.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WpfApp1.Utilities
{
    internal class ActionItemHp : Test.ExpEnvironment
    {
        public void a()
        {
            object obj = null;
            ActionItem item = null;
            if (obj is string)
            {
                item = JsonConvert.DeserializeObject<ActionItem>((string)obj);
            }
            if (!(obj is ActionItem))
            {
                item = JsonConvert.DeserializeObject<ActionItem>(item.ToJson(false));
            }
            Clipboard.SetData("quicker-action-item", item);
        }
    }
}
