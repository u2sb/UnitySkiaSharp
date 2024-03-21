using System;
using System.Buffers;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

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
        var data0 = bitmap.Pixels.AsSpan();
        var writer = new ArrayBufferWriter<SKColor>();

        for (var i = height - 1; i >= 0; i--) writer.Write(data0.Slice(i * width, width));

        var data1 = writer.WrittenSpan.AsNativeArray();
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

      SKBitmap bitmap;
      var l = texture2D.format.TryConvertSkColorTypes(out var skColorType);

      if (l > 0 && texture2D.isReadable)
      {
        ReadOnlySpan<byte> data = texture2D.GetPixelData<byte>(0);

        var writer = new ArrayBufferWriter<byte>(width * height * l);

        for (var i = height - 1; i >= 0; i--) writer.Write(data.Slice(i * width * l, width * l));

        var span = writer.WrittenSpan;
        unsafe
        {
          fixed (byte* ptr = span)
          {
            bitmap = new SKBitmap(width, height, skColorType, SKAlphaType.Premul);
            bitmap.SetPixels((IntPtr)ptr);
          }
        }
      }
      else if (l > 0 && texture2D.GetTextureDataFromGpu(out var textureData))
      {
        ReadOnlySpan<byte> data = textureData;

        var writer = new ArrayBufferWriter<byte>(width * height * l);

        for (var i = height - 1; i >= 0; i--) writer.Write(data.Slice(i * width * l, width * l));

        var span = writer.WrittenSpan;
        unsafe
        {
          fixed (byte* ptr = span)
          {
            bitmap = new SKBitmap(width, height, skColorType, SKAlphaType.Premul);
            bitmap.SetPixels((IntPtr)ptr);
          }
        }
      }
      else
      {
        var data0 = (texture2D.isReadable ? texture2D : texture2D.GetTextureFromGpu()).GetPixels32();
        var data1 = new NativeArray<Color32>(data0, Allocator.TempJob);

        var data2 = ColorConverter.ConvertToSkColor(data1, width * 64);
        var skColors = data2.AsSpan();

        var writer = new ArrayBufferWriter<SKColor>();

        for (var i = height - 1; i >= 0; i--) writer.Write(skColors.Slice(i * width, width));

        bitmap = new SKBitmap(texture2D.width, texture2D.height, skColorType, SKAlphaType.Premul);

        bitmap.Pixels = writer.WrittenSpan.ToArray();
        data1.Dispose();
        data2.Dispose();
      }

      if (resize) bitmap = bitmap.Resize(new SKSizeI(width, height), options ?? SKSamplingOptions.Default);

      return bitmap;
    }

    /// <summary>
    ///   从GUP读取贴图
    /// </summary>
    /// <param name="texture"></param>
    /// <returns></returns>
    public static Texture2D GetTextureFromGpu(this Texture texture)
    {
      var width = texture.width;
      var height = texture.height;
      Texture2D tex2;
      if (texture.GetTextureDataFromGpu(out var data))
      {
        tex2 = new Texture2D(width, height, texture.graphicsFormat, TextureCreationFlags.None);
        tex2.SetPixelData(data, 0);
      }
      else
      {
        var renderTexture = new RenderTexture(width, height, 32);
        Graphics.Blit(texture, renderTexture);
        var tmpTexture = RenderTexture.active;
        RenderTexture.active = renderTexture;
        tex2 = new Texture2D(width, height);
        tex2.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        RenderTexture.active = tmpTexture;
      }

      return tex2;
    }

    /// <summary>
    ///   从GPU读取数据
    /// </summary>
    /// <param name="texture"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static bool GetTextureDataFromGpu(this Texture texture, out NativeArray<byte> data)
    {
#if UNITY_2023_2_OR_NEWER
      if (SystemInfo.IsFormatSupported(texture.graphicsFormat, GraphicsFormatUsage.ReadPixels))
#else
      if (SystemInfo.IsFormatSupported(texture.graphicsFormat, FormatUsage.ReadPixels))
#endif
      {
        var request = AsyncGPUReadback.Request(texture, 0, texture.graphicsFormat);
        request.WaitForCompletion();
        if (request.hasError) throw new Exception("");
        data = request.GetData<byte>();
        return true;
      }

      data = new NativeArray<byte>();
      return false;
    }
  }
}