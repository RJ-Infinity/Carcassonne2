using PGL;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using System.Reflection;
using RJJSON;

namespace Carcassonne2
{
    public partial class CarcassonneForm : PGLForm
    {
        TileManager CarcasonneTileManager;
        public CarcassonneForm()
        {
            InitializeComponent();

            List<TileDefinition> defaultTiles = TileManager.ParseJSONFile(File.ReadAllText(".\\Tiles.json"));

            CarcasonneTileManager = new TileManager(defaultTiles.ToList());
            //tileManager[0,0] = new Tile()

            layers.Background bg = new layers.Background();
            layers.HUD hud = new layers.HUD(100);
            layers.TileLayer tileLayer = new layers.TileLayer();
            tileLayer.TileManager = CarcasonneTileManager;
            bg.AddLinkedLayer(tileLayer);
            bg.MouseDown += Bg_MouseDown;
            bg.KeyDown += Bg_KeyDown;
            Layers.Add(bg);
            Layers.Add(tileLayer);
            Layers.Add(hud);
        }

        private void Bg_KeyDown(object sender, EventArgs_KeyDown e)
        {
            Console.WriteLine(e.KeyCode);
        }

        private void Bg_MouseDown(object sender, EventArgs_Click e)
        {
            if (e.Button == PGL.MouseButtons.Left)
            {
                layers.Background bg = (layers.Background)sender;
                try{CarcasonneTileManager.GenerateNextTile();}
                catch(InvalidOperationException)
                {
                    Console.WriteLine("REGENERATEING STACK");
                    CarcasonneTileManager.GenerateTilePool();
                    CarcasonneTileManager.GenerateNextTile();
                }
                SKPoint position = bg.ScreenToWorld(e.Position);
                CarcasonneTileManager[
                    (int)Math.Floor(position.X / 100),
                    (int)Math.Floor(position.Y / 100)
                ] = new Tile(CarcasonneTileManager.CurrentTile, Orientation.North);
            }
        }
    }
}