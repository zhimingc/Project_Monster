using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum LEADERBOARD
{
  FIRST, SECOND, THIRD, FOURTH, FIFTH, SIXTH, SEVENTH, EIGHTH, NINTH, TENTH
};

[System.Serializable]
public class ScoreManager : MonoBehaviour {

  public int numServed;
  public float scoreSpeed;  // how long it takes for the numbers to add

  public GameObject scoreObj, numServedObj;
  // trueScore is the true amount, incScore is the amount to add per frame
  public int totalScore, incScore;
  public int curScore, curInstantScore;
  private Vector3 scoreOriginalScale;
  //private int[] shiftScoreAmt;

  // to track score breakdown
  private int[][] scoreBreakdown;

  public bool toUpdateLeaderboard = false, newScoreToAdd;

  // to track leaderboard
  private List<Pair<string, int>> leaderboard;

  void Start()
  {
    newScoreToAdd = false;

    //shiftScoreAmt = new int[3]
    //{
    //  10, 20, 30
    //};

    // init leaderboard from player pref
    leaderboard = new List<Pair<string, int>>();
    for (int i = 0; i < 9; ++i)
    {
      leaderboard.Add(new Pair<string, int>("-", 0));
    }
    LoadLeaderboard();

    Reset();
  }

  public void TriggerUpdateLeaderboard()
  {
    toUpdateLeaderboard = true;
  }

  public void Reset()
  {
    numServed = 0;
    incScore = 0;
    curScore = 0;
    curInstantScore = 0;
    totalScore = 0;

    scoreBreakdown = new int[3][];
    for (int i = 0; i < scoreBreakdown.Length; ++i)
    {
      scoreBreakdown[i] = new int[4] { 0, 0, 0, 0 };
    }
  }

  public int AddScore()
  {
    //int dayNum = (int)GameManager.Instance.dayMan.dayState;
    //int comboNum = GameManager.Instance.comboMan.GetComboCount();

    //int amt = shiftScoreAmt[(int)GameManager.Instance.dayMan.dayState];
    int amt = 10 + (numServed / 5) * 5;

    amt = (int) (amt * GameManager.Instance.comboMan.GetComboMultiplier());
    totalScore += amt;
    curInstantScore += amt;

    // compute the amount of be added every frame
    ComputeIncScore();
    AnimateAdding();

    // update score breakdown
    //++scoreBreakdown[dayNum][comboNum - 1];

    return amt;
  }

  public void AddNumberServed(int amt)
  {
    numServed += amt;

    // Update text
    numServedObj.GetComponent<TextMesh>().text = "Served: " + numServed.ToString();
  }

  public void InitScore()
  {
    if (GameManager.Instance.IsInGame())
    {
      scoreObj = GameObject.Find("money_text");
      numServedObj = GameObject.Find("served_text");
      scoreOriginalScale = scoreObj.transform.localScale;
    }

    newScoreToAdd = true;

    // for kitchen setup scene
    numServed = 0;
    incScore = 0;
    curScore = 0;
    curInstantScore = 0;
    totalScore = GameManager.Instance.GetTotalPopularity();

    scoreSpeed = 0.5f;
    DisplayScore();
  }

  void Update()
  {
    if (toUpdateLeaderboard)
    {
      toUpdateLeaderboard = false;
      DisplayLeaderboard();
    }

    if (curScore != curInstantScore)
    {
      curScore += incScore;
      if (curScore >= curInstantScore) curScore = curInstantScore;
      DisplayScore();
    }
  }

  void AnimateAdding()
  {
    Vector3 newScale = scoreOriginalScale * 1.25f;
    //scoreObj.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
    scoreObj.transform.localScale = scoreOriginalScale;

    LeanTween.cancel(scoreObj);
    LeanTween.cancel(gameObject);
    LeanTween.scale(scoreObj, newScale, scoreSpeed);
    LeanTween.delayedCall(scoreSpeed, () => { LeanTween.scale(scoreObj, scoreOriginalScale, 0.25f); });
  }

  void ComputeIncScore()
  {
    int diff = curInstantScore - curScore;
    int tmpInc = (int)Mathf.Ceil((diff * Time.deltaTime) / scoreSpeed);
    if (tmpInc > incScore) incScore = tmpInc;
  }

  public void DisplayScore()
  {
    if (scoreObj)
      scoreObj.GetComponent<TextMesh>().text = "$" + curScore.ToString();
  }

  public void UpdateLocalLeaderboard()
  {
    string name = GameManager.Instance.gameData.playerName;
    string tmpName = "";
    int tmpScore = 0;
    bool replaced = false;

    for (int i = 0; i < leaderboard.Count; ++i)
    {
      if (replaced)
      {
        Utility.Swap(ref leaderboard[i].first, ref tmpName);
        Utility.Swap(ref leaderboard[i].second, ref tmpScore);
      }
      else if (curInstantScore > leaderboard[i].second)
      {
        replaced = true;
        tmpName = leaderboard[i].first;
        tmpScore = leaderboard[i].second;
        leaderboard[i].first = name;
        leaderboard[i].second = curInstantScore;

        // trigger for new highscore
        GameManager.Instance.leaderboardMan.TriggerNewHighScore();
      }
    }

    //DisplayLeaderboard();
  }

  public void DisplayLeaderboard()
  {
    // update leaderboard text
    Text leaderNames = GameObject.Find("leader_names").GetComponent<Text>();
    Text leaderNumbers = GameObject.Find("leader_numbers").GetComponent<Text>();
    string leaderNameText = "", leaderNumText = "";
    for (int i = 0; i < leaderboard.Count; ++i)
    {
      leaderNameText += (i + 1) + ". " + leaderboard[i].first + "\n";
      leaderNumText += "$" + leaderboard[i].second.ToString() + "\n";
    }
    leaderNames.text = leaderNameText;
    leaderNumbers.text = leaderNumText;

    // update player prefs
    for (int i = 0; i < leaderboard.Count; ++i)
    {
      PlayerPrefs.SetInt(leaderboard[i].first, leaderboard[i].second);

      // to find the leaders on load
      PlayerPrefs.SetString(((LEADERBOARD)i).ToString(), leaderboard[i].first);
    }
  }

  public void LoadLeaderboard()
  {
    // update player prefs
    for (int i = 0; i < leaderboard.Count; ++i)
    {
      string name = PlayerPrefs.GetString(((LEADERBOARD)i).ToString());
      // skip if entry could not be found
      if (name == "") continue;

      leaderboard[i].first = name;
      leaderboard[i].second = PlayerPrefs.GetInt(name); }
  }

}
