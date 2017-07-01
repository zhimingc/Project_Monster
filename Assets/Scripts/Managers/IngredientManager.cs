﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class IngredientDistribution
{
  public int[] layoutDistribution = new int[] { 50, 75, 100 };
}

public class IngredientManager : MonoBehaviour {

  public bool isSpawnHorizontal, isSpawnLeft;
  public GameObject block, center, connector; 
  public List<GameObject> ingredientList;
  public int numberOfIngredients;
  public IngredientDistribution distribution;
  public float idleScaling = 0.5f; // scale of block when in queue

  private float spacing; 
  private Vector2 gridBlockSize;

  // ingredient queue properties
  private int usableAmt = 4;
  private int viewableAmt = 5;

  // For scripted ingredient generation
  private int ingredientTracker;
  private int ingredientFlipflop;
  private int ingredientCountdown; // Spawns bread until countdown is up
  private int maxCountdown;
  private int ingredientCountup;   // For specific ingredients spawn, e.g. eater
  //private int sauceFlipflop;       // Switches between all sauce types

  // Ingredient block layouts 
  private int maxLayout = 2; 

  private Vector2[][] blockLayouts = new Vector2[][] 
  {
    new Vector2[] { },                      // single block
    new Vector2[] { new Vector2( 1, 0 ) },  // double horizontal 
    new Vector2[] { new Vector2( 0, -1 ) }, // double vertical 
  }; 
 

  // Use this for initialization 
  void Awake () { 
    spacing = GameObject.Find("grid_manager").GetComponent<GridManager>().spacing;
    gridBlockSize = GameObject.Find("grid_manager").GetComponent<GridManager>().gridBlockSize;

    // Init for sequential ingredient gen.
    ingredientTracker = 0;
    ingredientFlipflop = 1;
    maxCountdown = 1;
    ingredientCountdown = maxCountdown;
    ingredientCountup = 0;
    ++numberOfIngredients;  // Because random range excludes max value
    //sauceFlipflop = 1;
  }

  void Start()
  {
    // Start with 3 blocks
    while (viewableAmt-- > 0)
    {
      AddToIngredientQ();
    }

    GameManager.Instance.turnCounter = 0;
  }
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Z)) 
    { 
      GameObject newIngredient = SequentialIngredient(); 
      AddIngredientToList(newIngredient); 
    }
	}

  void UpdateQueueUsability()
  {
    for (int i = 0; i < ingredientList.Count; ++i)
    {
      bool isUsable = i < usableAmt;
      ingredientList[i].GetComponent<IngredientBlock>().ToggleUsability(isUsable);
    }
  }

  public void RemoveFromIngredientQ(GameObject block)
  {
    ingredientList.Remove(block);

    // Automatically add an ingredient to the queue after removing
    AddToIngredientQ();
  }

  public void AddToIngredientQ()
  {
    //GameObject newIngredient = RandomizeIngredient();
    GameObject newIngredient = SequentialIngredient();
    AddIngredientToList(newIngredient);
  }

  // Generates ingredients by scrolling through ingredient types
  GameObject SequentialIngredient()
  {
    // Get queued type 
    //++ingredientCountup;
    GameManager.Instance.IncrementTurnCounter();
    ingredientCountup = GameManager.Instance.turnCounter;

    SAUCE_TYPE sauceTracker = SAUCE_TYPE.EMPTY;
    //if (GameManager.Instance.dayMan.IsOrPastShift(DAY_STATE.LUNCH) &&
    //  ingredientCountup % 5 == 0)
    //{
    //  ingredientTracker = (int)INGREDIENT_TYPE.SAUCE;
    //  sauceTracker = (SAUCE_TYPE) ((++sauceFlipflop) % (int)SAUCE_TYPE.NUM_SAUCE);
    //}
    if (ingredientCountup % 7 == 0)
    {
      // spawns an eater every X turns
      ingredientTracker = (int)INGREDIENT_TYPE.EATER;
    }
    else if (ingredientCountdown-- <= 0)
    {
      ingredientCountdown = maxCountdown;
      ingredientTracker = ingredientFlipflop;
      ingredientFlipflop = ++ingredientFlipflop % numberOfIngredients;
      if (ingredientFlipflop == 0) ++ingredientFlipflop;
    }
    else
    {
      ingredientTracker = 0;
    }
    INGREDIENT_TYPE type = (INGREDIENT_TYPE)ingredientTracker;
    SAUCE_TYPE sauce = sauceTracker;

    // Get random layout 
    int layout = GenerateLayout();

    // Generate ingredient 
    return GenerateIngredient(type, sauce, layout, transform);
  }

  GameObject RandomizeIngredient() 
  { 
    // Get random type 
    INGREDIENT_TYPE type = (INGREDIENT_TYPE)Random.Range(0, numberOfIngredients);
    SAUCE_TYPE sauce = SAUCE_TYPE.EMPTY;

    // Get random layout 
    int layout = Random.Range(0, blockLayouts.Length); 
 
    // Generate ingredient 
    return GenerateIngredient(type, sauce, layout, transform); 
  }

  void AddIngredientToList(GameObject ingredient) 
  { 
    // Add ingredient to list 
    ingredientList.Add(ingredient);

    // Offset all ingredients 
    UpdateQueuePositions();
    UpdateQueueUsability();
  }

  void UpdateQueuePositions()
  {
    for (int i = ingredientList.Count - 1, c = 0; i >= 0; --i, ++c)
    {
      Vector3 pos = ingredientList[i].transform.position;
      int swing = isSpawnLeft ? -1 : 1;

      if (isSpawnHorizontal)
      {
        pos.x = transform.position.x + swing * (c * (-maxLayout - spacing) * maxLayout * idleScaling - (gridBlockSize.x * idleScaling) / 2.0f);
      }
      else
      {
        pos.y = transform.position.y + swing * (c * (-maxLayout - spacing) * maxLayout * idleScaling - (gridBlockSize.y * idleScaling) / 2.0f);
      }
      ingredientList[i].transform.position = pos;
      ingredientList[i].GetComponent<IngredientBlock>().oldPos = pos;

    }
  }

  public int GenerateLayout()
  {
    // Get random layout 
    int layoutRoll = Random.Range(0, 101);
    int layout = 0;
    for (layout = 0; layout < blockLayouts.Length; ++layout)
    {
      if (layoutRoll <= distribution.layoutDistribution[layout])
        break;
    }

    return layout;
  }

  public GameObject GenerateIngredient(INGREDIENT_TYPE type, SAUCE_TYPE sauce, int layout, Transform t) 
  { 
    GameObject parent = Instantiate(block); 
    IngredientBlock blockScript = parent.GetComponent<IngredientBlock>();
    blockScript.SetIdleScale(idleScaling);
    blockScript.layout = blockLayouts[layout];

    Vector2 ingredientSize = gridBlockSize;
    Vector2 newScale = (ingredientSize + new Vector2(spacing, spacing)) * maxLayout; 
    parent.transform.localScale = newScale; 
    parent.transform.position = t.position + new Vector3(newScale.x, -newScale.y, 0.0f) / (maxLayout * 2.0f);

    // Create core of ingredient 
    GameObject ingredientHolder = new GameObject();
    ingredientHolder.transform.position = t.position;
    ingredientHolder.transform.localScale = ingredientSize;
    ingredientHolder.transform.SetParent(parent.transform);

    GameObject ingredientObj = Instantiate(center, t.position, Quaternion.identity);
    ingredientObj.transform.SetParent(ingredientHolder.transform);
    ingredientObj.GetComponent<IngredientScript>().InitializeIngredientScript(type, sauce, new Vector2(1.0f, 1.0f));
    blockScript.AddIngredient(ingredientObj); 
 
    // Create ingredient layout 
    foreach (Vector2 vec in blockLayouts[layout]) 
    {
      GameObject ingredientHolder2 = new GameObject();
      ingredientHolder2.transform.position = t.position;
      ingredientHolder2.transform.localScale = ingredientSize;
      ingredientHolder2.transform.SetParent(parent.transform);

      // Create ingredients within connection 
      GameObject newIngredient = Instantiate(center, t.position, Quaternion.identity);

      // Initialize new ingredient 
      Vector3 offset = Vector2.Scale(vec, ingredientSize + new Vector2(spacing, spacing));
      newIngredient.transform.position += offset;
 
      newIngredient.transform.SetParent(ingredientHolder2.transform);
      newIngredient.GetComponent<IngredientScript>().InitializeIngredientScript(type, sauce, new Vector2(1.0f, 1.0f));

      blockScript.AddIngredient(newIngredient); 
    } 
 
    return parent; 
  }
}
