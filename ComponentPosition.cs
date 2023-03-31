using SkiaSharp;

namespace Carcassonne2
{
    [Flags]
    public enum ComponentPosition
    {
        // each number in a flags enum should be a single different bit selected
        None = 0,

        NorthLeft = 1,
        NorthCentre = 2,
        NorthRight = 4,
        North = NorthLeft | NorthCentre | NorthRight,

        EastLeft = 8,
        EastCentre = 16,
        EastRight = 32,
        East = EastLeft | EastCentre | EastRight,

        SouthLeft = 64,
        SouthCentre = 128,
        SouthRight = 256,
        South = SouthLeft | SouthCentre | SouthRight,

        WestLeft = 512,
        WestCentre = 1024,
        WestRight = 2048,
        West = WestLeft | WestCentre | WestRight,

        Middle = 4096,

        All = North | East | South | West | Middle,
    }
    public static class ComponentPositionEx
    {
        public static SKPath GenerateSKPath(this ComponentPosition pos, SKRect position)
        {
            SKPath path = new();

            SKPoint loc = position.Location;

            float width = position.Width;
            float thirdX = width / 3;
            float twoThirdX = thirdX * 2;

            float height = position.Height;
            float thirdY = position.Height / 3;
            float twoThirdY = thirdY * 2;

            if (pos.HasFlag(ComponentPosition.NorthLeft))
            {
                path.AddPoly(new SKPoint[] {
                loc+new SKPoint(thirdX, thirdY),
                loc+new SKPoint(0, 0),
                loc+new SKPoint(thirdX, 0)
            });
            }
            if (pos.HasFlag(ComponentPosition.NorthCentre))
            {
                path.AddPoly(new SKPoint[] {
                loc+new SKPoint(thirdX, thirdY),
                loc+new SKPoint(twoThirdX, thirdY),
                loc+new SKPoint(twoThirdX, 0),
                loc+new SKPoint(thirdX, 0)
            });
            }
            if (pos.HasFlag(ComponentPosition.NorthRight))
            {
                path.AddPoly(new SKPoint[] {
                loc+new SKPoint(twoThirdX, thirdY),
                loc+new SKPoint(twoThirdX, 0),
                loc+new SKPoint(width, 0)
            });
            }
            if (pos.HasFlag(ComponentPosition.EastLeft))
            {
                path.AddPoly(new SKPoint[] {
                loc+new SKPoint(twoThirdX, thirdY),
                loc+new SKPoint(width, 0),
                loc+new SKPoint(width, thirdY)
            });
            }
            if (pos.HasFlag(ComponentPosition.EastCentre))
            {
                path.AddPoly(new SKPoint[] {
                loc+new SKPoint(twoThirdX, thirdY),
                loc+new SKPoint(width, thirdY),
                loc+new SKPoint(width, twoThirdY),
                loc+new SKPoint(twoThirdX, twoThirdY)
            });
            }
            if (pos.HasFlag(ComponentPosition.EastRight))
            {
                path.AddPoly(new SKPoint[] {
                loc+new SKPoint(twoThirdX, twoThirdY),
                loc+new SKPoint(width, height),
                loc+new SKPoint(width, twoThirdY)
            });
            }
            if (pos.HasFlag(ComponentPosition.SouthLeft))
            {
                path.AddPoly(new SKPoint[] {
                loc+new SKPoint(twoThirdX, twoThirdY),
                loc+new SKPoint(width, height),
                loc+new SKPoint(twoThirdX, height)
            });
            }
            if (pos.HasFlag(ComponentPosition.SouthCentre))
            {
                path.AddPoly(new SKPoint[] {
                loc+new SKPoint(thirdX, twoThirdY),
                loc+new SKPoint(twoThirdX, twoThirdY),
                loc+new SKPoint(twoThirdX, height),
                loc+new SKPoint(thirdX, height)
            });
            }
            if (pos.HasFlag(ComponentPosition.SouthRight))
            {
                path.AddPoly(new SKPoint[] {
                loc+new SKPoint(thirdX, twoThirdY),
                loc+new SKPoint(thirdX, height),
                loc+new SKPoint(0, height)
            });
            }
            if (pos.HasFlag(ComponentPosition.WestLeft))
            {
                path.AddPoly(new SKPoint[] {
                loc+new SKPoint(thirdX, twoThirdY),
                loc+new SKPoint(0, twoThirdY),
                loc+new SKPoint(0, height)
            });
            }
            if (pos.HasFlag(ComponentPosition.WestCentre))
            {
                path.AddPoly(new SKPoint[] {
                loc+new SKPoint(thirdX, thirdY),
                loc+new SKPoint(thirdX, twoThirdY),
                loc+new SKPoint(0, twoThirdY),
                loc+new SKPoint(0, thirdY)
            });
            }
            if (pos.HasFlag(ComponentPosition.WestRight))
            {
                path.AddPoly(new SKPoint[] {
                loc+new SKPoint(thirdX, thirdY),
                loc+new SKPoint(0, thirdY),
                loc+new SKPoint(0, 0)
            });
            }
            if (pos.HasFlag(ComponentPosition.Middle))
            {
                path.AddPoly(new SKPoint[] {
                loc+new SKPoint(thirdX, thirdY),
                loc+new SKPoint(twoThirdX, thirdX),
                loc+new SKPoint(twoThirdX, twoThirdY),
                loc+new SKPoint(thirdX, twoThirdY)
            });
            }
            return path;
        }
        public static ComponentPosition Rotate(this ComponentPosition pos, Orientation orientation)
        {
            ComponentPosition newPos = ComponentPosition.None;
            switch (orientation)
            {
                case Orientation.North: return pos;
                case Orientation.East:
                    // East -> North
                    if (pos.HasFlag(ComponentPosition.EastLeft))
                    { newPos |= ComponentPosition.NorthLeft; }
                    if (pos.HasFlag(ComponentPosition.EastCentre))
                    { newPos |= ComponentPosition.NorthCentre; }
                    if (pos.HasFlag(ComponentPosition.EastRight))
                    { newPos |= ComponentPosition.NorthRight; }

                    // South -> East
                    if (pos.HasFlag(ComponentPosition.SouthLeft))
                    { newPos |= ComponentPosition.EastLeft; }
                    if (pos.HasFlag(ComponentPosition.SouthCentre))
                    { newPos |= ComponentPosition.EastCentre; }
                    if (pos.HasFlag(ComponentPosition.SouthRight))
                    { newPos |= ComponentPosition.EastRight; }

                    // West -> South
                    if (pos.HasFlag(ComponentPosition.WestLeft))
                    { newPos |= ComponentPosition.SouthLeft; }
                    if (pos.HasFlag(ComponentPosition.WestCentre))
                    { newPos |= ComponentPosition.SouthCentre; }
                    if (pos.HasFlag(ComponentPosition.WestRight))
                    { newPos |= ComponentPosition.SouthRight; }

                    // North -> West
                    if (pos.HasFlag(ComponentPosition.NorthLeft))
                    { newPos |= ComponentPosition.WestLeft; }
                    if (pos.HasFlag(ComponentPosition.NorthCentre))
                    { newPos |= ComponentPosition.WestCentre; }
                    if (pos.HasFlag(ComponentPosition.NorthRight))
                    { newPos |= ComponentPosition.WestRight; }
                    return newPos;
                case Orientation.South:
                    // South -> North
                    if (pos.HasFlag(ComponentPosition.SouthLeft))
                    { newPos |= ComponentPosition.NorthLeft; }
                    if (pos.HasFlag(ComponentPosition.SouthCentre))
                    { newPos |= ComponentPosition.NorthCentre; }
                    if (pos.HasFlag(ComponentPosition.SouthRight))
                    { newPos |= ComponentPosition.NorthRight; }

                    // West -> East
                    if (pos.HasFlag(ComponentPosition.WestLeft))
                    { newPos |= ComponentPosition.EastLeft; }
                    if (pos.HasFlag(ComponentPosition.WestCentre))
                    { newPos |= ComponentPosition.EastCentre; }
                    if (pos.HasFlag(ComponentPosition.WestRight))
                    { newPos |= ComponentPosition.EastRight; }

                    // North -> South
                    if (pos.HasFlag(ComponentPosition.NorthLeft))
                    { newPos |= ComponentPosition.SouthLeft; }
                    if (pos.HasFlag(ComponentPosition.NorthCentre))
                    { newPos |= ComponentPosition.SouthCentre; }
                    if (pos.HasFlag(ComponentPosition.NorthRight))
                    { newPos |= ComponentPosition.SouthRight; }

                    // East -> West
                    if (pos.HasFlag(ComponentPosition.EastLeft))
                    { newPos |= ComponentPosition.WestLeft; }
                    if (pos.HasFlag(ComponentPosition.EastCentre))
                    { newPos |= ComponentPosition.WestCentre; }
                    if (pos.HasFlag(ComponentPosition.EastRight))
                    { newPos |= ComponentPosition.WestRight; }
                    return newPos;
                case Orientation.West:
                    // West -> North
                    if (pos.HasFlag(ComponentPosition.WestLeft))
                    { newPos |= ComponentPosition.NorthLeft; }
                    if (pos.HasFlag(ComponentPosition.WestCentre))
                    { newPos |= ComponentPosition.NorthCentre; }
                    if (pos.HasFlag(ComponentPosition.WestRight))
                    { newPos |= ComponentPosition.NorthRight; }

                    // North -> East
                    if (pos.HasFlag(ComponentPosition.NorthLeft))
                    { newPos |= ComponentPosition.EastLeft; }
                    if (pos.HasFlag(ComponentPosition.NorthCentre))
                    { newPos |= ComponentPosition.EastCentre; }
                    if (pos.HasFlag(ComponentPosition.NorthRight))
                    { newPos |= ComponentPosition.EastRight; }

                    // East -> South
                    if (pos.HasFlag(ComponentPosition.EastLeft))
                    { newPos |= ComponentPosition.SouthLeft; }
                    if (pos.HasFlag(ComponentPosition.EastCentre))
                    { newPos |= ComponentPosition.SouthCentre; }
                    if (pos.HasFlag(ComponentPosition.EastRight))
                    { newPos |= ComponentPosition.SouthRight; }

                    // South -> West
                    if (pos.HasFlag(ComponentPosition.SouthLeft))
                    { newPos |= ComponentPosition.WestLeft; }
                    if (pos.HasFlag(ComponentPosition.SouthCentre))
                    { newPos |= ComponentPosition.WestCentre; }
                    if (pos.HasFlag(ComponentPosition.SouthRight))
                    { newPos |= ComponentPosition.WestRight; }
                    return newPos;
                default:
                    throw new ArgumentException("Error " + orientation + " is not a valid orientation");
            }
        }
        public static TileComponent GetComponentFromPosition(
            this ComponentPosition pos,
            TileComponent[] components
        )
        {
            foreach (TileComponent tileComp in components)
            {
                if (tileComp.Position.HasFlag(pos))
                { return tileComp; }
            }
            throw new ArgumentException("components incomplete");
        }
        public static TileComponentDefinition GetComponentDefFromPosition(
            this ComponentPosition pos,
            TileComponentDefinition[] components
        )
        {
            foreach (TileComponentDefinition tileComp in components)
            {
                if (tileComp.Position.HasFlag(pos))
                { return tileComp; }
            }
            throw new ArgumentException("components incomplete");
        }
        public static ComponentPosition GetComponentPositionAtPos(SKPoint pos)
        {
            // this assumes that the size of the tile is 99*99
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
        public static ComponentPosition FindBestComponentPosition(this ComponentPosition cp)
        {
            if (cp == ComponentPosition.None)
            { throw new ArgumentException("Component Position must have a component to chose from"); }
            if (cp.HasFlag(ComponentPosition.Middle))
            { return ComponentPosition.Middle; }
            if (cp.HasFlag(ComponentPosition.NorthCentre))
            { return ComponentPosition.NorthCentre; }
            if (cp.HasFlag(ComponentPosition.EastCentre))
            { return ComponentPosition.EastCentre; }
            if (cp.HasFlag(ComponentPosition.SouthCentre))
            { return ComponentPosition.SouthCentre; }
            if (cp.HasFlag(ComponentPosition.WestCentre))
            { return ComponentPosition.WestCentre; }
            if (cp.HasFlag(ComponentPosition.NorthLeft))
            { return ComponentPosition.NorthLeft; }
            if (cp.HasFlag(ComponentPosition.NorthRight))
            { return ComponentPosition.NorthRight; }
            if (cp.HasFlag(ComponentPosition.EastLeft))
            { return ComponentPosition.EastLeft; }
            if (cp.HasFlag(ComponentPosition.EastRight))
            { return ComponentPosition.EastRight; }
            if (cp.HasFlag(ComponentPosition.SouthLeft))
            { return ComponentPosition.SouthLeft; }
            if (cp.HasFlag(ComponentPosition.SouthRight))
            { return ComponentPosition.SouthRight; }
            if (cp.HasFlag(ComponentPosition.WestLeft))
            { return ComponentPosition.WestLeft; }
            if (cp.HasFlag(ComponentPosition.WestRight))
            { return ComponentPosition.WestRight; }
            throw new InvalidOperationException("Error Enum Is Invalid");
        }
        public static SKRect getComponentPositionMeepleRect(this ComponentPosition cp, Orientation or)
        {
            SKPoint pos = cp switch
            {
                ComponentPosition.Middle => new(33, 33),
                ComponentPosition.NorthCentre => new(33, 0),
                ComponentPosition.EastCentre => new(66, 33),
                ComponentPosition.SouthCentre => new(33, 66),
                ComponentPosition.WestCentre => new(0, 33),
                ComponentPosition.NorthLeft or
                ComponentPosition.WestRight => new(0, 0),
                ComponentPosition.NorthRight or
                ComponentPosition.EastLeft => new(66, 0),
                ComponentPosition.EastRight or
                ComponentPosition.SouthLeft => new(66, 66),
                ComponentPosition.SouthRight or
                ComponentPosition.WestLeft => new(0, 66),
                _ => throw new ArgumentException("Error Enum must be a value sanitised in `FindBestComponentPosition`"),
            };
            pos = or switch
            {
                Orientation.North => pos,
                Orientation.East => new SKPoint(66 - pos.Y, pos.X),
                Orientation.South => new SKPoint(66 - pos.X, 66 - pos.Y),
                Orientation.West => new SKPoint(pos.Y, 66 - pos.X),
                _ => throw new InvalidOperationException("Invalid Orientation"),
            };
            SKRect rv = SKRect.Create(pos, new(33, 33));
            rv.Inflate(new(-5, -5));
            return rv;
        }
    }
}
