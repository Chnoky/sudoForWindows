using System;
using System.Diagnostics;

public class RunPowershell
{
    public static void RunAsAdmin(String command, Configuration cfg)
	{
        Process proc = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = cfg.GetPowershell(),
                Arguments = @"Start-Process "+ cfg.GetPowershell() + @" "+ command + @" -Verb runAs",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            }
        };
        proc.Start();
    }
}
