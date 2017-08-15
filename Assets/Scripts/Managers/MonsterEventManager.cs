using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MONSTER_EVENT
{
  FIRST_DAY,
  ZEN_EVENT,
  MAIN_EVENT,
  FRENZY_EVENT,
  NUM_EVENTS,
};

public class MonsterEventManager : MonoBehaviour {

  public float scrollSpeed;
  public GameObject[] scrollButtons;
  public MONSTER_EVENT curEvent;
  public MONSTER_EVENT[] allEvents;
  public GameObject[] allEventObjs; 

	// Use this for initialization
	void Start () {
    curEvent = GameManager.Instance.gameData.eventType;
    int curIndex = (int)curEvent;
    if (curIndex == 0) scrollButtons[0].SetActive(false);
    if (curIndex == allEventObjs.Length - 1) scrollButtons[1].SetActive(false);

    Vector3 curPos = allEventObjs[curIndex].transform.position;
    curPos.x = 0.0f;
    allEventObjs[curIndex].transform.position = curPos;
  }

  // Update is called once per frame
  void Update () {
		
	}

  public void ScrollEventForward()
  {
    int curIndex = (int)curEvent;
    scrollButtons[0].SetActive(true);

    // move cur event obj out
    LeanTween.moveLocalX(allEventObjs[curIndex], -2.0f, scrollSpeed);

    // move next event obj in
    int nextIndex = curIndex + 1;
    Vector3 curPos = allEventObjs[nextIndex].transform.localPosition;
    curPos.x = 2.0f;
    allEventObjs[nextIndex].transform.localPosition = curPos;
    LeanTween.moveLocalX(allEventObjs[nextIndex], 0.0f, scrollSpeed);

    curEvent = (MONSTER_EVENT) nextIndex;
    GameManager.Instance.gameData.eventType = curEvent;
    if (nextIndex == allEventObjs.Length - 1)
    {
      scrollButtons[1].SetActive(false);
    }

    UpdateEventData();
  }

  public void ScrollEventBack()
  {
    int curIndex = (int)curEvent;
    scrollButtons[1].SetActive(true);

    // move cur event obj out
    LeanTween.moveLocalX(allEventObjs[curIndex], 2.0f, scrollSpeed);

    // move next event obj in
    int nextIndex = curIndex - 1;
    Vector3 curPos = allEventObjs[nextIndex].transform.localPosition;
    curPos.x = -2.0f;
    allEventObjs[nextIndex].transform.localPosition = curPos;
    LeanTween.moveLocalX(allEventObjs[nextIndex], 0.0f, scrollSpeed);

    curEvent = (MONSTER_EVENT)nextIndex;
    GameManager.Instance.gameData.eventType = curEvent;
    if (nextIndex == 0)
    {
      scrollButtons[0].SetActive(false);
    }

    UpdateEventData();
  }

  void UpdateEventData()
  {
    switch(curEvent)
    {
      case MONSTER_EVENT.FIRST_DAY:
        GameManager.Instance.gameData.pop_monsters = new float[(int)MONSTER_TYPE.NUM_TYPES];
        GameManager.Instance.gameData.pop_monsters[0] = 100.0f;
        break;
      case MONSTER_EVENT.MAIN_EVENT:
        GameManager.Instance.gameData.pop_monsters = new float[(int)MONSTER_TYPE.NUM_TYPES];
        GameManager.Instance.gameData.pop_monsters[0] = 34.0f;
        GameManager.Instance.gameData.pop_monsters[1] = 33.0f;
        GameManager.Instance.gameData.pop_monsters[2] = 33.0f;
        break;
      case MONSTER_EVENT.FRENZY_EVENT:
        GameManager.Instance.gameData.pop_monsters = new float[(int)MONSTER_TYPE.NUM_TYPES];
        GameManager.Instance.gameData.pop_monsters[0] = 34.0f;
        GameManager.Instance.gameData.pop_monsters[1] = 33.0f;
        GameManager.Instance.gameData.pop_monsters[2] = 33.0f;
        break;
      case MONSTER_EVENT.ZEN_EVENT:
        GameManager.Instance.gameData.pop_monsters = new float[(int)MONSTER_TYPE.NUM_TYPES];
        GameManager.Instance.gameData.pop_monsters[0] = 34.0f;
        GameManager.Instance.gameData.pop_monsters[1] = 33.0f;
        GameManager.Instance.gameData.pop_monsters[2] = 33.0f;
        break;
    }
  }
}
