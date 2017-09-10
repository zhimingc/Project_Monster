using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreManager : MonoBehaviour {

  public GameObject buySignObj;
  public GameObject earningsText, stampText;

  private int earnings, stamps;

	// Use this for initialization
	void Start () {
    // Get earnings and stamp numbers from game man
    earnings = GameManager.Instance.gameData.pop_total;
    stamps = GameManager.Instance.gameData.stamp_total;

    earningsText.GetComponent<TextMesh>().text = "$" + earnings.ToString();
    stampText.GetComponent<TextMesh>().text = stamps.ToString();
  }
	
	// Update is called once per frame
	void Update () {
		//if (Input.GetKeyDown(KeyCode.B))
  //  {
  //    buySignObj.GetComponent<Animator>().SetTrigger("enter");
  //  }
  //  if (Input.GetKeyDown(KeyCode.E))
  //  {
  //    buySignObj.GetComponent<Animator>().SetTrigger("exit");
  //  }
  }

  public void TriggerBuySign(bool flag)
  {
    if (flag) buySignObj.GetComponent<Animator>().SetTrigger("enter");
    else buySignObj.GetComponent<Animator>().SetTrigger("exit");
  }
}
