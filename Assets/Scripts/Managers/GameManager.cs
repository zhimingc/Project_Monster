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
  TUTORIAL,
  START_SEQUENCE,
  PLAYING,
  LOSE,
}

// data which determines player progress in the game 
[System.Serializable]
public class GameData
{
  // initialize game data with serialization later
  public void InitGameData()
  {
    // Init earnings
    if (PlayerPrefs.HasKey("earnings"))
    {
      pop_total = PlayerPrefs.GetInt("earnings");
    }

    if (PlayerPrefs.HasKey("support_total"))
    {
      support_total = PlayerPrefs.GetInt("support_total");
    }

    pop_monsters = new float[(int)MONSTER_TYPE.NUM_TYPES];
    pop_monsters[0] = 100.0f;

    // flags
    if (PlayerPrefs.HasKey("firstPlay"))
    {
      flag_firstPlay = false;
    }
    else
    {
      flag_firstPlay = true;
    }

    //playerName = PlayerPrefs.GetString("name");
    //if (playerName == "") playerName = "Leslie";

    eventType = MONSTER_EVENT.FRENZY_PROGRESS;

    // init monster variations
    monsterVars = new List<List<bool>>();
    for (int i = 0; i < 4; ++i)
    {
      monsterVars.Add(new List<bool>());
      monsterVars[i].Add(true); // first variation is default

      for (int j = 1; j < 4; ++j)
      {
        int isUnlocked = PlayerPrefs.GetInt("monster_type_" + i.ToString() + "_" + j.ToString());

        if (GameManager.Instance.EXHIBIT_BUILD) isUnlocked = 1;

        monsterVars[i].Add(isUnlocked == 1);
      }
    }
  }

  public void Reset()
  {
    pop_total = 0;
    //support_total = 0;
    //pop_rank = 0;
    ConsecutiveDayReset();

    // init monster variations
    monsterVars = new List<List<bool>>();
    for (int i = 0; i < 4; ++i)
    {
      monsterVars.Add(new List<bool>());
      monsterVars[i].Add(true); // first variation is default

      for (int j = 0; j < 3; ++j)
      {
        monsterVars[i].Add(false);
      }
    }

    // flags
    flag_firstPlay = true;
  }

  // does not clear total popularity
  public void ConsecutiveDayReset()
  {
    //count_days = 0;
    pop_monsters = new float[(int)MONSTER_TYPE.NUM_TYPES];
    pop_monsters[0] = 100.0f;
    num_ingredients = 1;
  }

  public int GetNumUnlocked()
  {
    int unlocked = 0;
    foreach (var list in monsterVars)
    {
      foreach(bool mon in list)
      {
        if (mon) ++unlocked;
      }
    }
    // remove initial monsters
    unlocked -= 4;
    return unlocked;
  }

  public int pop_total;
  public int stamp_total;
  public int support_total;
  public List<List<bool>> monsterVars;
  public float[] pop_monsters;
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
  public int tutorialStep;

  public ScoreManager scoreMan;
  public DayManager dayMan;
  public SFXManager sfxMan;
  public MonsterManager monsterMan;
  public GridManager gridMan;
  public ComboManager comboMan;
  public KitchenSetupManager setupMan;  // only in setup screen
  public IngredientManager ingredientMan;
  public LeaderboardManager leaderboardMan;
  public SpriteManager spriteMan;
  public SocialManager socialMan;

  // build flag
  public bool EXHIBIT_BUILD;

  private LoadManager loadMan;
  private MusicManager musicMan;
  private UIManager uiMan;
  //private BackgroundManager backMan;
  private Cursor cursorScript;
  //private ConsecutiveManager consecMan;
  private StoreManager storeMan;
  private AdsManager adsMan;
  public AnalyticsManager analyticMan;

#if UNITY_ANDROID
  //private GPGDemo gpgDemo;
#endif 

  private bool isPaused;

  public bool startWithHelp, helpToggler;
  public ItemInfo[] itemSlots;
  public Dictionary<CONTRACT_TYPE, ContractInfo> contracts;

  public void WakeUp() { }

  void Awake()
  {
    // BUILD FLAGS
    EXHIBIT_BUILD = true;
    //EXHIBIT_BUILD = false;

    GameFeel.GameFeelInit();
    gameData = new GameData();
    gameData.InitGameData();

    isPaused = false;
    startWithHelp = true;
    scoreMan = gameObject.AddComponent<ScoreManager>();
    sfxMan = gameObject.AddComponent<SFXManager>();
    musicMan = gameObject.AddComponent<MusicManager>();
    loadMan = gameObject.AddComponent<LoadManager>();
    spriteMan = gameObject.AddComponent<SpriteManager>();
    cursorScript = gameObject.AddComponent<Cursor>();
    socialMan = gameObject.AddComponent<SocialManager>();
    adsMan = gameObject.AddComponent<AdsManager>();
    analyticMan = gameObject.AddComponent<AnalyticsManager>();

    // testing for leaderboards
    //gpgDemo = gameObject.AddComponent<GPGDemo>();
    //

    //consecMan = gameObject.AddComponent<ConsecutiveManager>();
    contracts = new Dictionary<CONTRACT_TYPE, ContractInfo>();
    //GameManagerReset();

    // DEBUG HACK TO ADD CONTRACTS
    //contracts.Add(CONTRACT_TYPE.TIMER, new ContractInfo(CONTRACT_TYPE.TIMER));

    // Init for when scene loads
    InitializeManagers();

    // Delegate which gets called ever time a scene loads
    SceneManager.sceneLoaded += OnSceneLoaded;

    //Screen.SetResolution(1080, 1920, false);
    Screen.SetResolution(540, 960, false);

    InputMan.platform = Application.platform;

    // Loading
    //LoadMonsterVar();

    //SetGameState(GAME_STATE.TUTORIAL);
    //SetGameState(GAME_STATE.START_SEQUENCE);
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
    //consecMan.ApplyNewDayChanges(gameData.count_days);

    turnCounter = 0;
    currentLevel = 0;
    helpToggler = true;
    if (SceneManager.GetActiveScene().name.Contains("vertical-phone"))
    {
      if (gameState != GAME_STATE.TUTORIAL)
      {
        GameObject.Find("finger_hand_filled").SetActive(false);
        GameObject.Find("tutorial_backing").SetActive(false);
      }

      scoreMan.InitScore();

      dayMan = GameObject.Find("day_manager").GetComponent<DayManager>();
      uiMan = GameObject.Find("ui_manager").GetComponent<UIManager>();
      gridMan = GameObject.Find("grid_manager").GetComponent<GridManager>();
      monsterMan = GameObject.Find("monster_manager").GetComponent<MonsterManager>();
      //backMan = GameObject.Find("Background").GetComponent<BackgroundManager>();
      comboMan = GameObject.Find("combo_man").GetComponent<ComboManager>();
      ingredientMan = GameObject.Find("ingredient_manager").GetComponent<IngredientManager>();
      leaderboardMan = GameObject.Find("leaderboard_sign").GetComponent<LeaderboardManager>();

      // initialization for persistent classes
      socialMan.InitSocial();

      // contract icon display
      //InitContractIcons();
    }

    // Manager in Setup screen
    if (SceneManager.GetActiveScene().name.Contains("screen-setup"))
    {
      //setupMan = GameObject.Find("setup_man").GetComponent<KitchenSetupManager>();
    }

    if (SceneManager.GetActiveScene().name.Contains("leaderboard"))
    {
      FindObjectOfType<highscore>().GetScoresWrapper(9);
      scoreMan.TriggerUpdateLeaderboard();
      socialMan.InitSocial();
      socialMan.LoadLeaderboard();
    }

    if (SceneManager.GetActiveScene().name.Contains("store"))
    {
      storeMan = GameObject.Find("store_manager").GetComponent<StoreManager>();
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
    // debug event logging
    //if (Input.GetKeyDown(KeyCode.L))
    //{
    //  // Log an event with a float parameter
    //  Firebase.Analytics.FirebaseAnalytics
    //    .LogEvent("game_time", "single", 0.4f);
    //}

    if (SceneManager.GetActiveScene().name.Contains("vertical-phone"))
    {
      // Debug
      if (Input.anyKeyDown)
      {
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

  private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
  {
    // start the scene with pause off
    SetIsPaused(false);

    InitializeManagers();
    //if (startWithHelp) ToggleHelpScreen();
    helpToggler = true;

    // load screen
    loadMan.LoadIn();

    if (SceneManager.GetActiveScene().name.Contains("vertical-phone"))
    {
      // behaviour once scene is loaded
      LeanTween.delayedCall(loadMan.loadSpeed, () =>
      {
        // animate day sign
        if (gameState == GAME_STATE.START_SEQUENCE)
        {
          SetIsPaused(true);
          LeanTween.delayedCall(GameProgression.startSeqDelay + 1.0f, () =>
          {
            dayMan.PlayShiftSign(DAY_STATE.BREAKFAST);
          });
        }
      });
    }

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
    //dayMan.CheckForShiftChange();
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

    //backMan.ChangeSignColors(uiMan.loseSign, dayMan.dayState);
    gridMan.ToggleGrid(false);

    // Turn on lose text
    //uiMan.ToggleLoseText(true);
    dayMan.dayState = DAY_STATE.WIN;
    dayMan.TriggerShiftChange();

    // save earnings
    dayMan.leaderboardSign.GetComponent<LeaderboardManager>().TriggerLeaderboard();
    SaveEarnings();

    // Turn off monster request boxes
    monsterMan.ToggleMonsterRequests(false);

    SetGameState(GAME_STATE.LOSE);
    SetIsPaused(true);

    // reset to day 1
    //ResetToDayOne();

    // update leaderboard
    scoreMan.UpdateLocalLeaderboard();

    analyticMan.UpdateAnalytics(ANALYTICS.NUM_PLAY, analyticMan.num_play + 1);
    analyticMan.ToggleAnalysis(false, ANALYTICS.LENGTH_PLAY);
    analyticMan.UpdateAnalytics(ANALYTICS.NUM_PLAYRESULT, scoreMan.curInstantScore);

    // post result after every play
    analyticMan.PostAnalytics(ANALYTICS_EVENT.PLAY_END);
  }

  public void SaveMonsterVar()
  {
    for (int i = 0; i < gameData.monsterVars.Count; ++i)
    {
      for (int j = 0; j < gameData.monsterVars[i].Count; ++j)
      {
        string toSave = "monster_type_" + i.ToString() + "_" + j.ToString();
        bool val = gameData.monsterVars[i][j];
        PlayerPrefs.SetInt(toSave, val ? 1 : 0);
      }
    }

    PlayerPrefs.Save();
  }

  public void LoadMonsterVar()
  {
    for (int i = 0; i < gameData.monsterVars.Count; ++i)
    {
      for (int j = 1; j < gameData.monsterVars[i].Count; ++j)
      {
        string toLoad = "monster_type_" + i.ToString() + "_" + j.ToString();
        bool val = PlayerPrefs.GetInt(toLoad) == 1;
        gameData.monsterVars[i][j] = val;
      }
    }
  }

  public void SaveEarnings()
  {
    PlayerPrefs.SetInt("earnings", gameData.pop_total);
    PlayerPrefs.SetInt("support_total", gameData.support_total);
  }

  public void CheckLevelComplete()
  {
    // Lose the game is a grid is full
    if (gridMan && gridMan.IfGridFull())
    {
      dayMan.dayState = DAY_STATE.WIN;
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
        //ToggleHelpScreen();
        break;
      case BUTTON_TYPE.MUSIC:
        bool invert = !AudioListener.pause;
        AudioListener.pause = invert;
        AudioListener.volume = invert ? 0 : 1;
        break;
      case BUTTON_TYPE.START:
        if (gameData.flag_firstPlay == true)
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
        Button_ToGame(GAME_STATE.TUTORIAL);
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
        LoadSceneWithTransition("screen-first-timer");
        break;
      case BUTTON_TYPE.TO_GAME:
        // Start tracking play length
        analyticMan.ToggleAnalysis(true, ANALYTICS.LENGTH_PLAY);

        if (gameData.flag_firstPlay)
        {
          Button_ToGame(GAME_STATE.TUTORIAL);
        }
        else
        {
          Button_ToGame();
        }
        break;
      case BUTTON_TYPE.TO_TUTORIAL:
        // Start tracking play length
        analyticMan.ToggleAnalysis(true, ANALYTICS.LENGTH_PLAY);

        if (EXHIBIT_BUILD)
        {
          Button_ToGame(GAME_STATE.TUTORIAL);
        }
        else
        {
          Button_ToGame();
        }
        break;
      case BUTTON_TYPE.DEBUG_LEVELSKIP:
        //++gameData.count_days;
        //consecMan.ApplyNewDayChanges(gameData.count_days);
        //GameObject.Find("store_stats").GetComponent<PopularityManager>().UpdateDayText();
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
        //gpgDemo.OnShowLeaderBoard();
        //scoreMan.TriggerUpdateLeaderboard();
        break;
      case BUTTON_TYPE.GAME_EVENTSELECT:
        MonsterEventManager eventMan = GameObject.Find("event_parent").GetComponent<MonsterEventManager>();
        if (eventMan.curEvent == MONSTER_EVENT.FIRST_DAY)
        {
          LoadSceneWithTransition("screen-first-timer");
        }
        else
        {
          LoadSceneWithTransition("screen-setup-tools");
        }
        break;
      case BUTTON_TYPE.STORE_BUYPOSTER:
        storeMan.TriggerBuySign(true);
        break;
      case BUTTON_TYPE.STORE_CONFIRM:
        //storeMan.ToggleButtonCols(false);
        storeMan.TriggerBuySign(false);

        // watch ad
        if (EXHIBIT_BUILD)
        {
          RewardPlayer(); // only for exhibition build
        }
        else
        {
          adsMan.ShowRewardedAd();
        }
        break;
      case BUTTON_TYPE.STORE_CANCEL:
        storeMan.TriggerBuySign(false);
        break;
      case BUTTON_TYPE.TO_STORE:
        musicMan.ToggleBGM(BGM_CLIPS.MAIN_MENU);
        LoadSceneWithTransition("screen-store");
        break;
      case BUTTON_TYPE.RESET_DATA:
        GameObject.Find("reset_sign").GetComponent<Animator>().SetTrigger("enter");
        break;
      case BUTTON_TYPE.RESET_CONFIRM:
        gameData.Reset();
        scoreMan.Reset();
        GameManagerReset();
        GameObject.Find("reset_sign").GetComponent<Animator>().SetTrigger("exit");
        break;
      case BUTTON_TYPE.RESET_CANCEL:
        GameObject.Find("reset_sign").GetComponent<Animator>().SetTrigger("exit");
        break;
      case BUTTON_TYPE.QUIT_LEVEL:
        GameObject.Find("quit_sign").GetComponent<Animator>().SetTrigger("enter");
        break;
      case BUTTON_TYPE.QUIT_CONFIRM:
        musicMan.ToggleBGM(BGM_CLIPS.MAIN_MENU);
        LoadSceneWithTransition("screen-store");
        GameObject.Find("quit_sign").GetComponent<Animator>().SetTrigger("exit");
        break;
      case BUTTON_TYPE.QUIT_CANCEL:
        GameObject.Find("quit_sign").GetComponent<Animator>().SetTrigger("exit");
        break;
    }
  }

  void Button_ToGame(GAME_STATE state = GAME_STATE.START_SEQUENCE)
  {
    SetGameState(state);
    LoadSceneWithTransition("vertical-phone");
    musicMan.ToggleBGM(BGM_CLIPS.LEVEL);
  }
  
  void Button_ToSetup()
  {
    LoadSceneWithTransition("screen-setup-event");
    musicMan.ToggleBGM(BGM_CLIPS.MAIN_MENU);
  }

  //public void AddTotalPopularity(int amt)
  //{
  //  //gameData.pop_total += amt;
  //}

  //public void SetTotalPopularity(int amt)
  //{
  //  //gameData.pop_total = amt;
  //}

  public int GetTotalPopularity()
  {
    return gameData.pop_total;
  }

  void GameManagerReset()
  {
    itemSlots = new ItemInfo[2] { new ItemInfo(ITEM_TYPE.EATER), new ItemInfo(ITEM_TYPE.EATER) };

    PlayerPrefs.DeleteAll();
  }

  public void EndOfDayTrigger()
  {
    //if (!dayMan.toggleTimedShiftFeature)
    //{
    //  AddToDayCount(1);
    //}

    switch (gameData.eventType)
    {
      case MONSTER_EVENT.FIRST_DAY:
        break;
      case MONSTER_EVENT.MAIN_EVENT:
      case MONSTER_EVENT.FRENZY_EVENT:
      case MONSTER_EVENT.FRENZY_PROGRESS:
        //scoreMan.UpdateLocalLeaderboard();
        break;
    }
  }

  public void UpdateMonsterPopularity()
  {
    float swingSpeed = 5.0f;

    int numServed = scoreMan.numServed;
    if (numServed > 5)
    {
      if (gameData.pop_monsters[1] < 35.0f)
      {
        gameData.pop_monsters[0] -= swingSpeed;
        gameData.pop_monsters[1] += swingSpeed;
      }
    }
    if (numServed > 10)
    {
      if (gameData.pop_monsters[2] < 35.0f)
      {
        gameData.pop_monsters[0] -= swingSpeed;
        gameData.pop_monsters[2] += swingSpeed;
      }
    }
  }

  public void RewardPlayer()
  {
    storeMan.ConfirmBuy();
  }

  public void TutorialTrigger(int step = -1)
  {
    // step 0 triggers monster
    // step 1 triggers end of tutorial
    GameObject tutorialHand = GameObject.Find("finger_hand_filled");
    if (step < 0) ++tutorialStep;
    else tutorialStep = step;

    switch (tutorialStep)
    {
      case 0: // first monster request
        monsterMan.TriggerTutorialMonster(0);
        tutorialHand.GetComponent<Animator>().SetTrigger("serve");
        break;
      case 1: // load layout tutorial
        loadMan.LoadFull(()=>
        {
          monsterMan.TriggerTutorialMonster(1);
          gridMan.TutorialGridStates(1);
          // make ingredientMan output laid out ingredients
          ingredientMan.IngredientLayoutTutorial(1);
          // gotta make ingredient man spawn a new ingredient for step 2
          ingredientMan.AddToIngredientQ();

          tutorialHand.SetActive(false);
        });
        break;
      case 2: // start game
        startWithHelp = false;
        gameData.flag_firstPlay = false;
        ingredientMan.IngredientLayoutTutorial(-1);

        PlayerPrefs.SetInt("firstPlay", 1);
        Button_ToGame();

        PlayerPrefs.Save();
        break;
    }
  }

  void FullSave()
  {
    SaveEarnings();
    SaveMonsterVar();
    int firstPlay = gameData.flag_firstPlay ? 0 : 1;
    PlayerPrefs.SetInt("firstPlay", firstPlay);
    scoreMan.SaveLeaderboard();
  }

  void OnApplicationFocus(bool hasFocus)
  {
    if (hasFocus == false)
    {
      analyticMan.PostAnalytics(ANALYTICS_EVENT.SESSION_END);
    }
  }
}

