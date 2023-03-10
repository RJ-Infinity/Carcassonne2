﻿using PGL;
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
        TileManager TileManager;
        Player Player;
        public TileLayer(TileManager tileManager, Player player)
        {
            TileManager = tileManager;
            Player = player;
        }

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
            //draw tiles=======================================================================
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
            //component highlight==============================================================
            if (
                position == TileManager.LastTilePos &&
                TileManager.ContainsTile(position) &&
                Player.State == State.PlacingMeeple
            )
            {
                SKPoint posMod = new(WorldMousePos.X % 100, WorldMousePos.Y % 100);
                if (WorldMousePos.X < 0) { posMod.X = 100 + posMod.X; }
                if (WorldMousePos.Y < 0) { posMod.Y = 100 + posMod.Y; }
                Tile tile = TileManager[position];

                SKPoint rotatedPoint = tile.Orientation switch
                {
                    Orientation.North => posMod,
                    Orientation.East => new SKPoint(posMod.Y, 99 - posMod.X),
                    Orientation.South => new SKPoint(99 - posMod.X, 99 - posMod.Y),
                    Orientation.West => new SKPoint(99 - posMod.Y, posMod.X),
                    _ => throw new InvalidOperationException("Invalid Orientation"),
                };

                TileComponent selectedComp = TileManager.GetComponentFromPosition(
                    TileManager.GetComponentPositionAtPos(rotatedPoint),
                    tile.Components
                );
                if (selectedComp.DoubleScore) { paint.Color = new SKColor(180, 0, 0); }
                else { paint.Color = TileManager.GetColour(selectedComp.Type); }
                paint.Color = paint.Color.WithAlpha(100);
                SKPoint centre = WorldToScreen(new SKPoint(position.X * 100 + 50, position.Y * 100 + 50));
                e.Canvas.RotateRadians(
                    ((int)TileManager[TileManager.LastTilePos].Orientation + 3) * 0.5f * (float)Math.PI,
                    centre.X,
                    centre.Y
                );
                e.Canvas.DrawPath(TileManager.GenerateSKPath(
                    WorldToScreen(SKRect.Create(new SKPoint(position.X * 100, position.Y * 100), new SKSize(99,99))),
                    selectedComp.Position
                ), paint);
                e.Canvas.ResetMatrix();
            }
            //tile valid highlight=============================================================
            if (Player.State == State.PlacingTile)
            {
                paint.Color = (TileManager.IsValidLocation(
                    position,
                    TileManager.CurrentOrientation,
                    TileManager.CurrentTile
                ) ? SKColors.Green : SKColors.Red).WithAlpha(100);
                e.Canvas.DrawRect(WorldToScreen(new SKRect(
                    position.X * 100,
                    position.Y * 100,
                    position.X * 100 + 99,
                    position.Y * 100 + 99
                )), paint);
            }
            //meeple===========================================================================
            foreach (KeyValuePair<SKPointI, Tile> tile in TileManager)
            {
                foreach (TileComponent tc in tile.Value.Components.ToList().FindAll(
                    e => e.Claimee == null
                ))
                {
                    //paint.Color = tc.Claimee.Colour;
                    paint.Color = SKColors.Blue;
                    SKRect meeplePos = TileManager.getComponentPositionMeepleRect(
                        TileManager.FindBestComponentPosition(tc.Position),
                        tile.Value.Orientation
                    );
                    meeplePos.Left += tile.Key.X * 100;
                    meeplePos.Top += tile.Key.Y * 100;
                    meeplePos.Right += tile.Key.X * 100;
                    meeplePos.Bottom += tile.Key.Y * 100;
                    meeplePos = WorldToScreen(meeplePos);
                    if (
                        meeplePos.Bottom > e.Bounds.Top &&
                        meeplePos.Top < e.Bounds.Bottom &&
                        meeplePos.Right > e.Bounds.Left &&
                        meeplePos.Left < e.Bounds.Right
                    ) { e.Canvas.DrawMeepel(meeplePos, paint); }
                }
            }
            //current tile=====================================================================
            if (Player.State == State.PlacingTile)
            {
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
            }
            base.OnDraw(e);
        }
        public override bool AllowPanStart(EventArgs_Click e) => e.Button != PGL.MouseButtons.Left;
    }
}
