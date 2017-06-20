using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

  private Text loseText; 

	// Use this for initialization
	void Awake () {
    loseText = GameObject.Find("lose").GetComponent<Text>();
	}
	
  public void ToggleLoseText(bool flag)
  {
    loseText.enabled = flag;
  }

  void Start()
  {
    //loseText.enabled = false;
  }

	// Update is called once per frame
	void Update () {
		
	}
}
