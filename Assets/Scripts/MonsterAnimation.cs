using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAnimation : MonoBehaviour {

  // Animation properties
  public float animTime;
  public GameObject requestObj, parent;
  public Vector3 origin, landingPos;// originScale;
  public MONSTER_TYPE type;

  private float[] boundaries;
  private BackgroundManager backMan;
  //private Sprite[] monsterStates;
  //private int delayedCallID;

  // Use this for initialization
  void Awake () {
    backMan = GameObject.Find("Background").GetComponent<BackgroundManager>();

    animTime = 1.0f;
    boundaries = backMan.cloudPosLimits;
    origin = transform.position;
    //originScale = transform.localScale;

    // sprites
    //monsterStates = new Sprite[(int)MONSTER_TYPE.NUM_TYPES];
    //monsterStates[0] = Resources.Load<Sprite>("Sprites/monster_basic");
    //monsterStates[1] = Resources.Load<Sprite>("Sprites/monster_impatient");
    //monsterStates[2] = Resources.Load<Sprite>("Sprites/monster_rude2");
    //monsterStates[3] = Resources.Load<Sprite>("Sprites/monster_picky2");
    //monsterStates[4] = Resources.Load<Sprite>("Sprites/monster_picky2");
    //monsterStates[5] = Resources.Load<Sprite>("Sprites/monster_greedy1"); // garbage
  }

  void Start()
  {
    // init sprite
    if (requestObj != null)
    {
      MonsterRequest req = requestObj.GetComponent<MonsterRequest>();
      type = req.request.monsterType;
    }
  }

  // init the monster sprite according to monster type
  public void InitSprite(Request monReq)
  {
    type = monReq.monsterType;
    int spriteIndex = (int)monReq.monsterType;
    var varList = GameManager.Instance.gameData.monsterVars;

    int variationIndex = -1;
    // random pick of all unlocked
    while (variationIndex == -1)
    {
      int randVar = Random.Range(0, 4);
      if (varList[spriteIndex][randVar]) variationIndex = randVar;
    }

    GetComponent<SpriteRenderer>().color = Color.white;

    if (monReq.ingredients.Count == 3)
    {
      if (monReq.ingredients[2] == INGREDIENT_TYPE.BREAD)
      {
        if (varList[spriteIndex][2]) variationIndex = 2;
      }
      else if (monReq.ingredients[2] == INGREDIENT_TYPE.MEAT)
      {
        if (varList[spriteIndex][3]) variationIndex = 3;
      }
    }
    else if (monReq.ingredients.Count == 2)
    {
      if (monReq.ingredients[1] == INGREDIENT_TYPE.MEAT)
      {
        if (varList[spriteIndex][1]) variationIndex = 1;
      }
    }

    // apply chosen sprite
    GetComponent<SpriteRenderer>().sprite = GameManager.Instance.spriteMan.monsterSprites[spriteIndex][variationIndex];
  }

  public void Hide()
  {
    // move out of frame
    parent.transform.position = new Vector3(boundaries[1], 0, 0);
  }

  public void ForceMoveComplete()
  {
    LeanTween.cancel(parent);
    LeanTween.cancel(gameObject);
    //GetComponent<Animator>().SetBool("isMoving", false);
    parent.transform.position = origin;
  }

  public void MoveInFrom(Vector3 from)
  {
    GetComponent<SpriteRenderer>().enabled = true;
    parent.transform.position = from;

    MonsterRequest req = requestObj.GetComponent<MonsterRequest>();
    req.transform.localScale = new Vector3(0, 0, 0);

    LeanTween.delayedCall(0.5f, () =>
    {
      GetComponent<Animator>().SetBool("isMoving", true);

      LeanTween.move(parent, origin, animTime).setEase(LeanTweenType.easeInOutQuad);
      LeanTween.delayedCall(gameObject, animTime, () =>
      {
        GetComponent<Animator>().SetBool("isMoving", false);
      });

      if (req != null)
      {
        req.ToggleSpeechBubble(true);
      }
    });

  }

  public void MoveOutFrom(Vector3 from)
  {
    GetComponent<Animator>().SetBool("isMoving", true);
    LeanTween.delayedCall(animTime, () =>
    {
      GetComponent<Animator>().SetBool("isMoving", false);
    });

    Vector3 moveTo = from;
    moveTo.x = boundaries[1];

    parent.transform.position = from;
    LeanTween.move(parent, moveTo, animTime).setEase(LeanTweenType.easeInOutQuad);
  }

  public bool isAnimating()
  {
    return LeanTween.isTweening(gameObject);
  }
}
