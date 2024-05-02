using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using SkiaSharp;
using SkiaSharp.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace Tester
{
  [RequireComponent(typeof(RawImage))]
  public class SkiaSharpTester : MonoBehaviour
  {
    [SerializeField] private Texture2D t2d;

    // ReSharper disable once Unity.IncorrectMethodSignature
    [UsedImplicitly]
    private async UniTaskVoid Start()
    {
      var rawImage = GetComponent<RawImage>();

      var white = Texture2D.whiteTexture;

      var bitmap = t2d.ToSkBitmap();
      var t2d0 = bitmap.ToTexture2D();

      rawImage.texture = t2d0;
      
      await UniTask.WaitForSeconds(3);
      rawImage.texture = white;
      await UniTask.WaitForSeconds(1);
      
      var surface = SKSurface.Create(bitmap.Info);
      var canvas = surface.Canvas;
      canvas.DrawBitmap(bitmap, bitmap.Info.Rect);
      var t2d1 = surface.ToTexture2D(bitmap.Width, bitmap.Height);
      rawImage.texture = t2d1;
      
      await UniTask.WaitForSeconds(3);
      rawImage.texture = white;
      await UniTask.WaitForSeconds(1);

      var image = surface.Snapshot();
      var t2d2 = image.ToTexture2D(bitmap.Width, bitmap.Height);
      rawImage.texture = t2d2;
    }
  }
}