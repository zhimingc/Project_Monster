using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BinScript : MonoBehaviour {

  private PlayerScript playerScript;
  //private IngredientManager ingredientMan;

	// Use this for initialization
	void Start () {
    playerScript = GameObject.Find("player").GetComponent<PlayerScript>();
    //ingredientMan = GameObject.Find("ingredient_manager").GetComponent<IngredientManager>();
    GetComponent<SpriteRenderer>().color = Color.white;
  }

  // Update is called once per frame
  void Update () {
		
	}

  void OnMouseOver()
  {
    if (playerScript.playerState == PLAYER_STATE.DRAGGING)
    {
      GetComponent<SpriteRenderer>().color = Color.red;
      playerScript.blockBeingDragged.ToggleIngredients(false);
      playerScript.SetDeleteIngredient(true);
    }

  }

  void OnMouseExit()
  {
    GetComponent<SpriteRenderer>().color = Color.white;

    if (playerScript.playerState == PLAYER_STATE.DRAGGING)
    {
      playerScript.blockBeingDragged.ToggleIngredients(true);
      playerScript.SetDeleteIngredient(false);
    }
  }
}
