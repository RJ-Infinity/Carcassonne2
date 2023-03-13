using RJJSON;
using SkiaSharp;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

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
    }
    [Flags]
    public enum ComponentPosition
    {
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
    public enum Orientation
    {
        None,
        North,
        East,
        South,
        West
    }
    public static class OrientationEx
    {
        public static (
            ComponentPosition left,
            ComponentPosition centre,
            ComponentPosition right
        ) getSides(this Orientation Or) => Or switch
        {
            Orientation.North => (
                left: ComponentPosition.NorthLeft,
                centre: ComponentPosition.NorthCentre,
                right: ComponentPosition.NorthRight
            ),
            Orientation.East => (
                left: ComponentPosition.EastLeft,
                centre: ComponentPosition.EastCentre,
                right: ComponentPosition.EastRight
            ),
            Orientation.South => (
                left: ComponentPosition.SouthLeft,
                centre: ComponentPosition.SouthCentre,
                right: ComponentPosition.SouthRight
            ),
            Orientation.West => (
                left: ComponentPosition.WestLeft,
                centre: ComponentPosition.WestCentre,
                right: ComponentPosition.WestRight
            ),
            _ => throw new ArgumentException("posOr must have a non none enum value"),
        };
        public static Orientation Rotate(this Orientation or, Orientation rot) => rot switch
        {
            Orientation.North => or,
            Orientation.East => or switch
            {
                Orientation.North => Orientation.West,
                Orientation.East => Orientation.North,
                Orientation.South => Orientation.East,
                Orientation.West => Orientation.South,
                _ => throw new ArgumentException("orientation must be a non None value"),
            },
            Orientation.South => or switch
            {
                Orientation.North => Orientation.South,
                Orientation.East => Orientation.West,
                Orientation.South => Orientation.North,
                Orientation.West => Orientation.East,
                _ => throw new ArgumentException("orientation must be a non None value"),
            },
            Orientation.West => or switch
            {
                Orientation.North => Orientation.East,
                Orientation.East => Orientation.South,
                Orientation.South => Orientation.West,
                Orientation.West => Orientation.North,
                _ => throw new ArgumentException("orientation must be a non None value"),
            },
            _ => throw new ArgumentException("rotation must be a non None value"),
        };

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
    public struct TileDefinition
    {
        public readonly TileComponentDefinition[] Components;
        public readonly int Weighting;
        public readonly SKImage Texture;
        public readonly bool StartTile;
        //texture
        public TileDefinition(TileComponentDefinition[] Components, int Weighting, SKImage Texture, bool StartTile=false)
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
    public class TileManager : IEnumerable<KeyValuePair<SKPointI, Tile>>
    {
        public readonly List<TileDefinition> TileDefinitions;
        private readonly Random rand;
        public TileDefinition CurrentTile;
        public Orientation CurrentOrientation = Orientation.North;
        private Stack<TileDefinition> TilePool=new();
        public SKPointI LastTilePos;
        public TileManager(List<TileDefinition> definitions, int seed)
        {
            rand = new(seed);
            TileDefinitions = definitions;
            List<TileDefinition> StartTiles = TileDefinitions.Where(
                (TileDefinition td) => td.StartTile
            ).ToList();
            CurrentTile = StartTiles[rand.Next(StartTiles.Count)];
            this[0, 0] = new(CurrentTile, Orientation.North);
            GenerateTilePool();
        }
        private readonly Dictionary<int, Dictionary<int, Tile>> tiles = new();
        public bool ContainsTile(int x, int y) => tiles.ContainsKey(x) && tiles[x].ContainsKey(y);
        public bool ContainsTile(SKPointI pos) => ContainsTile(pos.X, pos.Y);
        public Tile this[int x, int y]
        {
            get
            {
                try { return tiles[x][y]; }
                catch (KeyNotFoundException e)
                { throw new KeyNotFoundException("Error the tile was not found", e); }
            }
            set
            {
                if (!tiles.ContainsKey(x))
                { tiles[x] = new(); }
                LastTilePos = new(x, y);
                tiles[x][y] = value;
            }
        }
        public Tile this[SKPointI pos] {
            get => this[pos.X, pos.Y];
            set => this[pos.X, pos.Y] = value;
        }
        public void GenerateTilePool()
        {
            foreach (TileDefinition td in TileDefinitions)
            {for (int i = 0; i < td.Weighting; i++){TilePool.Push(td);}}
            //randomise
            TilePool = new(TilePool.ToArray().OrderBy(_ => rand.Next()));
        }
        public void GenerateNextTile() => CurrentTile = TilePool.Pop();
        private bool sidesMatch(SKPointI pos, Orientation posOr, TileDefinition tile, Orientation tileOr)
        {
            (
                ComponentPosition posLeft,
                ComponentPosition posCentre,
                ComponentPosition posRight
            ) = posOr.Rotate(this[pos].Orientation).getSides();
            ComponentsType posCompLeft = posLeft.GetComponentFromPosition(
                this[pos].Components
            ).Type;
            ComponentsType posCompCentre = posCentre.GetComponentFromPosition(
                this[pos].Components
            ).Type;
            ComponentsType posCompRight = posRight.GetComponentFromPosition(
                this[pos].Components
            ).Type;
            (
                ComponentPosition tileLeft,
                ComponentPosition tileCentre,
                ComponentPosition tileRight
            ) = tileOr.getSides();
            ComponentsType tileCompLeft = tileLeft.GetComponentDefFromPosition(
                tile.Components
            ).Type;
            ComponentsType tileCompCentre = tileCentre.GetComponentDefFromPosition(
                tile.Components
            ).Type;
            ComponentsType tileCompRight = tileRight.GetComponentDefFromPosition(
                tile.Components
            ).Type;
            return posCompLeft == tileCompRight &&
            posCompCentre == tileCompCentre &&
            posCompRight == tileCompLeft;
        }
        public bool IsValidLocation(SKPointI pos, Orientation or, TileDefinition tile)
        => !ContainsTile(pos) && (
            ContainsTile(pos + new SKPointI(1, 0)) ||
            ContainsTile(pos + new SKPointI(0, 1)) ||
            ContainsTile(pos - new SKPointI(1, 0)) ||
            ContainsTile(pos - new SKPointI(0, 1))
        ) &&(
            !ContainsTile(pos + new SKPointI(1, 0)) || sidesMatch(
                pos + new SKPointI(1, 0),
                Orientation.West,
                tile,
                Orientation.East.Rotate(or)
            )
        ) && (
            !ContainsTile(pos + new SKPointI(0, 1)) || sidesMatch(
                pos + new SKPointI(0, 1),
                Orientation.North,
                tile,
                Orientation.South.Rotate(or)
            )
        ) && (
            !ContainsTile(pos - new SKPointI(1, 0)) || sidesMatch(
                pos - new SKPointI(1, 0),
                Orientation.East,
                tile,
                Orientation.West.Rotate(or)
            )
        ) && (
            !ContainsTile(pos - new SKPointI(0, 1)) || sidesMatch(
                pos - new SKPointI(0, 1),
                Orientation.South,
                tile,
                Orientation.North.Rotate(or)
            )
        );

        public IEnumerator<KeyValuePair<SKPointI, Tile>> GetEnumerator()
        {
            foreach (KeyValuePair<int, Dictionary<int, Tile>> collum in tiles)
            {
                foreach (KeyValuePair<int, Tile> item in collum.Value)
                { yield return new(new(collum.Key, item.Key),item.Value); }
            }
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
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
}
