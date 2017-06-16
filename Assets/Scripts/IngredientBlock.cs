using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientBlock : MonoBehaviour {

  public List<GameObject> ingredients;
  public bool beingDragged;
  public Vector2[] layout;

  private Vector3 oldPos;
    
  // Use this for initialization
	void Start () {
    beingDragged = false;
	}
	
	// Update is called once per frame
	void Update () {
    if (beingDragged)
    {
      DragUpdate();
    }

    // Update border display 
    GetComponent<SpriteRenderer>().enabled = GetComponent<BoxCollider2D>().enabled;
  }

  public void AddIngredient(GameObject ingredient) 
  { 
    ingredients.Add(ingredient); 
  }

  void DragUpdate()
  {
    Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    mousePos.z = 0.0f;
    SetBlockPosition(mousePos);

  }

  public void SetBlockPosition(Vector3 pos)
  {
    Vector3 vecToMove = pos - ingredients[0].transform.position;

    // Move parent such that mouse aligns with core ingredient
    transform.position += vecToMove;
  }

  public void StopDrag(GridScript grid)
  {
    if (grid == null)
    {
      GetComponent<BoxCollider2D>().enabled = true;
      transform.position = oldPos;
      beingDragged = false;
    }
  }

  void StartDrag()
  {
    oldPos = transform.position;
    beingDragged = true;
    GetComponent<BoxCollider2D>().enabled = false;

    // Update player script
    GameObject.Find("player").GetComponent<PlayerScript>().DragIngredientBlock(this);
  }

  void OnMouseOver() 
  { 
    if (Input.GetMouseButtonDown(0)) 
    {
      StartDrag();
    } 
  }
}
