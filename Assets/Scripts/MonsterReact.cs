using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MonsterReact : MonoBehaviour {

  public int[] curType;

	// Use this for initialization
	void Start () {
    curType = new int[2] { -1, -1 };
    RandomSprite();
  }

  void OnMouseOver()
  {
    OnTouchStay();
  }

  public void OnTouchStay()
  {
    // Serve request if able to
    if (InputMan.OnUp())
    {
      // debug
      //GameManager.Instance.gameData.pop_total += 150;
      //GameManager.Instance.socialMan.ReportScore(10 + (int)Time.time, SOCIALBOARD.EARNINGS);
      //StoreManager storeMan = FindObjectOfType<StoreManager>();
      //if (storeMan != null)
      //{
      //  storeMan.AddEarnings(200);
      //  storeMan.ConfirmBuy();
      //}

      if (GetComponent<SpriteRenderer>().color != Color.black)
        MonsterJump();
    }
  }

  public void MonsterJump()
  {
    GetComponent<Animator>().SetTrigger("jump");

    if (!SceneManager.GetActiveScene().name.Contains("store"))
    {
      LeanTween.delayedCall(0.8f, () =>
      {
        RandomSprite();
      });
    }
  }

  public void RandomSprite()
  {
    int[] monType = new int[2] { Random.Range(0, 4), Random.Range(0, 4) };

    while(monType[0] == curType[0])
    {
      monType[0] = Random.Range(0, 4);
    }

    var varList = GameManager.Instance.gameData.monsterVars;

    while (varList[monType[0]][monType[1]] != true)
    {
      monType[1] = Random.Range(0, 4);
    }

    curType = monType;
    GetComponent<SpriteRenderer>().sprite = GameManager.Instance.spriteMan.monsterSprites[monType[0]][monType[1]];
  }
}
