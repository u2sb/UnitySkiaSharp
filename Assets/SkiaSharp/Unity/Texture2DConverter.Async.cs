using System;
using System.Buffers;
using System.Linq;
using System.Threading.Tasks;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace SkiaSharp.Unity
{
  public static partial class Texture2DConverter
  {
    public static async ValueTask<SKBitmap> ToSkBitmapAsync(this Texture2D texture2D, int width = 0, int height = 0,
      SKSamplingOptions? options = null)
    {
      var resize = width != 0 || height != 0;
      width = width == 0 ? texture2D.width : width;
      height = height == 0 ? texture2D.height : height;

      SKBitmap bitmap;
      var l = texture2D.format.TryConvertSkColorTypes(out var skColorType);

      if (l > 0)
      {
        var data = texture2D.isReadable
          ? texture2D.GetPixelData<byte>(0)
          : await texture2D.GetTextureFromGpuAsync();

        var writer = new ArrayBufferWriter<byte>(width * height * l);

        for (var i = height - 1; i >= 0; i--) writer.Write(data.GetSubArray(i * width * l, width * l));

        var memory = writer.WrittenMemory;

        using var memoryHandle = memory.Pin();
        unsafe
        {
          var ptr = memoryHandle.Pointer;
          bitmap = new SKBitmap(width, height, skColorType, SKAlphaType.Premul);
          bitmap.SetPixels((IntPtr)ptr);
        }
      }
      else
      {
        var data = (texture2D.isReadable ? texture2D : texture2D.GetTextureFromGpu()).GetPixels32();

        var skColors = data.Select(s => s.ToSkColor()).ToArray().AsMemory();

        var writer = new ArrayBufferWriter<SKColor>();

        for (var i = height - 1; i >= 0; i--) writer.Write(skColors.Slice(i * width, width).Span);

        bitmap = new SKBitmap(texture2D.width, texture2D.height, skColorType, SKAlphaType.Premul);

        bitmap.Pixels = writer.WrittenSpan.ToArray();
      }

      if (resize) bitmap = bitmap.Resize(new SKSizeI(width, height), options ?? SKSamplingOptions.Default);

      return bitmap;
    }

    /// <summary>
    ///   从 GPU 获取数据
    /// </summary>
    /// <param name="texture2D"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private static async ValueTask<NativeArray<byte>> GetTextureFromGpuAsync(this Texture2D texture2D)
    {
      var request = await AsyncGPUReadback.RequestAsync(texture2D, 0, texture2D.graphicsFormat);
      if (request.hasError) throw new Exception("");
      return request.GetData<byte>();
    }
  }
}