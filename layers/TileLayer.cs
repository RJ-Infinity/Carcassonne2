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
            foreach (KeyValuePair<SKPointI, Tile> tile in TileManager)
            {
                e.Canvas.DrawImage(tile.Value.Texture, WorldToScreen(SKRect.Create(
                    new SKPoint(tile.Key.X * 100, tile.Key.Y * 100),
                    new SKSize(99, 99)
                )));
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
            base.OnDraw(e);
        }
    }
}
