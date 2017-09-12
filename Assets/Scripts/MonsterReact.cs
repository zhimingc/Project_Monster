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
      GetComponent<Animator>().SetTrigger("jump");
    }
  }
}
