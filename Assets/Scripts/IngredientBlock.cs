using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientBlock : MonoBehaviour {

  public List<GameObject> ingredients;
  public bool beingDragged, isUsable;
  public Vector2[] layout;
  public Vector3 oldPos;
  public bool isReverseLayout;

  private PlayerScript playerScript;
  private IngredientManager ingredientMan;

  // UI for being dragged
  private Vector3 draggedScale, idleScale;
  //private bool isBigScale;
  private LTDescr moveDescr;

  void Awake()
  {
    isReverseLayout = false;
    //isBigScale = false;
    playerScript = GameObject.Find("player").GetComponent<PlayerScript>();
    ingredientMan = GameObject.Find("ingredient_manager").GetComponent<IngredientManager>();
  }

  // Use this for initialization
	void Start () {
    isUsable = true;
    beingDragged = false;
    draggedScale = transform.localScale;

    // initialize as idle scale
    transform.localScale = idleScale;
  }
	
	// Update is called once per frame
	void Update () {
    if (beingDragged)
    {
      DragUpdate();
    }
  }

  public void SetIdleScale(float scaling)
  {
    idleScale = transform.localScale * scaling;
  }

  public void SetIdleScale(Vector3 scaling)
  {
    idleScale = scaling;
  }

  public void AddIngredient(GameObject ingredient) 
  { 
    ingredients.Add(ingredient); 
  }

  public void ToggleIngredients(bool flag)
  {
    foreach (GameObject obj in ingredients)
      obj.GetComponent<SpriteRenderer>().enabled = flag;
  }

  void DragUpdate()
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

  public void StopDrag(bool deleteIngredient)
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

  void ReturnToOrigin()
  {
    if (moveDescr != null) LeanTween.cancel(moveDescr.id);
    moveDescr = LeanTween.move(gameObject, oldPos, 0.25f).setEase(LeanTweenType.easeOutQuad);
  }

  public void ToggleScale()
  {
    if (beingDragged)
    {
      //isBigScale = true;
      transform.localScale = idleScale;
      LeanTween.scale(gameObject, draggedScale, 0.25f).setEase(LeanTweenType.easeOutQuad);
    }
    else
    {
      //isBigScale = false;
      transform.localScale = draggedScale;
      LeanTween.scale(gameObject, idleScale, 0.25f).setEase(LeanTweenType.easeOutQuad);
    }
  }

  public void StartDrag()
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

  void OnMouseOver()
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

  public void ToggleUsability(bool flag)
  {
    isUsable = flag;
    if (isUsable == true)
    {
      GetComponent<BoxCollider2D>().enabled = true;
      GetComponent<SpriteRenderer>().enabled = true;
      GetComponent<SpriteRenderer>().color = Color.green;
    }
    else
    {
      GetComponent<BoxCollider2D>().enabled = false;
      GetComponent<SpriteRenderer>().enabled = true;
      GetComponent<SpriteRenderer>().color = Color.white;
    }
  }
}
