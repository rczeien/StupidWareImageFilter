namespace StupidWareGames.CrazyFilter.Utils;

using System.Collections.Generic;
using OpenCvSharp;

public static class CameraService
{
    public static List<string> ListAvailableCameras(int maxTest = 10)
    {
        var list = new List<string>();

        for (int i = 0; i < maxTest; i++)
        {
            using var cap = new VideoCapture(i);
            if (cap.IsOpened())
                list.Add($"Camera {i}");
        }

        return list;
    }
}
