using RJJSON;
using SkiaSharp;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Carcassonne2
{
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
}
