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

  private float initialProgressSize;

	// Use this for initialization
	void Start () {
    dayState = DAY_STATE.BREAKFAST;
    dayFeedback = GameObject.Find("shift_text");

    initialProgressSize = progressBar.transform.localScale.x;
    UpdateProgressBar();
    CheckForShiftChange();

  }

  public void UpdateProgressBar()
  {
    float progressScaler = (float)GameManager.Instance.scoreMan.score / shiftIntervals[2];
    if (progressScaler >= 1.0f) progressScaler = 1.0f;
    float progressSize = (progressScaler) * initialProgressSize;
    progressBar.transform.localScale = new Vector3(progressSize, 0.25f, 1.0f);
    progressBar.transform.position = new Vector3(-initialProgressSize / 2.0f + progressSize / 2.0f, 7.5f, 0.0f);
  }

	public void CheckForShiftChange()
  {
    int score = GameManager.Instance.scoreMan.score;
    for (int i = 0; i < (int)DAY_STATE.NUM_SHIFTS; ++i)
    {
      if (i >= shiftIntervals.Length || score < shiftIntervals[i])
      {
        // Change the feedback text to reflect shift
        dayState = ((DAY_STATE)i);
        string dayText = dayState.ToString();
        dayFeedback.GetComponent<Text>().text = dayText;
        break;
      }
    }

    if (IsOrPastShift(DAY_STATE.LUNCH))
    {
      sauces.SetActive(true);
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
