using System;
using UnityEngine;

namespace SkiaSharp.Unity
{
  internal static class ColorTypeConverter
  {
    private static readonly SKColorType[] SkColorTypes =
    {
      SKColorType.Rgba8888,
      SKColorType.Bgra8888
    };

    private static readonly TextureFormat[] TextureFormats =
    {
      TextureFormat.RGBA32,
      TextureFormat.BGRA32
    };


    public static bool TryConvertToTextureFormat(this SKColorType skColorType, out TextureFormat textureFormat)
    {
      var index = Array.IndexOf(SkColorTypes, skColorType);
      if (index >= 0)
      {
        textureFormat = TextureFormats[index];
        return true;
      }

      textureFormat = TextureFormat.RGBA32;
      return false;
    }

    public static bool TryConvertSkColorTypes(this TextureFormat textureFormat, out SKColorType skColorType)
    {
      var index = Array.IndexOf(TextureFormats, textureFormat);
      if (index >= 0)
      {
        skColorType = SkColorTypes[index];
        return true;
      }

      skColorType = SKColorType.Unknown;
      return false;
    }
  }
}