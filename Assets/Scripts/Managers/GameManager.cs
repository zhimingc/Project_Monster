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

  private LoadManager loadMan;
  private MusicManager musicMan;
  private UIManager uiMan;
  private GridManager gridMan;
  private MonsterManager monsterMan;
  private BackgroundManager backMan;

  public bool startWithHelp, helpToggler;

  public void WakeUp() { }

  void Awake()
  {
    startWithHelp = false;
    scoreMan = new ScoreManager();
    sfxMan = gameObject.AddComponent<SFXManager>();
    musicMan = gameObject.AddComponent<MusicManager>();
    loadMan = gameObject.AddComponent<LoadManager>();

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

  void ToggleSplashScreens()
  {
    LeanTween.delayedCall(2.5f, () =>
    {
      LoadSceneWithTransition("screen-start");
      musicMan.ToggleBGM(BGM_CLIPS.MAIN_MENU);
    });
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
      backMan = GameObject.Find("Background").GetComponent<BackgroundManager>();
    }
  }

  public SFXManager SFX()
  {
    return sfxMan;
 }

  public void ButtonInit(BUTTON_TYPE type, ButtonBehaviour btn)
  {
    switch (type)
    {
      case BUTTON_TYPE.TOG_MUSIC:
        if (!musicMan.isMute)
        {
          Sprite sp = Resources.Load<Sprite>("Sprites/UI/musicOn");
          btn.GetComponentsInChildren<SpriteRenderer>()[1].sprite = sp;
        }
        else
        {
          Sprite sp = Resources.Load<Sprite>("Sprites/UI/musicOff");
          btn.GetComponentsInChildren<SpriteRenderer>()[1].sprite = sp;
        }
        break;
      case BUTTON_TYPE.TOG_SFX:
        if (!sfxMan.isMute)
        {
          Sprite sp = Resources.Load<Sprite>("Sprites/UI/audioOn");
          btn.GetComponentsInChildren<SpriteRenderer>()[1].sprite = sp;
        }
        else
        {
          Sprite sp = Resources.Load<Sprite>("Sprites/UI/audioOff");
          btn.GetComponentsInChildren<SpriteRenderer>()[1].sprite = sp;
        }
        break;
    }
  }

  public void ButtonBehaviour(BUTTON_TYPE type, ButtonBehaviour btn)
  {
    switch (type)
    {
      case BUTTON_TYPE.RESTART:
        // Resets level
        LoadSceneWithTransition(SceneManager.GetActiveScene().name);
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
        LoadSceneWithTransition("vertical-phone");
        musicMan.ToggleBGM(BGM_CLIPS.LEVEL);
        break;
      case BUTTON_TYPE.CREDITS:
        LoadSceneWithTransition("screen-credits");
        break;
      case BUTTON_TYPE.SETTINGS:
        LoadSceneWithTransition("screen-options");
        break;
      case BUTTON_TYPE.TO_START:
        LoadSceneWithTransition("screen-start");
        musicMan.ToggleBGM(BGM_CLIPS.MAIN_MENU);
        break;
      case BUTTON_TYPE.START_HELP_BASIC:
        if (SceneManager.GetActiveScene().name == "screen-howtoplay-tips")
        {
          LoadSceneVanilla("screen-howtoplay-basic");
        }
        else
        {
          LoadSceneWithTransition("screen-howtoplay-basic");
        }
        break;
      case BUTTON_TYPE.START_HELP_TIPS:
        LoadSceneVanilla("screen-howtoplay-tips");
        break;
      case BUTTON_TYPE.TOG_MUSIC:
        if (!musicMan.ToggleMute())
        {
          Sprite sp = Resources.Load<Sprite>("Sprites/UI/musicOn");
          btn.GetComponentsInChildren<SpriteRenderer>()[1].sprite = sp;
        }
        else
        {
          Sprite sp = Resources.Load<Sprite>("Sprites/UI/musicOff");
          btn.GetComponentsInChildren<SpriteRenderer>()[1].sprite = sp;
        }
        break;
      case BUTTON_TYPE.TOG_SFX:
        if (!sfxMan.ToggleMute())
        {
          Sprite sp = Resources.Load<Sprite>("Sprites/UI/audioOn");
          btn.GetComponentsInChildren<SpriteRenderer>()[1].sprite = sp;
        }
        else
        {
          Sprite sp = Resources.Load<Sprite>("Sprites/UI/audioOff");
          btn.GetComponentsInChildren<SpriteRenderer>()[1].sprite = sp;
        }
        break;
      case BUTTON_TYPE.CONTINUE_GAME:
        dayMan.endOfDaySign.GetComponent<Animator>().SetTrigger("isExit");
        break;
    }
  }

  void LoadSceneVanilla(string name)
  {
    SceneManager.LoadScene(name);
  }

  void LoadSceneWithTransition(string name)
  {
    loadMan.LoadOut();
    LeanTween.delayedCall(loadMan.loadSpeed, () =>
    {
      SceneManager.LoadScene(name);
    });
  }

  void Update()
  {
    if (SceneManager.GetActiveScene().name == "vertical-phone")
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
        SceneManager.LoadScene("screen-start");
      }
      else
      {
        Application.Quit();
      }
      
    }
  }

  void ToggleHelpScreen()
  {
    if (SceneManager.GetActiveScene().name == "vertical-phone")
    {
      bool flag = uiMan.helpText.enabled;
      uiMan.ToggleHelpText(!flag);
      gridMan.ToggleGrid(flag);

      // hack
      helpToggler = false;
    }
  }

  private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
  {
    InitializeManagers();
    if (startWithHelp) ToggleHelpScreen();
    helpToggler = true;

    // load screen
    loadMan.LoadIn();

    // splash screen behaviour
    if (SceneManager.GetActiveScene().name == "screen-splash")
    {
      ToggleSplashScreens();
    }
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

  void SetLoseBehaviour()
  {
    backMan.ChangeSignColors(uiMan.loseSign, dayMan.dayState);

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


