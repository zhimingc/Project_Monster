using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientBlock : BlockBehaviour
{
  public List<GameObject> ingredients;

  public bool IsSauceBlock()
  {
    return ingredients[0].GetComponent<IngredientScript>().type == INGREDIENT_TYPE.SAUCE;
  }

  public void AddIngredient(GameObject ingredient) 
  { 
    ingredients.Add(ingredient);
    childenObjs.Add(ingredient);
  }

  //public void ToggleIngredients(bool flag)
  //{
  //  foreach (GameObject obj in ingredients)
  //    obj.GetComponent<SpriteRenderer>().enabled = flag;
  //}

  public IngredientScript GetIngredientScript(int index)
  {
    return ingredients[index].GetComponent<IngredientScript>();
  }

  new void DragUpdate()
  {
    Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    mousePos.z = 0.0f;
    SetBlockPosition(mousePos);

    // Update border display 
    GetComponent<SpriteRenderer>().enabled = false;
  }

  public void SetBlockPosition(Vector3 pos)
  {
    Vector3 vecToMove = pos - ingredients[0].transform.position;

    // Move parent such that mouse aligns with core ingredient
    //Vector3 dest = transform.position + vecToMove;
    //LeanTween.move(gameObject, dest, 0.1f).setEase(LeanTweenType.easeOutQuad);
    transform.position += vecToMove;
    transform.position = pos;
  }

  new public void StopDrag(bool deleteIngredient)
  {
    if (deleteIngredient == false)
    {
      GetComponent<BoxCollider2D>().enabled = true;
      //transform.position = oldPos;
      ReturnToOrigin();
      beingDragged = false;

      // return to idle scale when in queue 
      //transform.localScale = idleScale;
      ToggleScale();
      GetComponent<SpriteRenderer>().enabled = true;
    }
    else
    {
      // Ingredient block is used up and deleted from Q
      if (ingredients[0].GetComponent<IngredientScript>().type != INGREDIENT_TYPE.SAUCE)
        ingredientMan.RemoveFromIngredientQ(gameObject);
      Destroy(gameObject);
    }

  }

  new public void StartDrag()
  {
    beingDragged = true;
    GetComponent<BoxCollider2D>().enabled = false;

    // Update player script
    GameObject.Find("player").GetComponent<PlayerScript>().DragIngredientBlock(this);
    playerScript.SetPlayerState(PLAYER_STATE.DRAGGING);

    // Original scale when dragging
    //transform.localScale = draggedScale;
    ToggleScale();
  }

  protected void OnMouseOver()
  {
    OnTouchStay();
  }

  public void OnTouchStay()
  {
    //if (Input.GetMouseButtonDown(0) && playerScript.GetPlayerState() == PLAYER_STATE.IDLE) 
    if (InputMan.OnDown() && playerScript.GetPlayerState() == PLAYER_STATE.IDLE)
    {
      StartDrag();
    }
  }
}
