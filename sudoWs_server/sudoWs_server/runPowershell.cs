using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Configuration;

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
                FileName = ConfigurationManager.AppSettings.Get("powershell"),
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

        proc.Close();

        return response;

    }

	public static String RunSudoersCommand(String command) {

        Process proc = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = ConfigurationManager.AppSettings.Get("powershell"),
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

        proc.Close();

        return response;
    }
}




