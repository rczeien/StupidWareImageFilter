namespace StupidWareGames.CrazyFilter.BodyParts;

using System;
using Avalonia;
using StupidWareGames.CrazyFilter.Interfaces;
using System.Collections.Generic;

public class ChestPart : IBodyPart
{
    public string Name => "Chest";
    public int SortOrder => 1;

    public int GetPlaceCount() => 1;

    public Rect GetPlacementArea(ImageAnalysisData data, int place)
    {
        if (!data.Landmarks.TryGetValue("left_shoulder", out var left) ||
            !data.Landmarks.TryGetValue("right_shoulder", out var right))
        {
            return new Rect(0, 0, 0, 0);
        }

        var centerX = (left.X + right.X) / 2;
        var centerY = (left.Y + right.Y) / 2 + 40; // Offset downward from shoulders

        var width = Math.Abs(right.X - left.X) * 1.6;
        var height = width * 0.9;

        return new Rect(centerX - width / 2, centerY - height / 2, width, height);
    }

    public bool ShouldFlip(ImageAnalysisData data, int place)
    {
        return false;
    }

    public float GetRotationAngle(ImageAnalysisData data, int place)
    {
        if (!data.Landmarks.TryGetValue("left_shoulder", out var left) ||
            !data.Landmarks.TryGetValue("right_shoulder", out var right))
        {
            return 0f;
        }

        return (float)(Math.Atan2(right.Y - left.Y, right.X - left.X) * (180.0 / Math.PI));
    }

    public float GetScaleFactor(ImageAnalysisData data, int place)
    {
        return 1.0f;
    }
}
