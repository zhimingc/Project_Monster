using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAnimation : MonoBehaviour {

  // Animation properties
  public float animTime;
  public GameObject requestObj;

  private float[] boundaries;
  private Vector3 origin, originScale;
  private BackgroundManager backMan;

  // Use this for initialization
  void Awake () {
    backMan = GameObject.Find("Background").GetComponent<BackgroundManager>();

    animTime = 0.5f;
    boundaries = backMan.cloudPosLimits;
    origin = transform.position;
    originScale = transform.localScale;
  }

  public void Hide()
  {
    // move out of frame
    transform.position = new Vector3(boundaries[1], 0, 0);
  }

  public void MoveInFrom(Vector3 from)
  {
    transform.position = from;

    //float offset = origin.x - boundaries[0];
    //transform.localScale += new Vector3(offset / 2.75f, 0, 0);

    //LeanTween.scale(gameObject, originScale, animTime);
    LeanTween.move(gameObject, origin, animTime);

    MonsterRequest req = requestObj.GetComponent<MonsterRequest>();
    if (req != null)
    {
      req.ToggleSpeechBubble(true);
    }
  }

  public void MoveOutFrom(Vector3 from)
  {
    Vector3 moveTo = from;
    moveTo.x = boundaries[1];

    transform.position = from;
    LeanTween.move(gameObject, moveTo, animTime);
  }

  public bool isAnimating()
  {
    return LeanTween.isTweening(gameObject);
  }
}
