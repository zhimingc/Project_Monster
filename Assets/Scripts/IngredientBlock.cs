using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientBlock : MonoBehaviour {

  public List<GameObject> ingredients;
  public bool beingDragged, isUsable;
  public Vector2[] layout;

  private Vector3 oldPos;
  private PlayerScript playerScript;
  private IngredientManager ingredientMan;

  // UI for being dragged
  private Vector3 draggedScale, idleScale;

  void Awake()
  {
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
    transform.position += vecToMove;
  }

  public void StopDrag(bool deleteIngredient)
  {
    if (deleteIngredient == false)
    {
      GetComponent<BoxCollider2D>().enabled = true;
      transform.position = oldPos;
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

  public void ToggleScale()
  {
    if (transform.localScale == idleScale) transform.localScale = draggedScale;
    else transform.localScale = idleScale;
  }

  public void StartDrag()
  {
    oldPos = transform.position;
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
    if (Input.GetMouseButtonDown(0) && playerScript.GetPlayerState() == PLAYER_STATE.IDLE) 
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
