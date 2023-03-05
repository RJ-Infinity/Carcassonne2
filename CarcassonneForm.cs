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
        public CarcassonneForm()
        {
            InitializeComponent();

            List<TileDefinition> defaultTiles = TileManager.ParseJSONFile(File.ReadAllText(".\\Tiles.json"));

            CarcasonneTileManager = new TileManager(defaultTiles.ToList());
            //assuming that there are tiles in the tile pool
            CarcasonneTileManager.GenerateNextTile();

            bg = new layers.Background();
            hud = new layers.HUD(100);
            tileLayer = new layers.TileLayer();
            tileLayer.TileManager = CarcasonneTileManager;
            bg.AddLinkedLayer(tileLayer);
            bg.MouseDown += Bg_MouseDown;
            tileLayer.KeyDown += TileLayer_KeyDown;
            Layers.Add(bg);
            Layers.Add(tileLayer);
            Layers.Add(hud);
        }

        private void TileLayer_KeyDown(object sender, EventArgs_KeyDown e)
        {
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
        }

        private void Bg_MouseDown(object sender, EventArgs_Click e)
        {
            SKPoint position = bg.ScreenToWorld(e.Position);
            SKPointI positionIndex = new SKPointI(
                (int)Math.Floor(position.X / 100),
                (int)Math.Floor(position.Y / 100)
            );
            if (e.Button == PGL.MouseButtons.Left && CarcasonneTileManager.IsValidLocation(
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
            }
        }
    }
}