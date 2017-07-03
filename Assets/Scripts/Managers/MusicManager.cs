using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour {

  public bool isMute;
  public GameObject bgmObj;

  void Start()
  {
    isMute = false;
  }

  // Use this for initialization
  public void Init() {
    bgmObj = GameObject.Find("BGM");

    if (isMute && bgmObj)
      bgmObj.GetComponent<AudioSource>().Stop();
	}
	
  public bool ToggleMute()
  {
    return isMute = !isMute;
  }
}
