using System;
using System.IO;

public class Configuration
{
	private readonly String powershell;

    public Configuration()
    {
        //String settingsFile = @"C:\Program Files\sudoWs\client\settings_client.cfg";
        String settingsFile = @"C:\Workspace\sudoWs_client\sudoWs_client\settings_client.cfg";
        String[] settings = File.ReadAllText(settingsFile).Split(@"\r\n");

        foreach (String setting in settings)
        {
            String param = setting.Split("=")[0];
            String value = setting.Split("=")[1];

            if (param == "powershell")
            {
                this.powershell = value;
            }
        }
    }

	public String GetPowershell()
	{
		return this.powershell;
	}

}
