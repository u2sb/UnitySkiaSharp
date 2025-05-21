using System;
using Unity.Collections;
using UnityEngine;

// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global

namespace SkiaSharp.Unity;

public static partial class Texture2DConverter
{
  public static Texture2D ToTexture2D(this SKBitmap bitmap, int width = 0, int height = 0,
    SKSamplingOptions? options = null)
  {
    var resize = width != 0 || height != 0;

    width = width == 0 ? bitmap.Width : width;
    height = height == 0 ? bitmap.Height : height;

    if (resize) bitmap = bitmap.Resize(new SKSizeI(width, height), options ?? SKSamplingOptions.Default);

    Texture2D texture2D;
    var l = bitmap.ColorType.TryConvertToTextureFormat(out var textureFormat);
    if (l > 0)
    {
      var data = bitmap.GetPixelSpan();

      var result = FlipRows(data, width, height);

      texture2D = new Texture2D(width, height, textureFormat, false);
      texture2D.SetPixelData(result, 0);
    }
    else
    {
      var data0 = bitmap.Pixels.AsSpan();

      var data1 = FlipRows(data0, width, height).AsSpan().AsNativeArray();
      var colors = ColorConverter.ConvertToColor32(data1, width * 64);

      texture2D = new Texture2D(width, height, textureFormat, false);
      texture2D.SetPixels32(colors.ToArray());

      data1.Dispose();
      colors.Dispose();
    }

    texture2D.Apply();
    return texture2D;
  }

  public static SKBitmap ToSkBitmap(this Texture2D texture2D, int width = 0, int height = 0,
    SKSamplingOptions? options = null)
  {
    var resize = width != 0 || height != 0;
    width = width == 0 ? texture2D.width : width;
    height = height == 0 ? texture2D.height : height;

    var l = texture2D.format.TryConvertSkColorTypes(out var skColorType);
    var bitmap = new SKBitmap(texture2D.width, texture2D.height, skColorType, SKAlphaType.Premul);

    if (l > 0 && texture2D.isReadable)
    {
      ReadOnlySpan<byte> data = texture2D.GetPixelData<byte>(0);
      var data1 = FlipRows(data, width, height);

      unsafe
      {
        fixed (byte* ptr = &data1[0])
        {
          bitmap.SetPixels((IntPtr)ptr);
        }
      }
    }
    else if (l > 0)
    {
      ReadOnlySpan<byte> data = GetTextureDataFromGpu(texture2D);

      unsafe
      {
        fixed (byte* ptr = data)
        {
          bitmap.SetPixels((IntPtr)ptr);
        }
      }
    }
    else
    {
      var data0 = texture2D.isReadable ? texture2D.GetPixels32() : texture2D.GetTextureColor32FromGpu();
      var data1 = new NativeArray<Color32>(data0, Allocator.TempJob);

      var data2 = ColorConverter.ConvertToSkColor(data1, width * 64);

      bitmap.Pixels = data2.ToArray();
      data1.Dispose();
      data2.Dispose();
    }

    if (resize) bitmap = bitmap.Resize(new SKSizeI(width, height), options ?? SKSamplingOptions.Default);

    return bitmap;
  }
}