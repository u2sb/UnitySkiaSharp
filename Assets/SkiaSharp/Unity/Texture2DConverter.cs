using System;
using System.Runtime.InteropServices;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global

namespace SkiaSharp.Unity;

public static partial class Texture2DConverter
{
  /// <summary>
  ///   从GUP读取贴图，注意，这里已经翻转过了，不需要再次翻转
  /// </summary>
  /// <param name="texture"></param>
  /// <returns></returns>
  public static Color32[] GetTextureColor32FromGpu(this Texture texture)
  {
    var width = texture.width;
    var height = texture.height;
#if UNITY_2023_2_OR_NEWER
      if (SystemInfo.IsFormatSupported(texture.graphicsFormat, GraphicsFormatUsage.ReadPixels))
#else
    if (SystemInfo.IsFormatSupported(texture.graphicsFormat, FormatUsage.ReadPixels))
#endif
    {
      var renderTexture = RenderTexture.GetTemporary(width, height, 32);
      VerticalFlipCopy(texture, renderTexture);
      var request = AsyncGPUReadback.Request(renderTexture, 0, texture.graphicsFormat);
      request.WaitForCompletion();
      if (request.hasError) throw new Exception("");
      var data = request.GetData<Color32>();
      RenderTexture.ReleaseTemporary(renderTexture);
      return data.ToArray();
    }
    else
    {
      var t2d = new Texture2D(width, height);
      var renderTexture = RenderTexture.GetTemporary(texture.width, texture.height, 32);
      VerticalFlipCopy(texture, renderTexture);
      var p = RenderTexture.active;
      RenderTexture.active = renderTexture;
      t2d.ReadPixels(new Rect(0, 0, width, height), 0, 0);
      RenderTexture.active = p;
      RenderTexture.ReleaseTemporary(renderTexture);
      return t2d.GetPixels32();
    }
  }
  
  /// <summary>
  ///   从GUP读取贴图，注意，这里已经翻转过了，不需要再次翻转
  /// </summary>
  /// <param name="texture"></param>
  /// <returns></returns>
  public static byte[] GetTextureDataFromGpu(this Texture texture)
  {
    var width = texture.width;
    var height = texture.height;
#if UNITY_2023_2_OR_NEWER
      if (SystemInfo.IsFormatSupported(texture.graphicsFormat, GraphicsFormatUsage.ReadPixels))
#else
    if (SystemInfo.IsFormatSupported(texture.graphicsFormat, FormatUsage.ReadPixels))
#endif
    {
      var renderTexture = RenderTexture.GetTemporary(width, height, 32);
      VerticalFlipCopy(texture, renderTexture);
      var request = AsyncGPUReadback.Request(renderTexture, 0, texture.graphicsFormat);
      request.WaitForCompletion();
      if (request.hasError) throw new Exception("");
      var data = request.GetData<byte>();
      RenderTexture.ReleaseTemporary(renderTexture);
      return data.ToArray();
    }

    else
    {
      var t2d = new Texture2D(width, height);
      var renderTexture = RenderTexture.GetTemporary(texture.width, texture.height, 32);
      VerticalFlipCopy(texture, renderTexture);
      var p = RenderTexture.active;
      RenderTexture.active = renderTexture;
      t2d.ReadPixels(new Rect(0, 0, width, height), 0, 0);
      RenderTexture.active = p;
      RenderTexture.ReleaseTemporary(renderTexture);
      return GetData(t2d);
    }


    byte[] GetData(Texture2D t2d)
    {
      var m0 = t2d.GetPixels32(0);
      var m1 = new ReadOnlySpan<Color32>(m0);
      var d = MemoryMarshal.Cast<Color32, byte>(m1);
      return d.ToArray();
    }
  }

  /// <summary>
  ///   从GPU读取数据 注意读到的贴图是反的
  /// </summary>
  /// <param name="texture"></param>
  /// <returns></returns>
  /// <exception cref="Exception"></exception>
  public static async UniTask<Color32[]> GetTextureColor32FromGpuAsync(this Texture texture)
  {
    var width = texture.width;
    var height = texture.height;

    await UniTask.SwitchToMainThread();

    if (SystemInfo.IsFormatSupported(texture.graphicsFormat, FormatUsage.ReadPixels))
    {
      var renderTexture = RenderTexture.GetTemporary(width, height, 32);
      VerticalFlipCopy(texture, renderTexture);
      var request = AsyncGPUReadback.Request(renderTexture, 0, texture.graphicsFormat);
      await request.ToUniTask();
      if (request.hasError) throw new Exception("");
      var data = request.GetData<Color32>();
      RenderTexture.ReleaseTemporary(renderTexture);
      return data.ToArray();
    }
    else
    {
      var t2d = new Texture2D(width, height);
      var renderTexture = RenderTexture.GetTemporary(texture.width, texture.height, 32);
      VerticalFlipCopy(texture, renderTexture);
      var p = RenderTexture.active;
      RenderTexture.active = renderTexture;
      t2d.ReadPixels(new Rect(0, 0, width, height), 0, 0);
      RenderTexture.active = p;
      RenderTexture.ReleaseTemporary(renderTexture);
      return t2d.GetPixels32();
    }
  }
  
  /// <summary>
  ///   从GPU读取数据 注意读到的贴图是反的
  /// </summary>
  /// <param name="texture"></param>
  /// <returns></returns>
  /// <exception cref="Exception"></exception>
  public static async UniTask<byte[]> GetTextureDataFromGpuAsync(this Texture texture)
  {
    var width = texture.width;
    var height = texture.height;

    await UniTask.SwitchToMainThread();

    if (SystemInfo.IsFormatSupported(texture.graphicsFormat, FormatUsage.ReadPixels))
    {
      var renderTexture = RenderTexture.GetTemporary(width, height, 32);
      VerticalFlipCopy(texture, renderTexture);
      var request = AsyncGPUReadback.Request(renderTexture, 0, texture.graphicsFormat);
      await request.ToUniTask();
      if (request.hasError) throw new Exception("");
      var data = request.GetData<byte>();
      RenderTexture.ReleaseTemporary(renderTexture);
      return data.ToArray();
    }
    else
    {
      var t2d = new Texture2D(width, height);
      var renderTexture = RenderTexture.GetTemporary(texture.width, texture.height, 32);
      VerticalFlipCopy(texture, renderTexture);
      var p = RenderTexture.active;
      RenderTexture.active = renderTexture;
      t2d.ReadPixels(new Rect(0, 0, width, height), 0, 0);
      RenderTexture.active = p;
      RenderTexture.ReleaseTemporary(renderTexture);
      return GetData(t2d);
    }

    byte[] GetData(Texture2D t2d)
    {
      var m0 = t2d.GetPixels32(0);
      var m1 = new ReadOnlySpan<Color32>(m0);
      var data = MemoryMarshal.Cast<Color32, byte>(m1);
      return data.ToArray();
    }
  }

  #region 翻转图片
  
  /// <summary>
  ///   翻转图片
  /// </summary>
  /// <param name="source"></param>
  /// <param name="width"></param>
  /// <param name="height"></param>
  /// <returns></returns>
  private static SKColor[] FlipRows(ReadOnlySpan<SKColor> source, int width, int height)
  {
    var rowLength = width;
    var totalBytes = height * rowLength;
    var result = new SKColor[totalBytes];
    var resultSpan = result.AsSpan();

    for (var y = 0; y < height / 2; y++)
    {
      var topOffset = y * rowLength;
      var bottomOffset = (height - y - 1) * rowLength;

      // 交换上下行
      source.Slice(bottomOffset, rowLength).CopyTo(resultSpan.Slice(topOffset, rowLength));
      source.Slice(topOffset, rowLength).CopyTo(resultSpan.Slice(bottomOffset, rowLength));
    }

    // 处理奇数行中间层
    if (height % 2 != 0)
    {
      var middleOffset = height / 2 * rowLength;
      source.Slice(middleOffset, rowLength).CopyTo(resultSpan.Slice(middleOffset, rowLength));
    }

    return result;
  }

  /// <summary>
  ///   翻转图片
  /// </summary>
  /// <param name="source"></param>
  /// <param name="width"></param>
  /// <param name="height"></param>
  /// <returns></returns>
  private static byte[] FlipRows(ReadOnlySpan<byte> source, int width, int height)
  {
    var rowLength = width * 4;
    var totalBytes = height * rowLength;
    var result = new byte[totalBytes];
    var resultSpan = result.AsSpan();

    for (var y = 0; y < height / 2; y++)
    {
      var topOffset = y * rowLength;
      var bottomOffset = (height - y - 1) * rowLength;

      // 交换上下行
      source.Slice(bottomOffset, rowLength).CopyTo(resultSpan.Slice(topOffset, rowLength));
      source.Slice(topOffset, rowLength).CopyTo(resultSpan.Slice(bottomOffset, rowLength));
    }

    // 处理奇数行中间层
    if (height % 2 != 0)
    {
      var middleOffset = height / 2 * rowLength;
      source.Slice(middleOffset, rowLength).CopyTo(resultSpan.Slice(middleOffset, rowLength));
    }

    return result;
  }

  /// <summary>
  ///   翻转图片
  /// </summary>
  /// <param name="ptr"></param>
  /// <param name="width"></param>
  /// <param name="height"></param>
  /// <returns></returns>
  private static unsafe byte[] FlipRows(byte* ptr, int width, int height)
  {
    var rowLength = width * 4;
    var result = new byte[height * rowLength];
    var resultSpan = result.AsSpan();

    for (var y = 0; y < height / 2; y++)
    {
      var topRowIndex = y * rowLength;
      var bottomRowIndex = (height - y - 1) * rowLength;

      // 从 ptr 中按需读取数据并写入到 resultSpan 中
      CopyRow(ptr + bottomRowIndex, resultSpan.Slice(topRowIndex, rowLength));
      CopyRow(ptr + topRowIndex, resultSpan.Slice(bottomRowIndex, rowLength));
    }

    // 如果高度是奇数，中间一行不需要移动，直接拷贝
    if (height % 2 != 0)
    {
      var middleRowIndex = height / 2 * rowLength;
      CopyRow(ptr + middleRowIndex, resultSpan.Slice(middleRowIndex, rowLength));
    }

    return result;

    static void CopyRow(byte* source, Span<byte> destination)
    {
      fixed (byte* destPtr = &MemoryMarshal.GetReference(destination))
      {
        Buffer.MemoryCopy(source, destPtr, destination.Length, destination.Length);
      }
    }
  }

  #endregion


  #region 拷贝贴图

  // Blit parameter to flip vertically 
  private static readonly Vector2 SVerticalScale = new(1f, -1f);

  private static readonly Vector2 SVerticalOffset = new(0f, 1f);

  // Blit parameter to flip horizontally 
  private static readonly Vector2 SHorizontalScale = new(-1f, 1f);

  private static readonly Vector2 SHorizontalOffset = new(1f, 0f);

  // Blit parameter to flip diagonally 
  private static readonly Vector2 SDiagonalScale = new(-1f, -1f);
  private static readonly Vector2 SDiagonalOffset = new(1f, 1f);

  public static void HorizontalFlipCopy(Texture source, RenderTexture dest)
  {
    Graphics.Blit(source, dest, SHorizontalScale, SHorizontalOffset);
  }

  public static void VerticalFlipCopy(Texture source, RenderTexture dest)
  {
    Graphics.Blit(source, dest, SVerticalScale, SVerticalOffset);
  }

  public static void DiagonalFlipCopy(Texture source, RenderTexture dest)
  {
    Graphics.Blit(source, dest, SDiagonalScale, SDiagonalOffset);
  }

  #endregion
}