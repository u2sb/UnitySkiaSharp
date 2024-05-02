using UnityEngine;

namespace SkiaSharp.Unity
{
  internal static class ColorTypeConverter
  {
    public static int TryConvertToTextureFormat(this SKColorType skColorType, out TextureFormat textureFormat)
    {
      switch (skColorType)
      {
        case SKColorType.Alpha8:
          textureFormat = TextureFormat.Alpha8;
          return 1;
        case SKColorType.Rgb565:
          textureFormat = TextureFormat.RGB565;
          return 2;
        case SKColorType.Rgba8888:
        case SKColorType.Rgb888x:
          textureFormat = TextureFormat.RGBA32;
          return 4;
        case SKColorType.Bgra8888:
          textureFormat = TextureFormat.BGRA32;
          return 4;
        case SKColorType.RgbaF16:
        case SKColorType.RgbaF16Clamped:
          textureFormat = TextureFormat.RGBAHalf;
          return 8;
        case SKColorType.RgbaF32:
          textureFormat = TextureFormat.RGBAFloat;
          return 16;
        case SKColorType.Rg88:
          textureFormat = TextureFormat.RG16;
          return 2;
        case SKColorType.RgF16:
          textureFormat = TextureFormat.RGHalf;
          return 4;
        case SKColorType.Rg1616:
          textureFormat = TextureFormat.RG32;
          return 4;
        case SKColorType.Rgba16161616:
          textureFormat = TextureFormat.RGBA64;
          return 8;
        case SKColorType.Gray8:
          textureFormat = TextureFormat.RGBA32;
          break;
        case SKColorType.AlphaF16:
          textureFormat = TextureFormat.RGBAHalf;
          break;
        case SKColorType.Alpha16:
        case SKColorType.Rgba1010102:
        case SKColorType.Rgb101010x:
        case SKColorType.Bgra1010102:
        case SKColorType.Bgr101010x:
          textureFormat = TextureFormat.RGBA64;
          break;
        default:
          textureFormat = TextureFormat.RGBA32;
          break;
      }

      return -1;
    }

    public static int TryConvertSkColorTypes(this TextureFormat textureFormat, out SKColorType skColorType)
    {
      switch (textureFormat)
      {
        case TextureFormat.Alpha8:
          skColorType = SKColorType.Alpha8;
          return 1;
        case TextureFormat.RGB565:
          skColorType = SKColorType.Rgb565;
          return 2;
        case TextureFormat.RGBA32:
          skColorType = SKColorType.Rgba8888;
          return 4;
        case TextureFormat.RGBAHalf:
          skColorType = SKColorType.RgbaF16;
          return 8;
        case TextureFormat.RGBAFloat:
          skColorType = SKColorType.RgbaF32;
          return 16;
        case TextureFormat.RG16:
          skColorType = SKColorType.Rg88;
          return 2;
        case TextureFormat.RGHalf:
          skColorType = SKColorType.RgF16;
          return 4;
        case TextureFormat.RG32:
          skColorType = SKColorType.Rg1616;
          return 4;
        case TextureFormat.RGBA64:
          skColorType = SKColorType.Rgba16161616;
          return 8;

        default:
          skColorType = SKColorType.Rgba8888;
          break;
      }

      return -1;
    }
  }
}