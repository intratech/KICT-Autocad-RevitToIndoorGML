using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace IndoorGML.Exporter.Revit.Comnunication
{
    public class AsyncServer
    {
        List<Socket> clientSockets;

        bool isStarted = false;

        public event Action ClientConnected;

        private Stack<string> messages = new Stack<string>();

        public AsyncServer()
        {
            clientSockets = new List<Socket>();
        }

        public void Start()
        {
            if (isStarted)
                return;

            IPAddress ipAddress = IPAddress.Parse("127.0.0.1"); //ipHostInfo.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);

            // Create a TCP/IP socket.  
            Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            listener.Bind(localEndPoint);
            listener.Listen(100);

            SocketAsyncEventArgs e = new SocketAsyncEventArgs();
            e.Completed += AcceptCallback;
            if (!listener.AcceptAsync(e))
            {
                AcceptCallback(listener, e);
            }

            isStarted = true;
        }

        private void AcceptCallback(object sender, SocketAsyncEventArgs e)
        {
            Socket listenSocket = (Socket)sender;
            try
            {
                this.clientSockets.Add(e.AcceptSocket);
                Console.WriteLine("New client connceted");
                

                this.ClientConnected?.Invoke();
                var msg = this.messages.Pop();
                if (!string.IsNullOrEmpty(msg))
                {
                    this.Send(msg);
                }

                e.AcceptSocket = null;
                listenSocket.AcceptAsync(e);
            }
            catch
            {
                // handle any exceptions here;
            }
            finally
            {
                //e.AcceptSocket = null; // to enable reuse
            }
        }
        public void Send(string msg)
        {
            this.messages.Push(msg);
            foreach (var client in clientSockets)
            {
                try
                {
                    if (client.Connected)
                    {
                        var bytes = Encoding.ASCII.GetBytes(msg);
                        client.Send(bytes);
                    }
                    else
                    {
                        Console.WriteLine($"Client {client.Handle} disconnected...");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
