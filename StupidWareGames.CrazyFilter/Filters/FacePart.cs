namespace StupidWareGames.CrazyFilter.BodyParts;

using System;
using Avalonia;
using StupidWareGames.CrazyFilter.Interfaces;
using System.Collections.Generic;

public class FacePart : IBodyPart
{
    public string Name => "Face";
    public int SortOrder => 0;

    public int GetPlaceCount() => 1;

    public Rect GetPlacementArea(ImageAnalysisData data, int place)
    {
        if (!data.Landmarks.TryGetValue("left_eye", out var leftEye) ||
            !data.Landmarks.TryGetValue("right_eye", out var rightEye) ||
            !data.Landmarks.TryGetValue("nose_tip", out var nose))
        {
            return new Rect(0, 0, 0, 0);
        }

        double eyeDistance = Math.Abs(rightEye.X - leftEye.X);
        double width = eyeDistance * 2.2;
        double height = width;

        var centerX = (leftEye.X + rightEye.X) / 2;
        var centerY = (leftEye.Y + rightEye.Y) / 2 - height * 0.3;

        return new Rect(centerX - width / 2, centerY - height / 2, width, height);
    }

    public bool ShouldFlip(ImageAnalysisData data, int place)
    {
        return false;
    }

    public float GetRotationAngle(ImageAnalysisData data, int place)
    {
        if (!data.Landmarks.TryGetValue("left_eye", out var leftEye) ||
            !data.Landmarks.TryGetValue("right_eye", out var rightEye))
        {
            return 0f;
        }

        return (float)(Math.Atan2(rightEye.Y - leftEye.Y, rightEye.X - leftEye.X) * (180.0 / Math.PI));
    }

    public float GetScaleFactor(ImageAnalysisData data, int place)
    {
        return 1.0f;
    }
}
