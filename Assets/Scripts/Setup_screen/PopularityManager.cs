using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopularityManager : MonoBehaviour {

  public ToolManager toolMan;
  public TextMesh popTotal, popNext, rankText;
  public GameObject progressBar, dayText;
  public float addingSpeed;

  public int incScore, rankIndex;
  public float barProgress;

  public GameObject[] pop_monsters_obj;

	// Use this for initialization
	void Start () {
    //pop_monsters_obj = GameObject.FindGameObjectsWithTag("PopMonster");
    popTotal.text = GameManager.Instance.GetTotalPopularity().ToString();

    dayText.GetComponent<TextMesh>().text = "Day " + (GameManager.Instance.gameData.count_days).ToString();
    InitRankTracking();
    InitPopMonsters();
  }

  public void TriggerPopularityUpdate()
  {
    ComputeIncScore();
    if (incScore != 0)
    {
      AnimateAddingText(popTotal.gameObject);
    }
  }

  void Update()
  {
    UpdatePopularityDifference();
    UpdateRank();

    // debug
    if (Input.GetKeyDown(KeyCode.S))
    {
      GameManager.Instance.scoreMan.totalScore += 1000;
      TriggerPopularityUpdate();
    }
  }

	void UpdatePopularityDifference()
  {
    int curScore = GameManager.Instance.GetTotalPopularity();
    int trueScore = GameManager.Instance.scoreMan.totalScore;

    if (curScore != trueScore)
    {
      curScore += incScore;
      if (curScore >= trueScore) curScore = trueScore;
      GameManager.Instance.SetTotalPopularity(curScore);
      popTotal.text = curScore.ToString();
    }
  }

  void InitRankTracking()
  {
    rankIndex = -1;
    for (int i = 0; i < GameProgression.rankReq.Length; ++i)
    {
      if (GameProgression.rankReq[i] > GameManager.Instance.GetTotalPopularity())
      {
        rankIndex = i;
        break;
      }
    }

    // is the current score exceeds max rank
    if (rankIndex < 0)
    {
      rankIndex = (int)RANKS.NUM_MILESTONES;
      progressBar.GetComponentInChildren<Image>().fillAmount = 1.0f;
      popNext.gameObject.SetActive(false);
      return;
    }

    barProgress = GameManager.Instance.GetTotalPopularity() / GameProgression.rankReq[rankIndex];
    barProgress = Mathf.Min(1.0f, barProgress);
    progressBar.GetComponentInChildren<Image>().fillAmount = barProgress;
  }

  void UpdateRank()
  {
    int displayRank = rankIndex;

    // check if rank up
    if (rankIndex < (int)RANKS.NUM_MILESTONES &&
      GameProgression.rankReq[rankIndex] <= GameManager.Instance.GetTotalPopularity())
    {
      if (popNext.gameObject.activeSelf) progressBar.GetComponentInChildren<ParticleSystem>().Play();
      if (rankIndex + 1 >= GameProgression.rankReq.Length)
      {
        displayRank = rankIndex + 1;
        popNext.gameObject.SetActive(false);
      }

      // rank up behaviour
      toolMan.UpdateToolBox((RANKS)rankIndex);
      CheckMonsterUnlock();
      GameManager.Instance.gameData.pop_rank = rankIndex;

      rankIndex = Mathf.Min(rankIndex + 1, GameProgression.rankReq.Length);
    }

    // update display
    if (rankIndex < GameProgression.rankReq.Length)
    {
      popNext.text = GameProgression.rankReq[rankIndex].ToString();

      float lastRank = 0.0f;
      if (rankIndex > 0) lastRank = GameProgression.rankReq[rankIndex - 1];
      barProgress = (GameManager.Instance.GetTotalPopularity() - lastRank) / (GameProgression.rankReq[rankIndex] - lastRank);
      barProgress = Mathf.Min(1.0f, barProgress);
      progressBar.GetComponentInChildren<Image>().fillAmount = barProgress;
    }

    rankText.text = "Rank: " + displayRank.ToString();
  }

  void InitPopMonsters()
  {
    float[] pop_monsters = GameManager.Instance.gameData.pop_monsters;

    for(int i = 0; i < pop_monsters.Length; ++i)
    {
      if (pop_monsters[i] > 0.0f)
      {
        pop_monsters_obj[i].GetComponent<MonsterSetup>().ActivateMonster();
        pop_monsters_obj[i].GetComponentsInChildren<SpriteRenderer>()[1].color = Color.white;
        pop_monsters_obj[i].GetComponentInChildren<TextMesh>().text = pop_monsters[i].ToString("0") + "%";
      }
      else
      {
        pop_monsters_obj[i].GetComponentsInChildren<SpriteRenderer>()[1].color = Color.black;
        pop_monsters_obj[i].GetComponentInChildren<TextMesh>().text = "";
      }
    }
  }

  void CheckMonsterUnlock()
  {
    float[] pop_monsters = GameManager.Instance.gameData.pop_monsters;
    int FIRST_TIME_PERCENTAGE = 24;

    switch((RANKS)rankIndex)
    {
      case RANKS.IMPATIENT_MON:
        pop_monsters[0] -= FIRST_TIME_PERCENTAGE;
        pop_monsters[1] = FIRST_TIME_PERCENTAGE;
        pop_monsters_obj[1].GetComponent<MonsterSetup>().SetMonsterBubble();
        break;
      case RANKS.PICKY_MON:
        // take the first time percentage out of each active monster
        for (int i = 0; i < 2; ++i)
        {
          pop_monsters[i] -= FIRST_TIME_PERCENTAGE / 2;
        }
        pop_monsters[2] = FIRST_TIME_PERCENTAGE;
        pop_monsters_obj[2].GetComponent<MonsterSetup>().SetMonsterBubble();
        break;
    }

    // update game data
    GameManager.Instance.gameData.pop_monsters = pop_monsters;
    // re init the pop monster objects
    InitPopMonsters();
  }

  void ComputeIncScore()
  {
    int trueScore = GameManager.Instance.scoreMan.totalScore;
    int curScore = GameManager.Instance.GetTotalPopularity();

    int diff = trueScore - curScore;
    int tmpInc = (int)Mathf.Ceil((diff * Time.deltaTime) / addingSpeed);
    if (tmpInc > incScore) incScore = tmpInc;
  }

  void AnimateAddingText(GameObject text)
  {
    Vector3 scoreOriginalScale = text.transform.localScale;
    Vector3 newScale = scoreOriginalScale * 1.25f;

    LeanTween.cancel(text);
    LeanTween.scale(text, newScale, addingSpeed);
    LeanTween.delayedCall(text, addingSpeed, () => { LeanTween.scale(text, scoreOriginalScale, 0.25f); });
  }
}
