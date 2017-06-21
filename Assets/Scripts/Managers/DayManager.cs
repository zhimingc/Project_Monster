using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum DAY_STATE
{
  BREAKFAST,
  LUNCH,
  DINNER,
  NUM_SHIFTS
};

public class DayManager : MonoBehaviour {

  public DAY_STATE dayState;
  public GameObject dayFeedback;
  public int[] shiftIntervals;

	// Use this for initialization
	void Start () {
    dayState = DAY_STATE.BREAKFAST;
    dayFeedback = GameObject.Find("shift_text");

  }
	
	public void CheckForShiftChange()
  {
    int score = GameManager.Instance.scoreMan.score;
    for (int i = 0; i < (int)DAY_STATE.NUM_SHIFTS; ++i)
    {
      if (score < shiftIntervals[i])
      {
        // Change the feedback text to reflect shift
        dayState = ((DAY_STATE)i);
        dayFeedback.GetComponent<Text>().text = dayState.ToString();
        return;
      }
    }
  }

  public bool IsOrPastShift(DAY_STATE shift)
  {
    return (int)dayState >= (int)shift;
  }
}
