using SkiaSharp;
using System.Windows.Forms;

namespace Carcassonne2
{
    public class Player
    {
        public delegate void StateChangedHandler(object sender);
        public event StateChangedHandler? StateChanged;
        public Colours Colour;
        private State state;
        public int Score = 0;
        public int Meeple = 7;
        public State State { get=>state; set {
            state = value;
            StateChanged?.Invoke(this);
        } }
        public void AdvanceState() => State = State switch
        {
            State.FindingGame => State.WaitingForOpponent,
            State.WaitingForOpponent => State.Playing,
            State.Playing => State.WaitingForOpponent,
            State.PlacingTile => State.PlacingMeeple,
            State.PlacingMeeple => State.WaitingForOpponent,
            State.GameOver => State.GameOver,
            _ => throw new InvalidOperationException("State is not valid"),
        };
    }
    public enum State
    {
        FindingGame,
        WaitingForOpponent,
        Playing,
        PlacingTile,
        PlacingMeeple,
        GameOver,
    }
    public enum Colours
    {
        Blue,
        Green,
        Yellow,
        Red,
        Black,
    }
    public static class ColoursEx
    {
        public static SKColor ToSKColour(this Colours clrs) => clrs switch
        {
            Colours.Blue => new SKColor(0,0,255),
            Colours.Green => new SKColor(0,255,0),
            Colours.Yellow => new SKColor(255,255,0),
            Colours.Red => new SKColor(255,0,0),
            Colours.Black => new SKColor(0,0,0),
            _ => throw new InvalidOperationException("Invalid enum value"),
        };
    }
}
