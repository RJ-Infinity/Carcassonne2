using SkiaSharp;
using System.Collections.Generic;
using System.ComponentModel;

namespace Carcassonne2
{
    public class ComponentGraph
    {
        public Dictionary<SKPointI,HashSet<TileComponent>> Components = new();
        public ComponentsType Type;
        public List<Player> Claimee
        {
            get => Components.SelectMany(
                (KeyValuePair<SKPointI, HashSet<TileComponent>> component) => component.Value.Select(
                    (TileComponent tc) => tc.Claimee
                ).Where(
                    (Player? player) => player != null
                )
            ).ToList();
        }
        public List<ComponentGraph> Borders = new();
        public static ComponentGraph Merge(ComponentGraph g1, ComponentGraph g2)
        {
            if (g1.Type != g2.Type)
            { throw new ArgumentException("The two graphs must be the same type"); }
            ComponentGraph g3 = new();
            g3.Type = g1.Type;
            foreach (KeyValuePair<SKPointI, HashSet<TileComponent>> cp in g1.Components)
            { g3.Components.Add(cp.Key, cp.Value.ToHashSet()); }//to list copys it
            foreach (KeyValuePair<SKPointI, HashSet<TileComponent>> cp in g2.Components)
            {
                if (g3.Components.ContainsKey(cp.Key))
                { g3.Components[cp.Key].UnionWith(cp.Value); }
                else { g3.Components.Add(cp.Key, cp.Value.ToHashSet()); }
            }
            g3.Claimee.AddRange(g1.Claimee);
            g3.Claimee.AddRange(g2.Claimee);
            g3.Borders.AddRange(g1.Borders);
            g3.Borders.AddRange(g2.Borders);
            return g3;
        }
    }
}
