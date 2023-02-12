using PGL;
using SkiaSharp;
using SkiaSharp.Views.Desktop;

namespace Carcassonne2
{
    public partial class CarcassonneForm : PGLForm
    {
        public CarcassonneForm()
        {
            InitializeComponent();
            Layers.Add(new layers.Background());
            Layers.Add(new layers.HUD());
            Layers.Add(new layers.HUD());
            Layers.Add(new layers.HUD());
            Layers.Add(new layers.HUD());
            Layers.Add(new layers.HUD());
        }
    }
}