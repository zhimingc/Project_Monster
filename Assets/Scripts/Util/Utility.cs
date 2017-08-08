using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class Utility {

	static public void SetColorFromHex(GameObject obj, string hex)
  {
    Color tmpColor = new Color();
    ColorUtility.TryParseHtmlString(hex, out tmpColor);
    obj.GetComponent<SpriteRenderer>().color = tmpColor;
  }

  static public Color GetColorFromHex(string hex)
  {
    Color tmpColor = new Color();
    ColorUtility.TryParseHtmlString(hex, out tmpColor);
    return tmpColor;
  }

  static public void Swap<T>(ref T lhs, ref T rhs)
  {
    T temp;
    temp = lhs;
    lhs = rhs;
    rhs = temp;
  }
}

[System.Serializable]
public class Pair<T, U>
{
  public Pair()
  {
  }

  public Pair(T initFirst, U initSecond)
  {
    first = initFirst;
    second = initSecond;
  }

  public T first;
  public U second;
}
