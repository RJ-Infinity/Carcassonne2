using RJGL;
using SkiaSharp;

namespace Carcassonne2.layers
{
    public class TileLayer:PanAndZoomLayer
    {
        SKPoint WorldMousePos = new SKPoint(0, 0);
        TileManager TileManager;
        Player Player;
        public TileComponent? SelectedComp;
        public SKPointI Position = new SKPointI(0,0);
        public TileLayer(TileManager tileManager, Player player)
        {
            TileManager = tileManager;
            Player = player;
        }

        public override bool OnMouseMove(EventArgs_MouseMove e)
        {
            WorldMousePos = ScreenToWorld(e.Position);
            Position = new(
                (int)Math.Floor(WorldMousePos.X / 100),
                (int)Math.Floor(WorldMousePos.Y / 100)
            );
            Invalidate();
            return base.OnMouseMove(e);
        }
        public bool GetSelectedComp()
        {
            SKPoint posMod = new(WorldMousePos.X % 100, WorldMousePos.Y % 100);
            if (WorldMousePos.X < 0) { posMod.X = 100 + posMod.X; }
            if (WorldMousePos.Y < 0) { posMod.Y = 100 + posMod.Y; }
            if (TileManager.ContainsTile(Position))
            {
                SKPoint rotatedPoint = TileManager[Position].Orientation switch
                {
                    Orientation.North => posMod,
                    Orientation.East => new SKPoint(posMod.Y, 99 - posMod.X),
                    Orientation.South => new SKPoint(99 - posMod.X, 99 - posMod.Y),
                    Orientation.West => new SKPoint(99 - posMod.Y, posMod.X),
                    _ => throw new InvalidOperationException("Invalid Orientation"),
                };
                SelectedComp = ComponentPositionEx
                .GetComponentPositionAtPos(rotatedPoint)
                .GetComponentFromPosition(TileManager[Position].Components);
                return true;
            }
            SelectedComp = null;
            return false;
        }
        public override void OnDraw(EventArgs_Draw e)
        {
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
            //component highlight==============================================================
            if (
                Position == TileManager.LastTilePos &&
                TileManager.ContainsTile(Position) &&
                Player.State == State.PlacingMeeple
            )
            {
                GetSelectedComp();
                if (SelectedComp.DoubleScore) { paint.Color = new SKColor(180, 0, 0); }
                else { paint.Color = SelectedComp.Type.GetColour(); }
                paint.Color = paint.Color.WithAlpha(100);
                SKPoint centre = WorldToScreen(new SKPoint(Position.X * 100 + 50, Position.Y * 100 + 50));
                e.Canvas.RotateRadians(
                    ((int)TileManager[TileManager.LastTilePos].Orientation + 3) * 0.5f * (float)Math.PI,
                    centre.X,
                    centre.Y
                );
                e.Canvas.DrawPath(SelectedComp.Position.GenerateSKPath(
                    WorldToScreen(SKRect.Create(
                        new SKPoint(Position.X * 100, Position.Y * 100),
                        new SKSize(99,99)
                    ))
                ), paint);
                e.Canvas.ResetMatrix();
            }
            //tile valid highlight=============================================================
            if (Player.State == State.PlacingTile)
            {
                paint.Color = (TileManager.IsValidLocation(
                    Position,
                    TileManager.CurrentOrientation,
                    TileManager.CurrentTile
                ) ? SKColors.Green : SKColors.Red).WithAlpha(100);
                e.Canvas.DrawRect(WorldToScreen(new SKRect(
                    Position.X * 100,
                    Position.Y * 100,
                    Position.X * 100 + 99,
                    Position.Y * 100 + 99
                )), paint);
            }
            //meeple===========================================================================
            foreach (KeyValuePair<SKPointI, Tile> tile in TileManager)
            {
                foreach (TileComponent tc in tile.Value.Components.ToList().FindAll(
                    e => e.Claimee != null
                ))
                {
                    paint.Color = tc.Claimee.Colour;
                    SKRect meeplePos = tc
                    .Position
                    .FindBestComponentPosition()
                    .getComponentPositionMeepleRect(tile.Value.Orientation);
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
                e.Canvas.RotateRadians(((int)TileManager.CurrentOrientation + 3) * 0.5f * (float)Math.PI, WorldToScreen(WorldMousePos).X, WorldToScreen(WorldMousePos).Y);
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
        public override bool AllowPanStart(EventArgs_Click e) => e.Button != RJGL.MouseButtons.Left;
    }
}
