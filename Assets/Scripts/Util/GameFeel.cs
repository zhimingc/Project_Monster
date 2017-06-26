using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class GameFeel {

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
}
