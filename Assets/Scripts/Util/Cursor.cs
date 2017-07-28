using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cursor : MonoBehaviour {

  public GameObject clickParticles;

	// Use this for initialization
	void Start () {
    clickParticles = Instantiate(Resources.Load<GameObject>("Prefabs/Particles/click_particles"));
    DontDestroyOnLoad(clickParticles);
  }
	
	// Update is called once per frame
	void Update () {
		if (InputMan.OnDown())
    {
      Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
      mousePos.z = -5.0f;
      clickParticles.transform.position = mousePos;
      clickParticles.GetComponent<ParticleSystem>().Play();
    }
	}
}
