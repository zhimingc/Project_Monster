using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudScript : MonoBehaviour {

  //public GameObject cloudShadowObj;
  public float xLimit;
  public float speed;
  public GameObject cloudShadow;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
    UpdateMovement();
    CheckOutOfBounds();
  }

  void CheckOutOfBounds()
  {
    if (transform.position.x >= xLimit)
    {
      GameObject.Find("Background").GetComponent<BackgroundManager>().RemoveCloud(gameObject);
      Destroy(cloudShadow);
      Destroy(gameObject);
    }
  }

  void UpdateMovement()
  {
    Vector3 transAmt = Vector3.right * speed * Time.deltaTime;
    transform.Translate(transAmt);
    //cloudShadow.transform.Translate(transAmt);
  }

  public void SpawnShadow(GameObject shadowObj, float offsetFrom)
  {
    float totalOffset = offsetFrom - transform.position.y;
    cloudShadow = Instantiate(shadowObj, transform);
    cloudShadow.transform.position = new Vector3(transform.position.x, offsetFrom + totalOffset, 0);
  }
}
