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
  NUM_SAUCE,
  EMPTY
}

static public class IngredientFactory
{
  static public void InitializeSauce(GameObject ingredient, SAUCE_TYPE type)
  {
    switch (type)
    {
      case SAUCE_TYPE.SOYSAUCE:
        ingredient.GetComponent<SpriteRenderer>().color = Color.black;
        break;
      case SAUCE_TYPE.MUSTARD:
        ingredient.GetComponent<SpriteRenderer>().color = Color.yellow;
        break;
      case SAUCE_TYPE.EMPTY:
        ingredient.GetComponent<SpriteRenderer>().color = Color.white;
        break;
    }
  }

  static public void InitializeIngredient(GameObject ingredient, 
    INGREDIENT_TYPE type, SAUCE_TYPE sauce = SAUCE_TYPE.EMPTY) 
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
      case INGREDIENT_TYPE.SAUCE:
        InitializeSauce(ingredient, sauce);
        break;
    } 
  }
};

