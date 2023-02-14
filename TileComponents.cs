using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carcassonne2
{
    enum ComponentsType
    {
        Grass,
        Town,
        Road,
        Abbey
    }
    [Flags]
    enum ComponentPosition
    {
        NorthLeft = 0,
        NorthCentre = 1,
        NorthRight = 2,
        North = NorthLeft | NorthCentre | NorthRight,

        EastLeft = 4,
        EastCentre = 8,
        EastRight = 16,
        East = EastLeft | EastCentre | EastRight,

        SouthLeft = 32,
        SouthCentre = 64,
        SouthRight = 128,
        South = SouthLeft | SouthCentre | SouthRight,

        WestLeft = 256,
        WestCentre = 512,
        WestRight = 1024,
        West = WestLeft | WestCentre | WestRight,

        Middle = 2048,

        All = North | East | South | West | Middle,
    }
    internal struct TileComponent
    {
        public readonly ComponentsType Type;
        public readonly ComponentPosition Position;
        public readonly bool DoubleScore;
        public TileComponent(ComponentsType Type, ComponentPosition Position, bool DoubleScore = false)
        {
            this.Type = Type;
            this.Position = Position;
            this.DoubleScore = DoubleScore;
        }
    }
    internal struct TileDefinition
    {
        public readonly TileComponent[] Components;
        public readonly int Weighting;
        public readonly SKImage Texture;
        //texture
        public TileDefinition(TileComponent[] Components, int Weighting, SKImage Texture)
        {
            this.Components = Components;
            this.Weighting = Weighting;
            this.Texture = Texture;
        }
    }
}
