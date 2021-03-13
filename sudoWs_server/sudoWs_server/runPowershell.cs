using System;
using System.Diagnostics;
using System.Collections.Generic;

public class RunPowershell
{
	public static List<String> RunCommand(String command, bool isCSV)
	{
        String commandLine;
        if (isCSV)
        {
            commandLine = command + " | ConvertTo-Csv -Delimiter ';'";
        }
        else
        {
            commandLine = command;
        }

        Process proc = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = @"C:\Windows\system32\WindowsPowerShell\v1.0\powershell.exe",
                Arguments = commandLine,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            }
        };

        List<String> response = new List<string>();

        proc.Start();
        while (!proc.StandardOutput.EndOfStream)
        {
            string line = proc.StandardOutput.ReadLine();
            response.Add(line);
        }

        return response;

    }

	public static String RunSudoersCommand(String command) {

        Process proc = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = @"C:\Windows\system32\WindowsPowerShell\v1.0\powershell.exe",
                Arguments = command,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            }
        };

        String response = "";

        proc.Start();
        while (!proc.StandardOutput.EndOfStream)
        {
            string line = proc.StandardOutput.ReadLine();
            response = response + line + Environment.NewLine;
        }

        return response;
    }
}




