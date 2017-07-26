using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSetup : MonoBehaviour {

  public MonsterInfo info;
  public GameObject icon, monster_bubble;
  public bool isActive, isNewMonster;

  public void ActivateMonster()
  {
    isActive = true;
  }

  void Start()
  {
    isActive = false;
  }

  void OnMouseOver()
  {
    OnTouchStay();
  }

  public void OnTouchStay()
  {
    if (!isActive) return;

    // when pressed
    if (InputMan.OnDown())
    {
      if (isNewMonster) monster_bubble.SetActive(false);
      isNewMonster = false;

      // update for contract selection
      UpdateInfoPanel();
    }
  }

  void UpdateInfoPanel()
  {
    GameObject.Find("info_panel").GetComponent<InfoPanel>().UpdateInfoPanel(icon, info);
  }

  public void SetMonsterBubble()
  {
    isNewMonster = true;
    monster_bubble.SetActive(true);
    monster_bubble.transform.localPosition = transform.localPosition + new Vector3(0, 1, 0);
  }
}
