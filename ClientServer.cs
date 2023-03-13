using System.Net.Sockets;
using System.Net;
using System.Text;

namespace Carcassonne2
{
    //https://www.codeproject.com/Articles/1415/Introduction-to-TCP-client-server-in-C
    public class Server
    {
        public TcpListener TCPListener { get; private set; }
        int Port;
        string IpAdress;
        protected List<Socket> Sockets = new();
        public Server(int port, string ipAdress) { Port = port; IpAdress = ipAdress; }

        public delegate void MessageRecivedHandler(object sender, Message msg, Socket sock);
        public event MessageRecivedHandler? MessageRecived;
        protected virtual void OnMessageRecived(Message msg, Socket sock) => MessageRecived?.Invoke(this, msg, sock);

        public delegate void ClientConnectedHandler(object sender, Socket sock);
        public event ClientConnectedHandler? ClientConnected;
        protected virtual void OnClientConnected(Socket sock)
        {
            Console.WriteLine("Connection accepted from " + sock.RemoteEndPoint);
            ClientConnected?.Invoke(this, sock);
        }
        public void Start()
        {
            Thread thread = new(BlockingStart);
            thread.IsBackground = true;
            thread.Start();
        }
        public void BlockingStart()
        {
            TCPListener = new TcpListener(IPAddress.Parse(IpAdress), Port);

            /* Start Listeneting at the specified port */
            TCPListener.Start();

            Console.WriteLine("The local End point is: " + TCPListener.LocalEndpoint);

            while (true)
            {
                if (TCPListener.Pending())
                {
                    Sockets.Add(TCPListener.AcceptSocket());
                    OnClientConnected(Sockets[^1]);
                }
                foreach (Socket sock in Sockets.FindAll(sock => sock.Available > 0))
                { OnMessageRecived(Message.ReciveMessageFromSocket(sock),sock); }
            }
        }
        public void CloseSocket(Socket sock)
        {
            sock.Close();
            Sockets.Remove(sock);
        }
        public void Close()
        {
            foreach (Socket sock in Sockets) { sock.Close(); }
            TCPListener.Stop();
            Sockets.Clear();
        }
        public virtual void SendMessage(Socket sock, Message msg) => sock.Send(msg.ToBytes());

    }
    public class Client
    {
        public string IpAdress { get; private set; }
        public int Port { get; private set; }
        public TcpClient? TcpClient { get; private set; }
        private Queue<Message> messages = new();
        private bool open = true;
        public Client(string ipAdress, int port)
        {
            IpAdress = ipAdress;
            Port = port;
        }
        public void Connect()
        {
            TcpClient = new TcpClient();
            TcpClient.Connect(IpAdress, Port);
        }
        public void Start()
        {
            Thread thread = new(BlockingStart);
            thread.IsBackground = true;
            thread.Start();
        }
        public void BlockingStart()
        {
            if (TcpClient == null) { Connect(); }
            while (open) {
                Thread.Sleep(10);
                while (messages.Count > 0)
                {
                    byte[] bytes = messages.Dequeue().ToBytes();
                    TcpClient.GetStream().Write(bytes, 0, bytes.Length);
                }
                if (TcpClient.Available > 0)
                {
                    Stream stream = TcpClient.GetStream();
                    OnMessageRecived(Message.ReciveMessage(
                        (byte[] bytes) => stream.Read(bytes, 0, bytes.Length),
                        (int amount) => TcpClient.Available >= amount
                    ));
                }
            }
            TcpClient.Close();
        }
        public delegate void MessageRecivedHandler(object sender, Message msg);
        public event MessageRecivedHandler? MessageRecived;
        protected virtual void OnMessageRecived(Message msg) => MessageRecived?.Invoke(this, msg);
        public virtual void SendMessage(Message msg) => messages.Enqueue(msg);
        public virtual void Close() => open = false;
    }
    public struct Message
    {
        public string Key;
        public string Value;
        public Message(string key, string value)
        {
            Key = key;
            Value = value;
        }
        public byte[] ToBytes() => new ASCIIEncoding().GetBytes(
            //it uses a sort of sized string
            //name        |length
            //------------+------------
            //version     |3 // this should currently always be three '0's
            //keyLength   |3
            //valueLength |3
            //key         |keyLength
            //value       |valueLength
            "000" + // version
            Key.Length.ToString().PadLeft(3, '0') + // keyLength
            Value.Length.ToString().PadLeft(3, '0') + // valueLength
            Key + // key
            Value // value
        );
        private static int getSize(byte[] sizeBytes)
        {
            char[] numbers = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            if (
                !numbers.Contains(Convert.ToChar(sizeBytes[0])) ||
                !numbers.Contains(Convert.ToChar(sizeBytes[1])) ||
                !numbers.Contains(Convert.ToChar(sizeBytes[2]))
            ) { throw new ArgumentException("Error Data is in invalid format"); }
            int size = 0;
            for (int i = 0; i < 3; i++)
            { size += (int)Math.Pow(10, 2 - i) * (sizeBytes[i] - numbers[0]); }
            return size;
        }
        public static Message ReciveMessageFromSocket(Socket sock)
        => ReciveMessage(
            (byte[] bytes) => sock.Receive(bytes),
            (int amount)=> sock.Available >= amount
        );
        public static Message ReciveMessage(Action<byte[]> recive, Func<int, bool> areBytesReady)
        {
            //version
            byte[] bytes = new byte[3];
            if (!areBytesReady(bytes.Length))
            { throw new InvalidDataException("not enough bytes to read"); }
            recive(bytes);
            if (!bytes.SequenceEqual(new byte[] { (byte)'0', (byte)'0', (byte)'0' }))
            { throw new InvalidDataException("data stream is in an unsported verion"); }
            //keyLength
            bytes = new byte[3];
            if (!areBytesReady(bytes.Length))
            { throw new InvalidDataException("not enough bytes to read"); }
            recive(bytes);
            int keySize = getSize(bytes);
            //valueLength
            bytes = new byte[3];
            if (!areBytesReady(bytes.Length))
            { throw new InvalidDataException("not enough bytes to read"); }
            recive(bytes);
            int valueSize = getSize(bytes);
            //key
            bytes = new byte[keySize];
            if (keySize > 0)
            {
                if (!areBytesReady(bytes.Length))
                { throw new InvalidDataException("not enough bytes to read"); }
                recive(bytes);
            }
            string key = new(bytes.Select((byte b) => Convert.ToChar(b)).ToArray());
            //value
            bytes = new byte[valueSize];
            if (valueSize > 0)
            {
                if (!areBytesReady(bytes.Length))
                { throw new InvalidDataException("not enough bytes to read"); }
                recive(bytes);
            }
            string value = new(bytes.Select((byte b) => Convert.ToChar(b)).ToArray());
            return new Message(key,value);
        }
    }
}
