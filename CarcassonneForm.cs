using PGL;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using System.Reflection;

namespace Carcassonne2
{
    public partial class CarcassonneForm : PGLForm
    {
        TileManager CarcasonneTileManager;
        layers.Background bg;
        layers.HUD hud;
        layers.TileLayer tileLayer;
        Player localPlayer;
        public CarcassonneForm()
        {
            InitializeComponent();

            localPlayer = new Player(0, SKColors.Blue);

            localPlayer.StateChanged += LocalPlayer_StateChanged;

            List<TileDefinition> defaultTiles = TileDefinition.ParseJSONFile(
                File.ReadAllText(".\\Tiles.json")
            );

            CarcasonneTileManager = new TileManager(defaultTiles);
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
            tileLayer = new layers.TileLayer(CarcasonneTileManager, localPlayer);
            bg.AddLinkedLayer(tileLayer);
            bg.MouseDown += Bg_MouseDown;
            tileLayer.KeyDown += TileLayer_KeyDown;
            tileLayer.Pan(new SKPoint(Width / 2, Height / 2));
            Layers.Add(bg);
            Layers.Add(tileLayer);
            Layers.Add(hud);
            localPlayer.State = State.PlacingTile;
        }

        private void LocalPlayer_StateChanged(object sender)
        {
            hud.Invalidate();
            tileLayer.Invalidate();
        }

        private void Hud_OrientationButton(object sender, layers.EventArgs_OrientationButton e) => CarcasonneTileManager.CurrentOrientation = e.Orientation;

        private void TileLayer_KeyDown(object sender, EventArgs_KeyDown e)
        {
            CarcasonneTileManager.LastTilePos = new(17, 5);
            Console.WriteLine(e.KeyCode);
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
                localPlayer.State == State.PlacingMeeple &&
                tileLayer.Position == CarcasonneTileManager.LastTilePos &&
                tileLayer.GetSelectedComp()
            )
            {
                tileLayer.SelectedComp.Claimee = localPlayer;
            }
            SKPoint position = bg.ScreenToWorld(e.Position);
            SKPointI positionIndex = new SKPointI(
                (int)Math.Floor(position.X / 100),
                (int)Math.Floor(position.Y / 100)
            );
            if (localPlayer.State == State.PlacingTile && e.Button == PGL.MouseButtons.Left && CarcasonneTileManager.IsValidLocation(
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
    }
}