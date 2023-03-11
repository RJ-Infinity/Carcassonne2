using RJGL;
using SkiaSharp;

namespace Carcassonne2.layers
{
    public class Background : PanAndZoomLayer
    {
        public Background()
        {
            MaxZoom = 3;
        }
        public override void OnDraw(EventArgs_Draw e)
        {
            // Create a diagonal gradient fill from Blue to Green to use as the background
            SKPoint topLeft = new(e.Bounds.Left, e.Bounds.Top);
            SKPoint bottomRight = new(e.Bounds.Right, e.Bounds.Bottom);
            SKColor[] gradColors = new[] { SKColors.LightBlue, SKColors.LightGreen };

            using SKPaint gradientPaint = new();
            using SKShader shader = SKShader.CreateLinearGradient(topLeft, bottomRight, gradColors, SKShaderTileMode.Clamp);
            gradientPaint.Shader = shader;
            gradientPaint.Style = SKPaintStyle.Fill;
            e.Canvas.DrawRect(e.Bounds, gradientPaint);

            using SKPaint paint = new();
            paint.IsAntialias = true;

            paint.Color = SKColors.Gray; // Very dark gray
            paint.Style = SKPaintStyle.Stroke;
            paint.StrokeWidth = 1;
            //e.Canvas.DrawLine(new(0,0), new(100,100), paint);
            // Draw the Horizontal Grid Lines
            int j = (int)Offset.Y % (int)(100 * Zoom);
            while (j < e.Bounds.Height)
            {
                SKPoint leftPoint = new(e.Bounds.Left, j);
                SKPoint rightPoint = new(e.Bounds.Right, j);

                e.Canvas.DrawLine(leftPoint, rightPoint, paint);

                j += (int)(100 * Zoom);
            }

            // Draw the Vertical Grid Lines
            j = ((int)Offset.X % (int)(100 * Zoom));
            while (j < e.Bounds.Width)
            {
                SKPoint topPoint = new(j, e.Bounds.Top);
                SKPoint bottomPoint = new(j, e.Bounds.Bottom);

                e.Canvas.DrawLine(topPoint, bottomPoint, paint);

                j += (int)(100 * Zoom);
            }

            paint.Style = SKPaintStyle.Fill;

            e.Canvas.DrawCircle(
                WorldToScreen(new SKPoint(0, 0)),
                (WorldToScreen(new SKPoint(10, 0))-WorldToScreen(new SKPoint(0, 0))).X,
                paint
            );
            base.OnDraw(e);
        }
        public override bool AllowZoom(EventArgs_Scroll e) => false;
        public override bool AllowPanStart(EventArgs_Click e) => false;
    }
}
