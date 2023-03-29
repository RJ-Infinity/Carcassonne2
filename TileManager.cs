using SkiaSharp;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Carcassonne2
{
    public class TileManager : IEnumerable<KeyValuePair<SKPointI, Tile>>
    {
        public readonly HashSet<TileDefinition> TileDefinitions;
        private readonly Random rand;
        public TileDefinition CurrentTile;
        public Orientation CurrentOrientation = Orientation.North;
        private Stack<TileDefinition> TilePool=new();
        public SKPointI LastTilePos;
        public List<ComponentGraph> Graph = new();
        public TileManager(HashSet<TileDefinition> definitions, int seed)
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
                updateGraphWithPosition();
            }
        }
        public Tile this[SKPointI pos] {
            get => this[pos.X, pos.Y];
            set => this[pos.X, pos.Y] = value;
        }
        public SKPointI getBorderingTileLocation(Orientation Border, SKPointI loc)
        => Border.Rotate(this[loc].Orientation) switch
        {
            Orientation.North => loc - new SKPointI(0, 1),
            Orientation.East => loc + new SKPointI(1, 0),
            Orientation.South => loc + new SKPointI(0, 1),
            Orientation.West => loc - new SKPointI(1, 0),
            _ => throw new ArgumentException("orientation must be a non None value"),
        };
        public bool IsGraphComplete(ComponentGraph graph)
        {
            if (graph.Type == ComponentsType.Abbey)
            {
                if (graph.Components.Count != 1 || graph.Components.First().Value.Count != 1)
                { throw new InvalidOperationException("this graph must have exactly 1 component"); }
                SKPointI location = graph.Components.First().Key;
                // if all bordering components are placed except the current tile as
                // for this to have been called it must exist
                return ContainsTile(location + new SKPointI(-1, -1)) &&
                    ContainsTile(location + new SKPointI(-1, 0)) &&
                    ContainsTile(location + new SKPointI(-1, 1)) &&
                    ContainsTile(location + new SKPointI(0, -1)) &&
                    ContainsTile(location + new SKPointI(0, 1)) &&
                    ContainsTile(location + new SKPointI(1, -1)) &&
                    ContainsTile(location + new SKPointI(1, 0)) &&
                    ContainsTile(location + new SKPointI(1, 1));
            }
            foreach (KeyValuePair<SKPointI, HashSet<TileComponent>> comp in graph.Components)
            {
                foreach (TileComponent component in comp.Value)
                {
                    if (component.Position.HasFlag(ComponentPosition.NorthLeft) ||
                    component.Position.HasFlag(ComponentPosition.NorthCentre) ||
                    component.Position.HasFlag(ComponentPosition.NorthRight))
                    { if (!ContainsTile(getBorderingTileLocation(Orientation.North, comp.Key))) { return false; } }
                    if (component.Position.HasFlag(ComponentPosition.EastLeft) ||
                    component.Position.HasFlag(ComponentPosition.EastCentre) ||
                    component.Position.HasFlag(ComponentPosition.EastRight))
                    { if (!ContainsTile(getBorderingTileLocation(Orientation.East, comp.Key))) { return false; } }
                    if (component.Position.HasFlag(ComponentPosition.SouthLeft) ||
                    component.Position.HasFlag(ComponentPosition.SouthCentre) ||
                    component.Position.HasFlag(ComponentPosition.SouthRight))
                    { if (!ContainsTile(getBorderingTileLocation(Orientation.South, comp.Key))) { return false; } }
                    if (component.Position.HasFlag(ComponentPosition.WestLeft) ||
                    component.Position.HasFlag(ComponentPosition.WestCentre) ||
                    component.Position.HasFlag(ComponentPosition.WestRight))
                    { if (!ContainsTile(getBorderingTileLocation(Orientation.West, comp.Key))) { return false; } }
                }
            }
            return true;
        }
        public ComponentGraph? FindGraphWith(TileComponent comp)
        => Graph.ToList().Find(
            (ComponentGraph graph) => graph
            .Components
            .Values
            .SelectMany((HashSet<TileComponent> l) => l)
            .Contains(comp)
        );
        private void updateGraphWithPosition()
        {
            Dictionary<TileComponent, HashSet<ComponentGraph?>> adjoiningGraphs = this[LastTilePos]
            .Components.ToDictionary<TileComponent, TileComponent, HashSet<ComponentGraph?>>(
                (TileComponent tc) => tc,(TileComponent tc) => new()
            );
            foreach (Orientation or in new Orientation[] {
                Orientation.North,
                Orientation.East,
                Orientation.South,
                Orientation.West,
            })
            {
                if (!ContainsTile(or switch
                {
                    Orientation.North => LastTilePos - new SKPointI(0, 1),
                    Orientation.East => LastTilePos + new SKPointI(1, 0),
                    Orientation.South => LastTilePos + new SKPointI(0, 1),
                    Orientation.West => LastTilePos - new SKPointI(1, 0),
                    _ => throw new ArgumentException("orientation must be a non None value"),
                })) { continue; }
                (
                    TileComponent borderCompLeft,
                    TileComponent borderCompCentre,
                    TileComponent borderCompRight
                ) = GetBorderingTileComponents(or, LastTilePos);
                (
                    ComponentPosition left,
                    ComponentPosition centre,
                    ComponentPosition right
                ) sides = or.Rotate(this[LastTilePos].Orientation).getSides();
                adjoiningGraphs[
                    sides.right.GetComponentFromPosition(this[LastTilePos].Components)
                ].Add(FindGraphWith(borderCompLeft));
                adjoiningGraphs[
                    sides.centre.GetComponentFromPosition(this[LastTilePos].Components)
                ].Add(FindGraphWith(borderCompCentre));
                adjoiningGraphs[
                    sides.left.GetComponentFromPosition(this[LastTilePos].Components)
                ].Add(FindGraphWith(borderCompRight));
            }
            List<ComponentGraph> newGraphs = new();
            foreach (KeyValuePair<
                TileComponent,
                HashSet<ComponentGraph?>
            > graphs in adjoiningGraphs)
            {
                TileComponent key = graphs.Key;
                List<ComponentGraph> value = graphs.Value.Where(
                    (ComponentGraph? graph) => graph != null
                ).ToList();
                if (value.Count == 0)
                {
                    ComponentGraph g = new(key.Type);
                    g.Components.Add(LastTilePos, new HashSet<TileComponent> { key });
                    if (key.Claimee != null) { g.Claimee.Add(key.Claimee); }
                    newGraphs.Add(g);
                }
                else
                {
                    ComponentGraph newGraph = value[0];
                    if (value.Count > 2)
                    {
                        foreach (ComponentGraph graph in value.Skip(1))
                        { newGraph = ComponentGraph.Merge(newGraph, graph); }
                    }
                    if (newGraph.Components.ContainsKey(LastTilePos))
                    { newGraph.Components[LastTilePos].Add(key); }
                    else
                    { newGraph.Components.Add(LastTilePos, new HashSet<TileComponent> { key } ); }
                    if (newGraph.Type != key.Type)
                    { throw new InvalidOperationException("The two graphs must have the same type"); }
                    if (key.Claimee != null) { newGraph.Claimee.Add(key.Claimee); }
                    newGraphs.Add(newGraph);
                }
            }
            foreach (ComponentGraph graph in newGraphs)
            { graph.Borders.UnionWith(newGraphs.Where(g => g != graph)); }
            foreach (KeyValuePair<
                TileComponent,
                HashSet<ComponentGraph?>
            > graphs in adjoiningGraphs)
            {
                foreach (ComponentGraph graph in graphs.Value)
                { if (graph != null) { Graph.Remove(graph); } }
            }
            Graph.AddRange(newGraphs);
        }
        public void GenerateTilePool()
        {
            foreach (TileDefinition td in TileDefinitions)
            {for (int i = 0; i < td.Weighting; i++){TilePool.Push(td);}}
            //randomise
            TilePool = new(TilePool.OrderBy(_ => rand.Next()));
        }
        public void GenerateNextTile() => CurrentTile = TilePool.Pop();
        private (
            TileComponent left,
            TileComponent centre,
            TileComponent right
        ) GetBorderingTileComponents(Orientation or, SKPointI loc)
        {
            SKPointI borderingTile = or switch
            {
                Orientation.North => loc - new SKPointI(0, 1),
                Orientation.East => loc + new SKPointI(1, 0),
                Orientation.South => loc + new SKPointI(0, 1),
                Orientation.West => loc - new SKPointI(1, 0),
                _ => throw new ArgumentException("orientation must be a non None value"),
            };
            Orientation borderingOr = or.Rotate(Orientation.South);
            (
                ComponentPosition posLeft,
                ComponentPosition posCentre,
                ComponentPosition posRight
            ) = borderingOr.Rotate(this[borderingTile].Orientation).getSides();
            return (
                posLeft.GetComponentFromPosition(this[borderingTile].Components),
                posCentre.GetComponentFromPosition(this[borderingTile].Components),
                posRight.GetComponentFromPosition(this[borderingTile].Components)
            );
        }
        private bool sidesMatch(
            SKPointI pos,
            Orientation posOr,
            TileDefinition tile,
            Orientation tileOr
        )
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
