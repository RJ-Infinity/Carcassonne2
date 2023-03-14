namespace Carcassonne2
{
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
}
