using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RJGL;
using SkiaSharp;

namespace Carcassonne2.layers
{
    public class HUD: Layer
    {
        public readonly float Height;
        public int Points = 56;
        public float Padding = 10;
        public bool FinishTurnButtonHovered = false;
        private SKRect finishTurnButton;
        private Orientation hoveredOrientationButton = Orientation.None;
        private Player Player;
        public HUD(float height,Player player)
        {
            Player = player;
            Height = height;
            finishTurnButton = new SKRect(200, Padding, 400 - Padding, Height - Padding);
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
                Padding,
                Padding,
                0.9f * (Height - Padding),
                Height - Padding
            ), paint);
            //meeple count=====================================================
            paint.TextSize = 16;
            paint.TextAlign = SKTextAlign.Right;
            e.Canvas.DrawText(Player.Meeple.ToString(), new SKPoint(
                0.9f * (Height - 2 * Padding) + Padding,
                Padding + paint.TextSize/2
            ), paint);
            //score text=======================================================
            paint.Color = SKColors.Black;
            paint.TextSize = (float)(Height/2 - Padding * 1.5);
            paint.TextAlign = SKTextAlign.Left;
            e.Canvas.DrawText("Score", new SKPoint(
                0.9f * (Height - Padding) + Padding,
                Padding + paint.TextSize * 0.75f
            ), paint);
            //score============================================================
            e.Canvas.DrawText(Points.ToString(), new SKPoint(
                0.9f * (Height - Padding) + Padding,
                Height/2 + Padding + paint.TextSize * 0.75f
            ), paint);
            //finish turn button===============================================
            paint.Color = Player.State==State.PlacingMeeple && FinishTurnButtonHovered ? SKColors.DarkGray:SKColors.Black;
            e.Canvas.DrawRect(finishTurnButton, paint);
            paint.Color = Player.State == State.PlacingMeeple ? SKColors.White:SKColors.LightGray;
            e.Canvas.DrawRect(SKRect.Inflate(finishTurnButton,-1,-1),paint);
            paint.Color = Player.State == State.PlacingMeeple && FinishTurnButtonHovered ? SKColors.DarkGray : SKColors.Black;
            e.Canvas.DrawText("Finish Turn", new SKPoint(
                205,
                Padding + paint.TextSize * 0.75f + (Height - Padding - paint.TextSize) / 2
            ), paint);
            //Arrow Buttons====================================================
            paint.Style = SKPaintStyle.Stroke;
            paint.Color = SKColors.Black;
            SKPath path = new SKPath();
            //======UP background==============================================
            path.MoveTo(new(400 + 0.5f * Padding, Padding));
            path.LineTo(new(400 + 0.5f * Height - Padding, 0.5f * (Height - Padding)));
            path.LineTo(new(400 + Height - 2.5f * Padding, Padding));
            path.Close();
            //======DOWN background============================================
            path.MoveTo(new(400 + 0.5f * Padding, Height - Padding));
            path.LineTo(new(400 + 0.5f * Height - Padding, 0.5f * (Height + Padding)));
            path.LineTo(new(400 + Height - 2.5f * Padding, Height - Padding));
            path.Close();
            //======LEFT background============================================
            path.MoveTo(new(400, 1.5f * Padding));
            path.LineTo(new(400 + 0.5f * Height - 1.5f * Padding, 0.5f * Height));
            path.LineTo(new(400, Height - 1.5f * Padding));
            path.Close();
            //======RIGHT background===========================================
            path.MoveTo(new(400 + Height - 2 * Padding, 1.5f * Padding));
            path.LineTo(new(400 + 0.5f * (Height - Padding), 0.5f * Height));
            path.LineTo(new(400 + Height - 2 * Padding, Height - 1.5f * Padding));
            path.Close();

            e.Canvas.DrawPath(path, paint);

            paint.Style = SKPaintStyle.Fill;

            //======Up Arrow===================================================
            paint.Color = hoveredOrientationButton==Orientation.North?SKColors.Black:SKColors.Red;
            path = new SKPath();
            path.MoveTo(new(400 + 0.5f * Height - Padding, 1.5f * Padding));
            path.LineTo(new(400 + 0.5f * Height, 2.5f * Padding));
            path.LineTo(new(400 + 0.5f * (Height - Padding), 2.5f * Padding));
            path.LineTo(new(400 + 0.5f * (Height - Padding), 0.5f * Height - 1.5f * Padding));
            path.LineTo(new(400 + 0.5f * Height - 1.5f * Padding, 0.5f * Height - 1.5f * Padding));
            path.LineTo(new(400 + 0.5f * Height - 1.5f * Padding, 2.5f * Padding));
            path.LineTo(new(400 + 0.5f * Height - 2 * Padding, 2.5f * Padding));
            path.Close();
            if (Player.State != State.PlacingTile){paint.Color = SKColors.LightGray;}
            e.Canvas.DrawPath(path, paint);
            //======Down Arrow=================================================
            paint.Color = hoveredOrientationButton==Orientation.South?SKColors.Black:SKColors.Red;
            path = new SKPath();
            path.MoveTo(new(400 + 0.5f * Height - Padding, Height - 1.5f * Padding));
            path.LineTo(new(400 + 0.5f * Height, Height - 2.5f * Padding));
            path.LineTo(new(400 + 0.5f * (Height - Padding), Height - 2.5f * Padding));
            path.LineTo(new(400 + 0.5f * (Height - Padding), Height - 0.5f * Height + 1.5f * Padding));
            path.LineTo(new(400 + 0.5f * Height - 1.5f * Padding, Height - 0.5f * Height + 1.5f * Padding));
            path.LineTo(new(400 + 0.5f * Height - 1.5f * Padding, Height - 2.5f * Padding));
            path.LineTo(new(400 + 0.5f * Height - 2 * Padding, Height - 2.5f * Padding));
            path.Close();
            if (Player.State != State.PlacingTile) {paint.Color = SKColors.LightGray;}
            e.Canvas.DrawPath(path, paint);
            //======Left Arrow=================================================
            paint.Color = hoveredOrientationButton==Orientation.West?SKColors.Black:SKColors.Red;
            path = new SKPath();
            path.MoveTo(new(400 + 0.5f * Padding, 0.5f * Height));
            path.LineTo(new(400 + 1.5f * Padding, 0.5f * Height - Padding));
            path.LineTo(new(400 + 1.5f * Padding, 0.5f * (Height - Padding)));
            path.LineTo(new(400 + 0.5f * Height - 2.5f * Padding, 0.5f * (Height - Padding)));
            path.LineTo(new(400 + 0.5f * Height - 2.5f * Padding, 0.5f * (Height + Padding)));
            path.LineTo(new(400 + 1.5f * Padding, 0.5f * (Height + Padding)));
            path.LineTo(new(400 + 1.5f * Padding, 0.5f * Height + Padding));
            path.Close();
            if (Player.State != State.PlacingTile) {paint.Color = SKColors.LightGray;}
            e.Canvas.DrawPath(path, paint);
            //======Right Arrow================================================
            paint.Color = hoveredOrientationButton==Orientation.East ? SKColors.Black:SKColors.Red;
            path = new SKPath();
            path.MoveTo(new(400 + Height - 2.5f * Padding, 0.5f * Height));
            path.LineTo(new(400 + Height - 3.5f * Padding, 0.5f * Height - Padding));
            path.LineTo(new(400 + Height - 3.5f * Padding, 0.5f * (Height - Padding)));
            path.LineTo(new(400 + 0.5f * (Height + Padding), 0.5f * (Height - Padding)));
            path.LineTo(new(400 + 0.5f * (Height + Padding), 0.5f * (Height + Padding)));
            path.LineTo(new(400 + Height - 3.5f * Padding, 0.5f * (Height + Padding)));
            path.LineTo(new(400 + Height - 3.5f * Padding, 0.5f * Height + Padding));
            path.Close();
            if (Player.State != State.PlacingTile) {paint.Color = SKColors.LightGray;}
            e.Canvas.DrawPath(path, paint);

        }
        public override bool OnMouseMove(EventArgs_MouseMove e)
        {
            bool newFinishTurnButtonHovered = finishTurnButton.Contains(e.Position);
            if (newFinishTurnButtonHovered != FinishTurnButtonHovered)
            {
                FinishTurnButtonHovered = newFinishTurnButtonHovered;
            }
            float x = e.Position.X - 400;
            float y = e.Position.Y - Padding;
            float size = Height - 2 * Padding;
            hoveredOrientationButton = Orientation.None;
            if (x>0 && x<size && y>0 && y<size) { if (x > y) {
                if (size - x > y) { hoveredOrientationButton = Orientation.North; }
                else { hoveredOrientationButton = Orientation.East; }
            } else {
                if (size - x > y) { hoveredOrientationButton = Orientation.West; }
                else { hoveredOrientationButton = Orientation.South; }
            } }
            Invalidate();
            return base.OnMouseMove(e);
        }

        public delegate void OrientationButtonHandler(object sender, EventArgs_OrientationButton e);
        public event OrientationButtonHandler OrientationButton;
        public delegate void FinishTurnButtonHandler(object sender);
        public event FinishTurnButtonHandler FinishTurnButton;
        public override bool OnMouseDown(EventArgs_Click p)
        {
            if (Player.State == State.PlacingTile)
            {
                switch (hoveredOrientationButton)
                {
                    case Orientation.North:
                    case Orientation.East:
                    case Orientation.South:
                    case Orientation.West:{
                        OrientationButton?.Invoke(
                            this,new EventArgs_OrientationButton(hoveredOrientationButton)
                        );
                    }break;
                    case Orientation.None:break;
                    default:throw new InvalidOperationException("Enum is in an invalid state");
                }
            }
            if (FinishTurnButtonHovered && Player.State == State.PlacingMeeple)
            { FinishTurnButton?.Invoke(this); }
            base.OnMouseDown(p);
            return true;
        }
        public override void OnMouseLeave(EventArgs_MouseMove e)
        {
            FinishTurnButtonHovered = false;
            hoveredOrientationButton = Orientation.None;
            Invalidate();
            base.OnMouseLeave(e);
        }
    }
    public class EventArgs_OrientationButton
    {
        public Orientation Orientation;
        public EventArgs_OrientationButton(Orientation orientation)
        {
            Orientation = orientation;
        }
    }
}
