using System;
using System.Drawing;
using System.IO;
using Svg;

namespace GameLauncher
{
    public static class AssetManager
    {
        private static string assetsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "assets", "sprites");

        public static Image LoadSvgAsImage(string fileName, int width, int height)
        {
            try
            {
                string filePath = Path.Combine(assetsPath, fileName);
                if (!File.Exists(filePath))
                {
                    // Fallback to creating a simple colored rectangle
                    Bitmap bmp = new Bitmap(width, height);
                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        g.Clear(Color.Gray);
                    }
                    return bmp;
                }

                var svgDoc = SvgDocument.Open(filePath);
                var bitmap = svgDoc.Draw(width, height);
                return bitmap;
            }
            catch
            {
                // Fallback
                Bitmap bmp = new Bitmap(width, height);
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.Clear(Color.Gray);
                }
                return bmp;
            }
        }
    }
}
