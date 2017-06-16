using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridScript : MonoBehaviour {

  public GameObject ingredientSide;
  public List<INGREDIENT_TYPE> ingredientStack;
  public int[] coordinates;

  private List<GameObject> stackObjs;
  private PlayerScript playerScript;
  private GridManager gridMan;

	// Use this for initialization
	void Start () {
    playerScript = GameObject.Find("player").GetComponent<PlayerScript>();
    gridMan = GameObject.Find("grid_manager").GetComponent<GridManager>();

    // Generate ingredient stack game objs
    GenerateIngredientMold();
  }
	
	void GenerateIngredientMold()
  {
    stackObjs = new List<GameObject>();

    for (int i = 0; i < 4; ++i)
    {
      GameObject ingredient = Instantiate(ingredientSide);
      ingredient.transform.position = transform.position + new Vector3(0, -0.75f + i * 0.5f, 0);
      stackObjs.Add(ingredient);
    }
  }

  void OnMouseOver()
  {
    // Update the grid if an ingredient block is being dragged
    if (playerScript.blockBeingDragged != null)
    {
      if (!CheckIfLegalMove()) return;

      // Stop grid from moving
      playerScript.blockBeingDragged.GetComponent<IngredientBlock>().beingDragged = false;

      // Update grid with move
      playerScript.blockBeingDragged.SetBlockPosition(transform.position);
    }
  }

  void OnMouseExit()
  {
    // Update the grid if an ingredient block is being dragged
    if (playerScript.blockBeingDragged != null)
    {
      // Stop grid from moving
      playerScript.blockBeingDragged.GetComponent<IngredientBlock>().beingDragged = true;
    }
  }

  public bool CheckIfLegalMove()
  {
    IngredientBlock block = playerScript.blockBeingDragged;
    Vector2[] layout = block.layout;

    return gridMan.CheckIfLegalMove(this, layout);
  }

  public void ClearStack()
  {
    ingredientStack.Clear();
  }

  public void AddToStack(INGREDIENT_TYPE type)
  {
    ingredientStack.Add(type);
  }
}
