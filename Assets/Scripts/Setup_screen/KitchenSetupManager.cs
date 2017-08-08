using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KitchenSetupManager : MonoBehaviour {

  public bool isCounterUp;
  public GameObject counterObj;

	// Use this for initialization
	void Start () {
    isCounterUp = false;
    //ToggleCounterColliders(isCounterUp);

  }
	
	// Update is called once per frame
	void Update () {
		
	}

  void ToggleCounterColliders(bool flag)
  {
    // for toggling all colliders so it wont overlap with buttons
    BoxCollider2D[] cols = counterObj.GetComponentsInChildren<BoxCollider2D>();
    foreach (BoxCollider2D col in cols) col.enabled = flag;
  }

  public void ToggleCounter(bool flag)
  {
    isCounterUp = flag;
    LeanTween.cancel(counterObj);

    //ToggleCounterColliders(isCounterUp);

    if (isCounterUp)
    {
      LeanTween.moveY(counterObj, -2.0f, 0.75f).setEase(LeanTweenType.easeInOutQuad);
    }
    else
    {
      LeanTween.moveY(counterObj, -5.0f, 0.75f).setEase(LeanTweenType.easeInOutQuad);
    }
  }

  public void RaiseCounter()
  {
    if (isCounterUp == true) return;
    //ToggleCounter(true);
  }

  public void LowerCounter()
  {
    if (isCounterUp == false) return;
    //ToggleCounter(false);
  }
}
