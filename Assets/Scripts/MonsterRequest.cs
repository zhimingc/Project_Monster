using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MONSTER_TYPE
{
  NORMAL,
  TIMED,
};

public struct MonsterTypeParams
{
  // timed monsters
  public float curTimer, maxTimer;
}

public class MonsterRequest : MonoBehaviour {

  public GameObject ingredientObj, speechBubble, monsterObj;
  public Request request;
  public MONSTER_TYPE monsterType;

  private List<GameObject> ingredientStackObjs;
  private Vector3 originScale;

  // monster specific behaviour
  private MonsterTypeParams typeParams;
  public GameObject stopwatch;

  // Use this for initialization
  void Start () {
    originScale = transform.localScale;
  }
	
	// Update is called once per frame
	void Update () {
    MonsterUpdate();

  }

  void MonsterUpdate()
  {
    if (GameManager.Instance.IsPaused()) return;

    switch (monsterType)
    {
      case MONSTER_TYPE.TIMED:
        if (typeParams.curTimer <= 0.0f)
        {
          // times up
          GameManager.Instance.SetLoseBehaviour();
        }
        else
        {
          typeParams.curTimer -= Time.deltaTime;
          float cutoff = 0.001f + (1.0f - typeParams.curTimer / typeParams.maxTimer);
          stopwatch.GetComponentInChildren<MeshRenderer>().material.SetFloat("_Cutoff", cutoff);

          // text feedback
          string timeString = "";
          if (typeParams.curTimer > 10.0f) timeString = typeParams.curTimer.ToString("0");
          else timeString = typeParams.curTimer.ToString("0.0");
          stopwatch.GetComponentInChildren<TextMesh>().text = timeString;
        }
        break;
    }
  }

  void SetMonsterType(Request req)
  {
    monsterType = req.monsterType;
    typeParams = req.typeParams;

    // init depending on monster type
    switch (monsterType)
    {
      case MONSTER_TYPE.NORMAL:
        stopwatch.SetActive(false);
        break;
      case MONSTER_TYPE.TIMED:
        stopwatch.SetActive(true);
        break;
    }
  }

  public void ToggleSpeechBubble(bool flag)
  {
    speechBubble.GetComponent<SpriteRenderer>().enabled = flag;
    if (flag == true)
    {
      transform.localScale = new Vector3(0, 0, 0);
      LeanTween.delayedCall(0.5f, () => 
      {
        LeanTween.scale(gameObject, originScale, 1.0f).setEase(LeanTweenType.easeInOutQuad);
      });
    }
  }

  public void SetRequest(Request req)
  {
    request = req;

    // Set new monster type
    SetMonsterType(req);

    // reset ingredient stack obj
    for (int i = 0; i < ingredientStackObjs.Count; ++i)
    {
      Sprite sprite = Resources.Load<Sprite>("Sprites/ingredient_side");
      ingredientStackObjs[i].GetComponent<SpriteRenderer>().sprite = sprite;
    }

    // Set the display according to ingredient
    for (int i = 0; i < req.ingredients.Count; ++i)
    {
      ObjectFactory.InitializeIngredientSide(ingredientStackObjs[i], req.ingredients[i]);

      if (i != 0 && req.ingredients[i] == INGREDIENT_TYPE.BREAD)
      {
        Sprite sprite = Resources.Load<Sprite>("Sprites/bread_side_top");
        ingredientStackObjs[i].GetComponent<SpriteRenderer>().sprite = sprite;
      }
    }

    // Set display according to sauce type
    ObjectFactory.InitializeSauce(gameObject, req.sauce);

    // Set display according to grid type
    ObjectFactory.InitializeGrid(gameObject, req.gridType);
  }

  public void InitStack()
  {
    ingredientStackObjs = new List<GameObject>();

    for (int i = 0; i < 4; ++i)
    {
      GameObject sideObj = Instantiate(ingredientObj, transform.position, Quaternion.identity);
      Vector3 localScale = transform.localScale;
      localScale = Vector3.Scale(localScale, new Vector3(0.4f, 0.5f, 1.0f));
      sideObj.transform.SetParent(transform);
      sideObj.transform.localScale = localScale;
      sideObj.transform.localPosition += new Vector3(0, -transform.localScale.y / 3.0f + i * localScale.y / 2.5f, 0);

      ingredientStackObjs.Add(sideObj);
    }
  }


}
