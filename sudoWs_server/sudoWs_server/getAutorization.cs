using System;
using System.Collections.Generic;
using System.Configuration;

public class GetAuthorization
{
	private static List<String[]> ReadPriviledges()
    {
		string file = System.IO.File.ReadAllText(ConfigurationManager.AppSettings.Get("sudoers"));
		string[] readPriviledges = file.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

		List<String[]> privilegesList = new List<String[]>();
		for (int i = 0; i < readPriviledges.Length; i++)
		{
			if (readPriviledges[i].Split(";").Length >= 3 && readPriviledges[i].Split(";").Length <= 4)
			{
				privilegesList.Add(readPriviledges[i].Split(";"));
			}
		}

		return privilegesList;

	}

	public static int IsUserGranted(String username, String commandLine)
    {
		List<String[]> privileges = ReadPriviledges();

		foreach(String[] priviledge in privileges)
        {
			if(priviledge[0] == "u" && priviledge[1] == username)
            {
				if (priviledge[2] == commandLine)
                {
					if(priviledge[3] == "NOPASSWD")
                    {
						return 0;
					}
                    else
                    {
						return 1;
					}
                }
            }
            else if(priviledge[0] == "g")
            {
				bool isAMember = GetUserInfo.IsUserGroupMember(username, priviledge[1]);

				if (priviledge[2] == commandLine && isAMember)
				{
					if (priviledge[3] == "NOPASSWD")
					{
						return 0;
					}
					else
					{
						return 1;
					}

				}

			}
        }

		return 2;

    }
}
