using PGL;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carcassonne2.layers
{
    public class TileLayer:PanAndZoomLayer
    {
        public override void OnDraw(EventArgs_Draw e)
        {
            using SKPaint paint = new SKPaint();
            e.Canvas.DrawCircle(
                WorldToScreen(new(0, 0)),
                (WorldToScreen(new(10, 0)) - WorldToScreen(new(0, 0))).X,
                paint
            );
            e.Canvas.DrawLine(
                new SKPoint(200,0),
                new SKPoint(200,e.Bounds.Height),
                paint
            );
            base.OnDraw(e);
            Console.WriteLine(Zoom);
        }
        // dont pass these events through to the background layer behind
        // else it will make panning and zooming double speed
        public override bool OnMouseDown(EventArgs_Click e)=>!base.OnMouseDown(e);
        public override bool OnMouseMove(EventArgs_MouseMove e)=>!base.OnMouseMove(e);
        public override bool OnMouseWheel(EventArgs_Scroll e)=>!base.OnMouseWheel(e);
    }
}
