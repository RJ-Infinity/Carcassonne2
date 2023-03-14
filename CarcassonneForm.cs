using RJGL;
using SkiaSharp;

namespace Carcassonne2
{
    public partial class CarcassonneForm : RJGLForm
    {
        TileManager CarcasonneTileManager;
        layers.Background bg;
        layers.HUD hud;
        layers.TileLayer tileLayer;
        Player localPlayer;
        Client Client;
        List<Player> Players = new();
        public CarcassonneForm(CarcassonneInit init)
        {
            if (
                init.Client == null ||
                init.PlayerColour == null ||
                init.Seed == null ||
                init.Slots == null
            ) { throw new ArgumentNullException("init's components must not be null"); }
            Client = init.Client;

            InitializeComponent();
            for (int i = 0; i < init.Slots.Value; i++)
            {
                Player p = new();
                p.Colour = (Colours)i;
                Players.Add(p);
                if (init.PlayerColour == i) { localPlayer = p; }
            }
            // this means that you can target the next element and it will target the first element
            Players.Add(Players[0]);

            localPlayer.StateChanged += LocalPlayer_StateChanged;

            List<TileDefinition> defaultTiles = TileDefinition.ParseJSONFile(
                File.ReadAllText(".\\Tiles.json")
            );

            CarcasonneTileManager = new TileManager(defaultTiles, init.Seed.Value);
            //assuming that there are tiles in the tile pool
            CarcasonneTileManager.GenerateNextTile();
            for (int i = 0; i < defaultTiles.Count; i++)
            {
                CarcasonneTileManager[i, 5] = new Tile(defaultTiles[i], Orientation.North);
            }
            CarcasonneTileManager.LastTilePos = new(7, 5);
            bg = new layers.Background();
            hud = new layers.HUD(100, localPlayer);
            hud.OrientationButton += Hud_OrientationButton;
            hud.FinishTurnButton += Hud_FinishTurnButton;
            tileLayer = new layers.TileLayer(CarcasonneTileManager, localPlayer);
            bg.AddLinkedLayer(tileLayer);
            bg.MouseDown += Bg_MouseDown;
            tileLayer.KeyDown += TileLayer_KeyDown;
            tileLayer.Pan(new SKPoint(Width / 2, Height / 2));
            Layers.Add(bg);
            Layers.Add(tileLayer);
            Layers.Add(hud);
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Client.MessageRecived += Client_MessageRecived;
            Client.SendMessage(new("Ready", ""));
        }
        private void PlaceTileFromString(string tileString)
        {
            if (
                !int.TryParse(tileString.Substring(
                    0,
                    tileString.IndexOf(',')
                ), out int x) ||
                !int.TryParse(tileString.Substring(
                    tileString.IndexOf(',') + 1,
                    tileString.IndexOf('@') - tileString.IndexOf(',') - 1
                ), out int y)
            ){ throw new InvalidDataException("Error tile string not parsing correctly"); }

            CarcasonneTileManager[x,y] = new Tile(
                CarcasonneTileManager.CurrentTile, extensions.StringToEnumType(
                    tileString.Substring(
                        tileString.IndexOf('@') + 1,
                        tileString.IndexOf('#') - tileString.IndexOf('@') - 1
                    ),
                    Orientation.None
                )
            );

            try { CarcasonneTileManager.GenerateNextTile(); }
            catch (InvalidOperationException)
            {
                Console.WriteLine("REGENERATEING STACK");
                CarcasonneTileManager.GenerateTilePool();
                CarcasonneTileManager.GenerateNextTile();
            }

            Player currentPlayer = Players.Find((Player pl) => pl.State == State.Playing);
            foreach (
                int i in tileString
                .Substring(tileString.IndexOf('#') + 1)
                .Split(',')
                .Where((string c) => c.Length > 0)
                .Select((string c) => int.Parse(c))
            ) { CarcasonneTileManager[x, y].Components[i].Claimee = currentPlayer; }
            tileLayer.Invalidate();
        }
        private void Client_MessageRecived(object sender, Message msg)
        {
            switch (msg.Key)
            {
                case "Error": throw new Exception("ERROR SERVER MISUNDERSTOOD THE DATA");
                case "AllReady":
                    foreach (Player pl in Players) { pl.AdvanceState(); }
                    break;
                case "PlaceTile":
                    PlaceTileFromString(msg.Value);
                    Player currentPlayer = Players.Find(
                        (Player pl) => pl.State == State.Playing
                    );
                    currentPlayer.AdvanceState();
                    Players[Players.IndexOf(currentPlayer) + 1].AdvanceState();
                    break;
                default: throw new InvalidDataException(
                    "Error Unknown Message from Server "+msg.Key+":"+msg.Value
                );
            }
            UpdateDrawing();
        }


        private void Hud_FinishTurnButton(object sender)
        {
            Players[Players.IndexOf(localPlayer) + 1].AdvanceState();
            localPlayer.AdvanceState();
            Client.SendMessage(new Message(
                "PlaceTile",
                CarcasonneTileManager.LastTilePos.X.ToString() + "," +
                CarcasonneTileManager.LastTilePos.Y.ToString() + "@" +
                CarcasonneTileManager[
                    CarcasonneTileManager.LastTilePos
                ].Orientation.ToString() + "#" +
                string.Join(
                    ',',
                    CarcasonneTileManager[CarcasonneTileManager.LastTilePos].Components.Select(
                        (TileComponent tc, int i) => (tc.Claimee != null, i)
                    ).Where(
                        ((bool, int) v) => v.Item1
                    ).Select(
                        ((bool, int) v) => v.Item2
                    )
                )
            ));
        }

        private void LocalPlayer_StateChanged(object sender)
        {
            if (localPlayer.State == State.Playing)
            { localPlayer.State = State.PlacingTile; }
            hud.Invalidate();
            tileLayer.Invalidate();
        }

        private void Hud_OrientationButton(object sender, layers.EventArgs_OrientationButton e)
        => CarcasonneTileManager.CurrentOrientation = e.Orientation;

        private void TileLayer_KeyDown(object sender, EventArgs_KeyDown e)
        {
            CarcasonneTileManager.LastTilePos = new(17, 5);
            if (e.KeyCode >= 37 && e.KeyCode <= 40)
            {
                CarcasonneTileManager.CurrentOrientation = e.KeyCode switch
                {
                    38 => Orientation.North,
                    39 => Orientation.East,
                    40 => Orientation.South,
                    37 => Orientation.West,
                    _ => throw new InvalidOperationException("Unreachable Code Reached. Your Memory is probably corrupt."),
                };
                tileLayer.Invalidate();
            }
            if (e.KeyCode == 113)
            {
                //reset pan if f2 pressed
                tileLayer.Pan(new SKPoint(Width/2,Height/2) -tileLayer.Offset);
            }
        }

        private void Bg_MouseDown(object sender, EventArgs_Click e)
        {
            if (
                localPlayer.Meeple > 0 &&
                localPlayer.State == State.PlacingMeeple &&
                tileLayer.Position == CarcasonneTileManager.LastTilePos &&
                tileLayer.GetSelectedComp() &&
                tileLayer.SelectedComp.Claimee == null
            )
            {
                tileLayer.SelectedComp.Claimee = localPlayer;
                localPlayer.Meeple--;
                hud.Invalidate();
            }
            SKPoint position = bg.ScreenToWorld(e.Position);
            SKPointI positionIndex = new(
                (int)Math.Floor(position.X / 100),
                (int)Math.Floor(position.Y / 100)
            );
            if (localPlayer.State == State.PlacingTile && e.Button == RJGL.MouseButtons.Left && CarcasonneTileManager.IsValidLocation(
                positionIndex,
                CarcasonneTileManager.CurrentOrientation,
                CarcasonneTileManager.CurrentTile
            ) && CarcasonneTileManager.IsValidLocation(
                positionIndex,
                CarcasonneTileManager.CurrentOrientation,
                CarcasonneTileManager.CurrentTile
            ))
            {
                CarcasonneTileManager[positionIndex] = new Tile(CarcasonneTileManager.CurrentTile, CarcasonneTileManager.CurrentOrientation);
                try{CarcasonneTileManager.GenerateNextTile();}
                catch(InvalidOperationException)
                {
                    Console.WriteLine("REGENERATEING STACK");
                    CarcasonneTileManager.GenerateTilePool();
                    CarcasonneTileManager.GenerateNextTile();
                }
                CarcasonneTileManager.CurrentOrientation = Orientation.North;
                localPlayer.AdvanceState();
            }
        }
        protected override void OnClosed(EventArgs e)
        {
            Client.Close();
            base.OnClosed(e);
        }
    }
}