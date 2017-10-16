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
  public GameObject scoreText;
  public GameObject pickyIndicator;
  public GameObject comboSignObj;
  public bool canServe;         // flag to indicate if this grid meets any requests
  public MonsterRequest monReq;   // request this grid meets
  public bool isBeingServed; // toggle for when the grid is about to be served

  public List<GameObject> stackObjs;
  private PlayerScript playerScript;
  private GridManager gridMan;
  private INGREDIENT_TYPE tmpHold; // Holds ingredient for eater
  private SAUCE_TYPE tmpSauce;     // Holds sauce type when hovering new sauce
  private int maxIngredients;      // maximum ingredients the grid can hold
  private bool isPickedUp;

  // feedback
  public ParticleSystem psObj;
  public GameObject exclaimObj;
  //private Vector3 originalScale;

  // turntable variables
  private float turnSpeed;
  private Vector3[] rotateFromPos;
  private List<INGREDIENT_TYPE> tmpIngredientStack;
  private List<GameObject> tmpStackObjs;

  // picky monster 
  public Request pickedBy;

  // follow player
  private List<Vector3> ingredientSidePos;
  private Vector3 origin;
  private Vector3 originScale;

  void Awake()
  {
    playerScript = GameObject.Find("player").GetComponent<PlayerScript>();
    gridMan = GameObject.Find("grid_manager").GetComponent<GridManager>();
    tmpHold = INGREDIENT_TYPE.EMPTY;
    sauceType = SAUCE_TYPE.EMPTY;
    maxIngredients = 5;
    scoreText.SetActive(false);
    comboSignObj.SetActive(false);
    origin = transform.position;
    originScale = transform.localScale;

    // turntable variables
    turnSpeed = 0.25f;
    isPickedUp = false;
    pickedBy = null;

    // Init can serve variables
    SetCanServe(false);
    pickyIndicator.SetActive(false);
  }

	// Use this for initialization
	void Start () {
    // Generate ingredient stack game objs
    //GenerateIngredientMold();

    UpdateStackDisplay();

    // reposition particle system obj
    psObj.transform.position = transform.position;
  }

  void Update()
  {
    if (isPickedUp && !isBeingServed)
    {
      UpdatePickedUp();
    }
  }

  void UpdatePickedUp()
  {
    Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    mousePos.z = -5.0f;

    for (int i = 0; i < stackObjs.Count; ++i)
    {
      //stackObjs[i].transform.position = mousePos + ingredientSidePos[i];
    }

    transform.localScale = originScale;
    transform.position = mousePos;
  }

  void GenerateIngredientMold()
  {
  //  stackObjs = new List<GameObject>();
  //  ingredientSidePos = new List<Vector3>();
  //  rotateFromPos = new Vector3[maxIngredients];

  //  for (int i = 0; i < maxIngredients; ++i)
  //  {
  //    GameObject ingredient = Instantiate(ingredientSide);
  //    Vector3 localScale = transform.localScale;
  //    localScale = Vector3.Scale(localScale, new Vector3(0.4f, 0.5f, 1.0f));
  //    ingredient.transform.localScale = localScale;

  //    ingredient.transform.position = transform.position + new Vector3(0, -transform.localScale.y / 3.25f + i * localScale.y / 2.5f, -1.0f);
  //    ingredient.transform.SetParent(transform);
  //    //ingredient.transform.localPosition = new Vector3(0, -transform.localScale.y / 10.0f + i * localScale.y / 5.0f, 0);
  //    //ingredient.transform.localPosition = new Vector3(0, i * localScale.y / 5.0f, 0);

  //    stackObjs.Add(ingredient);
  //    rotateFromPos[i] = ingredient.transform.position;
  //    ingredientSidePos.Add(ingredient.transform.position - transform.position);
  //  }

  //  // Deactivate the top ingredient
  //  stackObjs[stackObjs.Count - 1].GetComponent<SpriteRenderer>().enabled = false;
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
    if (GameManager.Instance.IsPaused()) return;

    // Update the grid if an ingredient block is being dragged
    if (playerScript.blockBeingDragged != null)
    {
      if (!CheckIfLegalMove()) return;

      if (playerScript.IsTypeOfBlock<IngredientBlock>())
      {
        IngredientBlock ingredientBlock = (IngredientBlock)playerScript.blockBeingDragged;

        // Stop grid from moving
        ingredientBlock.beingDragged = false;
      }

      // Toggle object visuals
      playerScript.blockBeingDragged.ToggleObjects(false);

      // Update grid with move
      playerScript.SetHoveredGrid(this);
      gridMan.AddBlockToGrid(this, playerScript.blockBeingDragged);

      // Update mouse up event
      playerScript.SetMouseUpDel(gameObject, GridMouseUp);

      // Update ability to serve
      //GameManager.Instance.monsterMan.CheckRequestMetAll();
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

      // Visual feedback for grid
      UpdateStackDisplay();
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
            ingredientStack[ingredientStack.Count - 1] = INGREDIENT_TYPE.EATER;
          }
          // Visual feedback for grid
          UpdateStackDisplay();
          break;
        case ITEM_TYPE.TURNTABLE:
          gridMan.ApplyTurntable(true);
          break;
      }
    }
  }

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

      // Visual feedback for grid
      UpdateStackDisplay();
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

          // Visual feedback for grid
          UpdateStackDisplay();
          break;
        case ITEM_TYPE.TURNTABLE:
          gridMan.ApplyTurntable(false);
          break;
      }
    }
  }

  public void ToggleGridType(GRID_TYPE type)
  {
    gridType = type;
    UpdateStackDisplay();

    // update any picky monsters
    if (pickedBy != null)
    {
      pickedBy.gridType = gridType;
    }

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
    ObjectFactory.InitializeSaucePlate(gameObject, sauceType);
    //(gameObject, sauceType);

    // Update grid type feedback
    ObjectFactory.InitializeGrid(gameObject, gridType);

    // Update ability to serve
    GameManager.Instance.monsterMan.CheckRequestMetAll();
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
    if (GameManager.Instance.IsPaused()) return;

    // Update the grid if an ingredient block is being dragged
    if (playerScript.blockBeingDragged != null && playerScript.hoveredGrid == this)
    {
      playerScript.SetHoveredGrid(null);

      if (playerScript.IsTypeOfBlock<IngredientBlock>())
      {
        IngredientBlock ingredientBlock = (IngredientBlock)playerScript.blockBeingDragged;
        // Stop grid from moving
        ingredientBlock.beingDragged = true;
      }

      // Toggle object visuals
      playerScript.blockBeingDragged.ToggleObjects(true);

      // Update mouse up event
      playerScript.ResetMouseUpDel(gameObject);
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
      //if (canServe)
      {
        playerScript.SetPlayerState(PLAYER_STATE.DRAGGING);
        playerScript.SetMouseUpDel(gameObject, PlateDragMouseUp);
        playerScript.SetHoveredGrid(this);
        TogglePickUp(true);

        //GameManager.Instance.monsterMan.ServeMonsterRequest(this, monReq);
      }
    }
  }

  void TogglePickUp(bool flag)
  {
    isPickedUp = flag;
    GetComponent<BoxCollider2D>().enabled = !flag;
  }

  void PlateDragMouseUp()
  {
    TogglePickUp(false);

    // moving back to origin
    LeanTween.move(gameObject, origin, 0.25f);
  }

  public bool IsGridFull()
  {
    return ingredientStack.Count >= maxIngredients;
  }

  public bool CheckIfLegalMove()
  {
    return gridMan.CheckIfLegalMove(this, playerScript.blockBeingDragged);
  }

  public void TriggerServed(MonsterRequest req)
  {
    ingredientStack.Clear();
    UpdateStackDisplay();
    if (req.request.monsterType == MONSTER_TYPE.PICKY)
    {
      pickyIndicator.SetActive(false);
      pickedBy = null;
    }

    //EmitEatenParticles();
  }

  // Reset any grid data if player mouse up
  public void ResetHoveredGrid()
  {
    tmpHold = INGREDIENT_TYPE.EMPTY;
    tmpSauce = SAUCE_TYPE.EMPTY;
    RemoveEaterIngredient();
    UpdateStackDisplay();
  }

  public void SetCanServe(bool flag, MonsterRequest setReq = null, Color chairColor = new Color())
  {
    canServe = flag;
    isBeingServed = false;

    if (setReq != null &&
      setReq.request.monsterType == MONSTER_TYPE.GARBAGE) return;

    monsterServeObj.SetActive(flag);
    if (setReq)
    {
      monsterServeObj.GetComponent<SpriteRenderer>().sprite = setReq.monsterObj.GetComponent<SpriteRenderer>().sprite;
      Color newCol = setReq.monsterObj.GetComponent<SpriteRenderer>().color;
      newCol.a = monsterServeObj.GetComponent<SpriteRenderer>().color.a;
      monsterServeObj.GetComponent<SpriteRenderer>().color = newCol;

      // chair being served
      newCol = chairColor;
      newCol.a = monsterServeObj.GetComponentsInChildren<SpriteRenderer>()[1].color.a;
      monsterServeObj.GetComponentsInChildren<SpriteRenderer>()[1].color = newCol;
    }
    monReq = setReq;

    transform.localScale = originScale;
  }

  public void MoveStackTo(GameObject toObj)
  {
    LeanTween.cancel(gameObject);

    SetCanServe(false);
    GridScript toScript = toObj.GetComponent<GridScript>();

    for (int i = 0; i < stackObjs.Count; ++i)
    {
      LeanTween.cancel(stackObjs[i]);

      LeanTween.move(stackObjs[i], toScript.rotateFromPos[i], turnSpeed).setEase(LeanTweenType.easeOutQuad);
      toScript.tmpIngredientStack = ingredientStack;
      toScript.tmpStackObjs = stackObjs;
    }
    LeanTween.delayedCall(gameObject, turnSpeed, () =>
    {
      Utility.Swap(ref stackObjs, ref tmpStackObjs);
      Utility.Swap(ref ingredientStack, ref tmpIngredientStack);

      UpdateStackDisplay();
    });
  }

  public void MoveStackBack()
  {
    // if the delayed call is not done then transfer was not complete
    if (!LeanTween.isTweening(gameObject))
    {
      // else undo the transfer
      Utility.Swap(ref stackObjs, ref tmpStackObjs);
      Utility.Swap(ref ingredientStack, ref tmpIngredientStack);
      UpdateStackDisplay();
    }

    LeanTween.cancel(gameObject);

    for (int i = 0; i < stackObjs.Count; ++i)
    {
      LeanTween.cancel(stackObjs[i]);
      LeanTween.move(stackObjs[i], rotateFromPos[i], turnSpeed).setEase(LeanTweenType.easeOutQuad);
    }
  }

  // returns true if application of effect was successful
  public bool MonsterEffect(Request monReq)
  {
    switch (monReq.monsterType)
    {
      case MONSTER_TYPE.PICKY:
        if (pickedBy != null) return false;

        pickedBy = monReq;
        pickyIndicator.SetActive(true);
        monReq.typeParams.specificGrid = this;
        monReq.gridType = gridType;
        pickyIndicator.GetComponent<SpriteRenderer>().color = monReq.chairColor;
        break;
    }

    return true;
  }

  public void SetGridIngredients(List<INGREDIENT_TYPE> ing)
  {
    ingredientStack = new List<INGREDIENT_TYPE>(ing);
    UpdateStackDisplay();
  }
}
