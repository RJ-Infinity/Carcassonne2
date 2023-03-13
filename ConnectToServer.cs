using System.Net.Sockets;

namespace Carcassonne2
{
    public partial class ConnectToServer : Form
    {
        CarcassonneInit init;
        void SetErrorMessage(string error)
        => ErrorMessageToolTip.Show(error, this, new Point(7, Height- 27), 5000);
        public ConnectToServer(CarcassonneInit init)
        {
            this.init = init;
            init.Cancel = true;//this is changed when we exit correctly
            InitializeComponent();
            PortInput.Controls.RemoveAt(0);
        }
        private void LaunchButton_Click(object sender, EventArgs e)
        {
            if (init.Client != null)
            { SetErrorMessage("Still Waiting for Server to Initialise"); }
            else
            {
                try
                {
                    init.Client = new Client(IPAdressInput.Text, (int)PortInput.Value);
                    init.Client.Connect();
                    init.Client.MessageRecived += Client_MessageRecived;
                    init.Client.Start();
                }
                catch (SocketException ex) {
                    init.Client = null;
                    SetErrorMessage(ex.Message);
                }
            }
        }

        private void Client_MessageRecived(object sender, Message msg)
        {
            if (msg.Key == "ServerFull")
            {
                Invoke(SetErrorMessage, "ServerFull retry when there are available slots or try another server");
                init.Client = null;
                return;
            }
            if (msg.Key == "PlayerID")
            {
                init.Player = new Player();
                if (!int.TryParse(msg.Value, out init.Player.ID))
                { throw new InvalidDataException("Data Recived from server invalid"); }
            }
            if (msg.Key == "Seed")
            {
                if (!int.TryParse(msg.Value, out int seed))
                { throw new InvalidDataException("Data Recived from server invalid"); }
                init.Seed = seed;
            }
            if (msg.Key == "Slots")
            {
                string[] slotsOptions = { "2", "3", "4", "5" };
                if (!slotsOptions.Contains(msg.Value))
                { throw new InvalidDataException("Data Recived from server invalid"); }
                init.Slots = Array.IndexOf(slotsOptions, msg.Value) + 2;
            }
            if (init.Player != null && init.Seed != null && init.Slots != null) {
                init.Client.MessageRecived -= Client_MessageRecived;
                init.Cancel = false;
                if (InvokeRequired) { Invoke(Close); } else { Close(); }
            }
        }

        private void ErrorMessageToolTip_Draw(object sender, DrawToolTipEventArgs e)
        {
            e.DrawBackground();
            e.DrawBorder();
            e.DrawText();
        }
    }
}
