using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientScript : MonoBehaviour {

  public INGREDIENT_TYPE type;
  public SAUCE_TYPE sauceType;


  public void InitializeIngredientScript(INGREDIENT_TYPE _type, SAUCE_TYPE sauce, Vector3 _size)
  {
    type = _type;
    sauceType = sauce;
    transform.localScale = _size;
    IngredientFactory.InitializeIngredient(gameObject, type, sauce);
    LoadIngredientSprite();
    if (sauce != SAUCE_TYPE.EMPTY)
    {
      Sprite sauceSprite = Resources.Load<Sprite>("Sprites/bottle_empty");
      GetComponent<SpriteRenderer>().sprite = sauceSprite;
      transform.localEulerAngles = new Vector3(0, 0, 45.0f);
    }
  }

  void LoadIngredientSprite()
  {
    Sprite sprite = null;

    switch (type)
    {
      case INGREDIENT_TYPE.MEAT:
        sprite = Resources.Load<Sprite>("Sprites/ham");
        GetComponent<SpriteRenderer>().sprite = sprite;
        GetComponent<SpriteRenderer>().color = Color.white;
        break;
      case INGREDIENT_TYPE.BREAD:
        sprite = Resources.Load<Sprite>("Sprites/bread_dark");
        GetComponent<SpriteRenderer>().sprite = sprite;
        GetComponent<SpriteRenderer>().color = Color.white;
        break;
      case INGREDIENT_TYPE.EATER:
        break;
    }
  }

  public void SetIngredientSauceType(SAUCE_TYPE sauce)
  {
    sauceType = sauce;
  }
}
