using RJJSON;
using SkiaSharp;
using System.Diagnostics.CodeAnalysis;

namespace Carcassonne2
{
    public class Tile
    {
        public Orientation Orientation;
        public readonly TileComponent[] Components;
        public readonly SKImage Texture;
        public Tile(TileDefinition definition, Orientation orientation)
        {
            Orientation = orientation;
            Components = definition.Components.Select(
                (TileComponentDefinition def) => new TileComponent(def)
            ).ToArray();
            Texture = definition.Texture;
        }
    }
    public struct TileDefinition
    {
        public readonly TileComponentDefinition[] Components;
        public readonly int Weighting;
        public readonly SKImage Texture;
        public readonly bool StartTile;
        //texture
        public TileDefinition(TileComponentDefinition[] Components, int Weighting, SKImage Texture, bool StartTile = false)
        {
            this.Components = Components;
            this.Weighting = Weighting;
            this.Texture = Texture;
            this.StartTile = StartTile;
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
        private static void JsonAssert(bool condition, string message)
        {
            if (!condition)
            {
                // TODO: better Error Handling
                throw new Exception(message);
            }
        }
        public static List<TileDefinition> ParseJSONFile(string JsonStringData)
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
                bool startTile = false;
                if (tileDef.DictData.ContainsKey("StartTile"))
                {
                    JsonAssert(
                        tileDef.DictData["StartTile"].Type == JSON.Types.BOOL,
                        "the StartTile property must be a bool"
                    );
                    startTile = tileDef.DictData["StartTile"].BoolData;
                }
                JsonAssert(
                    tileDef.DictData.ContainsKey("TileComponents"),
                    "each TileDefinition must have a TileComponents list"
                );
                JsonAssert(
                    tileDef.DictData["TileComponents"].Type == JSON.Types.LIST,
                    "each TileDefinition must have a TileComponents list"
                );
                List<TileComponentDefinition> components = new();
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
                    ComponentsType ct = extensions.StringToEnumType(
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
                        ComponentPosition pos = extensions.StringToEnumType(
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
                    components.Add(new TileComponentDefinition(ct, cp, doubleScore));
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
                    def.Add(new TileDefinition(components.ToArray(), (int)weighting, tex, startTile));
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
            return def;
        }
    }
}
