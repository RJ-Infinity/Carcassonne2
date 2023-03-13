using System.Net.Sockets;

namespace Carcassonne2
{
    public partial class ConnectToServer : Form
    {
        TcpClient Client;
        Player Player;
        void SetErrorMessage(string error)
        {
            ErrorLabel.Text = error;
            ErrorPanel.Visible = true;
            ErrorPanel.Width = ErrorLabel.Width;
        }
        public ConnectToServer(TcpClient client, Player player)
        {
            Client = client;
            Player = player;
            InitializeComponent();
            PortInput.Controls.RemoveAt(0);
        }
        private void LaunchButton_Click(object sender, EventArgs e)
        {
            try
            {
                Client.Connect(IPAdressInput.Text, (int)PortInput.Value);
                Close();
            }
            catch (SocketException ex) { SetErrorMessage(ex.Message); }
        }
        private void ErrorPanel_Click(object sender, EventArgs e) => ErrorPanel.Visible = false;
        private void ErrorLabel_Click(object sender, EventArgs e) => ErrorPanel.Visible = false;
    }
}
