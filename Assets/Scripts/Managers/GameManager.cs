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
  public MonsterManager monsterMan;
  public GridManager gridMan;
  public ComboManager comboMan;

  private LoadManager loadMan;
  private MusicManager musicMan;
  private UIManager uiMan;
  private BackgroundManager backMan;

  public bool startWithHelp, helpToggler;
  private bool isPaused;
  public ItemInfo[] itemSlots;
  public Dictionary<CONTRACT_TYPE, ContractInfo> contracts;

  public void WakeUp() { }

  void Awake()
  {
    GameFeel.GameFeelInit();

    isPaused = false;
    startWithHelp = false;
    scoreMan = gameObject.AddComponent<ScoreManager>();
    sfxMan = gameObject.AddComponent<SFXManager>();
    musicMan = gameObject.AddComponent<MusicManager>();
    loadMan = gameObject.AddComponent<LoadManager>();
    itemSlots = new ItemInfo[2] { new ItemInfo(ITEM_TYPE.EATER), new ItemInfo(ITEM_TYPE.BIN) };
    contracts = new Dictionary<CONTRACT_TYPE, ContractInfo>();

    // DEBUG HACK TO ADD CONTRACTS
    contracts.Add(CONTRACT_TYPE.TIMER, new ContractInfo(CONTRACT_TYPE.TIMER));

    // Init for when scene loads
    if (SceneManager.GetActiveScene().name.Contains("vertical-phone"))
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

  void InitContractIcons()
  {
    GameObject[] icons = GameObject.FindGameObjectsWithTag("ContractIcon");
    foreach (GameObject obj in icons) obj.SetActive(false);

    // iterate from back because find adds to the array from front?
    int i = 0;
    var ite = contracts.GetEnumerator();
    while(ite.MoveNext())
    {
      if (!ite.Current.Value.isActive) continue;
      icons[i].GetComponentsInChildren<SpriteRenderer>()[1].sprite = ite.Current.Value.contractIcon;
      icons[i].SetActive(true);
      ++i;
    }
  }

  void InitializeManagers()
  {
    turnCounter = 0;
    currentLevel = 0;
    helpToggler = true;
    if (SceneManager.GetActiveScene().name.Contains("vertical-phone"))
    {
      scoreMan.InitScore();

      dayMan = GameObject.Find("day_manager").GetComponent<DayManager>();
      uiMan = GameObject.Find("ui_manager").GetComponent<UIManager>();
      gridMan = GameObject.Find("grid_manager").GetComponent<GridManager>();
      monsterMan = GameObject.Find("monster_manager").GetComponent<MonsterManager>();
      backMan = GameObject.Find("Background").GetComponent<BackgroundManager>();
      comboMan = GameObject.Find("combo_man").GetComponent<ComboManager>();

      // contract icon display
      InitContractIcons();
    }
  }

  public bool IsPaused()
  {
    return isPaused;
  }

  public void SetIsPaused(bool flag)
  {
    isPaused = flag;
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
    if (SceneManager.GetActiveScene().name.Contains("vertical-phone"))
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
      //scoreMan.DisplayScore();
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
    // start the scene with pause off
    SetIsPaused(false);

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

  public void InitItem(ItemSpawn spawner, int num)
  {
    spawner.info = itemSlots[num];
  }

  public void IncrementTurnCounter()
  {
    ++turnCounter;

    // Whenever the turn counter increases reset the combo
    comboMan.ResetComboCount();
  }

  public int AddScore(int amt)
  {
    return scoreMan.AddScore(amt);
  }

  public void AddNumServed(int amt)
  {
    scoreMan.AddNumberServed(amt);

    // Check for day change
    dayMan.UpdateProgressBar();
    dayMan.CheckForShiftChange();
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
        //SetLoseBehaviour();
        break;
    }
  }

  public void SetLoseBehaviour()
  {
    backMan.ChangeSignColors(uiMan.loseSign, dayMan.dayState);

    // Turn on lose text
    uiMan.ToggleLoseText(true);
    // Turn off monster request boxes
    monsterMan.ToggleMonsterRequests(false);

    SetGameState(GAME_STATE.LOSE);
    SetIsPaused(true);
  }

  public void CheckLevelComplete()
  {
    // Lose the game is a grid is full
    if (gridMan && gridMan.IfGridFull())
    {
      SetLoseBehaviour();
    }
  }

  public ItemInfo GetItemSlot(int num)
  {
    return itemSlots[num];
  }

  public void SetUpItemSlot(int num, ItemInfo info)
  {
    itemSlots[num] = info;
  }

  public void UpdateContracts(ContractInfo info)
  {
    if (!info.isActive)
    {
      if (contracts.ContainsKey(info.type))
      {
        contracts[info.type].isActive = false;
      }
    }
    else if (info.isActive)
    {
      if (contracts.ContainsKey(info.type))
      {
        contracts[info.type].isActive = true;
      }
      else contracts.Add(info.type, info);
    }
  }

  public bool CheckForContract(CONTRACT_TYPE type)
  {
    if (contracts.ContainsKey(type)) return contracts[type].isActive;
    return false;
  }

  public bool IsInGame()
  {
    return SceneManager.GetActiveScene().name.Contains("vertical-phone");
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
        SetIsPaused(false);
        break;
      case BUTTON_TYPE.TO_SETUP:
        LoadSceneWithTransition("screen-setup");
        break;
    }
  }
}


