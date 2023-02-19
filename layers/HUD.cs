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
        public readonly float Height;
        int Meeple = 10;
        int Points = 56;
        public HUD(float height)
        {
            Height = 100;
        }
        public override bool IsInLayer(SKPoint p) => p.Y < Height;
        public override void OnDraw(EventArgs_Draw e)
        {
            float padding = 10;
            using SKPaint paint = new SKPaint();
            paint.IsAntialias = true;

            //background=======================================================
            paint.Color = SKColors.White;
            e.Canvas.DrawRect(new SKRect(0, 0, e.Bounds.Width, Height), paint);
            //meeple===========================================================
            paint.Color = SKColors.Blue;
            e.Canvas.DrawMeepel(new SKRect(
                padding,
                padding,
                0.9f * (Height - padding),
                Height - padding
            ), paint);
            //meeple count=====================================================
            paint.TextSize = 16;
            paint.TextAlign = SKTextAlign.Right;
            e.Canvas.DrawText(Meeple.ToString(), new SKPoint(
                0.9f * (Height - 2 * padding) + padding,
                padding + paint.TextSize/2
            ), paint);
            //score text=======================================================
            paint.Color = SKColors.Black;
            paint.TextSize = (float)(Height/2 - padding * 1.5);
            paint.TextAlign = SKTextAlign.Left;
            e.Canvas.DrawText("Score", new SKPoint(
                0.9f * (Height - padding) + padding,
                padding + paint.TextSize * 0.75f
            ), paint);
            //score============================================================
            e.Canvas.DrawText(Points.ToString(), new SKPoint(
                0.9f * (Height - padding) + padding,
                Height/2 + padding + paint.TextSize * 0.75f
            ), paint);
        }
        public override bool OnMouseDown(EventArgs_Click p)
        {
            Console.WriteLine(p.Position);
            base.OnMouseDown(p);
            return true;
        }
    }
}
