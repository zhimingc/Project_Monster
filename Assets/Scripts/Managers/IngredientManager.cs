using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class IngredientDistribution
{
  public int[] layoutDistribution = new int[] { 50, 75, 100 };
}

public class IngredientManager : MonoBehaviour {

  public bool isSpawnHorizontal, isSpawnLeft;
  public GameObject block, center; 
  public List<GameObject> ingredientList;
  public int numberOfIngredients;
  public IngredientDistribution distribution;
  public float idleScaling = 0.5f; // scale of block when in queue

  //private float spacing; 
  private Vector2 gridBlockSize;

  // ingredient queue properties
  private int usableAmt = 4;
  private int viewableAmt = 5;

  // For scripted ingredient generation
  private int ingredientTracker;
  private int ingredientFlipflop;
  private int ingredientCountdown; // Spawns bread until countdown is up
  private int maxCountdown;
  private int maxLayout;

  private bool startSeqFlag;
  private int tutorialFlag;
  private int tutorialIngCounter;

  // Use this for initialization 
  void Awake () { 
    //spacing = GameObject.Find("grid_manager").GetComponent<GridManager>().spacing;
    gridBlockSize = GameObject.Find("grid_manager").GetComponent<GridManager>().gridBlockSize;
    startSeqFlag = true;
    tutorialFlag = -1;
    tutorialIngCounter = 0;

    // Init for sequential ingredient gen.
    ingredientTracker = 0;
    ingredientFlipflop = 1;
    maxCountdown = 1;
    ingredientCountdown = maxCountdown;
    numberOfIngredients = GameManager.Instance.gameData.num_ingredients;
    ++numberOfIngredients;  // Because random range excludes max value
    //sauceFlipflop = 1;

    maxLayout = ObjectFactory.maxLayout;
  }

  void Start()
  {
    // init depending on starting sequence
    GameStateSettings();

    LeanTween.delayedCall(GameProgression.startSeqDelay, () =>
    {
      // Start with 3 blocks
      while (viewableAmt-- > 0)
      {
        AddToIngredientQ();
      }

      startSeqFlag = false;
    });

    GameManager.Instance.turnCounter = 0;
  }

  void GameStateSettings()
  {
    switch (GameManager.Instance.gameState)
    {
      case GAME_STATE.TUTORIAL:
        usableAmt = 1;
        viewableAmt = 2;
        tutorialFlag = 0;
        break;
      case GAME_STATE.START_SEQUENCE:

        break;
    }
  }

  // Update is called once per frame
  void Update()
  {
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
    if (tutorialFlag >= 0)
    {
      ++tutorialIngCounter;
      if (tutorialFlag == 0 && tutorialIngCounter == 5)
      {
        GameManager.Instance.TutorialTrigger(0);
        return;
      }
      else if (tutorialIngCounter == 8)
      {
        return;
      }
    }

    //GameObject newIngredient = RandomizeIngredient();
    GameObject newIngredient = SequentialIngredient();
    AddIngredientToList(newIngredient);
  }

  // Generates ingredients by scrolling through ingredient types
  GameObject SequentialIngredient()
  {
    // Get queued type 
    GameManager.Instance.IncrementTurnCounter();

    SAUCE_TYPE sauceTracker = SAUCE_TYPE.EMPTY;
    if (ingredientCountdown-- <= 0)
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

    // only gen single blocks for tutorial
    if (tutorialFlag >= 0)
    {
      if (tutorialIngCounter < 4) layout = 0;
      else
      {
        if (tutorialIngCounter < 6)
        {
          type = INGREDIENT_TYPE.MEAT;
          layout = 1;
        }
        else
        {
          type = INGREDIENT_TYPE.BREAD;
          layout = 2;
        }
      }
    }

    // Generate ingredient 
    GameObject genIngredient = GenerateIngredient(type, sauce, layout, transform);
    BlockBehaviour genScript = genIngredient.GetComponent<BlockBehaviour>();
    foreach(GameObject obj in genScript.childrenObjs)
    {
      obj.GetComponent<Animator>().SetBool("idle_anim", true);
    }

    return genIngredient;
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
    float hardSetSpacing = 0.1f;

    for (int i = ingredientList.Count - 1, c = 0; i >= 0; --i, ++c)
    {
      Vector3 pos = ingredientList[i].transform.position;
      int swing = isSpawnLeft ? -1 : 1;

      if (isSpawnHorizontal)
      {
        pos.x = transform.position.x +
          swing * c * (-maxLayout - hardSetSpacing) * maxLayout * idleScaling;// - (gridBlockSize.x * idleScaling) / 2.0f);
      }
      else
      {
        pos.y = transform.position.y + 
          swing * (c * (-maxLayout - hardSetSpacing) * maxLayout * idleScaling - (gridBlockSize.y * idleScaling) / 2.0f);
      }

      if (startSeqFlag)
      {
        ingredientList[i].GetComponent<IngredientBlock>().SlideIngredient(pos.x, 0.1f * (c+1), 0.25f * i);
      }
      else
      {
        ingredientList[i].GetComponent<IngredientBlock>().SlideIngredient(pos.x, 0.15f);
      }

      //ingredientList[i].transform.position = pos;
      ingredientList[i].GetComponent<IngredientBlock>().oldPos = pos;

    }
  }

  public int GenerateLayout()
  {
    // Get random layout 
    int layoutRoll = Random.Range(0, 101);
    int layout = 0;
    for (layout = 0; layout < ObjectFactory.blockLayouts.Length; ++layout)
    {
      if (layoutRoll <= distribution.layoutDistribution[layout])
        break;
    }

    return layout;
  }

  void PositionIngredientToCenter(GameObject obj, int layout)
  {
    Vector3 objPos = obj.transform.localPosition;

    switch (layout)
    {
      case 0: // single
        obj.transform.localPosition = new Vector3(0, 0, 0);
        obj.transform.localScale = new Vector3(0.75f, 0.75f, 1.0f);
        break;
      case 1: // horizontal
        objPos.x -= 0.25f;
        objPos.y = 0.0f;
        obj.transform.localPosition = objPos;
        break;
      case 2: // vertical
        objPos.x = 0.0f;
        obj.transform.localPosition = objPos;
        break;
    }
  }

  public GameObject GenerateIngredient(INGREDIENT_TYPE type, SAUCE_TYPE sauce, int layout, Transform t) 
  { 
    GameObject parent = Instantiate(block); 
    IngredientBlock blockScript = parent.GetComponent<IngredientBlock>();
    blockScript.SetIdleScale(idleScaling);
    blockScript.layout = ObjectFactory.blockLayouts[layout];

    Vector2 ingredientSize = gridBlockSize;
    //Vector2 newScale = (ingredientSize + new Vector2(spacing, spacing)) * maxLayout; 
    Vector2 newScale = (ingredientSize) * maxLayout;
    parent.transform.localScale = newScale; 
    parent.transform.position = t.position + new Vector3(0.0f, -newScale.y, 0.0f) / (maxLayout * 2.0f);

    // Create core of ingredient 
    GameObject ingredientHolder = new GameObject();
    ingredientHolder.transform.position = t.position;
    ingredientHolder.transform.localScale = ingredientSize;
    ingredientHolder.transform.SetParent(parent.transform);

    GameObject ingredientObj = Instantiate(center, t.position - new Vector3(0, 0, 1.5f), Quaternion.identity);
    ingredientObj.transform.SetParent(ingredientHolder.transform);
    ingredientObj.GetComponent<IngredientScript>().InitializeIngredientScript(type, sauce, new Vector2(1.0f, 1.0f));
    blockScript.AddIngredient(ingredientObj);

    // Center ingredient
    PositionIngredientToCenter(ingredientHolder, layout);

    // Create ingredient layout 
    foreach (Vector2 vec in ObjectFactory.blockLayouts[layout]) 
    {
      GameObject ingredientHolder2 = new GameObject();
      ingredientHolder2.transform.position = t.position;
      ingredientHolder2.transform.localScale = ingredientSize;
      ingredientHolder2.transform.SetParent(parent.transform);

      // Create ingredients within connection 
      GameObject newIngredient = Instantiate(center, t.position - new Vector3(0, 0, 1.5f), Quaternion.identity);

      // Initialize new ingredient 
      //Vector3 offset = Vector2.Scale(vec, ingredientSize + new Vector2(spacing, spacing));
      Vector3 offset = Vector2.Scale(vec, ingredientSize);
      newIngredient.transform.position += offset;
 
      newIngredient.transform.SetParent(ingredientHolder2.transform);
      newIngredient.GetComponent<IngredientScript>().InitializeIngredientScript(type, sauce, new Vector2(1.0f, 1.0f));

      PositionIngredientToCenter(ingredientHolder2, layout);

      blockScript.AddIngredient(newIngredient);
    }

    return parent; 
  }

  public void IngredientLayoutTutorial(int step)
  {
    tutorialFlag = step;
  }
}
