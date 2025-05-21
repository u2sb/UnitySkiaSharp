using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace SkiaSharp.Unity;

/// <summary>
///   颜色转换
/// </summary>
public static class ColorConverter
{
  /// <summary>
  ///   转换到 Unity 颜色
  /// </summary>
  /// <param name="skColorF"></param>
  /// <returns></returns>
  public static Color ToUnityColor(this SKColorF skColorF)
  {
    return new Color(skColorF.Red, skColorF.Green, skColorF.Blue, skColorF.Alpha);
  }

  /// <summary>
  ///   转换到 Unity 颜色
  /// </summary>
  /// <param name="skColor"></param>
  /// <returns></returns>
  public static Color32 ToUnityColor32(this SKColor skColor)
  {
    return new Color32(skColor.Red, skColor.Green, skColor.Blue, skColor.Alpha);
  }

  /// <summary>
  ///   转换到 SKColorF
  /// </summary>
  /// <param name="color"></param>
  /// <returns></returns>
  public static SKColorF ToSkColorF(this Color color)
  {
    return new SKColorF(color.r, color.g, color.b, color.a);
  }

  /// <summary>
  ///   转换到 SKColor
  /// </summary>
  /// <param name="color32"></param>
  /// <returns></returns>
  public static SKColor ToSkColor(this Color32 color32)
  {
    return new SKColor(color32.r, color32.g, color32.b, color32.a);
  }

  /// <summary>
  ///   批量转换到 Color32
  /// </summary>
  /// <param name="colors"></param>
  /// <param name="batchCount"></param>
  /// <returns></returns>
  public static NativeArray<Color32> ConvertToColor32(NativeArray<SKColor> colors, int batchCount = 512)
  {
    var handle = FastColorConverter(colors.Reinterpret<uint>(), out var data, batchCount);
    handle.Complete();
    return data.Reinterpret<Color32>();
  }

  /// <summary>
  ///   批量转换到 SKColor
  /// </summary>
  /// <param name="colors"></param>
  /// <param name="batchCount"></param>
  /// <returns></returns>
  public static NativeArray<SKColor> ConvertToSkColor(NativeArray<Color32> colors, int batchCount = 512)
  {
    var handle = FastColorConverter(colors.Reinterpret<uint>(), out var data, batchCount);
    handle.Complete();
    return data.Reinterpret<SKColor>();
  }

  /// <summary>
  ///   快速转换颜色
  /// </summary>
  /// <param name="dataIn"></param>
  /// <param name="dataOut"></param>
  /// <param name="batchCount"></param>
  public static JobHandle FastColorConverter(NativeArray<uint> dataIn, out NativeArray<uint> dataOut,
    int batchCount = 512)
  {
    dataOut = new NativeArray<uint>(dataIn.Length, Allocator.TempJob);

    var job = new ColorConverterJob
    {
      DataIn = dataIn,
      DataOut = dataOut
    };
    return job.Schedule(dataIn.Length, batchCount);
  }

  [BurstCompile]
  private struct ColorConverterJob : IJobParallelFor
  {
    [ReadOnly] public NativeArray<uint> DataIn;
    [WriteOnly] public NativeArray<uint> DataOut;

    private const uint Mask0 = 0x00FF0000;
    private const uint Mask1 = 0x000000FF;

    public void Execute(int index)
    {
      var color = DataIn[index];

      DataOut[index] = ((color & Mask0) >> 16) | ((color & Mask1) << 16) | (color & ~(Mask0 | Mask1));
    }
  }
}