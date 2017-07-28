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

  public void SetItemType(ITEM_TYPE item)
  {
    info = new ItemInfo(item);
    ObjectFactory.InitializeItem(gameObject, info.type);
  }

  void OnMouseOver()
  {
    OnTouchStay();
  }

  public void OnTouchStay()
  {
    if (info.type == ITEM_TYPE.EMPTY || GameManager.Instance.IsPaused()) return;

    // for items to be dragged and used
    if (InputMan.OnDown() && playerScript.GetPlayerState() == PLAYER_STATE.IDLE)
    {
      StartDrag();

      // update for item selection
      UpdateInfoPanel();
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
    ToggleScale();

    oldPos = transform.position;

    // update tool box manager if this is a new tool
    GetComponentInParent<ToolManager>().OffNewToolIndicator(gameObject);
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

  public void ClickBehaviour()
  {
    GameManager.Instance.setupMan.RaiseCounter();
    UpdateInfoPanel();
  }
}
