using PGL;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using System.Reflection;
using RJJSON;

namespace Carcassonne2
{
    public partial class CarcassonneForm : PGLForm
    {
        public CarcassonneForm()
        {
            InitializeComponent();

            TileDefinition[] defaultTiles = TileDefinitionHelper.parseJSONFile(File.ReadAllText(".\\Tiles.json"));

            TileManager tileManager = new TileManager();

            layers.Background bg = new layers.Background();
            bg.Tiles = defaultTiles;
            layers.HUD hud = new layers.HUD(100);
            layers.TileLayer tileLayer = new layers.TileLayer();
            bg.AddLinkedLayer(tileLayer);
            Layers.Add(bg);
            Layers.Add(tileLayer);
            Layers.Add(hud);
        }
    }
}