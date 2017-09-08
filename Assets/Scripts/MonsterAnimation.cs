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
  private Sprite[] monsterStates;
  //private int delayedCallID;

  // Use this for initialization
  void Awake () {
    backMan = GameObject.Find("Background").GetComponent<BackgroundManager>();

    animTime = 1.0f;
    boundaries = backMan.cloudPosLimits;
    origin = transform.position;
    //originScale = transform.localScale;

    // sprites
    monsterStates = new Sprite[(int)MONSTER_TYPE.NUM_TYPES];
    monsterStates[0] = Resources.Load<Sprite>("Sprites/monster_basic");
    monsterStates[1] = Resources.Load<Sprite>("Sprites/monster_impatient");
    monsterStates[2] = Resources.Load<Sprite>("Sprites/monster_rude");
    monsterStates[3] = Resources.Load<Sprite>("Sprites/monster_picky2");
    monsterStates[4] = Resources.Load<Sprite>("Sprites/monster_picky2");
    monsterStates[5] = Resources.Load<Sprite>("Sprites/monster_greedy1"); // garbage
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
    GetComponent<SpriteRenderer>().sprite = monsterStates[spriteIndex];
    GetComponent<SpriteRenderer>().color = Color.white;

    if (type == MONSTER_TYPE.NORMAL)
    {
      if (monReq.ingredients.Count == 3)
      {
        if (monReq.ingredients[2] == INGREDIENT_TYPE.BREAD)
        {
          GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/monster_basic_1");
        }
        else if (monReq.ingredients[2] == INGREDIENT_TYPE.MEAT)
        {
          GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/monster_basic_2");
        }
      }
      else if (monReq.ingredients.Count == 2)
      {
        if (monReq.ingredients[1] == INGREDIENT_TYPE.MEAT)
        {
          GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/monster_basic_0");
        }
      }
    }

    //if (type == MONSTER_TYPE.PICKY)
    //{
    //  GetComponent<SpriteRenderer>().color = Color.cyan;
    //}
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
    GetComponent<Animator>().SetBool("isMoving", true);
    parent.transform.position = from;

    LeanTween.move(parent, origin, animTime).setEase(LeanTweenType.easeInOutQuad);
    LeanTween.delayedCall(gameObject, animTime, () =>
    {
      GetComponent<Animator>().SetBool("isMoving", false);
    });

    MonsterRequest req = requestObj.GetComponent<MonsterRequest>();
    if (req != null)
    {
      req.ToggleSpeechBubble(true);
    }
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
