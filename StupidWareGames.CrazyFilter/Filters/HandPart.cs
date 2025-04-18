namespace StupidWareGames.CrazyFilter.BodyParts;

using System;
using Avalonia;
using StupidWareGames.CrazyFilter.Interfaces;
using System.Collections.Generic;

public class HandsPart : IBodyPart
{
    public string Name => "Hands";
    public int SortOrder => 2;

    public int GetPlaceCount() => 2; // Right and Left hands

    public Rect GetPlacementArea(ImageAnalysisData data, int place)
    {
        var key = place == 0 ? "right_hand" : "left_hand";

        if (!data.Landmarks.TryGetValue(key, out var hand))
        {
            return new Rect(0, 0, 0, 0);
        }

        double size = 100; // You can derive this from landmarks if available
        return new Rect(hand.X - size / 2, hand.Y - size / 2, size, size);
    }

    public bool ShouldFlip(ImageAnalysisData data, int place)
    {
        return place == 1; // Flip if it's the left hand
    }

    public float GetRotationAngle(ImageAnalysisData data, int place)
    {
        return 0f; // Can be enhanced using wrist/elbow landmarks
    }

    public float GetScaleFactor(ImageAnalysisData data, int place)
    {
        return 1.0f;
    }
}
