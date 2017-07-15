using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridScript : MonoBehaviour {

  public GameObject ingredientSide;
  public List<INGREDIENT_TYPE> ingredientStack;
  public SAUCE_TYPE sauceType;  // lunch mechanic
  public GRID_TYPE gridType;    // dinner mechanic
  public int[] coordinates;
  public GameObject monsterServeObj;  // graphics to show that you can serve
  public bool canServe;         // flag to indicate if this grid meets any requests

  private List<GameObject> stackObjs;
  private PlayerScript playerScript;
  private GridManager gridMan;
  private INGREDIENT_TYPE tmpHold; // Holds ingredient for eater
  private SAUCE_TYPE tmpSauce;     // Holds sauce type when hovering new sauce
  private int maxIngredients;      // maximum ingredients the grid can hold
  private MonsterRequest monReq;   // request this grid meets
  
  // feedback
  private ParticleSystem psObj;
  private GameObject exclaimObj;

  void Awake()
  {
    playerScript = GameObject.Find("player").GetComponent<PlayerScript>();
    gridMan = GameObject.Find("grid_manager").GetComponent<GridManager>();
    tmpHold = INGREDIENT_TYPE.EMPTY;
    sauceType = SAUCE_TYPE.EMPTY;
    maxIngredients = 5;

    psObj = Instantiate(Resources.Load<GameObject>("Prefabs/Particles/eaten_particles")).GetComponent<ParticleSystem>();
    exclaimObj = Instantiate(Resources.Load<GameObject>("Prefabs/Util/exclaimation"), transform);
    monsterServeObj = Instantiate(Resources.Load<GameObject>("Prefabs/Util/monster_serve"), transform);
    
    // Init can serve variables
    SetCanServe(false);
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
      localScale = Vector3.Scale(localScale, new Vector3(0.4f, 0.5f, 1.0f));
      ingredient.transform.localScale = localScale;

      //ingredient.transform.position = transform.position + new Vector3(0, -transform.localScale.y / 3.0f + i * localScale.y * 2.0f, 0);
      ingredient.transform.position = transform.position + new Vector3(0, -transform.localScale.y / 3.25f + i * localScale.y / 2.5f, 0);

      ingredient.transform.SetParent(transform);
      stackObjs.Add(ingredient);
    }

    // Deactivate the top ingredient
    stackObjs[stackObjs.Count - 1].GetComponent<SpriteRenderer>().enabled = false;
  }

  void GridMouseUp()
  {
    // Audio feedback
    string name = null;

    if (playerScript.IsTypeOfBlock<IngredientBlock>())
    {
      IngredientBlock ingredientBlock = (IngredientBlock)playerScript.blockBeingDragged;
      if (ingredientBlock.IsSauceBlock())
      {
        name = "splat";
      }
      else
      {
        int thud = Random.Range(0, 3);
        switch (thud)
        {
          case 0:
            name = "thud1";
            break;
          case 1:
            name = "thud2";
            break;
          case 2:
            name = "thud3";
            break;
        }
      }
    }

    if (name != null) GameManager.Instance.SFX().PlaySoundWithPitch(name, 0.7f, 0.9f);

  }

  void OnMouseEnter()
  {
    OnTouchDown();
  }

  public void OnTouchDown()
  {
    // Update the grid if an ingredient block is being dragged
    if (playerScript.blockBeingDragged != null)
    {
      if (!CheckIfLegalMove()) return;

      if (playerScript.IsTypeOfBlock<IngredientBlock>())
      {
        IngredientBlock ingredientBlock = (IngredientBlock)playerScript.blockBeingDragged;

        // Stop grid from moving
        ingredientBlock.beingDragged = false;
        //ingredientBlock.ToggleIngredients(false);
      }

      // Toggle object visuals
      playerScript.blockBeingDragged.ToggleObjects(false);

      // Update grid with move
      playerScript.SetHoveredGrid(this);
      gridMan.AddBlockToGrid(this, playerScript.blockBeingDragged);

      // Update mouse up event
      playerScript.SetMouseUpDel(GridMouseUp);

      // Update ability to serve
      GameManager.Instance.monsterMan.CheckRequestMetAll();
    }
  }

  public void AddToStack(int index)
  {
    if (playerScript.IsTypeOfBlock<IngredientBlock>())
    {
      IngredientScript ingredient = playerScript.blockBeingDragged.GetComponent<IngredientBlock>().GetIngredientScript(index);
      // Change behaviour depending on ingredient type
      switch (ingredient.type)
      {
        // Adds sauce to the grid
        case INGREDIENT_TYPE.SAUCE:
          tmpSauce = sauceType;
          sauceType = ingredient.sauceType;
          break;
        default:
          ingredientStack.Add(ingredient.type);
          break;
      }
    }

    if (playerScript.IsTypeOfBlock<ItemScript>())
    {
      ItemScript item = playerScript.blockBeingDragged.GetComponent<ItemScript>();
      // Change behaviour depending on ingredient type
      switch (item.itemType)
      {
        // Eats the top ingredient
        case ITEM_TYPE.EATER:
          if (ingredientStack.Count == 0)
          {
            ingredientStack.Add(INGREDIENT_TYPE.EATER);
          }
          else
          {
            tmpHold = ingredientStack[ingredientStack.Count - 1];
            //ingredientStack.RemoveAt(ingredientStack.Count - 1);
            ingredientStack[ingredientStack.Count - 1] = INGREDIENT_TYPE.EATER;
          }
          break;
      }
    }

    // Visual feedback for grid
    UpdateStackDisplay();
  }

  //public void AddToStack(IngredientScript ingredient)
  //{
  //  // Change behaviour depending on ingredient type
  //  switch(ingredient.type)
  //  {
  //    // Adds sauce to the grid
  //    case INGREDIENT_TYPE.SAUCE:
  //      tmpSauce = sauceType;
  //      sauceType = ingredient.sauceType;
  //      break;
  //    // Eats the top ingredient
  //    case INGREDIENT_TYPE.EATER:
  //      if (ingredientStack.Count == 0)
  //      {
  //        ingredientStack.Add(INGREDIENT_TYPE.EATER);
  //      }
  //      else
  //      {
  //        tmpHold = ingredientStack[ingredientStack.Count - 1];
  //        //ingredientStack.RemoveAt(ingredientStack.Count - 1);
  //        ingredientStack[ingredientStack.Count - 1] = INGREDIENT_TYPE.EATER;
  //      }
  //      break;
  //    default:
  //      ingredientStack.Add(ingredient.type);
  //      break;
  //  }

  //  // Visual feedback for grid
  //  UpdateStackDisplay();
  //}

  //public void RemoveFromStack(IngredientScript ingredient)
  //{
  //  // Change behaviour depending on ingredient type
  //  switch (ingredient.type)
  //  {
  //    case INGREDIENT_TYPE.SAUCE:
  //      sauceType = tmpSauce;
  //      break;
  //    // Eats the top ingredient
  //    case INGREDIENT_TYPE.EATER:
  //      // Only adds back ingredient if something was removed
  //      if (tmpHold != INGREDIENT_TYPE.EMPTY)
  //      {
  //        ingredientStack[ingredientStack.Count - 1] = tmpHold;
  //        //ingredientStack.Add(tmpHold);
  //        tmpHold = INGREDIENT_TYPE.EMPTY;
  //      }
  //      else
  //      {
  //        ingredientStack.Remove(INGREDIENT_TYPE.EATER);
  //      }
  //      break;
  //    default:
  //      ingredientStack.RemoveAt(ingredientStack.Count - 1);
  //      break;
  //  }

  //  UpdateStackDisplay();
  //}

  public void RemoveFromStack(int index)
  {
    if (playerScript.IsTypeOfBlock<IngredientBlock>())
    {
      IngredientScript ingredient = playerScript.blockBeingDragged.GetComponent<IngredientBlock>().GetIngredientScript(index);
      // Change behaviour depending on ingredient type
      switch (ingredient.type)
      {
        // Adds sauce to the grid
        case INGREDIENT_TYPE.SAUCE:
          sauceType = tmpSauce;
          break;
        default:
          ingredientStack.RemoveAt(ingredientStack.Count - 1);
          break;
      }
    }

    if (playerScript.IsTypeOfBlock<ItemScript>())
    {
      ItemScript item = playerScript.blockBeingDragged.GetComponent<ItemScript>();
      // Change behaviour depending on ingredient type
      switch (item.itemType)
      {
        // Eats the top ingredient
        case ITEM_TYPE.EATER:
          // Only adds back ingredient if something was removed
          if (tmpHold != INGREDIENT_TYPE.EMPTY)
          {
            ingredientStack[ingredientStack.Count - 1] = tmpHold;
            //ingredientStack.Add(tmpHold);
            tmpHold = INGREDIENT_TYPE.EMPTY;
          }
          else
          {
            ingredientStack.Remove(INGREDIENT_TYPE.EATER);
          }
          break;
      }
    }

    // Visual feedback for grid
    UpdateStackDisplay();
  }

  public void ToggleGridType(GRID_TYPE type)
  {
    gridType = type;
    UpdateStackDisplay();

    // Update ability to serve
    GameManager.Instance.monsterMan.CheckRequestMetAll();
  }

  void UpdateStackDisplay()
  {
    // feedback that the grid is full
    exclaimObj.SetActive(ingredientStack.Count >= stackObjs.Count);

    for (int i = 0; i < stackObjs.Count; ++i)
    {
      Sprite sprite = Resources.Load<Sprite>("Sprites/ingredient_side");
      stackObjs[i].GetComponent<SpriteRenderer>().sprite = sprite;
      stackObjs[i].GetComponent<SpriteRenderer>().color = Color.white;

      // Change color depending on ingredient type
      if (i < ingredientStack.Count)
      {
        ObjectFactory.InitializeIngredientSide(stackObjs[i], ingredientStack[i]);

        if (i != 0 && ingredientStack[i] == INGREDIENT_TYPE.BREAD)
        {
          sprite = Resources.Load<Sprite>("Sprites/bread_side_top");
          stackObjs[i].GetComponent<SpriteRenderer>().sprite = sprite;
        }
      }
    }

    // Update sauce feedback for grid
    ObjectFactory.InitializeSauce(gameObject, sauceType);

    // Update grid type feedback
    ObjectFactory.InitializeGrid(gameObject, gridType);
  }

  void RemoveEaterIngredient()
  {
    ingredientStack.Remove(INGREDIENT_TYPE.EATER);
  }

  void OnMouseExit()
  {
    OnTouchExit();
  }

  public void EmitEatenParticles()
  {
    // emit particles when eaten
    psObj.Play();
  }

  public void OnTouchExit()
  {
    // Update the grid if an ingredient block is being dragged
    if (playerScript.blockBeingDragged != null && playerScript.hoveredGrid == this)
    {
      playerScript.SetHoveredGrid(null);

      if (playerScript.IsTypeOfBlock<IngredientBlock>())
      {
        IngredientBlock ingredientBlock = (IngredientBlock)playerScript.blockBeingDragged;
        // Stop grid from moving
        ingredientBlock.beingDragged = true;
        //ingredientBlock.ToggleIngredients(true);
        //ingredientBlock.ToggleObjects(true);
      }

      // Toggle object visuals
      playerScript.blockBeingDragged.ToggleObjects(true);

      // Update mouse up event
      //playerScript.ResetMouseUpDel();
      playerScript.blockBeingDragged.beingDragged = true;
      gridMan.RemoveBlockFromGrid(this, playerScript.blockBeingDragged);

      // Update ability to serve
      GameManager.Instance.monsterMan.CheckRequestMetAll();
    }
  }

  void OnMouseOver()
  {
    OnTouchStay();
  }

  public void OnTouchStay()
  {
    // Serve request if able to
    if (InputMan.OnDown())
    {
      if (canServe)
      {
        GameManager.Instance.monsterMan.ServeMonsterRequest(this, monReq);
        SetCanServe(false);
      }
    }
  }

  public bool IsGridFull()
  {
    return ingredientStack.Count >= maxIngredients;
  }

  public bool CheckIfLegalMove()
  {
    return gridMan.CheckIfLegalMove(this, playerScript.blockBeingDragged);
  }

  public void ClearStack()
  {
    ingredientStack.Clear();
    UpdateStackDisplay();

    EmitEatenParticles();
  }

  // Reset any grid data if player mouse up
  public void ResetHoveredGrid()
  {
    tmpHold = INGREDIENT_TYPE.EMPTY;
    tmpSauce = SAUCE_TYPE.EMPTY;
    RemoveEaterIngredient();
    UpdateStackDisplay();
  }

  public void SetCanServe(bool flag, MonsterRequest setReq = null)
  {
    canServe = flag;
    monsterServeObj.SetActive(flag);
    monReq = setReq;
  }
}
