using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemScript : BlockBehaviour
{
  public ITEM_TYPE itemType;
  public bool canDrag, isStaticUse;

  public void SetItemType(ITEM_TYPE type)
  {
    itemType = type;

    // init for diff item types
    switch (itemType)
    {
      case ITEM_TYPE.BIN:
        isStaticUse = true;
        ToggleCanDrag(false);
        break;
      case ITEM_TYPE.EATER:
      case ITEM_TYPE.TURNTABLE:
        isStaticUse = false;
        ToggleCanDrag(true);
        break;
    }
  }

  // Use this for initialization
  new void Start()
  {
    //canDrag = true;
    oldPos = transform.position;
    idleScale = transform.localScale;
    draggedScale = idleScale * 1.5f;
  }

  new protected void Update()
  {
    if (beingDragged)
    {
      DragUpdate();
    }
  }

  new protected void DragUpdate()
  {
    Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    mousePos.z = 0.0f;
    transform.position = mousePos;

    // Update border display 
    //GetComponent<SpriteRenderer>().enabled = false;
  }

  new public void StopDrag(bool deleteIngredient)
  {
    if (deleteIngredient == false)
    {
      GetComponent<BoxCollider2D>().enabled = true;
      ReturnToOrigin();
      beingDragged = false;

      // return to idle scale when in queue 
      ToggleScale();
    }
    else
    {
      GameManager.Instance.IncrementTurnCounter();
      Destroy(gameObject);
    }

  }
  void OnMouseEnter()
  {
    OnTouchDown();
  }

  public void OnTouchDown()
  {
    // for items to be staticly used
    if (isStaticUse && playerScript.GetPlayerState() == PLAYER_STATE.DRAGGING)
    {
      // Stop grid from moving
      playerScript.blockBeingDragged.GetComponent<IngredientBlock>().beingDragged = false;
      // Update grid with move
      playerScript.blockBeingDragged.transform.position = transform.position;

      playerScript.blockBeingDragged.ToggleScale();
      playerScript.SetDeleteIngredient(true);

      // Set delegate to determine mouse up behaviour
      playerScript.SetMouseUpDel(gameObject, ItemMouseUp);
    }
  }

  void OnMouseOver()
  {
    OnTouchStay();
  }

  public void OnTouchStay()
  {
    // for items to be dragged and used
    if (InputMan.OnDown() && playerScript.GetPlayerState() == PLAYER_STATE.IDLE)
    {
      StartDrag();
    }
  }

  void ItemMouseUp()
  {
    switch (itemType)
    {
      case ITEM_TYPE.BIN:
        if (playerScript.playerState == PLAYER_STATE.DRAGGING)
        {
          // Set cooldown
          GetComponentInParent<ItemSpawn>().RemoveItem();
          // Audio feedback
          GameManager.Instance.SFX().PlaySoundWithPitch("trash", 0.75f, 1.0f);
          // disable use while cooling down
          ToggleCanUse(false);
        }
        break;
    }

    if (playerScript.hoveredGrid != null)
    {
      // Set cooldown
      GetComponentInParent<ItemSpawn>().RemoveItem();
      // disable use while cooling down
      ToggleCanUse(false);
    }
  }

  new public void StartDrag()
  {
    if (!canDrag)
    {
      // Return feedback that it can't be dragged here
      return;
    }

    beingDragged = true;
    GetComponent<BoxCollider2D>().enabled = false;

    // Update player script
    playerScript.DragIngredientBlock(this);
    playerScript.SetPlayerState(PLAYER_STATE.DRAGGING);

    // Original scale when dragging
    //transform.localScale = draggedScale;
    ToggleScale();

    // Set delegate to determine mouse up behaviour
    playerScript.SetMouseUpDel(gameObject, ItemMouseUp);
  }

  void OnMouseExit()
  {
    OnTouchExit();
  }

  public void OnTouchExit()
  {
    //GetComponent<SpriteRenderer>().color = Color.white;

    if (isStaticUse && playerScript.playerState == PLAYER_STATE.DRAGGING)
    {
      // Allow grid to move
      playerScript.blockBeingDragged.GetComponent<IngredientBlock>().beingDragged = true;
      // Update grid with move
      //playerScript.blockBeingDragged.SetBlockPosition(transform.position);

      playerScript.blockBeingDragged.ToggleScale();
      playerScript.SetDeleteIngredient(false);
      playerScript.ResetMouseUpDel(gameObject);
    }
  }

  public void ToggleCanDrag(bool flag)
  {
    canDrag = flag;
  }

  public void ToggleCanUse(bool flag)
  {
    GetComponent<BoxCollider2D>().enabled = flag;
  }

}
