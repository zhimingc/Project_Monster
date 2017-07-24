using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSetup : BlockBehaviour {

  public ItemInfo info;

  new void Start()
  {
    ObjectFactory.InitializeItem(gameObject, info.type);
    oldPos = transform.position;
    idleScale = transform.localScale;
    draggedScale = idleScale * 1.5f;
  }

  void OnMouseOver()
  {
    OnTouchStay();
  }

  public void OnTouchStay()
  {
    if (info.type == ITEM_TYPE.EMPTY) return;

    // for items to be dragged and used
    if (InputMan.OnDown() && playerScript.GetPlayerState() == PLAYER_STATE.IDLE)
    {
      StartDrag();

      // update for item selection
      UpdateInfoPanel();
    }
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
  }

  void UpdateInfoPanel()
  {
    GameObject.Find("info_panel").GetComponent<InfoPanel>().UpdateInfoPanel(gameObject, info);
  }
}
