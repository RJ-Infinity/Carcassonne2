using System.Net.Sockets;
using System.Net;
using System.Diagnostics;

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
            { Server(args); }
            else
            {
                if (Debugger.IsAttached) { Client(); }
                else {
                    try { Client(); }
                    catch (Exception e) { MessageBox.Show(e.Message,"ERROR"); }
                }
            }
        }
        static void Client()
        {
            CarcassonneInit init = new();
            ApplicationConfiguration.Initialize();
            Application.Run(new ConnectToServer(init));
            if (!init.Cancel) { Application.Run(new CarcassonneForm(init)); }
        }
        static void Server(string[] args)
        {
            if (args.Length == 1)
            {
                Console.WriteLine("no ip, port or slotCount");
                return;
            }
            if (args.Length == 2)
            {
                Console.WriteLine("no port or slotCount");
                return;
            }
            if (args.Length == 3)
            {
                Console.WriteLine("no slotCount");
                return;
            }
            char[] numbers = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            if (
                args[2].Length != 5 ||
                !numbers.Contains(args[2][0]) ||
                !numbers.Contains(args[2][1]) ||
                !numbers.Contains(args[2][2]) ||
                !numbers.Contains(args[2][3]) ||
                !numbers.Contains(args[2][4])
            )
            {
                Console.WriteLine("port must be in the format XXXXX where X is in the range {0-9}");
                return;
            }
            int port = 0;
            for (int i = 0; i < 5; i++)
            { port += (int)Math.Pow(10, 4 - i) * (args[2][i] - numbers[0]); }

            string[] slotsOptions = { "2", "3", "4", "5" };
            if (!slotsOptions.Contains(args[3]))
            {
                Console.WriteLine("slots must be in the range 2-5");
                return;
            }
            new CarcassonneServer(
                port,
                args[1],
                Array.IndexOf(slotsOptions, args[3]) + 2
            ).BlockingStart();
            return;
        }
    }
}