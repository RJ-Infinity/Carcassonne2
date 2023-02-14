using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carcassonne2
{
    internal class Tile
    {
        public TileComponent[] Components { get; }
        public int NTileIndex { get; }
        public int ETileIndex { get; }
        public int STileIndex { get; }
        public int WTileIndex { get; }
        public int Weighting { get; }
        public Tile (
            TileComponent[] components,
            int nIndex,
            int eIndex,
            int sIndex,
            int wIndex,
            int weighting
            // also need a texture
        )
        {
            Components = components;
            NTileIndex = nIndex;
            ETileIndex = eIndex;
            STileIndex = sIndex;
            WTileIndex = wIndex;
            Weighting = weighting;
        }
    }
}
