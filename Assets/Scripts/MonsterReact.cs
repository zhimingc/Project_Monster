using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterReact : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

  void OnMouseOver()
  {
    OnTouchStay();
  }

  public void OnTouchStay()
  {
    // Serve request if able to
    if (InputMan.OnDown())
    {
      // debug
      //GameManager.Instance.gameData.pop_total += 150;
      //GameManager.Instance.socialMan.ReportScore(10 + (int)Time.time, SOCIALBOARD.EARNINGS);

      if (GetComponent<SpriteRenderer>().color != Color.black)
        GetComponent<Animator>().SetTrigger("jump");
    }
  }
}
