using Quicker.Public;
using Quicker.Public.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.Expressions;

namespace WpfApp1.Test
{
    public abstract class ExpEnvironment
    {
        public static IActionContext _context;
        public static EvalContext _eval;
    }
    public class Test1 : ExpEnvironment
    {

    }
}
