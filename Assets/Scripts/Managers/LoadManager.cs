using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadManager : MonoBehaviour {

  public float loadSpeed;
  public GameObject portal, backing;

	// Use this for initialization
	void Start () {
    GameObject canvas = Instantiate(Resources.Load<GameObject>("Prefabs/Util/load_screen"));
    portal = canvas.GetComponentsInChildren<Image>()[1].gameObject;
    backing = canvas.GetComponentsInChildren<Image>()[0].gameObject;
    DontDestroyOnLoad(canvas);

    // init loading screen
    loadSpeed = 0.5f;
    LoadIn();
  }
	
  public void LoadOut()
  {
    // fade backing in

    LeanTween.alpha(backing.GetComponent<Image>().rectTransform, 1.0f, loadSpeed).setEase(LeanTweenType.easeInCirc);

    // scale portal up
    //LeanTween.scale(portal, new Vector3(25, 25, 1), loadSpeed).setEase(LeanTweenType.easeInQuad);

    // rotate portal
    LeanTween.rotate(portal.GetComponent<Image>().rectTransform, 360, loadSpeed).setEase(LeanTweenType.linear);
    LeanTween.alpha(portal.GetComponent<Image>().rectTransform, 1.0f, loadSpeed).setEase(LeanTweenType.easeInCirc);
  }

  public void LoadIn()
  {
    // fade backing out
    LeanTween.alpha(backing.GetComponent<Image>().rectTransform, 0.0f, loadSpeed).setEase(LeanTweenType.easeInCirc);

    // scale portal down
    //LeanTween.scale(portal, new Vector3(0, 0, 1), loadSpeed).setEase(LeanTweenType.easeInQuad);

    // rotate portal
    LeanTween.rotate(portal.GetComponent<Image>().rectTransform, 360, loadSpeed).setEase(LeanTweenType.linear);
    LeanTween.alpha(portal.GetComponent<Image>().rectTransform, 0.0f, loadSpeed).setEase(LeanTweenType.easeInCirc);

  }

  // Update is called once per frame
  void Update () {
		
	}
}
