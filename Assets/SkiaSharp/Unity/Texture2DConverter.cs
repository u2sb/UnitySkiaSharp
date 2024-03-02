using System;
using System.Buffers;
using System.Linq;
using UnityEngine;

namespace SkiaSharp.Unity
{
  public static class Texture2DConverter
  {
    public static Texture2D ToTexture2D(this SKBitmap bitmap, int width = 0, int height = 0,
      SKSamplingOptions? options = null)
    {
      var resize = width != 0 || height != 0;

      width = width == 0 ? bitmap.Width : width;
      height = height == 0 ? bitmap.Height : height;

      if (resize) bitmap = bitmap.Resize(new SKSizeI(width, height), options ?? SKSamplingOptions.Default);

      Texture2D texture2D;

      if (bitmap.ColorType.TryConvertToTextureFormat(out var textureFormat))
      {
        var data = bitmap.GetPixelSpan();
        var writer = new ArrayBufferWriter<byte>(width * height * 4);

        for (var i = height - 1; i >= 0; i--) writer.Write(data.Slice(i * width * 4, width * 4));

        texture2D = new Texture2D(width, height, textureFormat, false);
        texture2D.SetPixelData(writer.WrittenSpan.ToArray(), 0);
      }
      else
      {
        var data = bitmap.Pixels.AsSpan();
        var writer = new ArrayBufferWriter<SKColor>();

        for (var i = height - 1; i >= 0; i--) writer.Write(data.Slice(i * width, width));

        var colors = writer.WrittenSpan.ToArray().Select(s => s.ToUnityColor32()).ToArray();

        texture2D = new Texture2D(width, height, TextureFormat.RGBA32, false);
        texture2D.SetPixels32(colors);
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

      SKBitmap bitmap;

      if (texture2D.isReadable)
      {
        if (texture2D.format.TryConvertSkColorTypes(out var skColorType))
        {
          texture2D.GetNativeTexturePtr()
          var data = texture2D.GetPixelData<byte>(0).AsReadOnlySpan();

          var writer = new ArrayBufferWriter<byte>(width * height * 4);

          for (var i = height - 1; i >= 0; i--) writer.Write(data.Slice(i * width * 4, width * 4));

          var span = writer.WrittenSpan;
          var spanPtr = (IntPtr)span.GetPinnableReference();

          bitmap = new SKBitmap(texture2D.width, texture2D.height, skColorType, SKAlphaType.Premul);
          bitmap.SetPixels(spanPtr);
        }
        else
        {
          var data = texture2D.GetPixels32();
          var skColors = data.Select(s => s.ToSkColor()).ToArray().AsSpan();

          var writer = new ArrayBufferWriter<SKColor>();

          for (var i = height - 1; i >= 0; i--) writer.Write(skColors.Slice(i * width, width));

          bitmap = new SKBitmap(texture2D.width, texture2D.height, SKColorType.Rgba8888, SKAlphaType.Premul);

          bitmap.Pixels = writer.WrittenSpan.ToArray();
        }
      }
      else
      {
        var data = texture2D.EncodeToPNG();
        bitmap = SKBitmap.Decode(data);
      }
      

      if (resize) bitmap = bitmap.Resize(new SKSizeI(width, height), options ?? SKSamplingOptions.Default);

      return bitmap;
    }
  }
}