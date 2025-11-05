using System.Drawing;

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
        if (_imageCache.ContainsKey(filename))
        {
            return _imageCache[filename];
        }

        try
        {
            var path = Path.Combine(AssetsPath, filename);
            if (File.Exists(path))
            {
                var image = Image.FromFile(path);
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
        return LoadImage("background.png") ?? LoadImage("background.jpg") ?? LoadImage("Background.jpg");
    }

    public Image? GetLogoImage()
    {
        return LoadImage("logo.png") ?? LoadImage("logo.jpg") ?? LoadImage("coffeepause.png") ?? LoadImage("CoffeePause.png");
    }

    public Image? GetCardImage(string cardName)
    {
        return LoadImage($"cards/{cardName}.png") ?? LoadImage($"cards/{cardName}.jpg");
    }

    public void Dispose()
    {
        foreach (var image in _imageCache.Values)
        {
            image.Dispose();
        }
        _imageCache.Clear();
    }
}
