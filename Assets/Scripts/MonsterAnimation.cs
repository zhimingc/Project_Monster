﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAnimation : MonoBehaviour {

  // Animation properties
  public float animTime;
  public GameObject requestObj, parent;
  public Vector3 origin;// originScale;

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
    monsterStates = new Sprite[2];
    monsterStates[0] = Resources.Load<Sprite>("Sprites/monster_1");
    monsterStates[1] = Resources.Load<Sprite>("Sprites/monster_1_angry");

    // initial state
    //GetComponent<SpriteRenderer>().sprite = monsterStates[0];
  }

  // init the monster sprite according to monster type
  public void InitSprite()
  {
    int spriteIndex = (int)requestObj.GetComponent<MonsterRequest>().request.monsterType;
    GetComponent<SpriteRenderer>().sprite = monsterStates[spriteIndex];
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
    // monsters moving out are angry (sprite)
    InitSprite();

    GetComponent<Animator>().SetBool("isMoving", true);
    parent.transform.position = from;

    //float offset = origin.x - boundaries[0];
    //transform.localScale += new Vector3(offset / 2.75f, 0, 0);

    //LeanTween.scale(gameObject, originScale, animTime);
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
    // monsters moving out are happy (sprite)
    GetComponent<SpriteRenderer>().sprite = monsterStates[0];

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
