using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BUTTON_TYPE
{
  RESTART,
  HELP,
  MUSIC,
  START,
  START_HELP_BASIC,
  START_HELP_TIPS,
  SETTINGS,
  CREDITS,
  TO_START,
  TOG_MUSIC,
  TOG_SFX
}

public class ButtonBehaviour : MonoBehaviour {

  public BUTTON_TYPE type;

  void Start()
  {
    GameManager.Instance.ButtonInit(type, this);
  }

  void OnMouseOver()
  {
    OnTouchDown();
  }

	public void OnTouchDown()
  {
    if (InputMan.OnDown())
      GameManager.Instance.ButtonBehaviour(type, this);
  }
}
