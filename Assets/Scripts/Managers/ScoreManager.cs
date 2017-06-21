using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ScoreManager {

  public int score;
  private GameObject scoreObj;

  public void AddScore(int amt)
  {
    score += amt;
  }

  public void InitScore()
  {
    scoreObj = GameObject.Find("score");
    score = 0;
  }

  public void DisplayScore()
  {
    scoreObj.GetComponent<Text>().text = score.ToString();
  }

}
