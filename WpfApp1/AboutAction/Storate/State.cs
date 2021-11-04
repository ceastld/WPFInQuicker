using Quicker.Domain.Actions.X.BuiltinRunners.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.AboutAction.Storate
{
    internal class State
    {
        public void test()
        {
            ActionStateWriter.DeleteStateFile("");
            ActionStateWriter.WriteActionState("id", "key", "*NULL*");
        }
    }
}
