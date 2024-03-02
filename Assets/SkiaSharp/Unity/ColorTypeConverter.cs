using System;
using UnityEngine;

namespace SkiaSharp.Unity
{
  internal static class ColorTypeConverter
  {
    private static readonly SKColorType[] SkColorTypes =
    {
      SKColorType.Alpha8,
      SKColorType.Rgb565,
      SKColorType.Rgba8888,
      SKColorType.Rgb888x,
      SKColorType.Bgra8888,
      SKColorType.RgbaF16,
      SKColorType.RgbaF16Clamped,
      SKColorType.RgbaF32,
      SKColorType.Rg88,
      SKColorType.RgF16,
      SKColorType.Rg1616,
      SKColorType.Rgba16161616,

      SKColorType.Rgba1010102,
      SKColorType.Rgb101010x,
      SKColorType.Gray8,
      SKColorType.AlphaF16,
      SKColorType.Alpha16,
      SKColorType.Bgra1010102,
      SKColorType.Bgr101010x
    };

    private static readonly TextureFormat[] TextureFormats =
    {
      TextureFormat.Alpha8,
      TextureFormat.RGB565,
      TextureFormat.RGBA32,
      TextureFormat.RGBA32,
      TextureFormat.BGRA32,
      TextureFormat.RGBAHalf,
      TextureFormat.RGBAHalf,
      TextureFormat.RGBAFloat,
      TextureFormat.RG16,
      TextureFormat.RGHalf,
      TextureFormat.RG32,
      TextureFormat.RGBA64,

      TextureFormat.RGBA64,
      TextureFormat.RGBA64,
      TextureFormat.RGBA32,
      TextureFormat.RGBAHalf,
      TextureFormat.RGBA64,
      TextureFormat.RGBA64,
      TextureFormat.RGBA64
    };

    public static readonly int[] LInts = { 1, 2, 4, 4, 4, 8, 8, 16, 2, 4, 4, 8, 0, 0, 0, 0, 0, 0, 0 };


    public static int TryConvertToTextureFormat(this SKColorType skColorType, out TextureFormat textureFormat)
    {
      var index = Array.IndexOf(SkColorTypes, skColorType);
      if (index >= 0)
      {
        textureFormat = TextureFormats[index];
        return LInts[index];
      }

      textureFormat = TextureFormat.RGBA32;
      return 0;
    }

    public static int TryConvertSkColorTypes(this TextureFormat textureFormat, out SKColorType skColorType)
    {
      var index = Array.IndexOf(TextureFormats, textureFormat);
      if (index >= 0)
      {
        skColorType = SkColorTypes[index];
        return LInts[index];
      }

      skColorType = SKColorType.Rgba8888;
      return 0;
    }
  }
}