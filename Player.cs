﻿using SkiaSharp;

namespace Carcassonne2
{
    public class Player
    {
        public delegate void StateChangedHandler(object sender);
        public event StateChangedHandler StateChanged;
        public SKColor Colour;
        private State state;
        public int Score = 0;
        public int Meeple = 7;
        public State State { get=>state; set {
            state = value;
            StateChanged?.Invoke(this);
        } }
        public int ID;
        public Player(int playerID, SKColor colour)
        {
            Colour = colour;
            ID = playerID;
        }
        public void AdvanceState() => State = State switch
        {
            State.FindingGame => State.WaitingForOpponent,
            State.WaitingForOpponent => State.PlacingTile,
            State.PlacingTile => State.PlacingMeeple,
            State.PlacingMeeple => State.WaitingForOpponent,
            _ => throw new InvalidOperationException("State is not valid"),
        };
    }
    public enum State
    {
        FindingGame,
        WaitingForOpponent,
        PlacingTile,
        PlacingMeeple,
    }
}
