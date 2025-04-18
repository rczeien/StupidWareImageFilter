namespace StupidWareGames.CrazyFilter.Interfaces;

using Avalonia;
using Avalonia.Media.Imaging;
using System.Collections.Generic;

/// <summary>
/// Represents a body part filter logic provider.
/// </summary>
public interface IBodyPart
{
    /// <summary>
    /// Name of the body part (e.g., Face, Hands).
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Determines the rendering sort order. Lower renders first.
    /// </summary>
    int SortOrder { get; }

    /// <summary>
    /// Returns how many places this part may draw overlays.
    /// </summary>
    int GetPlaceCount();

    /// <summary>
    /// Gets the target area on the image for placing a filter.
    /// </summary>
    Rect GetPlacementArea(ImageAnalysisData data, int place);

    /// <summary>
    /// Determines if the overlay should be flipped horizontally.
    /// </summary>
    bool ShouldFlip(ImageAnalysisData data, int place);

    /// <summary>
    /// Returns the rotation angle in degrees to apply to the overlay.
    /// </summary>
    float GetRotationAngle(ImageAnalysisData data, int place);

    /// <summary>
    /// Gets the scale factor to apply to the overlay.
    /// </summary>
    float GetScaleFactor(ImageAnalysisData data, int place);
}

/// <summary>
/// Represents AI-generated landmark data.
/// </summary>
public class ImageAnalysisData
{
    public IReadOnlyDictionary<string, Point> Landmarks { get; set; } = new Dictionary<string, Point>();
    public int ImageWidth { get; set; }
    public int ImageHeight { get; set; }
}

/// <summary>
/// Defines where to draw an overlay.
/// </summary>
public class OverlayPlacement
{
    public Rect Area { get; set; }
    public bool Flip { get; set; }
    public float Rotation { get; set; }
    public float Scale { get; set; } = 1.0f;
}
