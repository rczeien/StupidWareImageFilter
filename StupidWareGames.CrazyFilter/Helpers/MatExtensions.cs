using OpenCvSharp;
using System.IO;
public static class MatExtensions
{
    public static MemoryStream ToMemoryStream(this Mat mat)
    {
        Cv2.ImEncode(".bmp", mat, out var data);
        return new MemoryStream(data);
    }
}
