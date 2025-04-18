namespace StupidWareGames.CrazyFilter.Utils;

using Avalonia.Media.Imaging;
using StupidWareGames.CrazyFilter.Interfaces;

public class FilterAsset
{
    public string Name { get; set; } = string.Empty;
    public IBodyPart? TargetPart { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public Bitmap? Image { get; set; }
}
