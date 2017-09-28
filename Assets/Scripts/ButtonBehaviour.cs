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
  TO_SETUP,
  SETUP_TOGGLE,
  TO_FIRSTTIME,
  RESET_DATA,
  TO_GAME,
  DEBUG_LEVELSKIP,
  DEBUG_GETPOP,
  GAME_CHANGENAME,
  GAME_EVENTFORWARD,
  GAME_EVENTBACK,
  GAME_LEADERBOARD,
  GAME_EVENTSELECT,
  STORE_BUYPOSTER,
  STORE_CONFIRM,
  STORE_CANCEL,
  STORE_PUTPOSTER,
  TO_STORE,
  RESET_CONFIRM,
  RESET_CANCEL,
  QUIT_CONFIRM,
  QUIT_CANCEL,
  QUIT_LEVEL
}

public class ButtonBehaviour : MonoBehaviour {

  public BUTTON_TYPE type;
  private Vector3 originScale;

  void Start()
  {
    originScale = transform.localScale;

    GameManager.Instance.ButtonInit(type, this);
  }

  void Update()
  {
    if (InputMan.OnUp())
    {
      //Color curCol = GetComponent<SpriteRenderer>().color;
      //curCol.a = 1.0f;
      //GetComponent<SpriteRenderer>().color = curCol;

      //transform.localScale = originScale;
      LeanTween.scale(gameObject, originScale, 0.1f);
    }
  }

  void OnMouseOver()
  {
    OnTouchUp();
    OnTouchDown();
  }

  public void ChangeButtonColors(int state, string[] skyColors, string[] shadowColors)
  {
    SpriteRenderer[] renders = GetComponentsInChildren<SpriteRenderer>();
    renders[0].color = Utility.GetColorFromHex(shadowColors[state]);
    renders[1].color = Utility.GetColorFromHex(shadowColors[state]);
    renders[2].color = Utility.GetColorFromHex(skyColors[state]);
  }

	public void OnTouchUp()
  {
    if (InputMan.OnUp())
    {
      GameManager.Instance.ButtonBehaviour(type, this);
    }
  }

  public void OnTouchDown()
  {
    //LeanTween.cancel(gameObject);

    if (InputMan.OnDown())
    {
      //GameManager.Instance.ButtonBehaviour(type, this);
      LeanTween.scale(gameObject, originScale * 1.15f, 0.1f);
      //GetComponent<SpriteRenderer>().color -= new Color(0, 0, 0, 0.5f);
    }
  }
}
