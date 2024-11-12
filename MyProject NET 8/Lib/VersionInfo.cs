using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MyProject_NET_8
{
    internal static class VersionInfo
    {
        public static string GetVersion()
        {
            string version = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;

            if (version != string.Empty)
            {
                return version;
            }

            return version;
        }
    }
}
