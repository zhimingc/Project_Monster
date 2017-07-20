using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class GameFeel {

  static float originalCamSize;

  static public void GameFeelInit()
  {
    originalCamSize = Camera.main.orthographicSize;
  }

  static public void ShakeCameraRandom(Vector3 max, Vector3 min, int num, float time)
  {
    Vector3[] pos = new Vector3[num * 4];
    Vector3 camOrigin = Camera.main.transform.position;

    for (int i = 0; i < (num * 4) - 1; ++i)
    {
      pos[i] = new Vector3(Random.Range(min.x, max.x), Random.Range(min.y, max.y), 0.0f);
      pos[i] += camOrigin;
    }

    pos[num * 4 - 1] = camOrigin;
    LeanTween.move(Camera.main.gameObject, pos, time).setEase(LeanTweenType.easeOutQuad);
  }

  static public void ShakeCameraRandom(float max, float min, int num, float time)
  {
    Vector3[] pos = new Vector3[num * 4];
    Vector3 camOrigin = Camera.main.transform.position;

    for (int i = 0; i < (num * 4) - 1; ++i)
    {
      pos[i] = new Vector3(Random.Range(min, max), Random.Range(min, max), 0.0f);
      pos[i] += camOrigin;
    }

    pos[num * 4 - 1] = camOrigin;
    LeanTween.move(Camera.main.gameObject, pos, time).setEase(LeanTweenType.easeOutQuad);
  }

  static public void ZoomCamera(GameObject obj, float amt, float timeIn, float hold, float timeOut)
  {
    Camera.main.orthographicSize = originalCamSize;

    LeanTween.value(obj, (float curSize)=>
    {
      Camera.main.orthographicSize = curSize;
    }, 
    Camera.main.orthographicSize, originalCamSize + amt, timeIn).setEase(LeanTweenType.easeOutQuad);

    LeanTween.delayedCall(timeIn + hold, () =>
    {
      LeanTween.value(obj, (float curSize) =>
      {
        Camera.main.orthographicSize = curSize;
      },
    Camera.main.orthographicSize, originalCamSize, timeOut).setEase(LeanTweenType.easeOutQuad);
    });
  }
}
