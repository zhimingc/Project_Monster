using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoPanel : MonoBehaviour {

  public GameObject info_icon;
  public TextMesh currencyText;
  public Text info_text, detail_neg, detail_pos, name_text;

  private bool panelTriggered;

	// Use this for initialization
	void Start () {
    EmptyInfoPanel();
    panelTriggered = false;
  }

  // Update is called once per frame
  void Update()
  {
    if (InputMan.OnDown())
    {
      if (!panelTriggered) EmptyInfoPanel();
      else panelTriggered = false;
    }
  }

  void EmptyInfoPanel()
  {
    info_text.text =
      "Tap icons to see more info.\n\nDrag from toolbox into tools to use tool.";
    detail_neg.text = "";
    detail_pos.text = "";
    name_text.text = "";
    info_icon.GetComponent<SpriteRenderer>().enabled = false;
  }

  public void UpdateInfoPanel(GameObject obj, Info info)
  {
    // flag for when the panel is changed
    panelTriggered = true;
    if (info.name != null) name_text.text = info.name;

    // update text
    ItemInfo item = info as ItemInfo;
    if (item != null)
    {
      switch (item.type)
      {
        case ITEM_TYPE.EATER:
          info_text.text = "The eater will remove the top most ingredient from the plate.";
          detail_pos.text = "Clears ingredient on plate\nUse: Drag eater onto plate";
          break;
        case ITEM_TYPE.BIN:
          info_text.text = "Throw away unwanted ingredients in the queue into the bin.";
          detail_pos.text = "Clears ingredient in queue\nUse: Drag ingredients into bin slot";
          break;
        case ITEM_TYPE.TURNTABLE:
          info_text.text = "Turn the table and rotate the ingredients on the table.";
          detail_pos.text = "Rotates ingredient stacks clockwise\nUse: Drag turn table onto any plate";
          break;
      }

      detail_neg.text = "Cooldown: " + item.itemCooldown.ToString() + " moves";
    }

    //ContractInfo contract = info as ContractInfo;
    //if (contract != null)
    //{
    //  switch (contract.type)
    //  {
    //    case CONTRACT_TYPE.TIMER:
    //      info_text.text = "Catering contract for impatient monsters who want their order quick!";
    //      detail_neg.text = "Chance for timed requests\n30 seconds to serve request";
    //      detail_pos.text = "Cash+";
    //      break;
    //  }
    //}

    MonsterInfo monster = info as MonsterInfo;
    if (monster != null)
    {
      switch(monster.type)
      {
        case MONSTER_TYPE.NORMAL:
          info_text.text = "Average dune monster just looking for a quick bite of some human food.";
          detail_neg.text = "";
          detail_pos.text = "Popularity+";
          break;
        case MONSTER_TYPE.TIMED:
          info_text.text = "The Lizmen are always in a hurry. They must have their order quick!";
          detail_neg.text = "Chance for timed requests\n30 seconds to serve request";
          detail_pos.text = "Popularity++";
          break;
      }
    }

    // update icon
    UpdateInfoIcon(obj);
  }

  void UpdateInfoIcon(GameObject updateTo)
  {
    info_icon.transform.localScale = updateTo.transform.localScale;
    info_icon.GetComponent<SpriteRenderer>().enabled = true;
    info_icon.GetComponent<SpriteRenderer>().sprite = updateTo.GetComponent<SpriteRenderer>().sprite;
    info_icon.GetComponent<SpriteRenderer>().color = updateTo.GetComponent<SpriteRenderer>().color;
  }
}
