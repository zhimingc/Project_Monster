﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContractSetup : MonoBehaviour {

  public ContractInfo info;
  public GameObject feedback_box, icon;

  // Use this for initialization
  void Start()
  {
    if (GameManager.Instance.CheckForContract(info.type))
    {
      info = GameManager.Instance.contracts[info.type];
    }
    else info.isActive = false;

    info.contractIcon = icon.GetComponent<SpriteRenderer>().sprite;
    UpdateDisplay();
  }
	
  void UpdateDisplay()
  {
    if (info.isActive)
    {
      Color newColor = Utility.GetColorFromHex("#86FF6FFF");
      GetComponent<SpriteRenderer>().color = newColor;
      feedback_box.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/UI/tick_box");
      feedback_box.GetComponent<SpriteRenderer>().enabled = false;
      feedback_box.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.25f);
    }
    else
    {
      Color newColor = Utility.GetColorFromHex("#FF6F6FFF");
      GetComponent<SpriteRenderer>().color = newColor;
      feedback_box.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/UI/cross_box");
      feedback_box.GetComponent<SpriteRenderer>().enabled = true;
      feedback_box.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.25f);
    }

    // save info into game manager
    GameManager.Instance.UpdateContracts(info);
  }

	void ToggleContract()
  {
    info.isActive = !info.isActive;
    UpdateDisplay();
  }

  void OnMouseOver()
  {
    OnTouchStay();
  }

  public void OnTouchStay()
  {
    // when pressed
    if (InputMan.OnDown())
    {
      ToggleContract();

      // update for contract selection
      UpdateInfoPanel();
    }
  }

  void UpdateInfoPanel()
  {
    GameObject.Find("info_panel").GetComponent<InfoPanel>().UpdateInfoPanel(icon, info);
  }
}
