using System;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Text;
using System.Security.Cryptography.X509Certificates;

namespace Examples.System.Net
{
    public class SslTcpClient
    {
        private static readonly string machineName = "localhost";
        private static readonly string serverName = "localhost";
        private static TcpClient client;


        public static SslStream RunClient()
        {
            client = new TcpClient(machineName, 443);
            // Create a TCP/IP client socket.
            // machineName is the host running the server application.

            // Create an SSL stream that will close the client's stream.
            SslStream sslStream = new SslStream(
                client.GetStream(),
                false,
                new RemoteCertificateValidationCallback(AcceptAllCertificates),
                null
                );
            // The server name must match the name on the server certificate.
            try
            {
                sslStream.AuthenticateAsClient(serverName);
                return sslStream;
            }
            catch (AuthenticationException e)
            {
                Console.WriteLine("Exception: {0}", e.Message);
                if (e.InnerException != null)
                {
                    Console.WriteLine("Inner exception: {0}", e.InnerException.Message);
                }
                Console.WriteLine("Authentication failed - closing the connection.");
                CloseConnection(client);
                return null;
            }

            
        }

        public static bool RequestNonce(SslStream sslStream)
        {
            byte[] message = Encoding.UTF8.GetBytes("nonceRequest;" + Environment.UserName + "<EOF>");
            // Send nonce request
            sslStream.Write(message);
            sslStream.Flush();

            // Read response from the server.
            string serverResponseCommand = ReadMessage(sslStream).Replace("<EOF>", "");

            if (serverResponseCommand == "Nonce send")
            {
                return true;
            }
            else
            {
                CloseConnection(client);
                return false;
            }      
        }

        public static bool RequestCommand(SslStream sslStream, String command_requested, String nonce, Configuration cfg)
        {
            byte[] message = Encoding.UTF8.GetBytes(command_requested + ";" + Environment.UserName + ";" + nonce + "<EOF>");
            // Send command request
            sslStream.Write(message);
            sslStream.Flush();

            // Read response from the server.
            string serverResponseCommand = ReadMessage(sslStream).Replace("<EOF>", "");

            switch (serverResponseCommand)
            {
                case "Unauthorized":
                    Console.WriteLine("Unauthorized command for this user or bad authentication.");
                    CloseConnection(client);
                    return false;
                case "Password":
                    Console.WriteLine("Command needs administrator password.");
                    CloseConnection(client);
                    RunPowershell.RunAsAdmin(command_requested, cfg);
                    return false;
                case "OK":
                    return true;
                default:
                    CloseConnection(client);
                    return false;
            }
        }

        public static bool GetCommandResponse(SslStream sslStream)
        {
            // Read response from the server.
            String serverResponseCommand = ReadMessage(sslStream).Replace("<EOF>", "");

            Console.WriteLine(serverResponseCommand);
            // Close the client connection.
            CloseConnection(client);
            return true;
        }


        private static string ReadMessage(SslStream sslStream)
        {
            // Read the  message sent by the server.
            // The end of the message is signaled using the
            // "<EOF>" marker.
            byte[] buffer = new byte[2048];
            StringBuilder messageData = new StringBuilder();
            int bytes = -1;
            do
            {
                bytes = sslStream.Read(buffer, 0, buffer.Length);

                // Use Decoder class to convert from bytes to UTF8
                // in case a character spans two buffers.
                Decoder decoder = Encoding.UTF8.GetDecoder();
                char[] chars = new char[decoder.GetCharCount(buffer, 0, bytes)];
                decoder.GetChars(buffer, 0, bytes, chars, 0);
                messageData.Append(chars);
                // Check for EOF.
                if (messageData.ToString().IndexOf("<EOF>") != -1)
                {
                    break;
                }
            } while (bytes != 0);

            return messageData.ToString();
        }

        // The following method is invoked by the RemoteCertificateValidationDelegate.
        private static bool AcceptAllCertificates(
              object sender,
              X509Certificate certificate,
              X509Chain chain,
              SslPolicyErrors sslPolicyErrors)
        {
            // TODO : improve security
            return true;
        }

        private static void CloseConnection(TcpClient client)
        {
            client.GetStream().Close();
            client.Close();
        }
    }
}