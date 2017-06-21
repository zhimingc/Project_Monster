using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridScript : MonoBehaviour {

  public GameObject ingredientSide;
  public List<INGREDIENT_TYPE> ingredientStack;
  public SAUCE_TYPE sauceType;  // lunch mechanic
  public GRID_TYPE gridType;    // dinner mechanic
  public int[] coordinates;

  private List<GameObject> stackObjs;
  private PlayerScript playerScript;
  private GridManager gridMan;
  private INGREDIENT_TYPE tmpHold; // Holds ingredient for eater
  private SAUCE_TYPE tmpSauce;     // Holds sauce type when hovering new sauce
  private int maxIngredients;      // maximum ingredients the grid can hold

  void Awake()
  {
    playerScript = GameObject.Find("player").GetComponent<PlayerScript>();
    gridMan = GameObject.Find("grid_manager").GetComponent<GridManager>();
    tmpHold = INGREDIENT_TYPE.EMPTY;
    sauceType = SAUCE_TYPE.EMPTY;
    maxIngredients = 5;
  }

	// Use this for initialization
	void Start () {
    // Generate ingredient stack game objs
    GenerateIngredientMold();

    UpdateStackDisplay();
  }
	
	void GenerateIngredientMold()
  {
    stackObjs = new List<GameObject>();

    for (int i = 0; i < maxIngredients; ++i)
    {
      GameObject ingredient = Instantiate(ingredientSide);
      Vector3 localScale = transform.localScale;
      localScale = Vector3.Scale(localScale, new Vector3(0.6f, 0.08f, 1.0f));
      ingredient.transform.localScale = localScale;

      ingredient.transform.position = transform.position + new Vector3(0, -transform.localScale.y / 3.0f + i * localScale.y * 2.0f, 0);
      
      stackObjs.Add(ingredient);
    }
  }

  void OnMouseEnter()
  {
    // Update the grid if an ingredient block is being dragged
    if (playerScript.blockBeingDragged != null)
    {
      if (!CheckIfLegalMove()) return;

      // Stop grid from moving
      playerScript.blockBeingDragged.GetComponent<IngredientBlock>().beingDragged = false;
      playerScript.blockBeingDragged.ToggleIngredients(false);

      // Update grid with move
      playerScript.blockBeingDragged.SetBlockPosition(transform.position);
      playerScript.SetHoveredGrid(this);
      gridMan.AddIngredientBlockToGrid(this, playerScript.blockBeingDragged);
    }
  }

  public void AddIngredientToStack(IngredientScript ingredient)
  {
    // Change behaviour depending on ingredient type
    switch(ingredient.type)
    {
      // Adds sauce to the grid
      case INGREDIENT_TYPE.SAUCE:
        tmpSauce = sauceType;
        sauceType = ingredient.sauceType;
        break;
      // Eats the top ingredient
      case INGREDIENT_TYPE.EATER:
        if (ingredientStack.Count == 0) break;
        tmpHold = ingredientStack[ingredientStack.Count - 1];
        ingredientStack.RemoveAt(ingredientStack.Count - 1);
        break;
      default:
        ingredientStack.Add(ingredient.type);
        break;
    }

    // Visual feedback for grid
    UpdateStackDisplay();
  }

  public void RemoveIngredientFromStack(IngredientScript ingredient)
  {
    // Change behaviour depending on ingredient type
    switch (ingredient.type)
    {
      case INGREDIENT_TYPE.SAUCE:
        sauceType = tmpSauce;
        break;
      // Eats the top ingredient
      case INGREDIENT_TYPE.EATER:
        // Only adds back ingredient if something was removed
        if (tmpHold != INGREDIENT_TYPE.EMPTY)
        {
          ingredientStack.Add(tmpHold);
          tmpHold = INGREDIENT_TYPE.EMPTY;
        }
        break;
      default:
        ingredientStack.RemoveAt(ingredientStack.Count - 1);
        break;
    }

    UpdateStackDisplay();
  }

  public void ToggleGridType(GRID_TYPE type)
  {
    gridType = type;
    UpdateStackDisplay();
  }

  void UpdateStackDisplay()
  {
    for (int i = 0; i < stackObjs.Count; ++i)
    {
      stackObjs[i].GetComponent<SpriteRenderer>().color = Color.grey;

      // Change color depending on ingredient type
      if (i < ingredientStack.Count)
      {
        IngredientFactory.InitializeIngredient(stackObjs[i], ingredientStack[i]);
      }
    }

    // Update sauce feedback for grid
    IngredientFactory.InitializeSauce(gameObject, sauceType);

    // Update grid type feedback
    IngredientFactory.InitializeGrid(gameObject, gridType);
  }

  void OnMouseExit()
  {
    // Update the grid if an ingredient block is being dragged
    if (playerScript.blockBeingDragged != null && playerScript.hoveredGrid == this)
    {
      playerScript.SetHoveredGrid(null);

      // Stop grid from moving
      playerScript.blockBeingDragged.GetComponent<IngredientBlock>().beingDragged = true;
      playerScript.blockBeingDragged.ToggleIngredients(true);
      gridMan.RemoveIngredientBlockFromGrid(this, playerScript.blockBeingDragged);
    }
  }

  public bool IsGridFull()
  {
    return ingredientStack.Count >= maxIngredients;
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
    UpdateStackDisplay();
  }

  // Reset any grid data if player mouse up
  public void ResetHoveredGrid()
  {
    tmpHold = INGREDIENT_TYPE.EMPTY;
    tmpSauce = SAUCE_TYPE.EMPTY;
  }
}
