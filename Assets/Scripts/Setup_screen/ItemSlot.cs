using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSlot : MonoBehaviour {

  public int slotNum;
  public ItemInfo info;
  public GameObject itemObj;
  private PlayerScript playerScript;

  // Use this for initialization
  void Start () {
    info = GameManager.Instance.GetItemSlot(slotNum);
    ObjectFactory.InitializeItem(itemObj, info.type);
    playerScript = GameObject.Find("player").GetComponent<PlayerScript>();
  }

  void OnMouseEnter()
  {
    OnTouchDown();
  }

  public void OnTouchDown()
  {
    // for items to be staticly used
    if (playerScript.GetPlayerState() == PLAYER_STATE.DRAGGING)
    {
      // Stop grid from moving
      playerScript.blockBeingDragged.beingDragged = false;
      // Update grid with move
      playerScript.blockBeingDragged.transform.position = transform.position;

      playerScript.blockBeingDragged.ToggleScale();

      // update sprite
      ItemSetup setup = (ItemSetup)playerScript.blockBeingDragged;
      ObjectFactory.InitializeItem(itemObj, setup.info.type);
    }
  }

  void OnMouseOver()
  {
    OnTouchStay();
  }

  public void OnTouchStay()
  {
    // for items to be dragged and used
    if (InputMan.OnUp() && playerScript.GetPlayerState() == PLAYER_STATE.DRAGGING)
    {
      ItemSetup setup = (ItemSetup)playerScript.blockBeingDragged;

      // apply change in item
      info = setup.info;
      GameManager.Instance.SetUpItemSlot(slotNum, info);
      ObjectFactory.InitializeItem(itemObj, info.type);
    }
  }

  void OnMouseExit()
  {
    OnTouchExit();
  }

  public void OnTouchExit()
  {
    //GetComponent<SpriteRenderer>().color = Color.white;

    if (playerScript.playerState == PLAYER_STATE.DRAGGING)
    {
      // Allow grid to move
      playerScript.blockBeingDragged.beingDragged = true;
      playerScript.blockBeingDragged.ToggleScale();

      ObjectFactory.InitializeItem(itemObj, info.type);
    }
  }
}
