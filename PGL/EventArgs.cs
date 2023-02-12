using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkiaSharp;
using SkiaSharp.Views.Desktop;

namespace PGL
{
    //https://stackoverflow.com/a/65056572/15755351
    public class EventArgs_Draw : EventArgs
    {
        public SKRect Bounds { get; }
        public SKCanvas Canvas { get; }

        public EventArgs_Draw(SKCanvas canvas, SKRect bounds)
        {
            Canvas = canvas;
            Bounds = bounds;
        }
    }
    public class EventArgs_Click : EventArgs
    {
        public SKPoint Position { get; }
        public int Button { get; }

        public EventArgs_Click(SKPoint pos, int button)
        {
            Position = pos;
            Button = button;
        }
    }
    public class EventArgs_Scroll : EventArgs
    {
        public int OldValue { get; }
        public int NewValue { get; }
        public bool IsHorizontal { get; }
        public SKPoint Position { get; }
        public EventArgs_Scroll(SKPoint pos, int oldV, int newV, bool isHorizontal)
        {
            Position = pos;
            OldValue = oldV;
            NewValue = newV;
            IsHorizontal = isHorizontal;
        }
    }
}
