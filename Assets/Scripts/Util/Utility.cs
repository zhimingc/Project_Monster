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
}
