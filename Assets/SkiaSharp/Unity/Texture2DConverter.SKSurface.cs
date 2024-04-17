using System.Runtime.InteropServices;
using UnityEngine;

namespace SkiaSharp.Unity
{
  public static partial class Texture2DConverter
  {
    public static Texture2D ToTexture2D(this SKSurface surface, int width, int height,
      SKSamplingOptions? options = null)
    {
      var ip = Marshal.AllocHGlobal(width * height * SKColorType.Rgba8888.GetBytesPerPixel() * sizeof(byte));
      using var pixmap = new SKPixmap(new SKImageInfo(width, height, SKColorType.Rgba8888), ip);
      surface.PeekPixels(pixmap);
      var texture2D = pixmap.ToTexture2D();
      Marshal.FreeHGlobal(ip);
      return texture2D;
    }
  }
}