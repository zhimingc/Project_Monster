using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour {

  public IngredientBlock blockBeingDragged;

  public GridScript hoveredGrid;

	// Use this for initialization
	void Start () {
    blockBeingDragged = null;
    hoveredGrid = null;
	}
	
  public void DragIngredientBlock(IngredientBlock block)
  {
    blockBeingDragged = block;
  }

  public void SetHoveredGrid(GridScript grid)
  {
    hoveredGrid = grid;
  }

	// Update is called once per frame
	void Update () {
    UpdateIngredientBlock();
    
	}

  void UpdateIngredientBlock()
  {
    if (Input.GetMouseButtonUp(0) && blockBeingDragged != null)
    {
      IngredientBlock blockScript = blockBeingDragged.GetComponent<IngredientBlock>();
      blockScript.StopDrag(hoveredGrid);
      blockBeingDragged = null;
    }
  }
}
