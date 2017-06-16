using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum INGREDIENT_TYPE 
{ 
  BREAD, 
  MEAT, 
  LETTUCE, 
  NUM_INGREDIENTS 
};

public class Connection 
{ 
  public Ingredient[] ingredient; 
  public GameObject connector; 
  public bool visited = false; 
}

[System.Serializable]
public class Ingredient 
{ 
  public Ingredient(INGREDIENT_TYPE _type, GameObject _obj, Vector3 _size) 
  { 
    type = _type; 
    obj = _obj; 
    obj.transform.localScale = _size; 
    IngredientFactory.InitializeIngredient(this); 
  } 
 
  public INGREDIENT_TYPE type; 
  public GameObject obj; 
  public List<Connection> connects = new List<Connection>(); 
};

static public class IngredientFactory
{
  static public void InitializeIngredient(Ingredient ingredient) 
  { 
    switch (ingredient.type) 
    { 
      case INGREDIENT_TYPE.BREAD: 
        ingredient.obj.GetComponent<SpriteRenderer>().color = new Color(165/255.0f, 42/255.0f, 42/255.0f); 
        break; 
      case INGREDIENT_TYPE.LETTUCE: 
        ingredient.obj.GetComponent<SpriteRenderer>().color = Color.green; 
        break; 
      case INGREDIENT_TYPE.MEAT: 
        ingredient.obj.GetComponent<SpriteRenderer>().color = Color.red; 
        break; 
    } 
  }
};

