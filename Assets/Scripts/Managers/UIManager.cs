using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

  public Text loseText;
  public Text helpText;

	// Use this for initialization
	void Awake () {
    loseText = GameObject.Find("lose").GetComponent<Text>();
    helpText = GameObject.Find("instructions_text").GetComponent<Text>();
  }
	
  public void ToggleLoseText(bool flag)
  {
    loseText.enabled = flag;
  }

  public void ToggleHelpText(bool flag)
  {
    helpText.enabled = flag;
  }

  void Start()
  {
    //loseText.enabled = false;
  }

	// Update is called once per frame
	void Update () {
		
	}
}
