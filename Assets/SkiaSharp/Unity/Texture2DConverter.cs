using System;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global

namespace SkiaSharp.Unity
{
  public static partial class Texture2DConverter
  {
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