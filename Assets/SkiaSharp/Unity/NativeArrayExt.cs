using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace SkiaSharp.Unity
{
  public static class NativeArrayExt
  {
    /// <summary>
    ///   转换到 NativeArray
    /// </summary>
    /// <param name="span"></param>
    /// <param name="allocator"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static unsafe NativeArray<T> AsNativeArray<T>(this ReadOnlySpan<T> span,
      Allocator allocator = Allocator.TempJob) where T : unmanaged
    {
      var data = new NativeArray<T>(span.Length, allocator);
      fixed (void* source = span)
      {
        var dest = data.GetUnsafePtr();
        UnsafeUtility.MemCpy(dest, source, span.Length * UnsafeUtility.SizeOf<T>());
      }

      return data;
    }

    /// <summary>
    ///   转换到 NativeArray
    /// </summary>
    /// <param name="span"></param>
    /// <param name="allocator"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static unsafe NativeArray<T> AsNativeArray<T>(this Span<T> span,
      Allocator allocator = Allocator.TempJob) where T : unmanaged
    {
      var data = new NativeArray<T>(span.Length, allocator);
      fixed (void* source = span)
      {
        var dest = data.GetUnsafePtr();
        UnsafeUtility.MemCpy(dest, source, span.Length * UnsafeUtility.SizeOf<T>());
      }

      return data;
    }
  }
}