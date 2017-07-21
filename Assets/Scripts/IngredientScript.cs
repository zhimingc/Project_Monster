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
    ObjectFactory.InitializeIngredientTop(gameObject, type, sauce);

    if (sauce != SAUCE_TYPE.EMPTY)
    {
      //Sprite sauceSprite = Resources.Load<Sprite>("Sprites/bottle_empty");
      //GetComponent<SpriteRenderer>().sprite = sauceSprite;

      ObjectFactory.InitializeSauce(gameObject, sauceType);
      //transform.localEulerAngles = new Vector3(0, 0, 45.0f);
    }
  }

  public void SetIngredientSauceType(SAUCE_TYPE sauce)
  {
    sauceType = sauce;
  }
}
