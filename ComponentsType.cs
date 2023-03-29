using SkiaSharp;

namespace Carcassonne2
{
    public enum ComponentsType
    {
        Grass,
        Town,
        Road,
        Abbey
    }
    public static class ComponentsTypeEx
    {
        public static SKColor GetColour(this ComponentsType type) => type switch
        {
            ComponentsType.Grass => new SKColor(0, 255, 0),
            ComponentsType.Town => new SKColor(255, 0, 0),
            ComponentsType.Road => new SKColor(255, 255, 255),
            ComponentsType.Abbey => new SKColor(255, 255, 0),
            _ => throw new ArgumentException(),//TODO: better error handling
        };
        public static int GetPoints(this ComponentsType type) => type switch
        {
            ComponentsType.Town => 2,
            ComponentsType.Road => 1,
            _ => throw new ArgumentException(),//TODO: better error handling
        };
    }
}
