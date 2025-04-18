namespace StupidWareGames.CrazyFilter.BodyParts;

using System;
using Avalonia;
using StupidWareGames.CrazyFilter.Interfaces;
using System.Collections.Generic;

public class GroinPart : IBodyPart
{
    public string Name => "Groin";
    public int SortOrder => 3;

    public int GetPlaceCount() => 1;

    public Rect GetPlacementArea(ImageAnalysisData data, int place)
    {
        if (!data.Landmarks.TryGetValue("left_hip", out var leftHip) ||
            !data.Landmarks.TryGetValue("right_hip", out var rightHip))
        {
            return new Rect(0, 0, 0, 0);
        }

        var centerX = (leftHip.X + rightHip.X) / 2;
        var centerY = (leftHip.Y + rightHip.Y) / 2;

        var width = Math.Abs(rightHip.X - leftHip.X) * 1.4;
        var height = width * 1.2;

        return new Rect(centerX - width / 2, centerY - height / 2, width, height);
    }

    public bool ShouldFlip(ImageAnalysisData data, int place)
    {
        return false;
    }

    public float GetRotationAngle(ImageAnalysisData data, int place)
    {
        if (!data.Landmarks.TryGetValue("left_hip", out var leftHip) ||
            !data.Landmarks.TryGetValue("right_hip", out var rightHip))
        {
            return 0f;
        }

        return (float)(Math.Atan2(rightHip.Y - leftHip.Y, rightHip.X - leftHip.X) * (180.0 / Math.PI));
    }

    public float GetScaleFactor(ImageAnalysisData data, int place)
    {
        return 1.0f;
    }
}
