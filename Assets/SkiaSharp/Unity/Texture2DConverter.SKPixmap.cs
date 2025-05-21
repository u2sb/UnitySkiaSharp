// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global

using System;
using System.Buffers;
using System.Runtime.InteropServices;
using UnityEngine;

namespace SkiaSharp.Unity;

public static partial class Texture2DConverter
{
  public static Texture2D ToTexture2D(this SKPixmap pixmap, int width = 0, int height = 0,
    SKSamplingOptions? options = null)
  {
    var resize = width != 0 || height != 0;

    width = width == 0 ? pixmap.Width : width;
    height = height == 0 ? pixmap.Height : height;

    var l = pixmap.ColorType.TryConvertToTextureFormat(out var textureFormat);

    IntPtr ip = default;
    SKPixmap pixmap1;

    if (!resize)
    {
      pixmap1 = l > 0 ? pixmap : pixmap.WithColorType(SKColorType.Rgba8888);
    }
    else
    {
      if (l > 0)
      {
        ip = Marshal.AllocHGlobal(width * height * pixmap.ColorType.GetBytesPerPixel() * sizeof(byte));
        pixmap1 = new SKPixmap(new SKImageInfo(width, height, pixmap.ColorType), ip);
        pixmap.ScalePixels(pixmap1, options ?? SKSamplingOptions.Default);
      }
      else
      {
        ip = Marshal.AllocHGlobal(width * height * SKColorType.Rgba8888.GetBytesPerPixel() * sizeof(byte));
        pixmap1 = new SKPixmap(new SKImageInfo(width, height, SKColorType.Rgba8888), ip);
        pixmap.WithColorType(SKColorType.Rgba8888)
          .ScalePixels(pixmap1, options ?? SKSamplingOptions.Default);
      }
    }

    var data = pixmap1.GetPixelSpan();

    var dl = l > 0 ? l : 4;
    var writer = new ArrayBufferWriter<byte>(width * height * dl);

    for (var i = height - 1; i >= 0; i--) writer.Write(data.Slice(i * width * dl, width * dl));

    var texture2D = new Texture2D(width, height, textureFormat, false);
    texture2D.SetPixelData(writer.WrittenSpan.ToArray(), 0);

    Marshal.FreeHGlobal(ip);
    pixmap1.Dispose();

    texture2D.Apply();
    return texture2D;
  }
}