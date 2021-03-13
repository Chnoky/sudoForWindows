using System;
using System.IO;
using System.Security.Cryptography;

public class Security
{
	public static String SendNonce(String username)
	{
		String clientNonce = GenerateNonce();
		String clientHome = @"C:\Users\" + username + @"\AppData\Local\WsudoNonce";

		File.WriteAllText(clientHome, clientNonce);

		return clientNonce;
	}

	private static String GenerateNonce()
    {
		RNGCryptoServiceProvider random = new RNGCryptoServiceProvider();
		Byte[] byte_array = new byte[512];
		random.GetNonZeroBytes(byte_array);

		String nonce_random = "";
		foreach(Byte b in byte_array)
        {
			nonce_random += b.ToString();
		}

		return nonce_random;
	}
}
