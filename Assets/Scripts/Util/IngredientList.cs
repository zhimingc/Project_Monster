using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum INGREDIENT_TYPE 
{ 
  BREAD, 
  MEAT,
  LETTUCE,
  CHEESE,
  SAUCE,
  //NUM_INGREDIENTS,
  EATER,
  EMPTY,
};

public enum SAUCE_TYPE
{
  SOYSAUCE,
  MUSTARD,
  WASABI,
  NUM_SAUCE,
  EMPTY
}

public enum GRID_TYPE
{
  PLATE,
  BOWL,
  NUM_GRID
}

public enum ITEM_TYPE
{
  EATER,
  BIN,
  TURNTABLE,
  MAGIC,
  SLOT,
  EMPTY
};

public enum CONTRACT_TYPE
{
  TIMER,
  NUM_CONTRACTS
}

// to be able to pass item and contract info as a base
public class Info
{
  public string name;
};

[System.Serializable]
public class ItemInfo : Info
{
  public ItemInfo (ItemInfo copy)
  {
    type = copy.type;
    InitItemSpecfic();
  }

  public ItemInfo(ITEM_TYPE _type)
  {
    type = _type;
    InitItemSpecfic();
  }

  public void InitItemSpecfic()
  {
    switch (type)
    {
      case ITEM_TYPE.EATER:
        name = "Eater";
        itemCooldown = 5;
        break;
      case ITEM_TYPE.BIN:
        name = "Bin";
        itemCooldown = 3;
        break;
      case ITEM_TYPE.TURNTABLE:
        name = "Turntable";
        itemCooldown = 4;
        break;
    }
  }

  public ITEM_TYPE type;
  public int itemCooldown;
};


[System.Serializable]
public class ContractInfo : Info
{
  public ContractInfo()
  {
    type = CONTRACT_TYPE.NUM_CONTRACTS;
    isActive = false;
  }

  public ContractInfo(CONTRACT_TYPE _type)
  {
    type = _type;
    isActive = true;

    switch(type)
    {
      case CONTRACT_TYPE.TIMER:
        contractIcon = Resources.Load<Sprite>("Sprites/UI/stopwatch_full");
        break;
    }
  }

  public CONTRACT_TYPE type;
  public bool isActive;
  public Sprite contractIcon;
}

[System.Serializable]

public class MonsterInfo : Info
{
  public MONSTER_TYPE type;
};

static public class ObjectFactory
{
  // Ingredient block layouts 
  static public int maxLayout = 2;
  static public Vector2[][] blockLayouts = new Vector2[][]
  {
    new Vector2[] { },                      // single block
    new Vector2[] { new Vector2( 1, 0 ) },  // double horizontal 
    new Vector2[] { new Vector2( 0, -1 ) }, // double vertical 
  };

  static public void GenerateObjectBlock(BlockBehaviour parent, Transform t)
  {
    Vector2 ingredientSize = t.localScale;
    Vector2 newScale = ingredientSize / maxLayout;
    parent.transform.localScale = newScale;
    parent.transform.position = t.position;
    parent.transform.SetParent(t);

    // Create core of ingredient 
    GameObject ingredientHolder = new GameObject("Holder");
    ingredientHolder.transform.position = t.position + new Vector3(-newScale.x, newScale.y, 0.0f) / maxLayout;
    ingredientHolder.transform.localScale = newScale;
    ingredientHolder.transform.SetParent(parent.transform);

    GameObject ingredientObj = new GameObject("Object");
    ingredientObj.transform.position = ingredientHolder.transform.position;
    ingredientObj.transform.localScale = newScale;
    ingredientObj.transform.SetParent(ingredientHolder.transform);
    parent.childrenObjs.Add(ingredientObj);

    // Create ingredient layout 
    foreach (Vector2 vec in parent.layout)
    {
      GameObject ingredientHolder2 = new GameObject("Holder");
      ingredientHolder2.transform.position = t.position;
      ingredientHolder2.transform.localScale = newScale;
      ingredientHolder2.transform.SetParent(parent.transform);

      // Create ingredients within connection 
      GameObject newIngredient = new GameObject("Object");
      newIngredient.transform.localScale = newScale;
      parent.childrenObjs.Add(newIngredient);

      // Initialize new ingredient 
      Vector3 offset = Vector2.Scale(vec, newScale);
      newIngredient.transform.position = ingredientHolder.transform.position;
      newIngredient.transform.position += offset;
      newIngredient.transform.SetParent(ingredientHolder2.transform);
    }

    //return parent.childenObjs;
  }

  static public void InitializeItem(GameObject block, ITEM_TYPE type)
  {
    Sprite itemSprite = null;

    switch (type)
    {
      case ITEM_TYPE.EMPTY:
        itemSprite = Resources.Load<Sprite>("Sprites/UI/locked");
        block.GetComponent<SpriteRenderer>().sprite = itemSprite;
        block.GetComponent<SpriteRenderer>().color = Color.black;
        break;
      case ITEM_TYPE.SLOT:
        block.GetComponent<SpriteRenderer>().sprite = null;
        //block.GetComponent<SpriteRenderer>().color = Color.black;
        break;
      case ITEM_TYPE.EATER:
        itemSprite = Resources.Load<Sprite>("Sprites/cross");
        block.GetComponent<SpriteRenderer>().sprite = itemSprite;
        block.GetComponent<SpriteRenderer>().color = Color.red;
        break;
      case ITEM_TYPE.BIN:
        itemSprite = Resources.Load<Sprite>("Sprites/UI/trashcanOpen");
        block.GetComponent<SpriteRenderer>().sprite = itemSprite;
        block.GetComponent<SpriteRenderer>().color = Color.black;
        break;
      case ITEM_TYPE.TURNTABLE:
        itemSprite = Resources.Load<Sprite>("Sprites/UI/tool_turntable2");
        block.GetComponent<SpriteRenderer>().sprite = itemSprite;
        block.GetComponent<SpriteRenderer>().color = Color.white;
        break;
    }
  }

  static public void InitializeGrid(GameObject grid, GRID_TYPE type)
  {
    Sprite gridSprite = Resources.Load<Sprite>("Sprites/slot_square");

    switch (type)
    {
      case GRID_TYPE.PLATE:
        grid.GetComponent<SpriteRenderer>().sprite = gridSprite;
        break;
      case GRID_TYPE.BOWL:
        gridSprite = Resources.Load<Sprite>("Sprites/slot_circle_2");
        grid.GetComponent<SpriteRenderer>().sprite = gridSprite;
        break;
    }
  }

  static public void InitializeSaucePlate(GameObject plate, SAUCE_TYPE type)
  {
    switch (type)
    {  
      case SAUCE_TYPE.SOYSAUCE: // sauce_grape
        Utility.SetColorFromHex(plate, "#D453F0FF");
        break;
      case SAUCE_TYPE.MUSTARD: // sauce_oj
        Utility.SetColorFromHex(plate, "#e6880b");
        break;
      case SAUCE_TYPE.WASABI: // sauce_tomato
        Utility.SetColorFromHex(plate, "#E34B4BFF");
        break;
      case SAUCE_TYPE.EMPTY:
        plate.GetComponent<SpriteRenderer>().color = Color.white;
        break;
    }
  }

  static public void InitializeSauce(GameObject ingredient, SAUCE_TYPE type)
  {
    Sprite sauceSprite = Resources.Load<Sprite>("Sprites/slot_square");

    switch (type)
    {
      // sauce_grape
      case SAUCE_TYPE.SOYSAUCE:
        //Utility.SetColorFromHex(ingredient, "#3C3C3CFF");
        //Utility.SetColorFromHex(ingredient, "#58EAFFFF");
        sauceSprite = Resources.Load<Sprite>("Sprites/sauce_grape");
        ingredient.GetComponent<SpriteRenderer>().sprite = sauceSprite;
        break;
      case SAUCE_TYPE.MUSTARD:
        //Utility.SetColorFromHex(ingredient, "#ffc04c");
        sauceSprite = Resources.Load<Sprite>("Sprites/sauce_oj");
        ingredient.GetComponent<SpriteRenderer>().sprite = sauceSprite;
        break;
      case SAUCE_TYPE.WASABI:
        //Utility.SetColorFromHex(ingredient, "#58FF6EFF");
        sauceSprite = Resources.Load<Sprite>("Sprites/sauce_tomato");
        ingredient.GetComponent<SpriteRenderer>().sprite = sauceSprite;
        break;
      case SAUCE_TYPE.EMPTY:
        ingredient.GetComponent<SpriteRenderer>().color = Color.white;
        break;
    }
  }

  static public void InitializeIngredientTop(GameObject ingredient, 
    INGREDIENT_TYPE type, SAUCE_TYPE sauce = SAUCE_TYPE.EMPTY) 
  {
    Sprite sprite = null;

    switch (type) 
    { 
      case INGREDIENT_TYPE.BREAD:
        sprite = Resources.Load<Sprite>("Sprites/bread_top");
        ingredient.GetComponent<SpriteRenderer>().sprite = sprite;
        break; 
      case INGREDIENT_TYPE.LETTUCE: 
        ingredient.GetComponent<SpriteRenderer>().color = Color.green; 
        break; 
      case INGREDIENT_TYPE.MEAT:
        sprite = Resources.Load<Sprite>("Sprites/meat_top4");
        ingredient.GetComponent<SpriteRenderer>().sprite = sprite;
        break;
      case INGREDIENT_TYPE.CHEESE:
        ingredient.GetComponent<SpriteRenderer>().color = Color.yellow;
        break;
      case INGREDIENT_TYPE.EATER:
        sprite = Resources.Load<Sprite>("Sprites/cross");
        ingredient.GetComponent<SpriteRenderer>().sprite = sprite;
        ingredient.GetComponent<SpriteRenderer>().color = Color.red;
        break;
      case INGREDIENT_TYPE.SAUCE:
        InitializeSauce(ingredient, sauce);
        break;
    } 
  }



  static public void InitializeIngredientSide(GameObject ingredient,
  INGREDIENT_TYPE type, SAUCE_TYPE sauce = SAUCE_TYPE.EMPTY)
  {
    Sprite sprite = null;

    switch (type)
    {
      case INGREDIENT_TYPE.BREAD:
        sprite = Resources.Load<Sprite>("Sprites/bread_side_bot");
        ingredient.GetComponent<SpriteRenderer>().sprite = sprite;
        ingredient.GetComponent<SpriteRenderer>().color = Color.white;
        break;
      case INGREDIENT_TYPE.LETTUCE:
        ingredient.GetComponent<SpriteRenderer>().color = Color.green;
        break;
      case INGREDIENT_TYPE.MEAT:
        sprite = Resources.Load<Sprite>("Sprites/meat_side4");
        ingredient.GetComponent<SpriteRenderer>().sprite = sprite;
        ingredient.GetComponent<SpriteRenderer>().color = Color.white;
        break;
      case INGREDIENT_TYPE.CHEESE:
        ingredient.GetComponent<SpriteRenderer>().color = Color.yellow;
        break;
      case INGREDIENT_TYPE.EATER:
        sprite = Resources.Load<Sprite>("Sprites/cross_bar");
        ingredient.GetComponent<SpriteRenderer>().sprite = sprite;
        ingredient.GetComponent<SpriteRenderer>().color = Color.red;
        break;
      case INGREDIENT_TYPE.SAUCE:
        //InitializeSaucePlate(ingredient, sauce);
        break;
      case INGREDIENT_TYPE.EMPTY:
        sprite = Resources.Load<Sprite>("Sprites/ingredient_side");
        ingredient.GetComponent<SpriteRenderer>().sprite = sprite;
        ingredient.GetComponent<SpriteRenderer>().color = Color.white;
        break;
    }
  }

};

