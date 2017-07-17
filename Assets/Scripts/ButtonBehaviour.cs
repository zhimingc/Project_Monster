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
  TOG_SFX,
  CONTINUE_GAME,
  TO_SETUP
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

  public void ChangeButtonColors(int state, string[] skyColors, string[] shadowColors)
  {
    SpriteRenderer[] renders = GetComponentsInChildren<SpriteRenderer>();
    renders[0].color = Utility.GetColorFromHex(shadowColors[state]);
    renders[1].color = Utility.GetColorFromHex(shadowColors[state]);
    renders[2].color = Utility.GetColorFromHex(skyColors[state]);
  }

	public void OnTouchDown()
  {
    if (InputMan.OnDown())
      GameManager.Instance.ButtonBehaviour(type, this);
  }
}
