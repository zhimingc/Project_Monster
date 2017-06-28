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

  // feedback
  private ParticleSystem psObj;

  void Awake()
  {
    playerScript = GameObject.Find("player").GetComponent<PlayerScript>();
    gridMan = GameObject.Find("grid_manager").GetComponent<GridManager>();
    tmpHold = INGREDIENT_TYPE.EMPTY;
    sauceType = SAUCE_TYPE.EMPTY;
    maxIngredients = 5;

    psObj = Instantiate(Resources.Load<GameObject>("Prefabs/Particles/eaten_particles")).GetComponent<ParticleSystem>();
  }

	// Use this for initialization
	void Start () {
    // Generate ingredient stack game objs
    GenerateIngredientMold();

    UpdateStackDisplay();

    // reposition particle system obj
    psObj.transform.position = transform.position;
  }

  void GenerateIngredientMold()
  {
    stackObjs = new List<GameObject>();

    for (int i = 0; i < maxIngredients; ++i)
    {
      GameObject ingredient = Instantiate(ingredientSide);
      Vector3 localScale = transform.localScale;
      localScale = Vector3.Scale(localScale, new Vector3(0.4f, 0.4f, 1.0f));
      ingredient.transform.localScale = localScale;

      //ingredient.transform.position = transform.position + new Vector3(0, -transform.localScale.y / 3.0f + i * localScale.y * 2.0f, 0);
      ingredient.transform.position = transform.position + new Vector3(0, -transform.localScale.y / 3.25f + i * localScale.y / 2.0f, 0);

      ingredient.transform.SetParent(transform);
      stackObjs.Add(ingredient);
    }

    // Deactivate the top ingredient
    stackObjs[stackObjs.Count - 1].GetComponent<SpriteRenderer>().enabled = false;
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
      Sprite sprite = Resources.Load<Sprite>("Sprites/ingredient_side");
      stackObjs[i].GetComponent<SpriteRenderer>().sprite = sprite;

      // Change color depending on ingredient type
      if (i < ingredientStack.Count)
      {
        IngredientFactory.InitializeIngredientSide(stackObjs[i], ingredientStack[i]);

        if (i != 0 && ingredientStack[i] == INGREDIENT_TYPE.BREAD)
        {
          sprite = Resources.Load<Sprite>("Sprites/bread_dark_side_top");
          stackObjs[i].GetComponent<SpriteRenderer>().sprite = sprite;
        }
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
    //Vector2[] layout = block.layout;

    return gridMan.CheckIfLegalMove(this, block);
  }

  public void ClearStack()
  {
    ingredientStack.Clear();
    UpdateStackDisplay();

    // emit particles when eaten
    psObj.Play();
  }

  // Reset any grid data if player mouse up
  public void ResetHoveredGrid()
  {
    tmpHold = INGREDIENT_TYPE.EMPTY;
    tmpSauce = SAUCE_TYPE.EMPTY;
  }
}
