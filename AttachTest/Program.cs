using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
namespace AttachTest
{
    public static class EnumExt
    {
        public static string GetDisplayName(this Enum enumValue)
        {
            return enumValue.GetType().GetMember(enumValue.ToString()).First()
                .GetCustomAttribute<DisplayAttribute>()?.Name ?? "";
        }
    }
    internal class Program
    {
        public enum test
        {
            [Display(Name = "babbaa")]
            aaa,
            [Display(Name = "felkef")]
            bbb,
            [Display(Name = "fsklejf")]
            ccc,
        }
        static void Main(string[] args)
        {
            var e = test.aaa;
            Console.WriteLine(e.GetDisplayName());
            Console.Read();
        }
    }
}
