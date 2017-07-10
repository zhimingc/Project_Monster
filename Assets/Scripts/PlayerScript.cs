using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PLAYER_STATE
{
  IDLE,
  DRAGGING
};

public delegate void MouseUpBehaviour();

public class PlayerScript : MonoBehaviour {

  public IngredientBlock blockBeingDragged;
  public PLAYER_STATE playerState;
  public GridScript hoveredGrid;

  private GridManager gridMan;
  //private MonsterManager monsterMan;
  private bool deleteIngredientFlag;
  private MouseUpBehaviour mouseUpDelegate;

  void Awake()
  {
    gridMan = GameObject.Find("grid_manager").GetComponent<GridManager>();
    //monsterMan = GameObject.Find("monster_manager").GetComponent<MonsterManager>();
  }

	// Use this for initialization
	void Start () {
    blockBeingDragged = null;
    hoveredGrid = null;
    mouseUpDelegate = DefaultMouseUpDel;

  }
	
  public void DragIngredientBlock(IngredientBlock block)
  {
    blockBeingDragged = block;
  }

  public void SetHoveredGrid(GridScript grid)
  {
    if (grid != null) deleteIngredientFlag = true;
    else deleteIngredientFlag = false;

    hoveredGrid = grid;
  }

	// Update is called once per frame
	void Update () {
    UpdateIngredientBlock();

	}

  // Gets MonsterManager to check if a request is met using the grid
  void CheckIfRequestMet()
  {
    //monsterMan.CheckRequestsMet(gridMan.grid);
    //monsterMan.CheckRequestMetAll();
  }

  public PLAYER_STATE GetPlayerState()
  {
    return playerState;
  }

  public void SetPlayerState(PLAYER_STATE state)
  {
    playerState = state;

    switch(playerState)
    {
      // Switching to idle
      case PLAYER_STATE.IDLE:
        //if (hoveredGrid != null) hoveredGrid.ResetHoveredGrid();
        gridMan.ResetGrid();
        SetHoveredGrid(null);

        CheckIfRequestMet();
        GameManager.Instance.CheckLevelComplete();
        break;
    }
  }

  public void SetDeleteIngredient(bool flag)
  {
    deleteIngredientFlag = flag;
  }

  public void SetMouseUpDel(MouseUpBehaviour del)
  {
    if (mouseUpDelegate == DefaultMouseUpDel)
      mouseUpDelegate = del;
  }

  public void ResetMouseUpDel()
  {
    mouseUpDelegate = DefaultMouseUpDel;  // reset mouse up behaviour
  }

  void DefaultMouseUpDel()
  {}

  void UpdateIngredientBlock()
  {
    // When an ingredient block is being dragged
    if (InputMan.OnUp() && playerState == PLAYER_STATE.DRAGGING &&
      blockBeingDragged != null)
    {
      IngredientBlock blockScript = blockBeingDragged.GetComponent<IngredientBlock>();
      blockScript.StopDrag(deleteIngredientFlag);
      mouseUpDelegate();
      blockBeingDragged = null;
      ResetMouseUpDel();

      SetPlayerState(PLAYER_STATE.IDLE);
    }
  }
}
