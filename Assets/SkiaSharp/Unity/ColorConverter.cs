using UnityEngine;

namespace SkiaSharp.Unity
{
  /// <summary>
  ///   颜色转换
  /// </summary>
  public static class ColorConverter
  {
    /// <summary>
    ///   转换到Unity颜色
    /// </summary>
    /// <param name="skColorF"></param>
    /// <returns></returns>
    public static Color ToUnityColor(this SKColorF skColorF)
    {
      return new Color(skColorF.Red, skColorF.Green, skColorF.Blue, skColorF.Alpha);
    }

    /// <summary>
    ///   转换到Unity颜色
    /// </summary>
    /// <param name="skColor"></param>
    /// <returns></returns>
    public static Color32 ToUnityColor32(this SKColor skColor)
    {
      return new Color32(skColor.Red, skColor.Green, skColor.Blue, skColor.Alpha);
    }

    /// <summary>
    ///   转换到SKColorF
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    public static SKColorF ToSkColorF(this Color color)
    {
      return new SKColorF(color.r, color.g, color.b, color.a);
    }

    /// <summary>
    ///   转换到SKColor
    /// </summary>
    /// <param name="color32"></param>
    /// <returns></returns>
    public static SKColor ToSkColor(this Color32 color32)
    {
      return new SKColor(color32.r, color32.g, color32.b, color32.a);
    }
  }
}