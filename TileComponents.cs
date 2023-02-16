using RJJSON;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carcassonne2
{
    public enum ComponentsType
    {
        Grass,
        Town,
        Road,
        Abbey
    }
    [Flags]
    public enum ComponentPosition
    {
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
    public struct TileComponent
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
        public static bool operator ==(TileComponent a, TileComponent b)
        => a.Type == b.Type &&
        a.Position == b.Position &&
        a.DoubleScore == b.DoubleScore;
        public static bool operator !=(TileComponent a, TileComponent b)
        => !(a == b);
        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (obj == null) { return false; }
            if (obj.GetType() != this.GetType()) { return false; }
            return this == (TileComponent)obj;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
    public struct TileDefinition
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
        public static bool operator ==(TileDefinition a, TileDefinition b)
        => a.Components.SequenceEqual(b.Components) &&
        a.Weighting == b.Weighting;// &&
        //TODO: add texture comparison
        //a.Texture. == b.Texture.EncodedData;
        public static bool operator !=(TileDefinition a, TileDefinition b)
        => !(a == b);
        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (obj == null) { return false; }
            if (obj.GetType() != this.GetType()) { return false; }
            return this == (TileDefinition)obj;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
    class TileDefinitionParser{
        private static void JsonAssert(bool condition, string message)
        {
            if (!condition)
            {
                // TODO: better Error Handling
                throw new Exception(message);
            }
        }
        public static TileDefinition[] parseJSONFile(string JsonStringData)
        {
            JSONType JSONObject = JSON.StringToObject(JsonStringData);
            JsonAssert(JSONObject.Type == JSON.Types.LIST, "root element must be a list");
            List<TileDefinition> def = new();
            foreach (JSONType tileDef in JSONObject.ListData)
            {
                JsonAssert(
                    tileDef.Type == JSON.Types.DICT,
                    "each TileDefinition must be an object"
                );
                JsonAssert(
                    tileDef.DictData.ContainsKey("TileComponents"),
                    "each TileDefinition must have a TileComponents list"
                );
                JsonAssert(
                    tileDef.DictData["TileComponents"].Type == JSON.Types.LIST,
                    "each TileDefinition must have a TileComponents list"
                );
                List<TileComponent> components = new();
                foreach (JSONType tileComp in tileDef.DictData["TileComponents"].ListData)
                {
                    JsonAssert(
                        tileComp.Type == JSON.Types.DICT,
                        "each TileComponent must be an object"
                    );
                    JsonAssert(
                        tileComp.DictData.ContainsKey("ComponentsType"),
                        "each TileComponent must contain a ComponentsType property"
                    );
                    JsonAssert(
                        tileComp.DictData["ComponentsType"].Type == JSON.Types.STRING,
                        "ComponentsType must be a string"
                    );
                    ComponentsType ct = extensions.StringToComponentType(
                        tileComp.DictData["ComponentsType"].StringData,
                        (ComponentsType)(-1)
                    );
                    JsonAssert(
                        (int)ct >= 0,
                        "ComponentsType must be one of `" +
                        String.Join(", ", Enum.GetNames(typeof(ComponentsType))) +
                        "`"
                    );
                    JsonAssert(
                        tileComp.DictData.ContainsKey("ComponentPosition"),
                        "each TileComponent must contain a ComponentPosition property"
                    );
                    JsonAssert(
                        tileComp.DictData["ComponentPosition"].Type == JSON.Types.LIST,
                        "ComponentPosition must be a list"
                    );
                    ComponentPosition cp = 0;
                    foreach (JSONType componentPosition in tileComp.DictData["ComponentPosition"].ListData)
                    {
                        JsonAssert(
                            componentPosition.Type == JSON.Types.STRING,
                            "each ComponentPosition must be a string"
                        );
                        ComponentPosition pos = extensions.StringToComponentType(
                            componentPosition.StringData,
                            (ComponentPosition)(-1)
                        );
                        JsonAssert(
                            (int)cp >= 0,
                            "ComponentPosition must be one of `" +
                            String.Join(", ", Enum.GetNames(typeof(ComponentPosition))) +
                            "`"
                        );
                        cp |= pos;
                    }
                    bool doubleScore = false;
                    if (tileComp.DictData.ContainsKey("DoubleScore"))
                    {
                        JsonAssert(
                            tileComp.DictData["DoubleScore"].Type == JSON.Types.BOOL,
                            "DoubleScore must be a bool"
                        );
                        doubleScore = tileComp.DictData["DoubleScore"].BoolData;
                    }
                    components.Add(new TileComponent(ct, cp, doubleScore));
                }
                JsonAssert(
                    tileDef.DictData.ContainsKey("Weighting"),
                    "each TileDefinition must have a Weighting"
                );
                JsonAssert(
                    tileDef.DictData["Weighting"].Type == JSON.Types.FLOAT,
                    "Weighting must be a number"
                );
                double weighting = tileDef.DictData["Weighting"].FloatData;
                JsonAssert(
                    weighting == (int)weighting,
                    "Weighting must be an int"
                );
                JsonAssert(
                    tileDef.DictData.ContainsKey("FilePath"),
                    "each TileDefinition must have a FilePath"
                );
                JsonAssert(
                    tileDef.DictData["FilePath"].Type == JSON.Types.STRING,
                    "FilePath must be a string"
                );
                try
                {
                    SKImage tex = extensions.SKImageFromFile(tileDef.DictData["FilePath"].StringData);
                    def.Add(new TileDefinition(components.ToArray(), (int)weighting, tex));
                }
                // better error handling
                catch
                {
                    JsonAssert(
                        false,
                        "FilePath failed to parse it might not be a valid image file"
                    );
                }
            }
            return def.ToArray();
        }
        public static SKColor GetColour(ComponentsType type)
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
        public static SKPath GenerateSKPath(ComponentPosition pos)
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
    }
}
