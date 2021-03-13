using System;
using System.Collections.Generic;

public class GetUserInfo
{
	public static bool IsUserSystemMember(String username)
    {
		List<String> userList = GetListUsers();

        if (!userList.Contains(username))
        {
			return false;
        }
        else
        {
			return true;
        }
	}

	public static bool IsUserGroupMember(String username, String groupName)
	{
        if (IsGroupExist(groupName))
        {
			if(IsGroupMember(username, groupName))
            {
				return true;
            }
            else
            {
				return false;
            }
        }
        else
        {
			return false;
        }

	}

	private static List<String> GetListUsers()
    {
        List<String> listUser = RunPowershell.RunCommand("Get-LocalUser", true);
		List<String> users = new List<String>();

		foreach(String line in listUser)
        {
			if(line.Split(";").Length == 14 && line.Split(";")[10].Replace("\"","") != "Name")
            {
				users.Add(line.Split(";")[10].Replace("\"", ""));
			}
        }

		return users;

	}

	private static bool IsGroupExist(String groupName)
	{

		List<String> isGroupExist = RunPowershell.RunCommand("(Get-LocalGroup -Name \""+groupName+"\").Name", false);

		if (isGroupExist[0] == groupName)
        {
			return true;
        }
        else
        {
			return false;
        }

	}

	private static bool IsGroupMember(String username, String groupName)
	{

		List<String> isGroupMember = RunPowershell.RunCommand("(Get-LocalGroupMember -Group \""+groupName+ "\" -Member \"" + username+ "\").Name", false);


		if (isGroupMember.Count > 0 && isGroupMember[0].Split("\\").Length > 1 && isGroupMember[0].Split("\\")[1] == username)
		{
			return true;
		}
		else
		{
			return false;
		}

	}


}
