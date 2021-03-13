using Examples.System.Net;
using System;
using System.Net.Security;

namespace Wsudo_client
{
    class Principal
    {
        static int Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Usage : wsudo <path to script | command to execute>");
                return 1;
            }

            String clientCommand = args[0];

            // read settings file
            Configuration client_cfg = new Configuration();

            // connection to server
            SslStream serverConnection = SslTcpClient.RunClient();
            // test connection
            if(serverConnection == null)
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
            Boolean commandRequest = SslTcpClient.RequestCommand(serverConnection, clientCommand, nonce, client_cfg);
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
