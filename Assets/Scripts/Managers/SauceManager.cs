using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SauceManager : MonoBehaviour {

  public int maxCooldown;
  public GameObject[] sauces;

  // Use this for initialization
  void Start() {
    //foreach (GameObject sauce in sauces) sauce.SetActive(false);

  }
	
  public void ActivateSauces()
  {
    float lagTimer = 0.0f;
    foreach(GameObject sauce in sauces)
    {
      LeanTween.delayedCall(lagTimer, () =>
      {
        sauce.SetActive(true);
        sauce.GetComponentInChildren<ParticleSystem>().Play();
        GameFeel.ShakeCameraRandom(new Vector3(0.05f, 0.05f, 0.0f), new Vector3(-0.05f, -0.05f, 0.0f), 4, 0.2f);

        // play sfx
        GameManager.Instance.SFX().PlaySoundWithPitch("boom1", 0.9f, 1.1f);
      });
      lagTimer += 0.5f;
    }
  }

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.A))
    {
      ActivateSauces();
    }
	}
}
