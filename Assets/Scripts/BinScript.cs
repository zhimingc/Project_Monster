using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BinScript : MonoBehaviour {

  public bool canThrowAway;
  public int maxCooldown, binCooldown;
  public Text cooldownText;
  private PlayerScript playerScript;
  //private IngredientManager ingredientMan;

	// Use this for initialization
	void Start () {
    canThrowAway = true;
    binCooldown = 0;

    playerScript = GameObject.Find("player").GetComponent<PlayerScript>();
    //ingredientMan = GameObject.Find("ingredient_manager").GetComponent<IngredientManager>();
    GetComponent<SpriteRenderer>().color = Color.white;
  }

  // Update is called once per frame
  void Update () {
    UpdateCooldown();
    UpdateStateFeedback();
  }

  void UpdateStateFeedback()
  {
    Sprite sprite = null;

    if (canThrowAway)
    {
      //GetComponent<SpriteRenderer>().color = Color.white;
      sprite = Resources.Load<Sprite>("Sprites/slot_circle_black");
      GetComponent<SpriteRenderer>().sprite = sprite;
    }
    else
    {
      //GetComponent<SpriteRenderer>().color = Color.red;
      sprite = Resources.Load<Sprite>("Sprites/slot_circle");
      GetComponent<SpriteRenderer>().sprite = sprite;
    }
  }

  void UpdateCooldown()
  {
    if (canThrowAway == false)
    {
      int curTurn = GameManager.Instance.turnCounter;
      int cooldownTime = maxCooldown - (curTurn - binCooldown);
      cooldownText.text = cooldownTime.ToString();

      if (curTurn - binCooldown >= maxCooldown)
      {
        canThrowAway = true;
      }
    }
    else
    {
      cooldownText.text = "";
    }
  }

  void OnMouseEnter()
  {
    OnTouchDown();
  }

  public void OnTouchDown()
  {
    if (canThrowAway && playerScript.playerState == PLAYER_STATE.DRAGGING)
    {
      //GetComponent<SpriteRenderer>().color = Color.red;
      // Stop grid from moving
      playerScript.blockBeingDragged.GetComponent<IngredientBlock>().beingDragged = false;
      // Update grid with move
      playerScript.blockBeingDragged.transform.position = transform.position;

      playerScript.blockBeingDragged.ToggleScale();
      playerScript.SetDeleteIngredient(true);

      // Set delegate to determine mouse up behaviour
      playerScript.SetMouseUpDel(gameObject, BinMouseUp);
    }
  }

  void BinMouseUp()
  {
    if (canThrowAway && playerScript.playerState == PLAYER_STATE.DRAGGING)
    {
      canThrowAway = false;
      // Set bin cooldown
      binCooldown = GameManager.Instance.turnCounter;

      // Audio feedback
      GameManager.Instance.SFX().PlaySoundWithPitch("trash", 0.75f, 1.0f);
    }
  }

  void OnMouseExit()
  {
    OnTouchExit();
  }

  public void OnTouchExit()
  {
    //GetComponent<SpriteRenderer>().color = Color.white;

    if (canThrowAway && playerScript.playerState == PLAYER_STATE.DRAGGING)
    {
      // Allow grid to move
      playerScript.blockBeingDragged.GetComponent<IngredientBlock>().beingDragged = true;
      // Update grid with move
      //playerScript.blockBeingDragged.SetBlockPosition(transform.position);

      playerScript.blockBeingDragged.ToggleScale();
      playerScript.SetDeleteIngredient(false);
      playerScript.ResetMouseUpDel(gameObject);
    }
  }
}
