using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadManager : MonoBehaviour {

  public float loadSpeed;
  public GameObject portal, backing;

  private float spinAmt;

	// Use this for initialization
	void Awake () {
    GameObject canvas = Instantiate(Resources.Load<GameObject>("Prefabs/Util/load_screen"));
    portal = canvas.GetComponentsInChildren<Image>()[1].gameObject;
    backing = canvas.GetComponentsInChildren<Image>()[0].gameObject;
    DontDestroyOnLoad(canvas);
  }
	
  void Start()
  {
    // init loading screen
    loadSpeed = 0.5f;
    spinAmt = 360.0f;
    LoadIn();
  }

  public void LoadOut()
  {
    // sfx for loading
    GameManager.Instance.SFX().PlaySound("portal_out", 0.25f);

    // fade backing in
    LeanTween.alpha(backing.GetComponent<Image>().rectTransform, 1.0f, loadSpeed).setEase(LeanTweenType.easeInCirc);

    // scale portal up
    //LeanTween.scale(portal, new Vector3(25, 25, 1), loadSpeed).setEase(LeanTweenType.easeInQuad);

    // rotate portal
    LeanTween.rotate(portal.GetComponent<Image>().rectTransform, spinAmt, loadSpeed).setEase(LeanTweenType.linear);
    LeanTween.alpha(portal.GetComponent<Image>().rectTransform, 1.0f, loadSpeed).setEase(LeanTweenType.easeInCirc);
  }

  public void LoadIn()
  {
    // sfx for loading
    GameManager.Instance.SFX().PlaySound("portal_in", 0.25f);

    // fade backing out
    LeanTween.alpha(backing.GetComponent<Image>().rectTransform, 0.0f, loadSpeed).setEase(LeanTweenType.easeInCirc);

    // scale portal down
    //LeanTween.scale(portal, new Vector3(0, 0, 1), loadSpeed).setEase(LeanTweenType.easeInQuad);

    // rotate portal
    LeanTween.rotate(portal.GetComponent<Image>().rectTransform, spinAmt, loadSpeed).setEase(LeanTweenType.linear);
    LeanTween.alpha(portal.GetComponent<Image>().rectTransform, 0.0f, loadSpeed).setEase(LeanTweenType.easeInCirc);

  }

  // Update is called once per frame
  void Update () {
		
	}
}
