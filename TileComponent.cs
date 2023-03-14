using System.Diagnostics.CodeAnalysis;

namespace Carcassonne2
{
    public class TileComponent
    {
        public readonly ComponentsType Type;
        public readonly ComponentPosition Position;
        public readonly bool DoubleScore;
        public Player? Claimee;
        public TileComponent(TileComponentDefinition definition)
        {
            Type = definition.Type;
            Position = definition.Position;
            DoubleScore = definition.DoubleScore;
        }
    }
    public struct TileComponentDefinition
    {
        public readonly ComponentsType Type;
        public readonly ComponentPosition Position;
        public readonly bool DoubleScore;
        public TileComponentDefinition(ComponentsType Type, ComponentPosition Position, bool DoubleScore = false)
        {
            this.Type = Type;
            this.Position = Position;
            this.DoubleScore = DoubleScore;
        }
        public static bool operator ==(TileComponentDefinition a, TileComponentDefinition b)
        => a.Type == b.Type &&
        a.Position == b.Position &&
        a.DoubleScore == b.DoubleScore;
        public static bool operator !=(TileComponentDefinition a, TileComponentDefinition b)
        => !(a == b);
        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (obj == null) { return false; }
            if (obj.GetType() != this.GetType()) { return false; }
            return this == (TileComponentDefinition)obj;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
