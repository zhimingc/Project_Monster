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
    if (canThrowAway)
    {
      GetComponent<SpriteRenderer>().color = Color.white;
    }
    else
    {
      GetComponent<SpriteRenderer>().color = Color.red;
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
    if (canThrowAway && playerScript.playerState == PLAYER_STATE.DRAGGING)
    {
      GetComponent<SpriteRenderer>().color = Color.red;
      // Stop grid from moving
      playerScript.blockBeingDragged.GetComponent<IngredientBlock>().beingDragged = false;
      // Update grid with move
      playerScript.blockBeingDragged.transform.position = transform.position;

      playerScript.blockBeingDragged.ToggleScale();
      playerScript.SetDeleteIngredient(true);

      // Set delegate to determine mouse up behaviour
      playerScript.SetMouseUpDel(BinMouseUp);
    }
  }

  void BinMouseUp()
  {
    if (canThrowAway && playerScript.playerState == PLAYER_STATE.DRAGGING)
    {
      if (Input.GetMouseButtonUp(0))
      {
        canThrowAway = false;
        // Set bin cooldown
        binCooldown = GameManager.Instance.turnCounter;
      }
    }
  }

  void OnMouseExit()
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
      playerScript.ResetMouseUpDel();
    }
  }
}
