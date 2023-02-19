using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carcassonne2
{
    public enum Orientation
    {
        North,
        East,
        South,
        West
    }
    public class TileManager
    {
        private Dictionary<int, Dictionary<int, Tile>> tiles = new();
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
                tiles[x][y] = value;
            }
        }
    }
    public class TileComponent
    {
        public readonly ComponentsType Type;
        public readonly ComponentPosition Position;
        public readonly bool DoubleScore;
        public Player claimee;
    }
    public class Tile
    {
        public Orientation Orientation;
        // this needs to be seperate
        public TileDefinition Definition;
        public Tile(TileDefinition definition, Orientation orientation)
        {
            Orientation = orientation;
            Definition = definition;
        }
    }
}
