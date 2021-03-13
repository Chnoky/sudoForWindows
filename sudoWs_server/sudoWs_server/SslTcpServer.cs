using System;
using System.Net;
using System.Net.Sockets;
using System.Net.Security;
using System.Security.Authentication;
using System.Text;
using System.Security.Cryptography.X509Certificates;

namespace Examples.System.Net
{
    public sealed class SslTcpServer
    {
        private static readonly String certificate = @"C:\certificates\server_private.pfx";
        private static readonly X509Certificate2 serverCertificate = new X509Certificate2(certificate, "Standard-1", X509KeyStorageFlags.MachineKeySet);
        // The certificate parameter specifies the name of the file
        // containing the machine certificate.
        public static TcpListener RunServer()
        {
            // Create a TCP/IP (IPv4) socket and listen for incoming connections.
            TcpListener listener = new TcpListener(IPAddress.Any, 443);
            listener.Start();
            return listener;
        }


        public static SslStream ProcessClient(TcpClient client)
        {
            // A client has connected. Create the
            // SslStream using the client's network stream.
            SslStream sslStream = new SslStream(
                client.GetStream(), false);
            // Authenticate the server but not require the client to authenticate.
            try
            {
                sslStream.AuthenticateAsServer(serverCertificate, clientCertificateRequired: false, checkCertificateRevocation: true);

                // Set timeouts for the read and write to 5 seconds.
                sslStream.ReadTimeout = 5000;
                sslStream.WriteTimeout = 5000;

                return sslStream;
                
            }
            catch
            {
                sslStream.Close();
                CloseConnection(client);
                return null;
            }
        }


        public static String GetNonceRequest(SslStream sslStream)
        {
            String messageData = ReadMessage(sslStream).Replace("<EOF>", "");
            String[] messageInfo = messageData.Split(";");

            if (messageInfo.Length == 2 && messageInfo[0] == "nonceRequest")
            {
                return messageInfo[1];
            }
            else
            {
                return null;
            }
            
        }
        public static void AnswerNonceRequest(SslStream sslStream, String answer)
        {
            // Write a message to the client.
            byte[] message = Encoding.UTF8.GetBytes(answer + "<EOF>");
            sslStream.Write(message);
        }


        public static String[] GetCommandRequest(SslStream sslStream)
        {
            String messageData = ReadMessage(sslStream).Replace("<EOF>", "");
            String[] messageInfo = messageData.Split(";");

            if (messageInfo.Length == 3)
            {
                return messageInfo;
            }
            else
            {
                return null;
            }
        }
        public static void AnswerCommandRequest(SslStream sslStream, String answer)
        {
            // Write a message to the client.
            byte[] message = Encoding.UTF8.GetBytes(answer+"<EOF>");
            sslStream.Write(message);
        }

        public static void SendCommandResult(SslStream sslStream, String answer)
        {
            // Write a message to the client.
            byte[] message = Encoding.UTF8.GetBytes(answer + "<EOF>");
            sslStream.Write(message);
        }

        private static string ReadMessage(SslStream sslStream)
        {
            // Read the  message sent by the client.
            // The client signals the end of the message using the
            // "<EOF>" marker.
            byte[] buffer = new byte[2048];
            StringBuilder messageData = new StringBuilder();
            int bytes = -1;
            do
            {
                // Read the client's test message.
                bytes = sslStream.Read(buffer, 0, buffer.Length);

                // Use Decoder class to convert from bytes to UTF8
                // in case a character spans two buffers.
                Decoder decoder = Encoding.UTF8.GetDecoder();
                char[] chars = new char[decoder.GetCharCount(buffer, 0, bytes)];
                decoder.GetChars(buffer, 0, bytes, chars, 0);
                messageData.Append(chars);
                // Check for EOF or an empty message.
                if (messageData.ToString().IndexOf("<EOF>") != -1)
                {
                    break;
                }
            } while (bytes != 0);

            return messageData.ToString();
        }

        private static void CloseConnection(TcpClient client)
        {
            client.GetStream().Close();
            client.Close();
        }
    }
}