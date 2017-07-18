using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoPanel : MonoBehaviour {

  public GameObject info_icon;
  public Text info_text, detail_neg, detail_pos;

	// Use this for initialization
	void Start () {
    EmptyInfoPanel();

  }
	
	// Update is called once per frame
	void Update () {
		
	}

  void EmptyInfoPanel()
  {
    info_text.text = "";
    detail_neg.text = "";
    detail_pos.text = "";
    info_icon.GetComponent<SpriteRenderer>().enabled = false;
  }

  public void UpdateInfoPanel(GameObject obj, Info info)
  {
    // update text
    ItemInfo item = info as ItemInfo;
    if (item != null)
    {
      switch (item.type)
      {
        case ITEM_TYPE.EATER:
          info_text.text = "The eater will remove the top most ingredient from the plate.";
          detail_pos.text = "Clears ingredient on plate";
          break;
        case ITEM_TYPE.BIN:
          info_text.text = "Throw away unwanted ingredients in the queue into the bin.";
          detail_pos.text = "Clears ingredient in queue";
          break;
      }

      detail_neg.text = "Cooldown: " + item.itemCooldown.ToString() + " moves";
    }

    ContractInfo contract = info as ContractInfo;
    if (contract != null)
    {
      switch (contract.type)
      {
        case CONTRACT_TYPE.TIMER:
          info_text.text = "Catering contract for impatient monsters who want their order quickly!";
          detail_neg.text = "Chance for timed requests";
          detail_pos.text = "Cash+";
          break;
      }

    }

    // update icon
    UpdateInfoIcon(obj);
  }

  void UpdateInfoIcon(GameObject updateTo)
  {
    info_icon.GetComponent<SpriteRenderer>().enabled = true;
    info_icon.GetComponent<SpriteRenderer>().sprite = updateTo.GetComponent<SpriteRenderer>().sprite;
    info_icon.GetComponent<SpriteRenderer>().color = updateTo.GetComponent<SpriteRenderer>().color;
  }
}
