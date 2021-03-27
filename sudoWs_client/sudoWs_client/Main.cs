using Examples.System.Net;
using System;
using System.Configuration;
using System.Net.Security;

namespace Wsudo_client
{
    class Principal
    {
        public static int Main(string[] args)
        {
           
            if (args.Length == 1 && args[0] != "-h")
            {
                return RunMain(args[0]);
            }
           else
            {
                // print help
                Console.WriteLine("Usage : sudoWs <path to powershell script | powershell command> \r\n -h print this help \r\n -> Config file is C:\\Program Files\\sudoWs\\client\\sudoWs_client.dll.config");
                return 1;
            }

        }

        private static int RunMain(String clientCommand)
        {
            // connection to server
            SslStream serverConnection = SslTcpClient.RunClient();
            // test connection
            if (serverConnection == null)
            {
                return 2;
            }

            // ask for nonce
            Boolean nonceRequest = SslTcpClient.RequestNonce(serverConnection);
            // test nonce request
            if (!nonceRequest)
            {
                return 3;
            }

            String nonce = GetUserSecurity.GetUserNonce();

            // ask for command
            Boolean commandRequest = SslTcpClient.RequestCommand(serverConnection, clientCommand, nonce);
            // test command request
            if (!commandRequest)
            {
                return 4;
            }

            // get command response
            Boolean commandResponse = SslTcpClient.GetCommandResponse(serverConnection);
            // test command response
            if (!commandResponse)
            {
                return 5;
            }

            return 0;
        }
    }
}
