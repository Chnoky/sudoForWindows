using SSLServer;
using System;
using System.Net.Security;
using System.Net.Sockets;

public class Principal
{
    public static void Main(string[] args)
    {      
        if (args.Length > 0)
        {
            // print help
            Console.WriteLine("Usage : sudoWs : run serveur, must be used as System account \r\n -h : print this help \r\n -> Config file is C:\\Program Files\\sudoWs\\server\\sudoWs_server.dll.config");
        }
        else
        {
            RunMain();
        }
    }
    

    private static void RunMain()
    {
        while (true)
        {
            // start listening
            while (true)
            {
                TcpListener listener = SslTcpServer.RunServer();
                TcpClient client = listener.AcceptTcpClient();
                SslStream sslStream = SslTcpServer.ProcessClient(client);

                // get nonce request from client
                String clientUsername = SslTcpServer.GetNonceRequest(sslStream);
                // verify nonce request
                if (clientUsername == null)
                {
                    SslTcpServer.AnswerNonceRequest(sslStream, "Error during nonce sending");
                    client.Close();
                    listener.Stop();
                    break;
                }
                // verify username
                if (!GetUserInfo.IsUserSystemMember(clientUsername))
                {
                    SslTcpServer.AnswerNonceRequest(sslStream, "Error during nonce sending");
                    client.Close();
                    listener.Stop();
                    break;
                }

                String clientNonce = Security.SendNonce(clientUsername);
                // answer nonce request
                if (clientNonce != null)
                {
                    SslTcpServer.AnswerNonceRequest(sslStream, "Nonce send");
                }

                // wait for command request
                String[] clientCommandRequest = SslTcpServer.GetCommandRequest(sslStream);
                // verify command request
                if (clientCommandRequest == null)
                {
                    SslTcpServer.AnswerCommandRequest(sslStream, "Error during command execution");
                    client.Close();
                    listener.Stop();
                    break;
                }

                String clientCommand = clientCommandRequest[0];
                String clientUsernameCommand = clientCommandRequest[1];
                String clientNonceCommand = clientCommandRequest[2];
                // verify username
                if (clientUsernameCommand != clientUsername || !GetUserInfo.IsUserSystemMember(clientUsernameCommand))
                {
                    SslTcpServer.AnswerCommandRequest(sslStream, "Unauthorized");
                    client.Close();
                    listener.Stop();
                    break;
                }
                // verify nonce
                if (clientNonceCommand != clientNonce)
                {
                    SslTcpServer.AnswerCommandRequest(sslStream, "Unauthorized");
                    client.Close();
                    listener.Stop();
                    break;
                }
                // verify access to command
                int retAuth = GetAuthorization.IsUserGranted(clientUsernameCommand, clientCommand);
                if (retAuth == 2)
                {
                    SslTcpServer.AnswerCommandRequest(sslStream, "Unauthorized");
                    client.Close();
                    listener.Stop();
                    break;
                }
                else if (retAuth == 1)
                {
                    SslTcpServer.AnswerCommandRequest(sslStream, "Password");
                    client.Close();
                    listener.Stop();
                    break;
                }


                // answer command request
                SslTcpServer.AnswerCommandRequest(sslStream, "OK");

                String commandResult = RunPowershell.RunSudoersCommand(clientCommand);

                // send command result
                SslTcpServer.SendCommandResult(sslStream, commandResult);
                client.Close();
                listener.Stop();

            }
        }
    }
}
