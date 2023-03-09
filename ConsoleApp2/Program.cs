using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;

namespace ConsoleApp2
{
    public class Program
    {
        const int MB_RETRYCANCEL = 5;

        static void Main()
        {
            Console.WriteLine(A.aaa);
            Console.ReadLine();
        }
        public class A
        {
            public static int aaa = 10;
            static A()
            {
                Console.WriteLine(aaa);
            }
        }
    }
}

/* This code example produces input similar to the following:

Testing module-level PInvoke method created with DefinePInvokeMethod...
Message box returned: 4
 */