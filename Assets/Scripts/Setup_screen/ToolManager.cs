using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolManager : MonoBehaviour {

  public bool overwriteLocks;
  public ItemSetup[] toolBox;
  public GameObject newTool_indicator, newTool;

	// Use this for initialization
	void Start () {
    int rank = GameManager.Instance.gameData.pop_rank;

    toolBox[0].SetItemType(ITEM_TYPE.EATER);

    //if (rank >= (int)RANKS.BIN_TOOL) toolBox[0].SetItemType(ITEM_TYPE.BIN);
    //if (rank >= (int)RANKS.EATER_TOOL) toolBox[1].SetItemType(ITEM_TYPE.EATER);
    if (rank >= (int)RANKS.TURNTABLE_TOOL || overwriteLocks) toolBox[1].SetItemType(ITEM_TYPE.TURNTABLE);
    //if (rank > (int)RANKS.BIN_TOOL) toolBox[3].SetItemType(ITEM_TYPE.BIN);

    //ToggleNewToolIndicator(GameManager.Instance.gameData.indicator_newTool);
  }

	public void UpdateToolBox(RANKS rankType)
  {
    switch(rankType)
    {
      //case RANKS.BIN_TOOL:
      //  toolBox[0].SetItemType(ITEM_TYPE.BIN);
      //  SetNewToolIndicator(toolBox[0].gameObject);
      //  break;
      //case RANKS.EATER_TOOL:
      //  toolBox[1].SetItemType(ITEM_TYPE.EATER);
      //  SetNewToolIndicator(toolBox[1].gameObject);
      //  break;
      case RANKS.TURNTABLE_TOOL:
        toolBox[1].SetItemType(ITEM_TYPE.TURNTABLE);
        SetNewToolIndicator(toolBox[1].gameObject);
        break;
    }
  }

  public void OffNewToolIndicator(GameObject obj)
  {
    if (newTool == obj)
    {
      newTool_indicator.SetActive(false);
    }
  }

  void SetNewToolIndicator(GameObject obj)
  {
    newTool = obj;

    // set the click behaviour of the indicator
    newTool_indicator.GetComponent<NewIndicatorBubble>().SetClickFn(newTool.GetComponent<ItemSetup>().ClickBehaviour);

    // change this to animation later
    newTool_indicator.SetActive(true);
    newTool_indicator.transform.position = obj.transform.position + new Vector3(0, 2.0f, -3.0f);
  }
}
