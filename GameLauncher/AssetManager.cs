using System.Drawing;
using System.Drawing.Imaging;
using Svg;

namespace CoffeePause;

public class AssetManager
{
    private static readonly string AssetsPath = Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory,
        "assets");

    private static AssetManager? _instance;
    private Dictionary<string, Image> _imageCache = new();

    public static AssetManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new AssetManager();
            }
            return _instance;
        }
    }

    public Image? LoadImage(string filename)
    {
        if (string.IsNullOrEmpty(filename)) return null;
        if (_imageCache.ContainsKey(filename)) return _imageCache[filename];

        try
        {
            var path = Path.Combine(AssetsPath, filename);
            if (File.Exists(path))
            {
                var ext = Path.GetExtension(path).ToLowerInvariant();
                if (ext == ".svg")
                {
                    // Render SVG to bitmap using Svg.NET
                    var doc = SvgDocument.Open(path);
                    var bmp = new Bitmap((int)Math.Max(1, doc.Width.Value), (int)Math.Max(1, doc.Height.Value));
                    doc.Draw(bmp);
                    _imageCache[filename] = bmp;
                    return bmp;
                }

                var image = Image.FromFile(path);
                _imageCache[filename] = image;
                return image;
            }

            // fallback: try same filename with png/jpg
            var noExt = Path.Combine(AssetsPath, Path.GetFileNameWithoutExtension(filename));
            var pngPath = noExt + ".png";
            var jpgPath = noExt + ".jpg";
            if (File.Exists(pngPath))
            {
                var image = Image.FromFile(pngPath);
                _imageCache[filename] = image;
                return image;
            }
            if (File.Exists(jpgPath))
            {
                var image = Image.FromFile(jpgPath);
                _imageCache[filename] = image;
                return image;
            }

            // try case-insensitive search in assets folder
            var files = Directory.GetFiles(AssetsPath, "*" + Path.GetFileName(filename), SearchOption.AllDirectories);
            if (files.Length > 0)
            {
                var image = Image.FromFile(files[0]);
                _imageCache[filename] = image;
                return image;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to load image {filename}: {ex.Message}");
        }

        return null;
    }

    public Image? GetBackgroundImage()
    {
        return LoadImage("background.svg") ?? LoadImage("background.png") ?? LoadImage("background.jpg");
    }

    public Image? GetLogoImage()
    {
        return LoadImage("logo.svg") ?? LoadImage("logo.png") ?? LoadImage("coffeepause.png") ?? LoadImage("CoffeePause.png");
    }

    public Image? GetCardImage(string cardName)
    {
        // cardName expected like A♠ or K♥ — sanitize for filenames
        var safe = cardName.Replace("♠", "S").Replace("♣", "C").Replace("♥", "H").Replace("♦", "D");
        return LoadImage($"cards/{{safe}}.svg") ?? LoadImage($"cards/{{safe}}.png") ?? LoadImage($"cards/{{safe}}.jpg");
    }

    public void Dispose()
    {
        foreach (var image in _imageCache.Values)
        {
            try { image.Dispose(); } catch { }
        }
        _imageCache.Clear();
    }
}