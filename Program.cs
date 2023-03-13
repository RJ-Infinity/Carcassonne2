using System.Net.Sockets;
using System.Net;

namespace Carcassonne2
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length > 0 && args[0] == "server")
            {
                if (args.Length == 1)
                {
                    Console.WriteLine("no port");
                    return;
                }
                char[] numbers = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
                if (
                    args[1].Length != 5 ||
                    !numbers.Contains(args[1][0]) ||
                    !numbers.Contains(args[1][1]) ||
                    !numbers.Contains(args[1][2]) ||
                    !numbers.Contains(args[1][3]) ||
                    !numbers.Contains(args[1][4])
                )
                {
                    Console.WriteLine("port must be in the format XXXXX where X is in the range {0-9}");
                    return;
                }
                int port = 0;
                for (int i = 0; i < 5; i++)
                { port += (int)Math.Pow(10, 4 - i) * (args[1][i] - numbers[0]); }
                new Server(port).BlockingStart();
                return;
            }
            Client client = new Client("172.31.160.1", 8001);
            client.BlockingStart();
            return;
            Player plr = new Player();
            ApplicationConfiguration.Initialize();
            //Application.Run(new ConnectToServer(client, plr));
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            //Application.Run(new CarcassonneForm(client, plr));
        }
    }
}