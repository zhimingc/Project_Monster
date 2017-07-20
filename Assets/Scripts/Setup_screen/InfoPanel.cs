﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoPanel : MonoBehaviour {

  public GameObject info_icon, upgradeButton;
  public TextMesh currencyText;
  public Text info_text, detail_neg, detail_pos;

  private bool panelTriggered;

	// Use this for initialization
	void Start () {
    EmptyInfoPanel();
    panelTriggered = false;

    UpdateCurrenyText();
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

  void UpdateCurrenyText()
  {
    currencyText.text = "$" + GameManager.Instance.scoreMan.totalCurrency.ToString();
  }

  void EmptyInfoPanel()
  {
    info_text.text = 
      "Toggle contracts by tapping contracts above. \n\n\nChoose tools by dragging from toolbox into tool slots.";
    detail_neg.text = "";
    detail_pos.text = "";
    info_icon.GetComponent<SpriteRenderer>().enabled = false;
    upgradeButton.SetActive(false);

  }

  public void UpdateInfoPanel(GameObject obj, Info info)
  {
    // flag for when the panel is changed
    panelTriggered = true;

    // update text
    ItemInfo item = info as ItemInfo;
    if (item != null)
    {
      upgradeButton.SetActive(true);
      upgradeButton.GetComponentInChildren<TextMesh>().text = "Upgrade";

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
      // cant upgrade contracts
      upgradeButton.SetActive(false);
      if (contract.isActive)
      {
        upgradeButton.GetComponentInChildren<TextMesh>().text = "Active";
      }
      else
      {
        upgradeButton.GetComponentInChildren<TextMesh>().text = "Inactive";
      }

      switch (contract.type)
      {
        case CONTRACT_TYPE.TIMER:
          info_text.text = "Catering contract for impatient monsters who want their order quick!";
          detail_neg.text = "Chance for timed requests\n30 seconds to serve request";
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
