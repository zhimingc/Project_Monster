using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreManager : MonoBehaviour {

  public GameObject buySignObj, notEnoughText;
  public GameObject[] buySignButtons;
  public GameObject earningsText, stampText;
  public GameObject unlockParticles;
  public int buyCost;
  public List<List<GameObject>> monsterVarObjs;

  private List<Pair<int, int>> lockedMonsters;
  private int earnings, stamps;

	// Use this for initialization
	void Start () {
    // Get earnings and stamp numbers from game man
    earnings = GameManager.Instance.gameData.pop_total;
    stamps = GameManager.Instance.gameData.stamp_total;

    UpdateDisplays();

    // get monster game objects
    /*
      0 - basic
      1 - greedy
      2 - impatient
      3 - picky
    */
    monsterVarObjs = new List<List<GameObject>>();
    PopulateMonsterVarObjs("monster_basic");
    PopulateMonsterVarObjs("monster_impatient");
    PopulateMonsterVarObjs("monster_rude");
    PopulateMonsterVarObjs("monster_greedy");

    InitiateMonsterDisplay();
  }
	
  void PopulateMonsterVarObjs(string name)
  {
    List<GameObject> monList = new List<GameObject>();

    for (int i = 0; i < 4; ++i)
    {
      monList.Add(GameObject.Find(name + "_" + i.ToString()));
    }

    monsterVarObjs.Add(monList);
  }

  void UpdateDisplays()
  {
    earningsText.GetComponent<TextMesh>().text = "$" + earnings.ToString();
    stampText.GetComponent<TextMesh>().text = stamps.ToString();
  }

  // Update is called once per frame
  void Update () {
    if (Input.GetKeyDown(KeyCode.B))
    {
      buySignObj.GetComponent<Animator>().SetTrigger("enter");
    }
    if (Input.GetKeyDown(KeyCode.E))
    {
      buySignObj.GetComponent<Animator>().SetTrigger("exit");
    }
    if (Input.GetKeyDown(KeyCode.U))
    {
      earnings += 1000;
      ConfirmBuy();
    }
  }

  bool CheckForMonsterToUnlock()
  {
    bool result = false;
    lockedMonsters = new List<Pair<int, int>>();
    var monsterVars = GameManager.Instance.gameData.monsterVars;

    for (int i = 0; i < monsterVars.Count; ++i)
    {
      for (int j = 0; j < monsterVars[i].Count; ++j)
      {
        if (monsterVars[i][j] == false)
        {
          result = true;
          lockedMonsters.Add(new Pair<int, int>(i, j));
        }
      }
    }

    // all monster var flags are true
    return result;
  }

  void InitiateMonsterDisplay()
  {
    var monsterVars = GameManager.Instance.gameData.monsterVars;

    for (int i = 0; i < monsterVars.Count; ++i)
    {
      for (int j = 0; j < monsterVars[i].Count; ++j)
      {
        if (monsterVars[i][j] == true)
        {
          float delayRange = Random.Range(0.0f, 1.0f);
          GameObject monsterObj = monsterVarObjs[i][j];
          monsterObj.GetComponent<SpriteRenderer>().color = Color.white;

          LeanTween.delayedCall(delayRange, () =>
          {
            monsterObj.GetComponent<Animator>().SetTrigger("breath");
          });
        }
      }
    }
  }

  void ToggleMonsterCols(bool flag)
  {
    for (int i = 0; i < monsterVarObjs.Count; ++i)
    {
      for (int j = 0; j < monsterVarObjs[i].Count; ++j)
      {
        monsterVarObjs[i][j].GetComponent<BoxCollider2D>().enabled = flag;
      }
    }
  }

  public void ToggleButtonCols(bool flag)
  {
    foreach (GameObject button in buySignButtons)
    {
      button.GetComponent<BoxCollider2D>().enabled = flag;
    }
  }

  public void TriggerBuySign(bool flag)
  {
    // turn off monster cols
    ToggleMonsterCols(!flag);

    // activate button cols when lowering sign else turn off
    ToggleButtonCols(flag);

    // check if player has enough earnings
    if (earnings < buyCost)
    {
      LeanTween.cancel(notEnoughText);

      notEnoughText.GetComponent<RectTransform>().localPosition = new Vector3(380.0f, -500.0f, 0.0f);
      notEnoughText.SetActive(true);
      LeanTween.moveY(notEnoughText.GetComponent<RectTransform>(), -350.0f, 2.0f);
      LeanTween.delayedCall(notEnoughText, 2.0f, () =>
      {
        notEnoughText.SetActive(false);
      });
      return;
    }

    Animator buySignAnim = buySignObj.GetComponent<Animator>();
    buySignAnim.ResetTrigger("enter");
    buySignAnim.ResetTrigger("exit");

    if (flag) buySignAnim.SetTrigger("enter");
    else buySignAnim.SetTrigger("exit");
  }

  public void ConfirmBuy()
  {
    // decrease earnings
    earnings -= buyCost;

    // unlock variation if it exists
    if (CheckForMonsterToUnlock())
    {
      // Get random monster to unlock
      int unlocked = Random.Range(0, lockedMonsters.Count);
      int monX = lockedMonsters[unlocked].first;
      int monY = lockedMonsters[unlocked].second;

      GameManager.Instance.gameData.monsterVars[monX][monY] = true;

      // trigger unlock
      monsterVarObjs[monX][monY].GetComponent<Animator>().SetTrigger("unlock");
      LeanTween.delayedCall(2.0f, () =>
      {
        GameFeel.ShakeCameraRandom(new Vector3(0.05f, 0.05f, 0.0f), new Vector3(-0.1f, -0.1f, 0.0f), 4, 0.2f);
        unlockParticles.transform.position = monsterVarObjs[monX][monY].transform.position;
        unlockParticles.GetComponent<ParticleSystem>().Play();
        monsterVarObjs[monX][monY].GetComponent<SpriteRenderer>().color = Color.white;
      });
    }

    // update game data
    GameManager.Instance.gameData.pop_total = earnings;
    UpdateDisplays();
    GameManager.Instance.SaveEarnings();
    GameManager.Instance.SaveMonsterVar();

    // analytics
    int numUnlocked = GameManager.Instance.gameData.GetNumUnlocked();
    GameManager.Instance.analyticMan.UpdateAnalytics(ANALYTICS.NUM_UNLOCKS, numUnlocked);
    GameManager.Instance.analyticMan.PostAnalytics(ANALYTICS_EVENT.PLAYER_UNLOCK);
  }

  public void AddEarnings(int add)
  {
    earnings += add;
  }
}
