using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum DAY_STATE
{
  BREAKFAST,
  LUNCH,
  DINNER,
  WIN,
  NUM_SHIFTS
};

public class DayManager : MonoBehaviour {

  public bool toggleTimedShiftFeature;
  public bool toggleAddedTime;
  public bool toggleFixedShiftAmts;
  public bool toggleShifts;

  public DAY_STATE dayState;
  public GameObject dayFeedback, progressBar, sauces;
  public int[] shiftIntervals;
  public int nextShiftNum;

  public float shiftChangeSpeed;
  public GameObject shiftChangeObj;
  public GameObject endOfDaySign, startDaySign, leaderboardSign;

  private MonsterManager monsterMan;
  private BackgroundManager backMan;  // To update background graphics
  private SauceManager sauceMan;      // To active sauces for lunch
  private GridManager gridMan;        // To activate grids for dinner
  private float initialProgressSize;

  // timed days prototype
  public float maxShiftTime;
  public GameObject shiftTimerCanvas;
  public GameObject shiftTimerBar, shiftTimerText;

  public GameObject[] timerAddedText;
  private int curTimeAddedIndex;

  public float timeAddedOnServe;
  private float shiftTimer;
  private bool timerPause;

  // Use this for initialization
  void Start () {
    dayState = DAY_STATE.BREAKFAST;
    //dayFeedback = GameObject.Find("shift_text");
    monsterMan = GameObject.Find("monster_manager").GetComponent<MonsterManager>();
    backMan = GameObject.Find("Background").GetComponent<BackgroundManager>();
    sauceMan = GameObject.Find("sauce_man").GetComponent<SauceManager>();
    gridMan = GameObject.Find("grid_manager").GetComponent<GridManager>();

    initialProgressSize = progressBar.transform.localScale.x;
    //UpdateProgressBar();
    CheckForShiftChange();
    //backMan.ChangeSignColors(shiftChangeObj, DAY_STATE.BREAKFAST);

    // update day counter
    //GameObject.Find("dayCount_text").GetComponent<TextMesh>().text = "Day " + (GameManager.Instance.gameData.count_days + 1).ToString();

    // timed shifts feature
    shiftTimer = maxShiftTime;// * (2.0f / 3.0f); // start with only 2/3 timer
    toggleAddedTime = false;
    toggleFixedShiftAmts = false;
    ToggleTimer(true);
    UpdateTimerText();

    switch (GameManager.Instance.gameData.eventType)
    {
      case MONSTER_EVENT.FIRST_DAY:
        shiftTimerCanvas.SetActive(false);
        toggleTimedShiftFeature = false;
        break;
      case MONSTER_EVENT.MAIN_EVENT:
        toggleTimedShiftFeature = true;
        break;
      case MONSTER_EVENT.ZEN_EVENT:
        toggleTimedShiftFeature = false;
        shiftTimerCanvas.SetActive(false);
        break;
      case MONSTER_EVENT.FRENZY_EVENT:
        toggleTimedShiftFeature = true;
        toggleAddedTime = true;
        toggleFixedShiftAmts = true;

        shiftIntervals[0] = 10;
        shiftIntervals[1] = 20;
        shiftIntervals[2] = 30;
        break;
      case MONSTER_EVENT.FRENZY_PROGRESS:
      //default:
        toggleTimedShiftFeature = true;
        toggleAddedTime = true;
        toggleFixedShiftAmts = true;
        toggleShifts = false;
        break;
    }


    // init vars
    nextShiftNum = shiftIntervals[0];
  }

  void ShiftTrigger(DAY_STATE shift)
  {
    switch (shift)
    {
      case DAY_STATE.LUNCH:
        monsterMan.AddSauceToAllRequests();
        break;
      case DAY_STATE.DINNER:
        break;
      case DAY_STATE.WIN:
        GameManager.Instance.EndOfDayTrigger();
        break;
    }
  }

  void Update()
  {
    if (toggleTimedShiftFeature)
    {
      UpdateTimedShifts();
    }
  }

  void UpdateTimedShifts()
  {
    if (timerPause || dayState == DAY_STATE.WIN) return;

    shiftTimer -= Time.deltaTime;
    if (shiftTimer <= 0.0f)
    {
      switch (GameManager.Instance.gameData.eventType)
      {
        case MONSTER_EVENT.MAIN_EVENT:
          ++dayState;
          if (dayState == DAY_STATE.WIN)
          {
            shiftTimer = 0.0f;
          }

          TriggerShiftChange();
          break;
        case MONSTER_EVENT.FRENZY_EVENT:
        case MONSTER_EVENT.FRENZY_PROGRESS:
          dayState = DAY_STATE.WIN;
          TriggerShiftChange();
          GameManager.Instance.SetLoseBehaviour(LOSE_REASON.OVERFLOW);
          break;
      }
    }
    UpdateTimerText();
  }

  void UpdateTimerText()
  {
    shiftTimerBar.GetComponent<Image>().fillAmount = shiftTimer / maxShiftTime;
    shiftTimerText.GetComponent<Text>().text = shiftTimer.ToString("0");
  }

  void ToggleTimer(bool flag)
  {
    timerPause = flag;
  }

  void ChangeShiftBehaviour()
  {
    switch (GameManager.Instance.gameData.eventType)
    {
      case MONSTER_EVENT.MAIN_EVENT:
        shiftTimer = maxShiftTime;
        break;
      case MONSTER_EVENT.FRENZY_EVENT:
      case MONSTER_EVENT.FRENZY_PROGRESS:
        AddToTimer(5.0f);
        break;
    }
  }

  float AddToTimer(float amt)
  {
    // scale amt by combo count
    amt = amt * GameManager.Instance.comboMan.GetComboMultiplier();

    float newTimeAmt = shiftTimer + amt;

    if (newTimeAmt > maxShiftTime)
    {
      newTimeAmt = maxShiftTime;
    }
    shiftTimer = newTimeAmt;

    // animate added time text
    //GameObject addedText = timerAddedText[curTimeAddedIndex];
    //curTimeAddedIndex = (curTimeAddedIndex + 1) % timerAddedText.Length;
    //addedText.transform.position = shiftTimerText.transform.position;
    //addedText.GetComponent<Text>().text = "+" + amt.ToString("0.0");
    //addedText.GetComponent<Text>().enabled = true;
    //LeanTween.moveLocalX(addedText, 0.3f, 1.0f);
    //LeanTween.delayedCall(1.0f, () =>
    //{
    //  addedText.GetComponent<Text>().enabled = false;
    //});

    return amt;
  }

  public void PlayShiftSign(DAY_STATE color)
  {
    // timed shift behaviour
    ToggleTimer(true);  // pause timer
    float timerDelay = 1.5f;

    if (dayState != DAY_STATE.BREAKFAST)
    {
      // play sfx
      GameManager.Instance.SFX().PlaySound("good");
    }

    // hack to count how many needed to serve
    //int toServe = 15;
    //int dayNum = GameManager.Instance.gameData.count_days;

    switch (dayState)
    {
      case DAY_STATE.BREAKFAST:
        //backMan.ChangeSignColors(startDaySign, color);
        
        //startDaySign.GetComponentsInChildren<Text>()[0].text = "Day " + (dayNum + 1).ToString() + ":";
        //startDaySign.GetComponentsInChildren<Text>()[1].text = "Goal: " + toServe.ToString();

        string flavText = "No more new content";
        //if (dayNum < GameProgression.dayFlavText.Length) flavText = GameProgression.dayFlavText[dayNum];
        flavText = GameProgression.eventFlavText[(int)GameManager.Instance.gameData.eventType];

        startDaySign.GetComponentsInChildren<Text>()[1].text = flavText;

        startDaySign.GetComponent<Animator>().SetTrigger("isEnter");
        GameManager.Instance.SetIsPaused(true);
        LeanTween.delayedCall(1.0f, () =>
        {
          GameManager.Instance.SetIsPaused(false);
          startDaySign.GetComponent<Animator>().SetTrigger("isExit");

          LeanTween.delayedCall(timerDelay, () =>
          {
            if (toggleTimedShiftFeature) ToggleTimer(false);
          });

        });
        break;
      case DAY_STATE.LUNCH:
        //backMan.ChangeSignColors(shiftChangeObj, color);
        shiftChangeObj.GetComponent<Animator>().SetTrigger("isEnter");
        shiftChangeObj.GetComponentInChildren<Text>().text = "LUNCH!";
        LeanTween.delayedCall(1.5f, () =>
        {
          sauceMan.ActivateSauces();
        });
        LeanTween.delayedCall(3.5f, () =>
        {
          shiftChangeObj.GetComponent<Animator>().SetTrigger("isExit");

          LeanTween.delayedCall(timerDelay, () =>
          {
            if (toggleTimedShiftFeature)
            {
              ToggleTimer(false);
              ChangeShiftBehaviour();
            }
          });

        });

        break;
      case DAY_STATE.DINNER:
        //backMan.ChangeSignColors(shiftChangeObj, color);
        shiftChangeObj.GetComponent<Animator>().SetTrigger("isEnter");
        shiftChangeObj.GetComponentInChildren<Text>().text = "DINNER!";
        LeanTween.delayedCall(1.5f, () =>
        {
          // change grid to bowls here
          gridMan.ToggleDinnerShiftGrids(true);
        });
        LeanTween.delayedCall(3.5f, () =>
        {
          shiftChangeObj.GetComponent<Animator>().SetTrigger("isExit");

          LeanTween.delayedCall(timerDelay, () =>
          {
            if (toggleTimedShiftFeature)
            {
              ToggleTimer(false);
              ChangeShiftBehaviour();
            }
          });

        });
        break;
      case DAY_STATE.WIN:
        switch (GameManager.Instance.gameData.eventType)
        {
          case MONSTER_EVENT.FIRST_DAY:
          case MONSTER_EVENT.ZEN_EVENT:
          case MONSTER_EVENT.MAIN_EVENT:
            //backMan.ChangeSignColors(endOfDaySign, color);
            endOfDaySign.GetComponent<Animator>().SetTrigger("isEnter");
            break;
          //case MONSTER_EVENT.MAIN_EVENT:
          case MONSTER_EVENT.FRENZY_EVENT:
          case MONSTER_EVENT.FRENZY_PROGRESS:
            //backMan.ChangeSignColors(leaderboardSign, color);
            leaderboardSign.GetComponent<LeaderboardManager>().TriggerLeaderboard();
            break;
        }

        GameManager.Instance.SetIsPaused(true);
        break;
    }
  }

  public void UpdateProgressBar()
  {
    int curShift = Mathf.Min((int)DAY_STATE.DINNER, (int)dayState);
    int numServed = GameManager.Instance.scoreMan.numServed;

    int curShiftBase = 0;
    if (curShift > 0) curShiftBase = shiftIntervals[curShift - 1];

    int curShiftLim = shiftIntervals[curShift];

    float progressScaler = ((float)numServed - curShiftBase) / (curShiftLim - curShiftBase);
    if (progressScaler >= 1.0f) progressScaler = 1.0f;
    backMan.UpdateSunPosition(progressScaler);

    progressScaler = (float)numServed / shiftIntervals[2];
    if (progressScaler >= 1.0f) progressScaler = 1.0f;
    float progressSize = (progressScaler) * initialProgressSize;
    progressBar.transform.localScale = new Vector3(progressSize, 0.25f, 1.0f);
    progressBar.transform.position = transform.position + new Vector3(-initialProgressSize / 2.0f + progressSize / 2.0f, 7.5f, 0.0f);
  }

  public void CheckForShiftChange()
  {
    // feature toggle
    if (!toggleFixedShiftAmts && toggleTimedShiftFeature) return;
    if (!toggleShifts) return;

    // don't end day under time is up for frenzy event
    if (GameManager.Instance.gameData.eventType == MONSTER_EVENT.FRENZY_EVENT)
    {
      if (dayState == DAY_STATE.DINNER) return;
    }

    // don't change shift or end the day if there are still serves
    if (gridMan.AnyGridsServable()) return;

    int numServed = GameManager.Instance.scoreMan.numServed;

    if (numServed >= nextShiftNum)
    {
      // Change the feedback text to reflect shift
      dayState = dayState + 1;

      // check if day ends early for day 1/2
      if (GameManager.Instance.CheckIfDayEnds(dayState))
      {
        dayState = DAY_STATE.WIN;
      }

      TriggerShiftChange();
    }
  }

  public void TriggerShiftChange()
  {
    int numServed = GameManager.Instance.scoreMan.numServed;
    DAY_STATE dayColor = (DAY_STATE)Mathf.Min((int)DAY_STATE.DINNER, (int)dayState);

    ShiftTrigger(dayState);
    PlayShiftSign(dayColor);

    backMan.ChangeTimeState((int)dayState); // update bg

    //string dayText = dayState.ToString();
    //dayFeedback.GetComponent<Text>().text = dayText;

    // update shift interval to it still needs X to finish
    int nextIndex = (int)dayState;
    if (nextIndex < (int)DAY_STATE.WIN)
    {
      nextShiftNum = numServed + shiftIntervals[nextIndex] - shiftIntervals[nextIndex - 1];
    }
  }

  public bool IsOrPastShift(DAY_STATE shift)
  {
    return (int)dayState >= (int)shift;
  }

  public bool FutureShiftCheck(DAY_STATE shift)
  {
    if (shift <= 0) return true;
    int shiftNum = shiftIntervals[(int)shift - 1];
    return GameManager.Instance.scoreMan.numServed + 3 >= shiftNum;
  }

  // effects of serving on the shift timer
  public void OnServeTimeEffect(ref float timeAdded)
  {
    int numServed = GameManager.Instance.scoreMan.numServed;
    if (numServed > 15) timeAddedOnServe = 1.5f;
    else if (numServed > 10) timeAddedOnServe = 2.5f;
    else if (numServed > 5) timeAddedOnServe = 5.0f;
    else timeAddedOnServe = 7.5f;

    if (toggleAddedTime)
    {
      timeAdded = AddToTimer(timeAddedOnServe);
    }
  }
}
