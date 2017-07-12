using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemScript : BlockBehaviour
{
  public ITEM_TYPE itemType;
  public int curCooldown, maxCooldown;

  public void SetItemType(ITEM_TYPE type)
  {
    itemType = type;
  }

  public void SpawnItem()
  {

  }

	// Use this for initialization
	void Start () {
    oldPos = transform.position;
    idleScale = transform.localScale;
    draggedScale = idleScale * 1.5f;
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
      //Destroy(gameObject);
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


}
