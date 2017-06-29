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

static public class IngredientFactory
{
  static public void InitializeGrid(GameObject grid, GRID_TYPE type)
  {
    Sprite gridSprite = Resources.Load<Sprite>("Sprites/slot_square");

    switch (type)
    {
      case GRID_TYPE.PLATE:
        grid.GetComponent<SpriteRenderer>().sprite = gridSprite;
        break;
      case GRID_TYPE.BOWL:
        gridSprite = Resources.Load<Sprite>("Sprites/slot_circle");
        grid.GetComponent<SpriteRenderer>().sprite = gridSprite;
        break;
    }
  }

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
      case SAUCE_TYPE.WASABI:
        ingredient.GetComponent<SpriteRenderer>().color = Color.green;
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
        sprite = Resources.Load<Sprite>("Sprites/bread_dark");
        ingredient.GetComponent<SpriteRenderer>().sprite = sprite;
        break; 
      case INGREDIENT_TYPE.LETTUCE: 
        ingredient.GetComponent<SpriteRenderer>().color = Color.green; 
        break; 
      case INGREDIENT_TYPE.MEAT:
        sprite = Resources.Load<Sprite>("Sprites/ham");
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
        sprite = Resources.Load<Sprite>("Sprites/bread_dark_side_bot");
        ingredient.GetComponent<SpriteRenderer>().sprite = sprite;
        ingredient.GetComponent<SpriteRenderer>().color = Color.white;
        break;
      case INGREDIENT_TYPE.LETTUCE:
        ingredient.GetComponent<SpriteRenderer>().color = Color.green;
        break;
      case INGREDIENT_TYPE.MEAT:
        sprite = Resources.Load<Sprite>("Sprites/ham_side");
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
        InitializeSauce(ingredient, sauce);
        break;
      case INGREDIENT_TYPE.EMPTY:
        sprite = Resources.Load<Sprite>("Sprites/ingredient_side");
        ingredient.GetComponent<SpriteRenderer>().sprite = sprite;
        ingredient.GetComponent<SpriteRenderer>().color = Color.white;
        break;
    }
  }

};

