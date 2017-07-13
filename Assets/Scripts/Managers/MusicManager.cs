using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum BGM_CLIPS
{
  MAIN_MENU,
  LEVEL
}

public class MusicManager : MonoBehaviour {

  public bool isMute;
  public AudioSource bgmObj;
  public float bgmVolume;
  public AudioClip[] bgmClips;

  void Start()
  {
    // init audio clips
    bgmClips = new AudioClip[2];
    // main menu
    bgmClips[0] = Resources.Load<AudioClip>("Audio/BGM/Bubbly-happy");
    // level music
    bgmClips[1] = Resources.Load<AudioClip>("Audio/BGM/Photo-would-be-nice");

    // properties
    bgmVolume = 0.5f;

    isMute = false;
    GameObject tmp = new GameObject("BGM");
    bgmObj = tmp.gameObject.AddComponent<AudioSource>();
    bgmObj.loop = true;
    bgmObj.volume = bgmVolume;
    if (SceneManager.GetActiveScene().name == "vertical-phone")
    {
      ToggleBGM(BGM_CLIPS.LEVEL);
    }
    else if (SceneManager.GetActiveScene().name != "screen-splash")
    {
      ToggleBGM(BGM_CLIPS.MAIN_MENU);
    }

    DontDestroyOnLoad(bgmObj);

    // Don't play while in editor
    if (Application.platform == RuntimePlatform.WindowsEditor)
    {
      bgmObj.mute = true;
    }
  }

  // Use this for initialization
  public void Init() {
    if (isMute && bgmObj)
      bgmObj.Stop();
	}
	
  public void ToggleBGM(BGM_CLIPS clip)
  {
    if (bgmObj.clip != bgmClips[(int)clip])
    {
      bgmObj.clip = bgmClips[(int)clip];
      if (!isMute) bgmObj.Play();
    }
  }

  public bool ToggleMute()
  {
    isMute = !isMute;
    bgmObj.volume = isMute ? 0.0f : bgmVolume;
    return isMute;
  }
}
