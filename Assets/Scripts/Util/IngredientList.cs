using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum INGREDIENT_TYPE 
{ 
  BREAD, 
  MEAT,
  LETTUCE,
  CHEESE,
  //NUM_INGREDIENTS,
  EATER,
  EMPTY,
};

static public class IngredientFactory
{
  static public void InitializeIngredient(GameObject ingredient, INGREDIENT_TYPE type) 
  { 
    switch (type) 
    { 
      case INGREDIENT_TYPE.BREAD: 
        ingredient.GetComponent<SpriteRenderer>().color = new Color(105/255.0f, 12/255.0f, 12/255.0f); 
        break; 
      case INGREDIENT_TYPE.LETTUCE: 
        ingredient.GetComponent<SpriteRenderer>().color = Color.green; 
        break; 
      case INGREDIENT_TYPE.MEAT: 
        ingredient.GetComponent<SpriteRenderer>().color = Color.red; 
        break;
      case INGREDIENT_TYPE.CHEESE:
        ingredient.GetComponent<SpriteRenderer>().color = Color.yellow;
        break;
      case INGREDIENT_TYPE.EATER:
        ingredient.GetComponent<SpriteRenderer>().color = Color.black;
        break;
    } 
  }
};

