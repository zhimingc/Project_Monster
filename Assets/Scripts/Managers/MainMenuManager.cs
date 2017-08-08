using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour {

  public bool isChangingName;
  public TextMesh playerNameText;
  public string playerName;

  private float timerForText = 0;
  private TouchScreenKeyboard keyboard;

  // Use this for initialization
  void Start () {
    playerName = GameManager.Instance.gameData.playerName;
    UpdatePlayerName();
  }
	
	// Update is called once per frame
	void Update () {
    UpdateChangeName();
  }

  void UpdatePlayerName()
  {
    playerNameText.text = playerName;
  }

  void UpdateChangeName()
  {
    if (!isChangingName) return;

    playerName = GameManager.Instance.gameData.playerName;

    if (InputMan.UsingPC())
    {
      playerName += Input.inputString;
      if (Input.inputString != "")
      {
        if (playerName.Contains("|"))
        {
          playerName = playerName.Remove(playerName.IndexOf('|'), 1);
        }
      }

      timerForText -= Time.deltaTime;
      if (timerForText <= 0.0f)
      {
        timerForText = 0.5f;
        if (playerName.Contains("|"))
        {
          playerName = playerName.Remove(playerName.IndexOf('|'), 1);
        }
        else
        {
          playerName += "|";
        }
      }

      if (Input.GetKeyDown(KeyCode.Return))
      {
        isChangingName = false;
        if (playerName.Contains("|"))
        {
          playerName = playerName.Remove(playerName.IndexOf('|'), 1);
        }

        if (playerName.Length >= 10)
        {
          playerName = playerName.Substring(0, 10);
        }
      }
    }
    else
    {
      if (keyboard.done)
      {
        playerName = keyboard.text;

        if (playerName.Length >= 10)
        {
          playerName = playerName.Substring(0, 10);
        }
      }
    }

    UpdatePlayerName();
    GameManager.Instance.gameData.playerName = playerName;
  }

  public void ActivateNameChange()
  {
    playerName = "";
    isChangingName = true;
    if (!InputMan.UsingPC())
    {
      keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default, false, false, false);
    }
  }
}
