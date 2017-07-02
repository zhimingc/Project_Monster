using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudScript : MonoBehaviour {

  //public GameObject cloudShadowObj;
  public float xLimit;
  public float speed;
  public GameObject cloudShadow;

  private Vector3 originScale;
  public float bounceAmt, bounceSpeed;

  private LTDescr scaleLT;
  private float bounceTimer;
  
	// Use this for initialization
	void Start () {
    originScale = transform.localScale;
    //DelayedBounce();
  }
	
	// Update is called once per frame
	void Update () {
    UpdateMovement();
    //UpdateScaling();
    CheckOutOfBounds();
  }

  void UpdateScaling()
  {
    bounceTimer -= Time.deltaTime;
    if (bounceTimer < 0.0f)
    {
      DelayedBounce();
    }
  }

  void DelayedBounce()
  {
    bounceTimer = bounceSpeed + 0.1f;
    Vector3 scaleTo = originScale + originScale * bounceAmt;
    scaleLT = LeanTween.scale(gameObject, scaleTo, bounceSpeed).setEase(LeanTweenType.easeInOutQuad);
    LeanTween.scale(cloudShadow, scaleTo, bounceSpeed).setEase(LeanTweenType.easeInOutQuad);

    bounceAmt = -bounceAmt;
  }

  void CheckOutOfBounds()
  {
    if (transform.position.x >= xLimit)
    {
      // CANCEL ALL IS SO DANGEROUS
      //LeanTween.cancelAll();
      GameObject.Find("Background").GetComponent<BackgroundManager>().RemoveCloud(gameObject);
      Destroy(cloudShadow);
      Destroy(gameObject);
    }
  }

  void UpdateMovement()
  {
    Vector3 transAmt = Vector3.right * speed * Time.deltaTime;
    transform.Translate(transAmt);
    cloudShadow.transform.Translate(transAmt);
  }

  public void SpawnShadow(GameObject shadowObj, float offsetFrom)
  {
    float totalOffset = offsetFrom - transform.position.y;
    cloudShadow = Instantiate(shadowObj);
    cloudShadow.transform.position = new Vector3(transform.position.x, offsetFrom + totalOffset, 0);
    cloudShadow.transform.localScale = transform.localScale;
  }
}
