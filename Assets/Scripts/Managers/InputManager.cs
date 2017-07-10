using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class InputMan {

  static public RuntimePlatform platform;

	static public bool OnDown()
  {
    bool res = false;
    switch (platform)
    {
      case RuntimePlatform.Android:
      case RuntimePlatform.IPhonePlayer:
        res = Input.GetTouch(0).phase == TouchPhase.Began;
        break;
      default:
      //case RuntimePlatform.WebGLPlayer:
      //case RuntimePlatform.WindowsPlayer:
      //case RuntimePlatform.OSXPlayer:
        res = Input.GetMouseButtonDown(0);
        break;
    }
    return res;
  }

  static public bool OnUp()
  {
    bool res = false;
    switch (platform)
    {
      case RuntimePlatform.Android:
      case RuntimePlatform.IPhonePlayer:
        res = Input.GetTouch(0).phase == TouchPhase.Ended;
        break;
      default:
        //case RuntimePlatform.WebGLPlayer:
        //case RuntimePlatform.WindowsPlayer:
        //case RuntimePlatform.OSXPlayer:
        res = Input.GetMouseButtonUp(0);
        break;
    }
    return res;
  }

  static public Vector2 InputPos()
  {
    Vector2 res = new Vector2();
    switch (platform)
    {
      case RuntimePlatform.Android:
      case RuntimePlatform.IPhonePlayer:
        res = Input.GetTouch(0).position;
        break;
      case RuntimePlatform.WebGLPlayer:
      case RuntimePlatform.WindowsPlayer:
      case RuntimePlatform.OSXPlayer:
        res = Input.mousePosition;
        break;
    }
    return res;
  }
}
