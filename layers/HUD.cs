using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PGL;
using SkiaSharp;

namespace Carcassonne2.layers
{
    public class HUD: Layer
    {
        public readonly float Height;
        public int Meeple = 10;
        public int Points = 56;
        public float padding = 10;
        public bool showFinishTurn = false;
        public bool finishTurnButtonHovered = false;
        SKRect finishTurnButton;
        public HUD(float height)
        {
            Height = 100;
            finishTurnButton = new SKRect(200, padding, 400 - padding, Height - padding);
        }
        public override bool IsInLayer(SKPoint p) => p.Y < Height;
        public override void OnDraw(EventArgs_Draw e)
        {
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
            //finish turn button===============================================
            paint.Color = showFinishTurn && finishTurnButtonHovered ? SKColors.DarkGray:SKColors.Black;
            e.Canvas.DrawRect(finishTurnButton, paint);
            paint.Color = showFinishTurn?SKColors.White:SKColors.LightGray;
            e.Canvas.DrawRect(SKRect.Inflate(finishTurnButton,-1,-1),paint);
            paint.Color = SKColors.Black;
            e.Canvas.DrawText("Finish Turn", new SKPoint(
                205,
                padding + paint.TextSize * 0.75f + (Height - padding - paint.TextSize) / 2
            ), paint);
            //Arrow Buttons====================================================
            paint.Style = SKPaintStyle.Stroke;
            SKPath path = new SKPath();
            //======UP=========================================================
            path.MoveTo(new(400 + 0.5f * padding, padding));
            path.LineTo(new(400 + 0.5f * Height - padding, 0.5f * (Height - padding)));
            path.LineTo(new(400 + Height - 2.5f * padding, padding));
            path.Close();
            //======DOWN=======================================================
            path.MoveTo(new(400 + 0.5f * padding, Height - padding));
            path.LineTo(new(400 + 0.5f * Height - padding, 0.5f * (Height + padding)));
            path.LineTo(new(400 + Height - 2.5f * padding, Height - padding));
            path.Close();
            //======LEFT=======================================================
            path.MoveTo(new(400, 1.5f * padding));
            path.LineTo(new(400 + 0.5f * Height - 1.5f * padding, 0.5f * Height));
            path.LineTo(new(400, Height - 1.5f * padding));
            path.Close();
            //======RIGHT======================================================
            path.MoveTo(new(400 + Height - 2 * padding, 1.5f * padding));
            path.LineTo(new(400 + 0.5f * (Height - padding), 0.5f * Height));
            path.LineTo(new(400 + Height - 2 * padding, Height - 1.5f * padding));
            path.Close();

            e.Canvas.DrawPath(path, paint);
        }
        public override bool OnMouseMove(EventArgs_MouseMove e)
        {
            bool newFinishTurnButtonHovered = finishTurnButton.Contains(e.Position);
            if (newFinishTurnButtonHovered != finishTurnButtonHovered)
            {
                finishTurnButtonHovered = newFinishTurnButtonHovered;
                Invalidate();
            }
            return base.OnMouseMove(e);
        }
        public override bool OnMouseDown(EventArgs_Click p)
        {
            base.OnMouseDown(p);
            return true;
        }
    }
}
