using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientManager : MonoBehaviour {

  public GameObject block, center, connector; 
  public List<GameObject> ingredientList;
  public int numberOfIngredients;

  private float spacing; 
  private Vector2 gridBlockSize;

  // ingredient queue properties
  private int usableAmt = 4;
  private int viewableAmt = 6;
  private float idleScaling = 0.5f; // scale of block when in queue

  // For scripted ingredient generation
  private int ingredientTracker;
  private int ingredientFlipflop;
  private int ingredientCountdown; // Spawns bread until countdown is up
  private int ingredientCountup;   // For specific ingredients spawn, e.g. eater

  // Ingredient block layouts 
  private int maxLayout = 2; 

  private Vector2[][] blockLayouts = new Vector2[][] 
  {
    new Vector2[] { },                      // single block
    new Vector2[] { new Vector2( 1, 0 ) },  // double horizontal 
    new Vector2[] { new Vector2( 0, -1 ) }, // double vertical 
  }; 
 

  // Use this for initialization 
  void Start () { 
    spacing = GameObject.Find("grid_manager").GetComponent<GridManager>().spacing;
    gridBlockSize = GameObject.Find("grid_manager").GetComponent<GridManager>().gridBlockSize;

    // Init for sequential ingredient gen.
    ingredientTracker = 0;
    ingredientFlipflop = 1;
    ingredientCountdown = 1;
    ingredientCountup = 0;
    ++numberOfIngredients;  // Because random range excludes max value

    // Start with 3 blocks
    while (viewableAmt-- > 0)
    {
      AddToIngredientQ();
    }
  }
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Z)) 
    { 
      GameObject newIngredient = RandomizeIngredient(); 
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
    Destroy(block);

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
    ++ingredientCountup;

    // spawns an eater every X turns
    if (ingredientCountup % 7 == 0)
    {
      ingredientTracker = (int)INGREDIENT_TYPE.EATER;
    }
    else if (ingredientCountdown-- <= 0)
    {
      ingredientCountdown = 1;
      ingredientTracker = ingredientFlipflop;
      ingredientFlipflop = ++ingredientFlipflop % numberOfIngredients;
      if (ingredientFlipflop == 0) ++ingredientFlipflop;
    }
    else
    {
      ingredientTracker = 0;
    }
    //ingredientTracker = ++ingredientTracker % (int)INGREDIENT_TYPE.NUM_INGREDIENTS;
    INGREDIENT_TYPE type = (INGREDIENT_TYPE)ingredientTracker;

    // Get random layout 
    int layout = Random.Range(0, blockLayouts.Length);

    // Generate ingredient 
    return GenerateIngredient(type, layout);
  }

  GameObject RandomizeIngredient() 
  { 
    // Get random type 
    INGREDIENT_TYPE type = (INGREDIENT_TYPE)Random.Range(0, numberOfIngredients); 
 
    // Get random layout 
    int layout = Random.Range(0, blockLayouts.Length); 
 
    // Generate ingredient 
    return GenerateIngredient(type, layout); 
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
      //pos.y = transform.position.y + (c * (-gridBlockSize.y - spacing) * maxLayout * idleScaling - (gridBlockSize.y * idleScaling) / 2.0f);
      pos.y = transform.position.y + (c * (-maxLayout - spacing) * maxLayout * idleScaling - (gridBlockSize.y * idleScaling) / 2.0f);
      ingredientList[i].transform.position = pos;
    }
  }

  GameObject GenerateIngredient(INGREDIENT_TYPE type, int layout) 
  { 
    GameObject parent = Instantiate(block); 
    IngredientBlock blockScript = parent.GetComponent<IngredientBlock>();
    blockScript.SetIdleScale(idleScaling);
    blockScript.layout = blockLayouts[layout];

    Vector2 ingredientSize = gridBlockSize;
    Vector2 newScale = (ingredientSize + new Vector2(spacing, spacing)) * maxLayout; 
    parent.transform.localScale = newScale; 
    parent.transform.position = transform.position + new Vector3(newScale.x, -newScale.y, 0.0f) / (maxLayout * 2.0f); 
 
    // Create core of ingredient 
    GameObject ingredientObj = Instantiate(center, transform.position, Quaternion.identity);
    ingredientObj.GetComponent<IngredientScript>().InitializeIngredientScript(type, ingredientSize);
    ingredientObj.transform.SetParent(parent.transform); 
    blockScript.AddIngredient(ingredientObj); 
 
    // Create ingredient layout 
    foreach (Vector2 vec in blockLayouts[layout]) 
    {  
      // Create ingredients within connection 
      GameObject newIngredient = Instantiate(center, transform.position, Quaternion.identity);
      newIngredient.GetComponent<IngredientScript>().InitializeIngredientScript(type, ingredientSize);
 
      // Initialize new ingredient 
      Vector3 offset = Vector2.Scale(vec, ingredientSize + new Vector2(spacing, spacing));
      newIngredient.transform.position += offset;
 
      newIngredient.transform.SetParent(parent.transform); 
      blockScript.AddIngredient(newIngredient); 
    } 
 
    return parent; 
  }
}
