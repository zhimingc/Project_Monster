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
  public int turnCounter;

  public ScoreManager scoreMan;
  public DayManager dayMan;

  private UIManager uiMan;
  private GridManager gridMan;
  private MonsterManager monsterMan;

  private bool startWithHelp;

  void Awake()
  {
    startWithHelp = true;
    scoreMan = new ScoreManager();

    // Init for when scene loads
    InitializeManagers();
    if (startWithHelp) ToggleHelpScreen();

    // Delegate which gets called ever time a scene loads
    SceneManager.sceneLoaded += OnSceneLoaded;

    //Screen.SetResolution(1080, 1920, false);
    Screen.SetResolution(540, 960, false);
  }

  void InitializeManagers()
  {
    turnCounter = 0;
    currentLevel = 0;
    scoreMan.InitScore();
    dayMan = GameObject.Find("day_manager").GetComponent<DayManager>();
    uiMan = GameObject.Find("ui_manager").GetComponent<UIManager>();
    gridMan = GameObject.Find("grid_manager").GetComponent<GridManager>();
    monsterMan = GameObject.Find("monster_manager").GetComponent<MonsterManager>();
  }

  void Update()
  {
    // Debug
    if (Input.anyKeyDown)
    {
      if (Input.GetKeyDown(KeyCode.H))
      {
        ToggleHelpScreen();
      }
      else if (uiMan.helpText.enabled == true)
      {
        uiMan.ToggleHelpText(false);
        gridMan.ToggleGrid(true);
      }
    }
    if (Input.GetKeyDown(KeyCode.R))
    {
      // Resets level
      SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    if (Input.GetKeyDown(KeyCode.Escape))
    {
      Application.Quit();
    }


    // Update to display score
    scoreMan.DisplayScore();
  }

  void ToggleHelpScreen()
  {
    bool flag = uiMan.helpText.enabled;
    uiMan.ToggleHelpText(!flag);
    gridMan.ToggleGrid(flag);
  }

  private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
  {
    InitializeManagers();
    if (startWithHelp) ToggleHelpScreen();
  }

  public void IncrementTurnCounter()
  {
    ++turnCounter;
  }

  public void AddScore(int amt)
  {
    scoreMan.AddScore(amt);

    // Check for day change
    dayMan.CheckForShiftChange();
    dayMan.UpdateProgressBar();

    // Change blocks for dinner shift
    if (dayMan.IsOrPastShift(DAY_STATE.DINNER))
    {
      gridMan.ToggleDinnerShiftGrids(true);
    }
    else
    {
      gridMan.ToggleDinnerShiftGrids(false);
    }
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


