using Newtonsoft.Json;
using Quicker.Domain;
using Quicker.Domain.Actions.X.Storage;
using Quicker.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp1.AboutAction.Designer
{
    public class CopyVariable
    {
        public ListBox listBox { get; set; }
        public string Template { get; set; }
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

            if (listBox.SelectedItem == null) return;
            var items = listBox.SelectedItems.OfType<ActionVariable>().ToList();
            ActionStepsDto stepsDto = JsonConvert.DeserializeObject<ActionStepsDto>(Template);
            var ip = stepsDto.Steps[0].InputParams;
            string note = "$$" + string.Join("", items.Select(x => "{" + x.Key + "}"));
            ip.Clear(); ip.Add("note", new ActionStepParam()
            {
                VarKey = "",
                Value = note
            });
            stepsDto.Variables = items;
            DataObject data = new DataObject();
            data.SetData(DataFormats.Text, string.Join("\r\n", items.Select(x => x.Key)));
            data.SetData("quicker-action-steps", JsonConvert.SerializeObject(stepsDto));
            Clipboard.SetDataObject(data, true);
            AppHelper.ShowSuccess("步骤已写入剪贴板");
        }
        public void TestCopy()
        {
            DataObject data = new DataObject();
            data.SetData(DataFormats.Text, "fefe");
            data.SetData("quicker-action-steps", "{}");
            Clipboard.SetDataObject(data, false);
        }
    }
}
