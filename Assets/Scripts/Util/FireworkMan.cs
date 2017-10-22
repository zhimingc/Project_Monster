using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireworkMan : MonoBehaviour {

  public ParticleSystem[] pSys;
  public Vector2[] posLimits;
  public MonsterReact[] monsAnim;

	// Use this for initialization
	void Start () {
    for(int i = 0; i < pSys.Length; ++i)
    {
      RecursiveFirework(i);
    }
    for (int i = 0; i < monsAnim.Length; ++i)
    {
      //RecursiveJump(i);
    }
  }
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown(KeyCode.Alpha1))
    {
      PlayFirework(0);
    }
    if (Input.GetKeyDown(KeyCode.Alpha2))
    {
      PlayFirework(1);
    }
    if (Input.GetKeyDown(KeyCode.Alpha3))
    {
      PlayFirework(2);
    }
    if (Input.GetKeyDown(KeyCode.Alpha4))
    {
      PlayFirework(3);
    }
    if (Input.GetKeyDown(KeyCode.Alpha5))
    {
      PlayFirework(4);
    }

  }

  void RecursiveJump(int num)
  {
    LeanTween.delayedCall(Random.Range(2.0f, 3.0f), () =>
    {
      monsAnim[num].MonsterJump();

      RecursiveJump(num);
    });
  }

  void RecursiveFirework(int num)
  {
    LeanTween.delayedCall(Random.Range(1.0f, 2.0f), () =>
    {
      PlayFirework(num);
      RecursiveFirework(num);
    });
  }

  void PlayFirework(int num)
  {
    ParticleSystem curParticle = pSys[num];
    curParticle.Play();
    curParticle.gameObject.transform.localPosition = GetRandomPos();

  }

  Vector3 GetRandomPos()
  {
    Vector3 randPos = new Vector3(0, 0, 0);
    randPos.x = Random.Range(posLimits[0].y, posLimits[0].x);
    randPos.y = Random.Range(posLimits[1].y, posLimits[1].x);
    return randPos;
  }
}
