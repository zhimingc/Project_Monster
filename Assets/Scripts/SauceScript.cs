﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SauceScript : MonoBehaviour {

  public bool isCoolingDown;
  public SAUCE_TYPE sauceType;
  public GameObject sauceObj;
  public int maxCooldown, curCooldown;
  public Text cooldownText;

  private IngredientManager ingredientMan;
  private PlayerScript playerScript;

  // Use this for initialization
  void OnEnable()
  {
    playerScript = GameObject.Find("player").GetComponent<PlayerScript>();
    ingredientMan = GameObject.Find("ingredient_manager").GetComponent<IngredientManager>();

    GenerateSauce();
    isCoolingDown = false;
  }
	
	// Update is called once per frame
	void Update () {
    UpdateCooldown();
    UpdateStateFeedback();
  }

  public void SauceMouseUp()
  {
    if (!isCoolingDown && playerScript.playerState == PLAYER_STATE.DRAGGING)
    {
      if (Input.GetMouseButtonUp(0) && playerScript.hoveredGrid != null)
      {
        isCoolingDown = true;
        // Set sauce cooldown
        curCooldown = GameManager.Instance.turnCounter;
      }
    }
  }

  void OnMouseOver()
  {
    if (Input.GetMouseButtonDown(0) &&
      !isCoolingDown && playerScript.playerState == PLAYER_STATE.IDLE)
    {
      sauceObj.GetComponent<IngredientBlock>().StartDrag();
      // Set delegate to determine mouse up behaviour
      playerScript.SetMouseUpDel(SauceMouseUp);
    }
  }

  void OnMouseExit()
  {
    if (!isCoolingDown && playerScript.playerState == PLAYER_STATE.IDLE)
    {
      playerScript.ResetMouseUpDel();
    }
  }

  void UpdateCooldown()
  {
    if (sauceObj == null) isCoolingDown = true; 

    if (isCoolingDown)
    {
      int curTurn = GameManager.Instance.turnCounter;
      int cooldownTime = maxCooldown - (curTurn - curCooldown);
      cooldownText.text = cooldownTime.ToString();

      if (curTurn - curCooldown >= maxCooldown)
      {
        isCoolingDown = false;
        GenerateSauce();
      }
    }
    else
    {
      cooldownText.text = "";
    }
  }

  void UpdateStateFeedback()
  {
    if (isCoolingDown)
    {
      GetComponent<SpriteRenderer>().color = Color.red;
    }
    else
    {
      GetComponent<SpriteRenderer>().color = Color.white;
    }
  }

  void GenerateSauce()
  {
    // Get random layout 
    int layout = ingredientMan.GenerateLayout();
    sauceObj = ingredientMan.GenerateIngredient(INGREDIENT_TYPE.SAUCE, sauceType, layout, transform);
    sauceObj.transform.position = transform.position;
    sauceObj.GetComponent<BoxCollider2D>().enabled = false;
  }

  
}
