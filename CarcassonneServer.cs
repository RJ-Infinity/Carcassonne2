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
                sock.Send(new Message("ServerFull", "").ToBytes());
                CloseSocket(sock);
            }
            else
            {
                sock.Send(new Message("PlayerID", (Sockets.Count - 1).ToString()).ToBytes());
                sock.Send(new Message("Seed", seed.ToString()).ToBytes());
                sock.Send(new Message("Slots", Slots.ToString()).ToBytes());
                ready.Add(false);
            }
        }
        protected override void OnMessageRecived(Message msg, Socket sock)
        {
            switch (msg.Key)
            {
                case "Ready":
                    ready[Sockets.IndexOf(sock)] = true;
                    if (Sockets.Count >= Slots && ready.All((r) => r))
                    {
                        foreach (Socket socket in Sockets)
                        { socket.Send(new Message("AllReady", "").ToBytes()); }
                    }
                    break;
                case "PlaceTile":
                    foreach (Socket socket in Sockets.Where((Socket socket) => socket != sock))
                    { socket.Send(msg.ToBytes()); }
                    break;
                default:
                    sock.Send(new Message("Error", "Unknown Message").ToBytes());
                    break;
            }
            base.OnMessageRecived(msg, sock);
        }
    }
}
