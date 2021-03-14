using System;
using System.Configuration;
using System.Diagnostics;

public class RunPowershell
{
    public static void RunAsAdmin(String command)
	{
        Process proc = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = ConfigurationManager.AppSettings.Get("powershell"),
                Arguments = @"Start-Process "+ ConfigurationManager.AppSettings.Get("powershell") + @" "+ command + @" -Verb runAs",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            }
        };
        proc.Start();
    }
}
