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
  public SFXManager sfxMan;

  private UIManager uiMan;
  private GridManager gridMan;
  private MonsterManager monsterMan;

  public bool startWithHelp, helpToggler;
 
  void Awake()
  {
    startWithHelp = true;
    scoreMan = new ScoreManager();
    sfxMan = gameObject.AddComponent<SFXManager>();

    // Init for when scene loads
    if (SceneManager.GetActiveScene().name == "vertical-phone")
    {
      InitializeManagers();

      if (startWithHelp) ToggleHelpScreen();
      helpToggler = true;
    }

    // Delegate which gets called ever time a scene loads
    SceneManager.sceneLoaded += OnSceneLoaded;

    //Screen.SetResolution(1080, 1920, false);
    Screen.SetResolution(540, 960, false);

    InputMan.platform = Application.platform;
  }

  void InitializeManagers()
  {
    turnCounter = 0;
    currentLevel = 0;
    helpToggler = true;
    scoreMan.InitScore();
    if (SceneManager.GetActiveScene().name == "vertical-phone")
    {
      dayMan = GameObject.Find("day_manager").GetComponent<DayManager>();
      uiMan = GameObject.Find("ui_manager").GetComponent<UIManager>();
      gridMan = GameObject.Find("grid_manager").GetComponent<GridManager>();
      monsterMan = GameObject.Find("monster_manager").GetComponent<MonsterManager>();
    }
  }

  public SFXManager SFX()
  {
    return sfxMan;
 }

  public void ButtonBehaviour(BUTTON_TYPE type)
  {
    switch (type)
    {
      case BUTTON_TYPE.RESTART:
        // Resets level
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        break;
      case BUTTON_TYPE.HELP:
        ToggleHelpScreen();
        break;
      case BUTTON_TYPE.MUSIC:
        bool invert = !AudioListener.pause;
        AudioListener.pause = invert;
        AudioListener.volume = invert ? 0 : 1;
        break;
      case BUTTON_TYPE.START:
        // Resets level
        SceneManager.LoadScene("vertical-phone");
        break;
    }
  }

  void Update()
  {
    if (SceneManager.GetActiveScene().name == "vertical-phone")
    {
      // Debug
      if (Input.anyKeyDown)
      {
        //if (Input.GetKeyDown(KeyCode.S))
        //{
        //  bool invert = !AudioListener.pause;
        //  AudioListener.pause = invert;
        //  AudioListener.volume = invert ? 0 : 1;
        //}

        if (Input.GetKeyDown(KeyCode.H))
        {
          ToggleHelpScreen();
        }
        else if (uiMan.helpText.enabled == true)
        {
          if (helpToggler)
          {
            uiMan.ToggleHelpText(false);
            gridMan.ToggleGrid(true);
            helpToggler = false;
          }
          else
          {
            helpToggler = true;
          }
        }
      }
      if (Input.GetKeyDown(KeyCode.R))
      {
        // Resets level
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
      }

      // Update to display score
      scoreMan.DisplayScore();
    }

    if (Input.GetKeyDown(KeyCode.Escape))
    {
      if (SceneManager.GetActiveScene().name == "vertical-phone")
      {
        SceneManager.LoadScene(1);
      }
      else
      {
        Application.Quit();
      }
      
    }
  }

  void ToggleHelpScreen()
  {
    bool flag = uiMan.helpText.enabled;
    uiMan.ToggleHelpText(!flag);
    gridMan.ToggleGrid(flag);

    // hack
    helpToggler = false;
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


