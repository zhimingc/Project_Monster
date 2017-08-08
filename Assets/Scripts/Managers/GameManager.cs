﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum LOSE_REASON
{
  OVERFLOW,
  TIME_UP,

}

public enum GAME_STATE
{
  PLAYING,
  LOSE,
}

// data which determines player progress in the game 
[System.Serializable]
public class GameData
{
  // initialize game data with serialization later
  public GameData()
  {
    pop_monsters = new float[(int)MONSTER_TYPE.NUM_TYPES];
    pop_monsters[0] = 100.0f;

    // flags
    flag_firstPlay = true;

    playerName = PlayerPrefs.GetString("name");
    if (playerName == "") playerName = "Leslie";
  }

  public void Reset()
  {
    pop_total = 0;
    pop_rank = 0;
    ConsecutiveDayReset();

    // flags
    flag_firstPlay = true;
  }

  // does not clear total popularity
  public void ConsecutiveDayReset()
  {
    count_days = 0;
    pop_monsters = new float[(int)MONSTER_TYPE.NUM_TYPES];
    pop_monsters[0] = 100.0f;
    num_ingredients = 1;
  }

  public int pop_total;
  public int pop_rank, count_days;
  public float[] pop_monsters;
  public bool indicator_newTool;
  public bool flag_firstPlay;

  // gameplay stat stuff, put in another class later
  public int num_ingredients;
  public int newMonsterIndex;
  public string playerName;
  public MONSTER_EVENT eventType;
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
  public GameData gameData;

  public ScoreManager scoreMan;
  public DayManager dayMan;
  public SFXManager sfxMan;
  public MonsterManager monsterMan;
  public GridManager gridMan;
  public ComboManager comboMan;
  public KitchenSetupManager setupMan;  // only in setup screen
  public IngredientManager ingredientMan;

  private LoadManager loadMan;
  private MusicManager musicMan;
  private UIManager uiMan;
  private BackgroundManager backMan;
  private Cursor cursorScript;
  private ConsecutiveManager consecMan;

  private bool isPaused;

  public bool startWithHelp, helpToggler;
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
    cursorScript = gameObject.AddComponent<Cursor>();
    consecMan = gameObject.AddComponent<ConsecutiveManager>();
    contracts = new Dictionary<CONTRACT_TYPE, ContractInfo>();
    gameData = new GameData();

    GameManagerReset();

    // DEBUG HACK TO ADD CONTRACTS
    //contracts.Add(CONTRACT_TYPE.TIMER, new ContractInfo(CONTRACT_TYPE.TIMER));

    // Init for when scene loads
    InitializeManagers();

    // Delegate which gets called ever time a scene loads
    SceneManager.sceneLoaded += OnSceneLoaded;

    //Screen.SetResolution(1080, 1920, false);
    Screen.SetResolution(540, 960, false);

    InputMan.platform = Application.platform;
  }

  void ResetToDayOne()
  {
    gameData.count_days = 0;
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
    // Apply consecutive changes from the day
    consecMan.ApplyNewDayChanges(gameData.count_days);

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
      ingredientMan = GameObject.Find("ingredient_manager").GetComponent<IngredientManager>();

      // contract icon display
      InitContractIcons();

      if (startWithHelp) ToggleHelpScreen();
    }

    // Manager in Setup screen
    if (SceneManager.GetActiveScene().name.Contains("screen-setup"))
    {
      setupMan = GameObject.Find("setup_man").GetComponent<KitchenSetupManager>();
    }

    if (SceneManager.GetActiveScene().name.Contains("leaderboard"))
    {
      scoreMan.TriggerUpdateLeaderboard();
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
      LeanTween.cancelAll();
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

      if (Input.GetKeyDown(KeyCode.W))
      {
        dayMan.dayState = DAY_STATE.WIN;
        dayMan.TriggerShiftChange();
      }
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

    // behaviour once scene is loaded
    LeanTween.delayedCall(loadMan.loadSpeed, () =>
    {
      if (GameObject.FindWithTag("PopularityMan"))
        GameObject.FindWithTag("PopularityMan").GetComponent<PopularityManager>().TriggerPopularityUpdate();

      // animate day sign
      if (dayMan) dayMan.PlayShiftSign(DAY_STATE.BREAKFAST);
    });

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

  public int AddScore()
  {
    return scoreMan.AddScore();
  }

  public void AddNumServed(int amt)
  {
    scoreMan.AddNumberServed(amt);

    // Check for day change
    //dayMan.UpdateProgressBar();
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

  public void SetLoseBehaviour(LOSE_REASON reason)
  {
    // change sign text
    switch(reason)
    {
      case LOSE_REASON.OVERFLOW:
        uiMan.loseSign.GetComponentInChildren<Text>().text = "Sandwich overflow! Kitchen closed.";
        break;
      case LOSE_REASON.TIME_UP:
        uiMan.loseSign.GetComponentInChildren<Text>().text = "Time up!\nKitchen closed.";
        break;
    }

    backMan.ChangeSignColors(uiMan.loseSign, dayMan.dayState);

    // Turn on lose text
    uiMan.ToggleLoseText(true);
    // Turn off monster request boxes
    monsterMan.ToggleMonsterRequests(false);

    SetGameState(GAME_STATE.LOSE);
    SetIsPaused(true);

    // reset to day 1
    ResetToDayOne();
  }

  public void CheckLevelComplete()
  {
    // Lose the game is a grid is full
    if (gridMan && gridMan.IfGridFull())
    {
      SetLoseBehaviour(LOSE_REASON.OVERFLOW);
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
        if (gameData.pop_total == 0)
        {
          gameData.flag_firstPlay = false;
          LoadSceneWithTransition("screen-first-timer");
        }
        else
        {
          Button_ToSetup();
        }
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
        Button_ToSetup();
        break;
      case BUTTON_TYPE.SETUP_TOGGLE:
        bool flipped = setupMan.isCounterUp;
        btn.GetComponentsInChildren<SpriteRenderer>()[1].flipX = !flipped;
        //setupMan.ToggleCounter(!flipped);
        break;
      case BUTTON_TYPE.TO_FIRSTTIME:
        if (gameData.flag_firstPlay)
        {
          gameData.flag_firstPlay = false;
          LoadSceneWithTransition("screen-first-timer");
        }
        else
        {
          Button_ToGame();
        }
        break;
      case BUTTON_TYPE.RESET_DATA:
        gameData.Reset();
        scoreMan.Reset();
        GameManagerReset();
        break;
      case BUTTON_TYPE.TO_GAME:
        Button_ToGame();
        break;
      case BUTTON_TYPE.DEBUG_LEVELSKIP:
        ++gameData.count_days;
        consecMan.ApplyNewDayChanges(gameData.count_days);
        GameObject.Find("store_stats").GetComponent<PopularityManager>().UpdateDayText();
        GameObject.Find("store_stats").GetComponent<PopularityManager>().InitPopMonsters();
        break;
      case BUTTON_TYPE.DEBUG_GETPOP:
        scoreMan.totalScore += 1000;
        GameObject.Find("store_stats").GetComponent<PopularityManager>().TriggerPopularityUpdate();
        break;
      case BUTTON_TYPE.GAME_CHANGENAME:
        gameData.playerName = "";
        GameObject.Find("main_menu").GetComponent<MainMenuManager>().ActivateNameChange();
        break;
      case BUTTON_TYPE.GAME_EVENTFORWARD:
        GameObject.Find("event_parent").GetComponent<MonsterEventManager>().ScrollEventForward();
        break;
      case BUTTON_TYPE.GAME_EVENTBACK:
        GameObject.Find("event_parent").GetComponent<MonsterEventManager>().ScrollEventBack();
        break;
      case BUTTON_TYPE.GAME_LEADERBOARD:
        LoadSceneWithTransition("screen-leaderboard");
        //scoreMan.TriggerUpdateLeaderboard();
        break;
    }
  }

  void Button_ToGame()
  {
    LoadSceneWithTransition("vertical-phone");
    musicMan.ToggleBGM(BGM_CLIPS.LEVEL);
  }
  
  void Button_ToSetup()
  {
    LoadSceneWithTransition("screen-setup");
    musicMan.ToggleBGM(BGM_CLIPS.MAIN_MENU);
  }

  public void AddTotalPopularity(int amt)
  {
    gameData.pop_total += amt;
  }

  public void SetTotalPopularity(int amt)
  {
    gameData.pop_total = amt;
  }

  public int GetTotalPopularity()
  {
    return gameData.pop_total;
  }

  // to check for days ending early for day 1/2
  public bool CheckIfDayEnds(DAY_STATE state)
  {
    //if ((DAYS)gameData.count_days == DAYS.BREAKFAST 
    //  && state == DAY_STATE.LUNCH) return true;
    //if ((DAYS)gameData.count_days == DAYS.LUNCH
    //  && state == DAY_STATE.DINNER) return true;

    return false;
  }

  public void AddToDayCount(int amt)
  {
    gameData.count_days += amt;
  }

  void GameManagerReset()
  {
    itemSlots = new ItemInfo[2] { new ItemInfo(ITEM_TYPE.EATER), new ItemInfo(ITEM_TYPE.EATER) };

  }

  public void EndOfDayTrigger()
  {
    if (!dayMan.toggleTimedShiftFeature)
    {
      AddToDayCount(1);
    }

    switch (gameData.eventType)
    {
      case MONSTER_EVENT.FIRST_DAY:
        break;
      case MONSTER_EVENT.MAIN_EVENT:
        scoreMan.UpdateLeaderboard();
        break;
    }
  }
}


