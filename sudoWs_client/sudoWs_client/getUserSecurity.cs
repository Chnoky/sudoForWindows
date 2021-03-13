using System;
using System.IO;

public class GetUserSecurity
{
	private static String GetCurrentUsername()
    {
        return Environment.UserName;
    }

    public static String GetUserNonce()
    {
        String username = GetCurrentUsername();
        String clientHome = @"C:\Users\" + username + @"\AppData\Local\WsudoNonce";
        String nonce = File.ReadAllText(clientHome);

        File.Delete(clientHome);

        return nonce;
    }
}
