using SkiaSharp;

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
        private static Dictionary<string, SKImage> SKImageCache = new();
        public static SKImage SKImageFromFile(string filePath)
        {
            filePath = NormalizePath(filePath);
            if (!SKImageCache.ContainsKey(filePath))
            {
                FileStream img = File.Open(filePath, FileMode.Open);
                byte[] imgData = new byte[img.Length];
                img.Read(imgData, 0, (int)img.Length);
                img.Close();
                SKImageCache[filePath] = SKImage.FromEncodedData(imgData);
            }
            return SKImageCache[filePath];
        }
        public static string NormalizePath(string path)
        =>Path.GetFullPath(path)
        .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
        .ToUpperInvariant();
        public static EnumType StringToComponentType<EnumType>
            (string EnumName, EnumType error) where EnumType : Enum
        {
            foreach (EnumType value in Enum.GetValues(typeof(EnumType)))
            {
                if (EnumName == Enum.GetName(typeof(EnumType), value))
                {
                    return value;
                }
            }
            return error;
        }
    }
}
