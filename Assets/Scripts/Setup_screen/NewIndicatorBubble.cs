using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void ClickBehaviour();

public class NewIndicatorBubble : MonoBehaviour {

  public ClickBehaviour clickFn;

  // Use this for initialization
  void Start () {
    //clickFn = null;
	}

  void OnMouseOver()
  {
    OnTouchStay();
  }

  public void OnTouchStay()
  {
    if (InputMan.OnDown())
    {
      if (clickFn != null) clickFn();
      gameObject.SetActive(false);
    }
  }

  public void SetClickFn(ClickBehaviour fn)
  {
    clickFn = fn;
  }
}
