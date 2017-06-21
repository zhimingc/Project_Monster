using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientScript : MonoBehaviour {

  public INGREDIENT_TYPE type;
  public SAUCE_TYPE sauceType;
  //public List<Connection> connects = new List<Connection>();

  public void InitializeIngredientScript(INGREDIENT_TYPE _type, SAUCE_TYPE sauce, Vector3 _size)
  {
    type = _type;
    sauceType = sauce;
    transform.localScale = _size;
    IngredientFactory.InitializeIngredient(gameObject, type, sauce);
    if (sauce != SAUCE_TYPE.EMPTY)
    {
      Sprite sauceSprite = Resources.Load<Sprite>("Sprites/grid_block");
      GetComponent<SpriteRenderer>().sprite = sauceSprite;
    }
  }

  public void SetIngredientSauceType(SAUCE_TYPE sauce)
  {
    sauceType = sauce;
  }
}
