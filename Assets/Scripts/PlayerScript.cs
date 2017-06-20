﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PLAYER_STATE
{
  IDLE,
  DRAGGING
};

public class PlayerScript : MonoBehaviour {

  public IngredientBlock blockBeingDragged;
  public PLAYER_STATE playerState;
  public GridScript hoveredGrid;

  private GridManager gridMan;
  private MonsterManager monsterMan;
  private bool deleteIngredientFlag;

  void Awake()
  {
    gridMan = GameObject.Find("grid_manager").GetComponent<GridManager>();
    monsterMan = GameObject.Find("monster_manager").GetComponent<MonsterManager>();
  }

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
    monsterMan.CheckRequestsMet(gridMan.grid);
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
        CheckIfRequestMet();
        GameManager.Instance.CheckLevelComplete();

        if (hoveredGrid != null) hoveredGrid.ResetHoveredGrid();
        SetHoveredGrid(null);
        break;
    }
  }

  public void SetDeleteIngredient(bool flag)
  {
    deleteIngredientFlag = flag;
  }

  void UpdateIngredientBlock()
  {
    // When an ingredient block is being dragged
    if (Input.GetMouseButtonUp(0) && playerState == PLAYER_STATE.DRAGGING &&
      blockBeingDragged != null)
    {
      IngredientBlock blockScript = blockBeingDragged.GetComponent<IngredientBlock>();
      blockScript.StopDrag(deleteIngredientFlag);
      blockBeingDragged = null;

      SetPlayerState(PLAYER_STATE.IDLE);
    }
  }
}