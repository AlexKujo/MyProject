using System;
using System.Reflection;
using System.Deployment.Application;

namespace RapidHelper;

internal static class VersionInfo
{
    public static string GetVersion()
    {
        try
        {
            string version = Environment.GetEnvironmentVariable("ClickOnce_CurrentVersion");

            if (!string.IsNullOrEmpty(version))
            {
                return version;
            }
            else
            {
                return "Version information not available.";
            }
        }
        catch(Exception ex)
        {
            return $"Ошибка: {ex.Message}";
        }
    }
}
