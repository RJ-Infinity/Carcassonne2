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
            layers.Background bg = new layers.Background();
            bg.Tiles = defaultTiles;
            Layers.Add(bg);
            //Layers.Add(new layers.HUD());
        }
    }
}