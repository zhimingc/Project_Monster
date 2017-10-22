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
    //feedSign.SetActive(flag);
  }

  void OnMouseOver()
  {
    OnTouchStay();
  }

  public void OnTouchStay()
  {
    if (playerScript.hoveredGrid == null) return;

    GridScript gs = playerScript.hoveredGrid.GetComponent<GridScript>();

    if (gs.ingredientStack.Count < 1) return;

    if (gs.monReq == null && 
      monReq.request.monsterType != MONSTER_TYPE.GARBAGE) return;

    // make sure the plate being dragged is the plate to be served
    if (monReq.request.monsterType == MONSTER_TYPE.GARBAGE ||
      gs.monReq.request.IsSameAs(monReq.request))
    {
      if (gs.isBeingServed == false)
      {
        gs.isBeingServed = true;
        Vector3 servePos = GetComponent<MonsterAnimation>().origin;
        servePos.y -= 1.0f;
        LeanTween.move(gs.gameObject, servePos, 0.1f);
        LeanTween.scale(gs.gameObject, new Vector3(1.5f, 1.5f, 1.5f), 0.1f);
      }

      if (InputMan.OnUp())
      { 
        LeanTween.cancel(gs.gameObject);
        gs.SetCanServe(false);

        GameManager.Instance.monsterMan.ServeMonsterRequest(gs, monReq);
      }
    }
  }

  void OnMouseExit()
  {
    OnTouchExit();
  }

  public void OnTouchExit()
  {
    if (playerScript.hoveredGrid == null) return;

    GridScript gs = playerScript.hoveredGrid.GetComponent<GridScript>();

    LeanTween.cancel(gs.gameObject);
    if (gs.isBeingServed == true)
    {
      gs.isBeingServed = false;
    }
  }
}
