using PGL;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using System.Reflection;

namespace Carcassonne2
{
    public partial class CarcassonneForm : PGLForm
    {
        List<TileDefinition> defaultTiles = new List<TileDefinition>
        {
            new TileDefinition(new TileComponent[] {
                new TileComponent(
                    ComponentsType.Town,
                    ComponentPosition.All,
                    true
                )
            },1,extensions.SKImageFromFile(".\\Base_Game_C1_Tile_01.jpg")),
            new TileDefinition(new TileComponent[] {
                new TileComponent(
                    ComponentsType.Town,
                    ComponentPosition.North|
                    ComponentPosition.South|
                    ComponentPosition.East|
                    ComponentPosition.Middle
                ),
                new TileComponent(
                    ComponentsType.Grass,
                    ComponentPosition.West
                )
            },3,extensions.SKImageFromFile(".\\Base_Game_C1_Tile_02.jpg")),
            new TileDefinition(new TileComponent[] {
                new TileComponent(
                    ComponentsType.Town,
                    ComponentPosition.North|
                    ComponentPosition.South|
                    ComponentPosition.East|
                    ComponentPosition.Middle,
                    true
                ),
                new TileComponent(
                    ComponentsType.Grass,
                    ComponentPosition.West
                )
            },1,extensions.SKImageFromFile(".\\Base_Game_C1_Tile_03.jpg")),
            new TileDefinition(new TileComponent[] {
                new TileComponent(
                    ComponentsType.Town,
                    ComponentPosition.North|
                    ComponentPosition.South|
                    ComponentPosition.East|
                    ComponentPosition.Middle,
                    true
                ),
                new TileComponent(
                    ComponentsType.Grass,
                    ComponentPosition.WestRight
                ),
                new TileComponent(
                    ComponentsType.Grass,
                    ComponentPosition.WestCentre
                ),
                new TileComponent(
                    ComponentsType.Grass,
                    ComponentPosition.WestLeft
                )
            },2, extensions.SKImageFromFile(".\\Base_Game_C1_Tile_04.jpg")),
            new TileDefinition(new TileComponent[] {
                new TileComponent(
                    ComponentsType.Town,
                    ComponentPosition.North|
                    ComponentPosition.South|
                    ComponentPosition.East|
                    ComponentPosition.Middle
                ),
                new TileComponent(
                    ComponentsType.Grass,
                    ComponentPosition.WestRight
                ),
                new TileComponent(
                    ComponentsType.Grass,
                    ComponentPosition.WestCentre
                ),
                new TileComponent(
                    ComponentsType.Grass,
                    ComponentPosition.WestLeft
                )
            },1, extensions.SKImageFromFile(".\\Base_Game_C1_Tile_05.jpg")),
            new TileDefinition(new TileComponent[] {
                new TileComponent(
                    ComponentsType.Grass,
                    ComponentPosition.West|
                    ComponentPosition.South|
                    ComponentPosition.Middle
                ),
                new TileComponent(
                    ComponentsType.Town,
                    ComponentPosition.East|
                    ComponentPosition.North
                )
            },3,extensions.SKImageFromFile(".\\Base_Game_C1_Tile_06.jpg")),
            new TileDefinition(new TileComponent[] {
                new TileComponent(
                    ComponentsType.Grass,
                    ComponentPosition.West|
                    ComponentPosition.South|
                    ComponentPosition.Middle
                ),
                new TileComponent(
                    ComponentsType.Town,
                    ComponentPosition.East|
                    ComponentPosition.North,
                    true
                )
            },2,extensions.SKImageFromFile(".\\Base_Game_C1_Tile_07.jpg")),
            new TileDefinition(new TileComponent[] {
                new TileComponent(
                    ComponentsType.Grass,
                    ComponentPosition.West|
                    ComponentPosition.South|
                    ComponentPosition.Middle
                ),
                new TileComponent(
                    ComponentsType.Town,
                    ComponentPosition.East
                ),
                new TileComponent(
                    ComponentsType.Town,
                    ComponentPosition.North
                )
            },2,extensions.SKImageFromFile(".\\Base_Game_C1_Tile_08.jpg")),
            new TileDefinition(new TileComponent[] {
                new TileComponent(
                    ComponentsType.Town,
                    ComponentPosition.North|
                    ComponentPosition.East
                ),
                new TileComponent(
                    ComponentsType.Grass,
                    ComponentPosition.WestRight|
                    ComponentPosition.Middle|
                    ComponentPosition.SouthLeft
                ),
                new TileComponent(
                    ComponentsType.Road,
                    ComponentPosition.WestCentre|
                    ComponentPosition.SouthCentre
                ),
                new TileComponent(
                    ComponentsType.Road,
                    ComponentPosition.WestLeft|
                    ComponentPosition.SouthRight
                )
            },3,extensions.SKImageFromFile(".\\Base_Game_C1_Tile_09.jpg")),
            new TileDefinition(new TileComponent[] {
                new TileComponent(
                    ComponentsType.Town,
                    ComponentPosition.North|
                    ComponentPosition.East,
                    true
                ),
                new TileComponent(
                    ComponentsType.Grass,
                    ComponentPosition.WestRight|
                    ComponentPosition.Middle|
                    ComponentPosition.SouthLeft
                ),
                new TileComponent(
                    ComponentsType.Road,
                    ComponentPosition.WestCentre|
                    ComponentPosition.SouthCentre
                ),
                new TileComponent(
                    ComponentsType.Road,
                    ComponentPosition.WestLeft|
                    ComponentPosition.SouthRight
                )
            },2,extensions.SKImageFromFile(".\\Base_Game_C1_Tile_10.jpg")),
            new TileDefinition(new TileComponent[] {
                new TileComponent(
                    ComponentsType.Town,
                    ComponentPosition.North
                ),
                new TileComponent(
                    ComponentsType.Town,
                    ComponentPosition.South
                ),
                new TileComponent(
                    ComponentsType.Grass,
                    ComponentPosition.West|
                    ComponentPosition.Middle|
                    ComponentPosition.East
                )
            },3,extensions.SKImageFromFile(".\\Base_Game_C1_Tile_11.jpg")),
            new TileDefinition(new TileComponent[] {
                new TileComponent(
                    ComponentsType.Town,
                    ComponentPosition.North|
                    ComponentPosition.South|
                    ComponentPosition.Middle,
                    true
                ),
                new TileComponent(
                    ComponentsType.Grass,
                    ComponentPosition.West
                ),
                new TileComponent(
                    ComponentsType.Grass,
                    ComponentPosition.East
                )
            },2,extensions.SKImageFromFile(".\\Base_Game_C1_Tile_12.jpg")),
            new TileDefinition(new TileComponent[] {
                new TileComponent(
                    ComponentsType.Town,
                    ComponentPosition.North|
                    ComponentPosition.South|
                    ComponentPosition.Middle
                ),
                new TileComponent(
                    ComponentsType.Grass,
                    ComponentPosition.West
                ),
                new TileComponent(
                    ComponentsType.Grass,
                    ComponentPosition.East
                )
            },2,extensions.SKImageFromFile(".\\Base_Game_C1_Tile_13.jpg")),
            new TileDefinition(new TileComponent[] {
                new TileComponent(
                    ComponentsType.Town,
                    ComponentPosition.North
                ),
                new TileComponent(
                    ComponentsType.Grass,
                    ComponentPosition.East|
                    ComponentPosition.South|
                    ComponentPosition.West|
                    ComponentPosition.Middle
                )
            },5,extensions.SKImageFromFile(".\\Base_Game_C1_Tile_14.jpg")),
            new TileDefinition(new TileComponent[] {
                new TileComponent(
                    ComponentsType.Town,
                    ComponentPosition.North
                ),
                new TileComponent(
                    ComponentsType.Grass,
                    ComponentPosition.East|
                    ComponentPosition.WestRight|
                    ComponentPosition.SouthLeft
                ),
                new TileComponent(
                    ComponentsType.Road,
                    ComponentPosition.WestCentre|
                    ComponentPosition.Middle|
                    ComponentPosition.SouthCentre
                ),
                new TileComponent(
                    ComponentsType.Grass,
                    ComponentPosition.WestLeft|
                    ComponentPosition.SouthRight
                )
            },3,extensions.SKImageFromFile(".\\Base_Game_C1_Tile_15.jpg")),
            //START TILE
            new TileDefinition(new TileComponent[] {
                new TileComponent(
                    ComponentsType.Town,
                    ComponentPosition.North
                ),
                new TileComponent(
                    ComponentsType.Grass,
                    ComponentPosition.EastLeft|
                    ComponentPosition.WestRight
                ),
                new TileComponent(
                    ComponentsType.Road,
                    ComponentPosition.WestCentre|
                    ComponentPosition.Middle|
                    ComponentPosition.EastCentre
                ),
                new TileComponent(
                    ComponentsType.Grass,
                    ComponentPosition.EastRight|
                    ComponentPosition.WestLeft|
                    ComponentPosition.South
                )
            },3,extensions.SKImageFromFile(".\\Base_Game_C1_Tile_16.jpg")),
            new TileDefinition(new TileComponent[] {
                new TileComponent(
                    ComponentsType.Town,
                    ComponentPosition.North
                ),
                new TileComponent(
                    ComponentsType.Grass,
                    ComponentPosition.West|
                    ComponentPosition.EastLeft|
                    ComponentPosition.SouthRight
                ),
                new TileComponent(
                    ComponentsType.Road,
                    ComponentPosition.EastCentre|
                    ComponentPosition.Middle|
                    ComponentPosition.SouthCentre
                ),
                new TileComponent(
                    ComponentsType.Grass,
                    ComponentPosition.EastRight|
                    ComponentPosition.SouthLeft
                )
            },3,extensions.SKImageFromFile(".\\Base_Game_C1_Tile_17.jpg")),
            new TileDefinition(new TileComponent[] {
                new TileComponent(
                    ComponentsType.Town,
                    ComponentPosition.North
                ),
                new TileComponent(
                    ComponentsType.Grass,
                    ComponentPosition.EastLeft|
                    ComponentPosition.WestRight|
                    ComponentPosition.Middle
                ),
                new TileComponent(
                    ComponentsType.Road,
                    ComponentPosition.EastCentre
                ),
                new TileComponent(
                    ComponentsType.Road,
                    ComponentPosition.SouthCentre
                ),
                new TileComponent(
                    ComponentsType.Road,
                    ComponentPosition.WestCentre
                ),
                new TileComponent(
                    ComponentsType.Grass,
                    ComponentPosition.EastRight|
                    ComponentPosition.SouthLeft
                ),
                new TileComponent(
                    ComponentsType.Grass,
                    ComponentPosition.SouthRight|
                    ComponentPosition.WestLeft
                )
            },3,extensions.SKImageFromFile(".\\Base_Game_C1_Tile_18.jpg")),
            new TileDefinition(new TileComponent[] {
                new TileComponent(
                    ComponentsType.Abbey,
                    ComponentPosition.Middle
                ),
                new TileComponent(
                    ComponentsType.Grass,
                    ComponentPosition.North|
                    ComponentPosition.East|
                    ComponentPosition.South|
                    ComponentPosition.West
                )
            },4,extensions.SKImageFromFile(".\\Base_Game_C1_Tile_19.jpg")),
            new TileDefinition(new TileComponent[] {
                new TileComponent(
                    ComponentsType.Abbey,
                    ComponentPosition.Middle
                ),
                new TileComponent(
                    ComponentsType.Grass,
                    ComponentPosition.WestRight|
                    ComponentPosition.North|
                    ComponentPosition.East|
                    ComponentPosition.South|
                    ComponentPosition.WestLeft
                ),
                new TileComponent(
                    ComponentsType.Road,
                    ComponentPosition.WestCentre
                )
            },2,extensions.SKImageFromFile(".\\Base_Game_C1_Tile_20.jpg")),
            new TileDefinition(new TileComponent[] {
                new TileComponent(
                    ComponentsType.Grass,
                    ComponentPosition.WestRight|
                    ComponentPosition.North|
                    ComponentPosition.East|
                    ComponentPosition.SouthLeft
                ),
                new TileComponent(
                    ComponentsType.Road,
                    ComponentPosition.WestCentre|
                    ComponentPosition.Middle|
                    ComponentPosition.SouthCentre
                ),
                new TileComponent(
                    ComponentsType.Grass,
                    ComponentPosition.SouthRight|
                    ComponentPosition.WestLeft
                )
            },9,extensions.SKImageFromFile(".\\Base_Game_C1_Tile_21.jpg")),
            new TileDefinition(new TileComponent[] {
                new TileComponent(
                    ComponentsType.Grass,
                    ComponentPosition.WestRight|
                    ComponentPosition.North|
                    ComponentPosition.EastLeft
                ),
                new TileComponent(
                    ComponentsType.Road,
                    ComponentPosition.EastCentre|
                    ComponentPosition.Middle|
                    ComponentPosition.WestCentre
                ),
                new TileComponent(
                    ComponentsType.Grass,
                    ComponentPosition.EastRight|
                    ComponentPosition.South|
                    ComponentPosition.WestLeft
                ),
            },9,extensions.SKImageFromFile(".\\Base_Game_C1_Tile_22.jpg")),
            new TileDefinition(new TileComponent[] {
                new TileComponent(
                    ComponentsType.Grass,
                    ComponentPosition.WestRight|
                    ComponentPosition.North|
                    ComponentPosition.EastLeft|
                    ComponentPosition.Middle
                ),
                new TileComponent(
                    ComponentsType.Road,
                    ComponentPosition.EastCentre
                ),
                new TileComponent(
                    ComponentsType.Road,
                    ComponentPosition.WestCentre
                ),
                new TileComponent(
                    ComponentsType.Road,
                    ComponentPosition.SouthCentre
                ),
                new TileComponent(
                    ComponentsType.Grass,
                    ComponentPosition.EastRight|
                    ComponentPosition.SouthLeft
                ),
                new TileComponent(
                    ComponentsType.Grass,
                    ComponentPosition.SouthRight|
                    ComponentPosition.WestLeft
                ),
            },9,extensions.SKImageFromFile(".\\Base_Game_C1_Tile_23.jpg")),
            new TileDefinition(new TileComponent[] {
                new TileComponent(
                    ComponentsType.Road,
                    ComponentPosition.NorthCentre
                ),
                new TileComponent(
                    ComponentsType.Road,
                    ComponentPosition.EastCentre
                ),
                new TileComponent(
                    ComponentsType.Road,
                    ComponentPosition.WestCentre
                ),
                new TileComponent(
                    ComponentsType.Road,
                    ComponentPosition.SouthCentre
                ),
                new TileComponent(
                    ComponentsType.Grass,
                    ComponentPosition.WestRight|
                    ComponentPosition.NorthLeft|
                    ComponentPosition.Middle
                ),
                new TileComponent(
                    ComponentsType.Grass,
                    ComponentPosition.NorthRight|
                    ComponentPosition.EastLeft
                ),
                new TileComponent(
                    ComponentsType.Grass,
                    ComponentPosition.EastRight|
                    ComponentPosition.SouthLeft
                ),
                new TileComponent(
                    ComponentsType.Grass,
                    ComponentPosition.SouthRight|
                    ComponentPosition.WestLeft
                ),
            },9,extensions.SKImageFromFile(".\\Base_Game_C1_Tile_24.jpg")),
        };
        public CarcassonneForm()
        {
            InitializeComponent();
            Layers.Add(new layers.Background());
            Layers.Add(new layers.HUD());
        }
    }
}