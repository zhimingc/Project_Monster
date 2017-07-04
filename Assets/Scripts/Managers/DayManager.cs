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
    shiftChangeObj = GameObject.Find("shift_object");

    initialProgressSize = progressBar.transform.localScale.x;
    UpdateProgressBar();
    CheckForShiftChange();
  }

  void Update()
  {
    // debug
    if (Input.GetKeyDown(KeyCode.K))
    {
      PlayShiftSign();
    }
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
    }
  }

  void PlayShiftSign()
  {
    switch (dayState)
    {
      case DAY_STATE.LUNCH:
        backMan.ChangeSignColors(dayState);
        shiftChangeObj.GetComponent<Animator>().SetTrigger("isEnter");
        shiftChangeObj.GetComponentInChildren<Text>().text = "L U N C H !";
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
        backMan.ChangeSignColors(dayState);
        shiftChangeObj.GetComponent<Animator>().SetTrigger("isEnter");
        shiftChangeObj.GetComponentInChildren<Text>().text = "D I N N E R !";
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
    }
  }

  public void UpdateProgressBar()
  {
    int curShift = Mathf.Min((int)DAY_STATE.DINNER, (int)dayState);

    int curShiftBase = 0;
    if (curShift > 0) curShiftBase = shiftIntervals[curShift - 1];

    int curShiftLim = shiftIntervals[curShift];

    float progressScaler = ((float)GameManager.Instance.scoreMan.score - curShiftBase) / (curShiftLim);
    if (progressScaler >= 1.0f) progressScaler = 1.0f;
    backMan.UpdateSunPosition(progressScaler);

    progressScaler = (float)GameManager.Instance.scoreMan.score / shiftIntervals[2];
    if (progressScaler >= 1.0f) progressScaler = 1.0f;
    float progressSize = (progressScaler) * initialProgressSize;
    progressBar.transform.localScale = new Vector3(progressSize, 0.25f, 1.0f);
    progressBar.transform.position = transform.position + new Vector3(-initialProgressSize / 2.0f + progressSize / 2.0f, 7.5f, 0.0f);
  }

  public void CheckForShiftChange()
  {
    int score = GameManager.Instance.scoreMan.score;
    if ((int)dayState < shiftIntervals.Length && score >= shiftIntervals[(int)dayState])
    {
      for (int i = 0; i < (int)DAY_STATE.NUM_SHIFTS; ++i)
      {
        if ((int)dayState == i) continue;

        if (i >= shiftIntervals.Length || score < shiftIntervals[i])
        {
          // Change the feedback text to reflect shift
          dayState = ((DAY_STATE)i);
          ShiftTrigger(dayState);
          PlayShiftSign();

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
    return GameManager.Instance.scoreMan.score + 3 >= shiftNum;
  }
}
