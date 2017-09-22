using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardManager : MonoBehaviour {

  public GameObject earningsText, highscoreObj;
  public GameObject globalNamesText, globalNumText;

	// Use this for initialization
	void Start () {
    globalNamesText.GetComponent<Text>().text = "Global leaderboards coming soon...";
    globalNumText.GetComponent<Text>().text = "";
    highscoreObj.SetActive(false);
  }

  public void TriggerLeaderboard()
  {
    UpdateGameEndBoard();
    GetComponent<Animator>().SetTrigger("isEnter");
  }

	void UpdateGameEndBoard()
  {
    earningsText.GetComponent<TextMesh>().text = "$" + GameManager.Instance.scoreMan.curInstantScore;
  }

  public void TriggerNewHighScore()
  {
    highscoreObj.SetActive(true);
  }
}
