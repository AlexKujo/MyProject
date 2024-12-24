using System;
using System.Reflection;

namespace MyProject_NET_8;

internal static class VersionInfo
{
    public static string GetVersion()
    {
        var assembly = Assembly.GetEntryAssembly();

        if (assembly != null)
        {
            var version = assembly.GetName().Version;
            return version != null ? version.ToString() : "Версия не найдена.";
        }

        return "Сборка не определена.";
    }
}
