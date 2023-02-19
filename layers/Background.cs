using PGL;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Carcassonne2.layers
{
    public class Background : PanAndZoomLayer
    {
        public Background()
        {
            MaxZoom = 3;
        }
        SKPoint MousePos = new SKPoint(0, 0);
        TileComponentDefinition getComponentFromPosition(ComponentPosition pos, TileComponentDefinition[] components)
        {
            foreach (TileComponentDefinition tileComp in components)
            {
                if (tileComp.Position.HasFlag(pos))
                {
                    return tileComp;
                }
            }
            return new TileComponentDefinition();
        }
        ComponentPosition getComponentPositionAtPos(SKPoint pos)
        {
            if (pos.X <= 33)
            {
                if (pos.Y <= 33)
                {
                    if (pos.X > pos.Y) { return ComponentPosition.NorthLeft; }
                    return ComponentPosition.WestRight;
                }
                if (pos.Y < 66) { return ComponentPosition.WestCentre; }
                if (pos.X > 99 - pos.Y) { return ComponentPosition.SouthRight; }
                return ComponentPosition.WestLeft;
            }
            if (pos.X <= 66)
            {
                if (pos.Y <= 33) { return ComponentPosition.NorthCentre; }
                if (pos.Y <= 66) { return ComponentPosition.Middle; }
                return ComponentPosition.SouthCentre;
            }
            if (pos.Y <= 33)
            {
                if (pos.X - 66 > 33 - pos.Y) { return ComponentPosition.EastLeft; }
                return ComponentPosition.NorthRight;
            }
            if (pos.Y <= 66) { return ComponentPosition.EastCentre; }
            if (pos.X > pos.Y) { return ComponentPosition.EastRight; }
            return ComponentPosition.SouthLeft;
        }
        public TileDefinition[] Tiles = { };
        int i = 0;
        public override bool OnMouseDown(EventArgs_Click e)
        {
            if (e.Button == MouseButtons.Left)
            {
                i++;
                if (i>= Tiles.Length)
                {
                    i = 0;
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                i--;
                if (i < 0)
                {
                    i = Tiles.Length - 1;
                }
            }
            Invalidate();
            return base.OnMouseDown(e);
        }
        public override bool OnMouseMove(EventArgs_MouseMove e)
        {
            MousePos = e.Position;
            Invalidate();
            return base.OnMouseMove(e);
        }
        public override void OnDraw(EventArgs_Draw e)
        {
            // Create a diagonal gradient fill from Blue to Green to use as the background
            SKPoint topLeft = new(e.Bounds.Left, e.Bounds.Top);
            SKPoint bottomRight = new(e.Bounds.Right, e.Bounds.Bottom);
            SKColor[] gradColors = new[] { SKColors.LightBlue, SKColors.LightGreen };

            using SKPaint gradientPaint = new();
            using SKShader shader = SKShader.CreateLinearGradient(topLeft, bottomRight, gradColors, SKShaderTileMode.Clamp);
            gradientPaint.Shader = shader;
            gradientPaint.Style = SKPaintStyle.Fill;
            e.Canvas.DrawRect(e.Bounds, gradientPaint);

            using SKPaint paint = new();
            paint.Color = SKColors.Gray; // Very dark gray
            paint.Style = SKPaintStyle.Stroke;
            paint.StrokeWidth = 1;
            //e.Canvas.DrawLine(new(0,0), new(100,100), paint);
            // Draw the Horizontal Grid Lines
            int i = ((int)Offset.Y % (int)(100 * Zoom));
            while (i < e.Bounds.Height)
            {
                SKPoint leftPoint = new(e.Bounds.Left, i);
                SKPoint rightPoint = new(e.Bounds.Right, i);

                e.Canvas.DrawLine(leftPoint, rightPoint, paint);

                i += (int)(100 * Zoom);
            }

            // Draw the Vertical Grid Lines
            i = ((int)Offset.X % (int)(100 * Zoom));
            while (i < e.Bounds.Width)
            {
                SKPoint topPoint = new(i, e.Bounds.Top);
                SKPoint bottomPoint = new(i, e.Bounds.Bottom);

                e.Canvas.DrawLine(topPoint, bottomPoint, paint);

                i += (int)(100 * Zoom);
            }

            paint.Style = SKPaintStyle.Fill;

            e.Canvas.DrawCircle(
                WorldToScreen(new(0, 0)),
                (WorldToScreen(new(10, 0))-WorldToScreen(new(0, 0))).X,
                paint
            );
            ///////////////////////////////////////////////////////////
            //using SKPaint paint = new SKPaint();
            //e.Canvas.DrawImage(Tiles[i].Texture, new SKPoint(0, 0));
            //TileComponent selectedComp = getComponentFromPosition(
            //    getComponentPositionAtPos(MousePos),
            //    Tiles[i].Components
            //);
            //if (selectedComp.DoubleScore)
            //{
            //    paint.Color = new SKColor(180, 0, 0);
            //}
            //else
            //{
            //    paint.Color = GetColour(selectedComp.Type);
            //}
            //paint.Color = paint.Color.WithAlpha(100);
            //e.Canvas.DrawPath(GenerateSKPath(selectedComp.Position),paint);
            ////e.Canvas.DrawPath(GenerateSKPath(getComponentPositionAtPos(MousePos)),paint);
            //paint.Color = paint.Color.WithAlpha(255);
            //e.Canvas.DrawText((i+1).ToString(), new SKPoint(10, 110), paint);
            base.OnDraw(e);
        }
        public override bool AllowZoom(EventArgs_Scroll e)
        {
            return true;
        }
        SKPoint point = new(0,0);
        public override bool OnMouseWheel(EventArgs_Scroll e)
        {
            //point = e.Position;
            return base.OnMouseWheel(e);
        }
    }
}
