using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KitchenSetupManager : MonoBehaviour {

  public bool isCounterUp;
  public GameObject counterObj;

	// Use this for initialization
	void Start () {
    isCounterUp = false;

	}
	
	// Update is called once per frame
	void Update () {
		
	}

  public void ToggleCounter(bool flag)
  {
    isCounterUp = flag;
    LeanTween.cancel(counterObj);

    if (isCounterUp)
    {
      LeanTween.moveY(counterObj, -2.0f, 0.75f).setEase(LeanTweenType.easeInOutQuad);
    }
    else
    {
      LeanTween.moveY(counterObj, -5.0f, 0.75f).setEase(LeanTweenType.easeInOutQuad);
    }
  }
}
