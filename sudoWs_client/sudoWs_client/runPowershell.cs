using System;
using System.Diagnostics;

public class RunPowershell
{
	public static void RunAsAdmin(String command)
	{
        Process proc = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = @"C:\Windows\system32\WindowsPowerShell\v1.0\powershell.exe",
                Arguments = @"Start-Process C:\Windows\system32\WindowsPowerShell\v1.0\powershell.exe "+command+@" -Verb runAs",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            }
        };
        proc.Start();
    }
}
