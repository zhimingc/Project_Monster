using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterBody : MonoBehaviour
{
  public GameObject feedSign;
  public MonsterRequest monReq;

  private PlayerScript playerScript;

  // Use this for initialization
  void Start()
  {
    playerScript = GameObject.Find("player").GetComponent<PlayerScript>();
    ToggleFeedSign(false);
  }

  public void ToggleFeedSign(bool flag)
  {
    feedSign.SetActive(flag);
  }

  void OnMouseOver()
  {
    OnTouchStay();
  }

  public void OnTouchStay()
  {
    if (playerScript.hoveredGrid == null) return;

    GridScript gs = playerScript.hoveredGrid.GetComponent<GridScript>();

    // make sure the plate being dragged is the plate to be served
    if (gs.monReq.request.IsSameAs(monReq.request))
    {
      if (InputMan.OnUp())
      {        
        gs.SetCanServe(false);

        GameManager.Instance.monsterMan.ServeMonsterRequest(gs, monReq);
      }
    }
  }
}
