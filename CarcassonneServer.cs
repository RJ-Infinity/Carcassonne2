using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Carcassonne2
{
    internal class CarcassonneServer:Server
    {
        private int seed = new Random().Next(int.MaxValue);
        private List<bool> ready = new();
        private int Slots;
        public CarcassonneServer(int port, string ip, int slots) : base(port, ip)
        {
            Slots = slots;
        }
        protected override void OnClientConnected(Socket sock)
        {
            base.OnClientConnected(sock);
            if (Sockets.Count > Slots) {
                SendMessage(sock,new Message("ServerFull", ""));
                CloseSocket(sock);
            }
            else
            {
                SendMessage(sock,new Message("PlayerID", (Sockets.Count - 1).ToString()));
                SendMessage(sock,new Message("Seed", seed.ToString()));
                SendMessage(sock,new Message("Slots", Slots.ToString()));
                ready.Add(false);
            }
        }
        protected override void OnMessageRecived(Message msg, Socket sock)
        {
            Console.WriteLine(sock.RemoteEndPoint + "=>" + msg.Key + ":" + msg.Value);
            switch (msg.Key)
            {
                case "Ready":
                    ready[Sockets.IndexOf(sock)] = true;
                    if (Sockets.Count >= Slots && ready.All((r) => r))
                    {
                        foreach (Socket socket in Sockets)
                        { SendMessage(socket, new Message("AllReady", "")); }
                    }
                    break;
                case "PlaceTile":
                    foreach (Socket socket in Sockets.Where((Socket socket) => socket != sock))
                    { SendMessage(socket, msg); }
                    break;
                default:
                    SendMessage(sock,new Message("Error", "Unknown Message"));
                    break;
            }
            base.OnMessageRecived(msg, sock);
        }
        public override void SendMessage(Socket sock, Message msg)
        {
            Console.WriteLine(sock.RemoteEndPoint + "<=" + msg.Key + ":" + msg.Value);
            base.SendMessage(sock, msg);
        }
    }
}
