using PGL;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carcassonne2.layers
{
    public class TileLayer:PanAndZoomLayer
    {
        SKPoint MousePos = new SKPoint(0, 0);
        public TileManager TileManager;

        public override bool OnMouseMove(EventArgs_MouseMove e)
        {
            MousePos = e.Position;
            Invalidate();
            return base.OnMouseMove(e);
        }
        public override void OnDraw(EventArgs_Draw e)
        {
            SKPoint WorldMousePos = ScreenToWorld(MousePos);
            using SKPaint paint = new SKPaint();
            paint.IsAntialias = true;
            foreach (KeyValuePair<SKPointI, Tile> tile in TileManager)
            {
                SKPoint centre = WorldToScreen(new SKPoint(
                    tile.Key.X * 100 + 50,
                    tile.Key.Y * 100 + 50
                ));
                e.Canvas.RotateRadians(
                    ((int)tile.Value.Orientation + 3) * 0.5f * (float)Math.PI,
                    centre.X,
                    centre.Y
                );
                e.Canvas.DrawImage(tile.Value.Texture, WorldToScreen(SKRect.Create(
                    new SKPoint(tile.Key.X * 100, tile.Key.Y * 100),
                    new SKSize(99, 99)
                )));
                e.Canvas.ResetMatrix();
            }
            SKPointI position = new(
                (int)Math.Floor(WorldMousePos.X/100),
                (int)Math.Floor(WorldMousePos.Y/100)
            );
            if (TileManager.ContainsTile(position))
            {
                SKPoint posMod = new(WorldMousePos.X % 100, WorldMousePos.Y % 100);
                if (WorldMousePos.X < 0) { posMod.X = 100 + posMod.X; }
                if (WorldMousePos.Y < 0) { posMod.Y = 100 + posMod.Y; }
                Tile tile = TileManager[position];
                TileComponent selectedComp = TileManager.GetComponentFromPosition(
                    TileManager.GetComponentPositionAtPos(posMod),
                    tile.Components
                );
                if (selectedComp.DoubleScore) { paint.Color = new SKColor(180, 0, 0); }
                else { paint.Color = TileManager.GetColour(selectedComp.Type); }
                paint.Color = paint.Color.WithAlpha(100);
                e.Canvas.DrawPath(TileManager.GenerateSKPath(
                    WorldToScreen(SKRect.Create(new SKPoint(position.X * 100, position.Y * 100), new SKSize(99,99))),
                    selectedComp.Position
                ), paint);
                //e.Canvas.DrawPath(GenerateSKPath(getComponentPositionAtPos(MousePos)),paint);
            }

            paint.Color = (TileManager.IsValidLocation(
                position,
                TileManager.CurrentOrientation,
                TileManager.CurrentTile
            ) ? SKColors.Green: SKColors.Red).WithAlpha(100);
            e.Canvas.DrawRect(WorldToScreen(new SKRect(
                position.X*100,
                position.Y*100,
                position.X*100+99,
                position.Y*100+99
            )), paint);
            e.Canvas.RotateRadians(((int)TileManager.CurrentOrientation + 3) * 0.5f * (float)Math.PI,MousePos.X, MousePos.Y);
            paint.Color = SKColors.Blue;
            e.Canvas.DrawImage(TileManager.CurrentTile.Texture, WorldToScreen(SKRect.Create(
                new SKPoint(WorldMousePos.X - 50, WorldMousePos.Y - 50),
                new SKSize(99, 99)
            )));
            e.Canvas.DrawCircle(WorldToScreen(new SKPoint(
                WorldMousePos.X,
                WorldMousePos.Y - 50
            )), 10*Zoom, paint);
            e.Canvas.ResetMatrix();
            //TileManager.CurrentTile
            base.OnDraw(e);
        }
    }
}
