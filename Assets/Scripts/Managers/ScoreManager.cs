using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ScoreManager : MonoBehaviour {

  public int numServed;
  public float scoreSpeed;  // how long it takes for the numbers to add

  private GameObject scoreObj, numServedObj;
  // trueScore is the true amount, incScore is the amount to add per frame
  public int totalScore, incScore;
  public int curScore, curInstantScore;
  private Vector3 scoreOriginalScale;

  public int AddScore(int amt)
  {
    amt = (int) (amt * GameManager.Instance.comboMan.GetComboMultiplier());
    totalScore += amt;
    curInstantScore += amt;

    // compute the amount of be added every frame
    ComputeIncScore();
    AnimateAdding();

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
      scoreObj.GetComponent<TextMesh>().text = curScore.ToString();
  }

}
