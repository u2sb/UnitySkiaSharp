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
      var l = bitmap.ColorType.TryConvertToTextureFormat(out var textureFormat);
      if (l > 0)
      {
        var data = bitmap.GetPixelSpan();
        var writer = new ArrayBufferWriter<byte>(width * height * l);

        for (var i = height - 1; i >= 0; i--) writer.Write(data.Slice(i * width * l, width * l));

        texture2D = new Texture2D(width, height, textureFormat, false);
        texture2D.SetPixelData(writer.WrittenSpan.ToArray(), 0);
      }
      else
      {
        var data = bitmap.Pixels.AsSpan();
        var writer = new ArrayBufferWriter<SKColor>();

        for (var i = height - 1; i >= 0; i--) writer.Write(data.Slice(i * width, width));

        var colors = writer.WrittenSpan.ToArray().Select(s => s.ToUnityColor32()).ToArray();

        texture2D = new Texture2D(width, height, textureFormat, false);
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
      var l = texture2D.format.TryConvertSkColorTypes(out var skColorType);

      if (l > 0)
      {
        unsafe
        {
          var data = texture2D.isReadable
            ? texture2D.GetPixelData<byte>(0).AsReadOnlySpan()
            : texture2D.GetRawTextureData<byte>().AsReadOnlySpan();


          var writer = new ArrayBufferWriter<byte>(width * height * l);

          for (var i = height - 1; i >= 0; i--) writer.Write(data.Slice(i * width * l, width * l));

          var span = writer.WrittenSpan;

          fixed (byte* ptr = span)
          {
            bitmap = new SKBitmap(texture2D.width, texture2D.height, skColorType, SKAlphaType.Premul);
            bitmap.SetPixels((IntPtr)ptr);
          }
        }
      }
      else
      {
        if (texture2D.isReadable)
        {
          var data = texture2D.GetPixels32();
          var skColors = data.Select(s => s.ToSkColor()).ToArray().AsSpan();

          var writer = new ArrayBufferWriter<SKColor>();

          for (var i = height - 1; i >= 0; i--) writer.Write(skColors.Slice(i * width, width));

          bitmap = new SKBitmap(texture2D.width, texture2D.height, skColorType, SKAlphaType.Premul);

          bitmap.Pixels = writer.WrittenSpan.ToArray();
        }
        else
        {
          var data = texture2D.GetRawTextureData<byte>();
          var png = ImageConversion.EncodeNativeArrayToPNG(data, texture2D.graphicsFormat, (uint)width, (uint)height);
          bitmap = SKBitmap.Decode(png);
        }
      }

      if (resize) bitmap = bitmap.Resize(new SKSizeI(width, height), options ?? SKSamplingOptions.Default);

      return bitmap;
    }
  }
}