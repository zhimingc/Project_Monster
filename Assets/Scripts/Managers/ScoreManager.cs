using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ScoreManager : MonoBehaviour {

  public int numServed;
  public float scoreSpeed;  // how long it takes for the numbers to add

  private GameObject scoreObj;
  // trueScore is the true amount, incScore is the amount to add per frame
  public int trueScore, incScore;
  public int totalCurrency;
  private int curScore;
  private Vector3 scoreOriginalScale;

  void Start()
  {
    totalCurrency = 0;
  }

  public int AddScore(int amt)
  {
    amt = (int) (amt * GameManager.Instance.comboMan.GetComboMultiplier());
    trueScore += amt;
    totalCurrency += amt;

    // compute the amount of be added every frame
    ComputeIncScore();
    AnimateAdding();

    return amt;
  }

  public void AddNumberServed(int amt)
  {
    numServed += amt;
  }


  public void InitScore()
  {
    if (GameManager.Instance.IsInGame())
    {
      scoreObj = GameObject.Find("money_text");
      scoreOriginalScale = scoreObj.transform.localScale;
    }
    numServed = 0;
    curScore = 0;
    trueScore = 0;

    scoreSpeed = 0.25f;
    DisplayScore();
  }

  void Update()
  {
    if (curScore != trueScore)
    {
      curScore += incScore;
      if (curScore >= trueScore) curScore = trueScore;
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
    int diff = trueScore - curScore;
    int tmpInc = (int) ((diff * Time.deltaTime) / scoreSpeed);
    if (tmpInc > incScore) incScore = tmpInc;
  }

  public void DisplayScore()
  {
    scoreObj.GetComponent<TextMesh>().text = "$" + curScore.ToString();
  }

}
