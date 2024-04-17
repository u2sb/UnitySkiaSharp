using System.Runtime.InteropServices;
using UnityEngine;

namespace SkiaSharp.Unity
{
  public static partial class Texture2DConverter
  {
    public static Texture2D ToTexture2D(this SKImage image, int width = 0, int height = 0,
      SKSamplingOptions? options = null)
    {
      var resize = width != 0 || height != 0;

      width = width == 0 ? image.Width : width;
      height = height == 0 ? image.Height : height;

      var ip = Marshal.AllocHGlobal(width * height * image.ColorType.GetBytesPerPixel() * sizeof(byte));
      using var pixmap = new SKPixmap(new SKImageInfo(width, height, image.ColorType), ip);

      if (resize) image.ScalePixels(pixmap, options ?? SKSamplingOptions.Default);
      else if (!image.PeekPixels(pixmap)) image.ReadPixels(pixmap);

      var texture2D = pixmap.ToTexture2D();
      
      Marshal.FreeHGlobal(ip);
      return texture2D;
    }
  }
}