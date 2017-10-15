using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MONSTER_TYPE
{
  NORMAL,
  TIMED,
  PICKY,
  GARBAGE,
  RUDE,
  GREEDY,
  NUM_TYPES
};

public struct MonsterTypeParams
{
  // timed monsters
  public float curTimer, maxTimer;

  // picky monster
  public GridScript specificGrid;
}

public class MonsterRequest : MonoBehaviour {

  public GameObject ingredientObj, speechBubble, monsterObj;
  public GameObject requestText; // i.e. garbage monster text
  public GameObject chairObj;
  public GameObject comboObj; // combo feedback obj
  public Request request;
  public MonsterFeedback monsterFbScript;

  private List<GameObject> ingredientStackObjs;
  private Vector3 originScale;

  // monster specific behaviour
  private MonsterTypeParams typeParams;
  public GameObject stopwatch;
  public GameObject pickyIndicator;

  // Use this for initialization
  void Awake () {
    originScale = transform.localScale;
  }
	
	// Update is called once per frame
	void Update () {
    MonsterUpdate();
  }

  void MonsterUpdate()
  {
    if (GameManager.Instance.IsPaused()) return;

    switch (request.monsterType)
    {
      case MONSTER_TYPE.TIMED:
        if (typeParams.curTimer <= 0.0f)
        {
          // times up        
          GameManager.Instance.SetLoseBehaviour(LOSE_REASON.TIME_UP);
        }
        else
        {
          typeParams.curTimer -= Time.deltaTime;
          float cutoff = 0.001f + (1.0f - typeParams.curTimer / typeParams.maxTimer);
          cutoff = Mathf.Clamp(cutoff, 0.001f, 1.0f);
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
    request.monsterType = req.monsterType;
    typeParams = req.typeParams;

    // reset specific type feedback
    stopwatch.SetActive(false);
    pickyIndicator.SetActive(false);

    // init depending on monster type
    switch (request.monsterType)
    {
      case MONSTER_TYPE.TIMED:
        stopwatch.SetActive(true);
        break;
      case MONSTER_TYPE.PICKY:
        pickyIndicator.SetActive(true);
        pickyIndicator.GetComponent<SpriteRenderer>().color = chairObj.GetComponent<SpriteRenderer>().color;
        break;
    }
  }

  public void ToggleSpeechBubble(bool flag)
  {
    LeanTween.cancel(gameObject);

    speechBubble.GetComponent<SpriteRenderer>().enabled = flag;
    if (flag == true)
    {
      LeanTween.delayedCall(gameObject, 0.25f, () => 
      {
        LeanTween.scale(gameObject, originScale, 1.0f).setEase(LeanTweenType.easeInOutQuad);
      });
    }
  }

  public void SetRequest(Request req)
  {
    request = req;

    // update chair color depending on which lane it is
    request.chairColor = chairObj.GetComponent<SpriteRenderer>().color;

    // Set new monster type
    SetMonsterType(req);
    monsterObj.GetComponent<MonsterAnimation>().InitSprite(req);

    // reset ingredient stack obj
    for (int i = 0; i < ingredientStackObjs.Count; ++i)
    {
      Sprite sprite = Resources.Load<Sprite>("Sprites/ingredient_side");
      ingredientStackObjs[i].GetComponent<SpriteRenderer>().sprite = sprite;
      ingredientStackObjs[i].GetComponent<SpriteRenderer>().color = Color.white;
      ingredientStackObjs[i].GetComponent<SpriteRenderer>().enabled = true;
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
    ObjectFactory.InitializeSaucePlate(gameObject, req.sauce);

    // Set display according to grid type
    ObjectFactory.InitializeGrid(gameObject, req.gridType);

    // Specific monster behaviour
    switch (req.monsterType)
    {
      case MONSTER_TYPE.GARBAGE:
        // reset ingredient stack obj
        for (int i = 0; i < ingredientStackObjs.Count; ++i)
        {
          ingredientStackObjs[i].GetComponent<SpriteRenderer>().enabled = false;
        }
        requestText.SetActive(true);
        break;
      default:
        requestText.SetActive(false);
        break;
    }
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

  public void UpdateRequest()
  {
    SetRequest(request);
  }

  public void TriggerComboObj()
  {
    // get combo number

  }

}
