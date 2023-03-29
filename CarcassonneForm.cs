using RJGL;
using SkiaSharp;
using System.Collections.Generic;
using System.Data;
using System.Linq;

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
                Player p = new(){Colour = (Colours)i};
                Players.Add(p);
                if (init.PlayerColour == i) { localPlayer = p; }
            }
            // this means that you can target the next element and it will target the first element
            Players.Add(Players[0]);

            localPlayer.StateChanged += LocalPlayer_StateChanged;

            HashSet<TileDefinition> defaultTiles = TileDefinition.ParseJSONFile(
                File.ReadAllText(".\\Tiles.json")
            ).ToHashSet();

            CarcasonneTileManager = new TileManager(defaultTiles, init.Seed.Value);
            //assuming that there are tiles in the tile pool
            CarcasonneTileManager.GenerateNextTile();
            //for (int i = 0; i < defaultTiles.Count; i++)
            //{
            //    CarcasonneTileManager[i*2, 5] = new Tile(defaultTiles[i], Orientation.North);
            //}
            //CarcasonneTileManager.LastTilePos = new(7, 5);
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

            Player currentPlayer = Players.Find((Player pl) => pl.State == State.Playing);
            foreach (
                int i in tileString
                .Substring(tileString.IndexOf('#') + 1)
                .Split(',')
                .Where((string c) => c.Length > 0)
                .Select((string c) => int.Parse(c))
            ) { CarcasonneTileManager[x, y].Components[i].Claimee = currentPlayer; }
            ReturnMeeple(new SKPointI(x,y));
            try {
                CarcasonneTileManager.GenerateNextTile();
            }
            catch (InvalidOperationException)
            {
                CountPoints();
                Players.ForEach((Player pl)=>pl.State = State.GameOver);
                Client.SendMessage(new("Score", localPlayer.Score.ToString()));
            }
            tileLayer.Invalidate();
        }
        private void CountPoints()
        //note this dosent return the meeple as it is intended to be used at the end of the game
        {
            foreach (ComponentGraph graph in CarcasonneTileManager.Graph)
            {
                if (graph.Claimee.Count > 0)
                {
                    Player MostMeeple = graph.Claimee[0];
                    int MostMeepleCount = 0;
                    // itterate over each player in the claimee list by merging them
                    foreach (IGrouping<Player, Player> group in graph.Claimee.GroupBy(p => p))
                    {
                        // this means that you can get the amount of each player in the
                        // claimee list by counting the amount in each Grouping
                        if (group.Count() > MostMeepleCount)
                        {
                            MostMeeple = group.Key;
                            MostMeepleCount = group.Count();
                        }
                    }
                    if (graph.Type == ComponentsType.Grass)
                    {
                        MostMeeple.Score += graph.Borders.Where(
                            (ComponentGraph g) =>
                            g.Type == ComponentsType.Town &&
                            CarcasonneTileManager.IsGraphComplete(g)
                        ).Count();
                    }
                    else if (graph.Type == ComponentsType.Abbey)
                    {
                        if (graph.Components.Count != 1 || graph.Components.First().Value.Count != 1)
                        { throw new InvalidOperationException("this graph must have exactly 1 component"); }
                        SKPointI location = graph.Components.First().Key;
                        MostMeeple.Score += new List<SKPointI>{
                            new(-1, -1), new(-1, 0), new(-1, 1),
                            new(0, -1), new(0, 1),
                            new(1, -1), new(1, 0), new(1, 1)
                        // +1 becuasue that includes the tile the abbey is on
                        }.Where(t => CarcasonneTileManager.ContainsTile(location + t)).Count()+1;
                    }
                    else
                    {
                        MostMeeple.Score += graph.Components.SelectMany(
                            (KeyValuePair<SKPointI, HashSet<TileComponent>> c) => c.Value
                        // use a hash set to remove duplicates
                        ).ToHashSet().Count * graph.Type.GetPoints();
                        // add it again for the components with double score
                        MostMeeple.Score += graph.Components.SelectMany(
                            (KeyValuePair<SKPointI, HashSet<TileComponent>> c) => c.Value
                        ).Where(
                            (TileComponent tc) => tc.DoubleScore
                        ).ToHashSet().Count * graph.Type.GetPoints();
                    }
                }
            }
        }
        private void ReturnMeeple(SKPointI pos)
        {
            // beacuse abbeys dont use the abbey type to compleate themselfs check
            // bordering tiles for abbey graphs and see if they are now completed
            foreach (
                TileComponent tc in new HashSet<SKPointI> {
                    // left collumn
                    new(-1, -1), new(-1, 0), new(-1, 1),
                    // centre collumn (ignore the current tile)
                    new(0, -1), new(0, 1),
                    // right collumn
                    new(1, -1), new(1, 0), new(1, 1)
                }.Where((SKPointI t) => CarcasonneTileManager.ContainsTile(pos + t)).SelectMany(
                    (SKPointI t) => CarcasonneTileManager[pos + t].Components.Where(
                        (TileComponent tc) => tc.Type == ComponentsType.Abbey
                    )
                //also check all the Components in the current tile
                ).Concat(CarcasonneTileManager[pos].Components)
            )
            {
                ComponentGraph graph = CarcasonneTileManager.FindGraphWith(tc);
                if (CarcasonneTileManager.IsGraphComplete(graph))
                {
                    HashSet<TileComponent> tileComponents = graph.Components.SelectMany(
                        (KeyValuePair<SKPointI, HashSet<TileComponent>> comp) => comp.Value
                    ).ToHashSet();
                    if (graph.Claimee.Count > 0)
                    {
                        Player MostMeeple = graph.Claimee[0];
                        int MostMeepleCount = 0;
                        foreach (IGrouping<Player,Player> group in graph.Claimee.GroupBy(p => p))
                        {
                            if (group.Count() > MostMeepleCount) {
                                MostMeeple = group.Key;
                                MostMeepleCount = group.Count();
                            }
                        }
                        // grass tiles cannot ever return there meeple
                        if (graph.Type == ComponentsType.Grass) { continue; }
                        else if (graph.Type == ComponentsType.Abbey) { MostMeeple.Score += 9; } else
                        { MostMeeple.Score += tileComponents.Count * graph.Type.GetPoints(); }
                    }
                    foreach (Player player in graph.Claimee)
                    { player.Meeple++; }
                    foreach (TileComponent component in tileComponents )
                    { component.Claimee = null; }
                }
            }
        }
        private void Client_MessageRecived(object sender, Message msg)
        {
            switch (msg.Key)
            {
                case "Error":
                    MessageBox.Show("ERROR SERVER MISUNDERSTOOD THE DATA");
                    Close();
                    break;
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
                case "Scores":
                    int i = 0;
                    string scoreString = "";
                    foreach (string scoreStr in msg.Value.Substring(0,msg.Value.IndexOf('#')).Split(','))
                    { scoreString += ((Colours)i).ToString() + ": " + scoreStr + "\n"; }
                    int.TryParse(msg.Value.Substring(msg.Value.IndexOf('#')+1),out i);
                    scoreString += "\n Winner: " + ((Colours)i).ToString();
                    new Thread(new ThreadStart(delegate {
                        MessageBox.Show(scoreString, "Scores");
                        if (InvokeRequired) { Invoke(Close); }
                        else { Close(); }
                    })).Start();
                    break;
                default:
                    MessageBox.Show(
                        "Error Unknown Message from Server "+msg.Key+":"+msg.Value
                    );
                    Close();
                    break;
            }
            UpdateDrawing();
        }


        private void Hud_FinishTurnButton(object sender)
        {

            CarcasonneTileManager.CurrentOrientation = Orientation.North;
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
            ReturnMeeple(CarcasonneTileManager.LastTilePos);
            //generate the next tile or set the state to game over
            try { CarcasonneTileManager.GenerateNextTile(); }
            catch (InvalidOperationException)
            {
                CountPoints();
                Players.ForEach((Player pl) => pl.State = State.GameOver);
                Client.SendMessage(new("Score", localPlayer.Score.ToString()));
            }
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
            if (e.KeyCode == 13 && localPlayer.State == State.PlacingMeeple)
            // if enter pressed then simulate the finish turn button being pressed
            { Hud_FinishTurnButton(hud); }
            if (e.KeyCode >= 37 && e.KeyCode <= 40)
            {
                CarcasonneTileManager.CurrentOrientation = e.KeyCode switch
                {
                    /*UpArrow*/38 => Orientation.North,
                    /*RightArrow*/39 => Orientation.East,
                    /*DownArrow*/40 => Orientation.South,
                    /*LeftArrow*/37 => Orientation.West,
                    _ => throw new InvalidOperationException("Unreachable Code Reached. Your Memory is probably corrupt."),
                };
                tileLayer.Invalidate();
            }
            if (e.KeyCode == /*F2*/113)
            {
                //reset pan if f2 pressed
                tileLayer.Pan(new SKPoint(Width/2,Height/2) -tileLayer.Offset);
            }
            //if (e.KeyCode == /*F3*/114)
            //{
            //    Console.WriteLine("************");
            //    foreach (ComponentGraph cg in CarcasonneTileManager.Graph)
            //    {
            //        Console.WriteLine("===========");
            //        Console.WriteLine(CarcasonneTileManager.IsGraphComplete(cg));
            //        Console.WriteLine(cg.Type);
            //        foreach (Player p in cg.Claimee)
            //        { Console.WriteLine(p.Colour); }
            //        foreach (KeyValuePair<SKPointI, HashSet<TileComponent>> tcpair in cg.Components)
            //        {
            //            Console.WriteLine(tcpair.Key);
            //            foreach (TileComponent tc in tcpair.Value)
            //            {
            //                Console.WriteLine("\t" + tc.Type);
            //                Console.WriteLine("\t" + tc.Position);
            //            }
            //        }
            //    }
            //}
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