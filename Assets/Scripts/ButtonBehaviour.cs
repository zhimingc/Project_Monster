using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BUTTON_TYPE
{
  RESTART,
  HELP,
  MUSIC,
}

public class ButtonBehaviour : MonoBehaviour {

  public BUTTON_TYPE type;

  void OnMouseOver()
  {
    OnTouchDown();
  }

	public void OnTouchDown()
  {
    if (InputMan.OnDown())
      GameManager.Instance.ButtonBehaviour(type);
  }
}
