using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum GAME_STATE
{
  PLAYING,
  LOSE,
}

public class GameManager : Singleton<GameManager>
{
  // guarantee this will be always a singleton only - can't use the constructor!
  protected GameManager()
  {}

  public GAME_STATE gameState;
  public bool levelCompleted;
  public int currentLevel;

  // For ui controls
  private UIManager uiMan;
  //private ScoreManager scoreMan;
  private GridManager gridMan;
  private MonsterManager monsterMan;

  void Awake()
  {
    InitializeManagers();
    //scoreMan = gameObject.AddComponent<ScoreManager>();
  }

  void InitializeManagers()
  {
    currentLevel = 0;
    uiMan = GameObject.Find("ui_manager").GetComponent<UIManager>();
    gridMan = GameObject.Find("grid_manager").GetComponent<GridManager>();
    monsterMan = GameObject.Find("monster_manager").GetComponent<MonsterManager>();
  }

  void Update()
  {
    // Debug
    if (Input.GetKeyDown(KeyCode.R))
    {
      // Resets level
      SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // HACK
    InitializeManagers();
  }

  public void SetGameState(GAME_STATE state)
  {
    gameState = state;

    // State being switched to
    switch (gameState)
    {
      case GAME_STATE.PLAYING:

        break;
      case GAME_STATE.LOSE:
        SetLoseBehaviour();
        break;
    }
  }

  void SetPlayingBehaviour()
  {
    // Turn off lose text
    uiMan.ToggleLoseText(false);
    monsterMan.ToggleMonsterRequests(true);
  }

  void SetLoseBehaviour()
  {
    // Turn on lose text
    uiMan.ToggleLoseText(true);
    // Turn off monster request boxes
    monsterMan.ToggleMonsterRequests(false);
  }

  public void CheckLevelComplete()
  {
    // Lose the game is a grid is full
    if (gridMan.IfGridFull())
    {
      SetGameState(GAME_STATE.LOSE);
    }
  }
}


