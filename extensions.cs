using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carcassonne2
{
    internal static class extensions
    {
        public static void DrawMeepel(
            this SKCanvas canvas,
            SKRect area,
            SKPaint paint
        ) => canvas.DrawMeepel(
            area.Width,
            area.Height,
            area.Top,
            area.Left,
            paint
        );
        public static void DrawMeepel(
            this SKCanvas canvas,
            float width,
            float height,
            float top,
            float left,
            SKPaint paint
        )
        {
            SKPath path = new SKPath();
            path.AddPoly(new SKPoint[] {
                new (left + width / 2, top+height / 4),
                new (left,top+height/2),
                new (left + width,top+height/2)
            });
            path.AddPoly(new SKPoint[] {
                new (left + width / 2, top+height / 6),
                new (left,top+height),
                new (left + width/3,top+height),
                new (left + width/2,top+height*3/4),
                new (left + width*2/3,top+height),
                new (left + width,top+height)
            });
            canvas.DrawPath(path, paint);
            canvas.DrawOval(new SKRect(left + width / 4, top, left + width * 3 / 4, top+height / 3), paint);
        }
    }
}
