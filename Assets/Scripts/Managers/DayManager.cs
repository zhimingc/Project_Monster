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

  public DAY_STATE dayState;
  public GameObject dayFeedback, progressBar, sauces;
  public int[] shiftIntervals;
  public float shiftChangeSpeed;
  public GameObject shiftChangeObj;
  public GameObject endOfDaySign, startDaySign;

  private MonsterManager monsterMan;
  private BackgroundManager backMan;  // To update background graphics
  private SauceManager sauceMan;      // To active sauces for lunch
  private GridManager gridMan;            // To activate grids for dinner
  private float initialProgressSize;

	// Use this for initialization
	void Start () {
    dayState = DAY_STATE.BREAKFAST;
    dayFeedback = GameObject.Find("shift_text");
    monsterMan = GameObject.Find("monster_manager").GetComponent<MonsterManager>();
    backMan = GameObject.Find("Background").GetComponent<BackgroundManager>();
    sauceMan = GameObject.Find("sauce_man").GetComponent<SauceManager>();
    gridMan = GameObject.Find("grid_manager").GetComponent<GridManager>();

    initialProgressSize = progressBar.transform.localScale.x;
    //UpdateProgressBar();
    CheckForShiftChange();
    backMan.ChangeSignColors(shiftChangeObj, DAY_STATE.BREAKFAST);

    // update day counter
    GameObject.Find("dayCount_text").GetComponent<TextMesh>().text = "Day " + (GameManager.Instance.gameData.count_days + 1).ToString();
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
        GameManager.Instance.AddToDayCount(1);
        break;
    }
  }

  public void PlayShiftSign(DAY_STATE color)
  {
    if (dayState != DAY_STATE.BREAKFAST)
    {
      // play sfx
      GameManager.Instance.SFX().PlaySound("good");
    }

    // hack to count how many needed to serve
    int toServe = 10;
    if (GameManager.Instance.gameData.count_days == 1)
    {
      toServe = 20;
    }
    else if (GameManager.Instance.gameData.count_days > 1)
    {
      toServe = 30;
    }

    switch (dayState)
    {
      case DAY_STATE.BREAKFAST:
        backMan.ChangeSignColors(startDaySign, color);
        startDaySign.GetComponentsInChildren<Text>()[0].text = "Day " + (GameManager.Instance.gameData.count_days + 1).ToString();
        startDaySign.GetComponentsInChildren<Text>()[1].text = "Goal: " + toServe.ToString();
        startDaySign.GetComponent<Animator>().SetTrigger("isEnter");
        GameManager.Instance.SetIsPaused(true);
        LeanTween.delayedCall(1.0f, () =>
        {
          GameManager.Instance.SetIsPaused(false);
          startDaySign.GetComponent<Animator>().SetTrigger("isExit");
        });
        break;
      case DAY_STATE.LUNCH:
        backMan.ChangeSignColors(shiftChangeObj, color);
        shiftChangeObj.GetComponent<Animator>().SetTrigger("isEnter");
        shiftChangeObj.GetComponentInChildren<Text>().text = "LUNCH!";
        LeanTween.delayedCall(1.5f, () =>
        {
          sauceMan.ActivateSauces();
        });
        LeanTween.delayedCall(3.5f, () =>
        {
          shiftChangeObj.GetComponent<Animator>().SetTrigger("isExit");
        });

        break;
      case DAY_STATE.DINNER:
        backMan.ChangeSignColors(shiftChangeObj, color);
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
        });
        break;
      case DAY_STATE.WIN:
        backMan.ChangeSignColors(endOfDaySign, color);
        endOfDaySign.GetComponent<Animator>().SetTrigger("isEnter");
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
    int numServed = GameManager.Instance.scoreMan.numServed;
    if ((int)dayState < shiftIntervals.Length && numServed >= shiftIntervals[(int)dayState])
    {
      for (int i = 0; i < (int)DAY_STATE.NUM_SHIFTS; ++i)
      {
        if ((int)dayState == i) continue;

        if (i >= shiftIntervals.Length || numServed < shiftIntervals[i])
        {
          // Change the feedback text to reflect shift
          dayState = ((DAY_STATE)i);
          DAY_STATE dayColor = (DAY_STATE)Mathf.Min((int)DAY_STATE.DINNER, i);

          // check if day ends early for day 1/2
          if (GameManager.Instance.CheckIfDayEnds(dayState))
          {
            dayState = DAY_STATE.WIN;
          }

          ShiftTrigger(dayState);
          PlayShiftSign(dayColor);

          backMan.ChangeTimeState(i); // update bg

          string dayText = dayState.ToString();
          dayFeedback.GetComponent<Text>().text = dayText;
          break;
        }
      }
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
}
