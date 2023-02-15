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
            MaxZoom = 2;
        }
        SKColor GetColour(ComponentsType type)
        {
            switch (type)
            {
                case ComponentsType.Grass:
                    return new SKColor(0, 255, 0);
                case ComponentsType.Town:
                    return new SKColor(255, 0, 0);
                case ComponentsType.Road:
                    return new SKColor(255, 255, 255);
                case ComponentsType.Abbey:
                    return new SKColor(255, 255, 0);
                default:
                    //TODO: better error handling
                    throw new ArgumentException();
            }
        }
        SKPath GenerateSKPath(ComponentPosition pos)
        {
            SKPath path = new SKPath();
            if (pos.HasFlag(ComponentPosition.NorthLeft))
            { path.AddPoly(new SKPoint[] { new(33, 33), new(0, 0), new(33, 0) }); }
            if (pos.HasFlag(ComponentPosition.NorthCentre))
            { path.AddPoly(new SKPoint[] { new(33, 33), new(66, 33), new(66, 0), new(33, 0) }); }
            if (pos.HasFlag(ComponentPosition.NorthRight))
            { path.AddPoly(new SKPoint[] { new(66, 33), new(66, 0), new(99, 0) }); }
            if (pos.HasFlag(ComponentPosition.EastLeft))
            { path.AddPoly(new SKPoint[] { new(66, 33), new(99, 0), new(99, 33) }); }
            if (pos.HasFlag(ComponentPosition.EastCentre))
            { path.AddPoly(new SKPoint[] { new(66, 33), new(99, 33), new(99, 66), new(66, 66) }); }
            if (pos.HasFlag(ComponentPosition.EastRight))
            { path.AddPoly(new SKPoint[] { new(66, 66), new(99, 99), new(99, 66) }); }
            if (pos.HasFlag(ComponentPosition.SouthLeft))
            { path.AddPoly(new SKPoint[] { new(66, 66), new(99, 99), new(66, 99) }); }
            if (pos.HasFlag(ComponentPosition.SouthCentre))
            { path.AddPoly(new SKPoint[] { new(33, 66), new(66, 66), new(66, 99), new(33, 99) }); }
            if (pos.HasFlag(ComponentPosition.SouthRight))
            { path.AddPoly(new SKPoint[] { new(33, 66), new(33, 99), new(0, 99) }); }
            if (pos.HasFlag(ComponentPosition.WestLeft))
            { path.AddPoly(new SKPoint[] { new(33, 66), new(0, 66), new(0, 99) }); }
            if (pos.HasFlag(ComponentPosition.WestCentre))
            { path.AddPoly(new SKPoint[] { new(33, 33), new(33, 66), new(0, 66), new(0, 33) }); }
            if (pos.HasFlag(ComponentPosition.WestRight))
            { path.AddPoly(new SKPoint[] { new(33, 33), new(0, 33), new(0, 0) }); }
            if (pos.HasFlag(ComponentPosition.Middle))
            { path.AddPoly(new SKPoint[] { new(33, 33), new(66, 33), new(66, 66), new(33, 66) }); }
            return path;
        }
        SKPoint MousePos = new SKPoint(0, 0);
        TileComponent getComponentFromPosition(ComponentPosition pos, TileComponent[] components)
        {
            foreach (TileComponent tileComp in components)
            {
                if (tileComp.Position.HasFlag(pos))
                {
                    return tileComp;
                }
            }
            return new TileComponent();
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
            using SKPaint paint = new SKPaint();
            
            //foreach (TileComponent tileComp in Tiles[i].Components)
            //{
            //    if (tileComp.DoubleScore)
            //    {
            //        paint.Color = new SKColor(180, 0, 0);
            //    }
            //    else
            //    {
            //        paint.Color = GetColour(tileComp.Type);
            //    }
            //    e.Canvas.DrawPath(GenerateSKPath(tileComp.Position),paint);
            //}
            e.Canvas.DrawImage(Tiles[i].Texture, new SKPoint(0, 0));
            TileComponent selectedComp = getComponentFromPosition(
                getComponentPositionAtPos(
                    MousePos
                ),
                Tiles[i].Components
            );
            if (selectedComp.DoubleScore)
            {
                paint.Color = new SKColor(180, 0, 0);
            }
            else
            {
                paint.Color = GetColour(selectedComp.Type);
            }
            paint.Color = paint.Color.WithAlpha(100);
            e.Canvas.DrawPath(GenerateSKPath(selectedComp.Position),paint);
            //e.Canvas.DrawPath(GenerateSKPath(getComponentPositionAtPos(MousePos)),paint);
            paint.Color = paint.Color.WithAlpha(255);
            e.Canvas.DrawText((i+1).ToString(), new SKPoint(10, 110), paint);
            base.OnDraw(e);
        }
    }
}
