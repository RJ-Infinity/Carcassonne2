using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Security.Cryptography;
using OpenTK.Input;
using System.Linq;
using Carcassonne2.layers;
using System.Windows.Forms;

namespace Carcassonne2
{
    //https://www.codeproject.com/Articles/1415/Introduction-to-TCP-client-server-in-C
    class Server
    {
        public TcpListener TCPListener { get; private set; }
        int Port;
        protected List<Socket> Sockets = new();
        public Server(int port) { Port = port; }

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
            Thread thread = new Thread(BlockingStart);
            thread.Start();
        }
        public void BlockingStart()
        {
            TCPListener = new TcpListener(IPAddress.Parse("172.31.160.1"), Port);

            /* Start Listeneting at the specified port */
            TCPListener.Start();

            Console.WriteLine("The local End point is: " + TCPListener.LocalEndpoint);

            while (true)
            {
                if (TCPListener.Pending())
                {
                    Sockets.Add(TCPListener.AcceptSocket());
                    OnClientConnected(Sockets[Sockets.Count - 1]);
                }
                foreach (Socket sock in Sockets.FindAll(sock => sock.Available > 0))
                { OnMessageRecived(Message.ReciveMessageFromSocket(sock),sock); }
            }
        }
        public void Close()
        {
            foreach (Socket sock in Sockets) { sock.Close(); }
            TCPListener.Stop();
        }
        public virtual void SendMessage(Socket sock, Message msg) => sock.Send(msg.GetMessageBytes());

    }
    class Client
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
        public void Start()
        {
            Thread thread = new Thread(BlockingStart);
            thread.Start();
        }
        public void BlockingStart()
        {
            TcpClient = new TcpClient();
            TcpClient.Connect(IpAdress, Port);
            while (open) {
                Thread.Sleep(10);
                while (messages.Count > 0)
                {
                    byte[] bytes = messages.Dequeue().GetMessageBytes();
                    TcpClient.GetStream().Write(bytes, 0, bytes.Length);
                }
                if (TcpClient.Available > 0)
                { OnMessageRecived(Message.ReciveMessageFromStream(TcpClient.GetStream())); }
            }
            TcpClient.Close();
        }
        public delegate void MessageRecivedHandler(object sender, Message msg);
        public event MessageRecivedHandler? MessageRecived;
        protected virtual void OnMessageRecived(Message msg) => MessageRecived?.Invoke(this, msg);
        public virtual void SendMessage(Message msg) => messages.Enqueue(msg);
        public virtual void Close() => open = false;
    }
    struct Message
    {
        public string Key;
        public string Value;
        public Message(string key, string value)
        {
            Key = key;
            Value = value;
        }
        public byte[] GetMessageBytes() => new ASCIIEncoding().GetBytes(
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
        public static Message ReciveMessageFromStream(Stream stream)
        => ReciveMessage((byte[] bytes) => stream.Read(bytes, 0, bytes.Length));
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
        {
            if (sock.Available < 3)
            { throw new ArgumentException("Error Socket has not enough data to read"); }
            return ReciveMessage((byte[] bytes) => sock.Receive(bytes));
        }
        public static Message ReciveMessage(Action<byte[]> recive)
        {
            //version
            byte[] bytes = new byte[3];
            recive(bytes);
            if (!bytes.SequenceEqual(new byte[] { (byte)'0', (byte)'0', (byte)'0' }))
            { throw new InvalidDataException("data stream is in an unsported verion"); }
            //keyLength
            bytes = new byte[3];
            recive(bytes);
            int keySize = getSize(bytes);
            //valueLength
            bytes = new byte[3];
            recive(bytes);
            int valueSize = getSize(bytes);
            //key
            bytes = new byte[keySize];
            recive(bytes);
            string key = new string(bytes.Select((byte b) => Convert.ToChar(b)).ToArray());
            //value
            bytes = new byte[keySize];
            recive(bytes);
            string value = new string(bytes.Select((byte b) => Convert.ToChar(b)).ToArray());
            return new Message(key,value);
        }
    }
}
