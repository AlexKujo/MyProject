using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MyProject_NET_8
{
    internal static class VersionInfo
    {
        public static string UpdateRequired { get; private set; } = "";
        public static string NoUpdateRequired { get; private set; }

        public static string GetCurrentVersion()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            Version version = assembly.GetName().Version;

            return version.ToString();
        }
    }
}
