using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PGL;
using SkiaSharp;

namespace Carcassonne2.layers
{
    public class HUD: Layer
    {
        public override bool IsInLayer(SKPoint p)
        {
            return true;
        }
        public override void OnDraw(EventArgs_Draw e)
        {
            using SKPaint paint = new SKPaint();
            e.Canvas.Clear();
            paint.Color = SKColors.Blue;
            e.Canvas.DrawMeepel(new SKRect(100, 100, 200, 220), paint);
        }
        public override bool OnMouseDown(EventArgs_Click p)
        {
            Console.WriteLine(p.Position);
            base.OnMouseDown(p);
            return true;
        }
    }
}
