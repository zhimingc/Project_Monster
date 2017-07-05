using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

  public GameObject loseSign;
  public Text helpText;


  // Use this for initialization
  void Awake () {
    loseSign = GameObject.Find("lose_sign");
    helpText = GameObject.Find("instructions_text").GetComponent<Text>();

  }

  public void ToggleLoseText(bool flag)
  {
    loseSign.GetComponent<Animator>().SetTrigger("isEnter");
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
