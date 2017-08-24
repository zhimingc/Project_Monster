using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioProps
{
  public AudioProps()
  {}

  public AudioProps(AudioClip c)
  {
    clip = c;
    pitch = 1.0f;
    vol = 1.0f;
  }

  public AudioClip clip;
  public float pitch;
  public float vol;
};

public class SFXManager : MonoBehaviour{

  public Dictionary<string, AudioClip> soundLib;
  List<AudioSource> audioSources;
  public bool isMute;

  void Awake()
  {
    isMute = false;
    audioSources = new List<AudioSource>();
    soundLib = new Dictionary<string, AudioClip>();
    soundLib.Add("nom", Resources.Load<AudioClip>("Audio/SFX/nom_1.0"));
    soundLib.Add("bite", Resources.Load<AudioClip>("Audio/SFX/bite_cartoon_1.1"));
    soundLib.Add("downslide", Resources.Load<AudioClip>("Audio/SFX/downslide_C_benboncan"));
    soundLib.Add("upslide", Resources.Load<AudioClip>("Audio/SFX/upslide_C_benboncan"));
    soundLib.Add("thud1", Resources.Load<AudioClip>("Audio/SFX/thud1_C_zerolagtime"));
    soundLib.Add("thud2", Resources.Load<AudioClip>("Audio/SFX/thud2_C_zerolagtime"));
    soundLib.Add("thud3", Resources.Load<AudioClip>("Audio/SFX/thud3_C_zerolagtime"));
    soundLib.Add("trash", Resources.Load<AudioClip>("Audio/SFX/trashfall"));
    soundLib.Add("splat", Resources.Load<AudioClip>("Audio/SFX/splat"));
    soundLib.Add("boom1", Resources.Load<AudioClip>("Audio/SFX/boom_C_group1"));
    soundLib.Add("boom2", Resources.Load<AudioClip>("Audio/SFX/boom_C_themfish"));
    soundLib.Add("good", Resources.Load<AudioClip>("Audio/SFX/good"));
    soundLib.Add("portal_in", Resources.Load<AudioClip>("Audio/SFX/portal_in"));
    soundLib.Add("portal_out", Resources.Load<AudioClip>("Audio/SFX/portal_out"));
  }

  public bool ToggleMute()
  {
    isMute = !isMute;

    if (isMute)
    {
      foreach (AudioSource src in audioSources)
      {
        src.Stop();
      }
    }

    return isMute;
  }

  public AudioProps GetAudio(string name)
  {
    if (soundLib.ContainsKey(name) == false)
    {
      print("Sound library does not contain " + name + ".");
    }

    return new AudioProps(soundLib[name]);
  }

  public void PlaySoundWithPitch(string name, float min, float max)
  {
    var clip = GetAudio(name);
    clip.pitch = Random.Range(min, max);
    GameManager.Instance.SFX().PlaySound(clip);
  }

  public void PlaySound(string name, float vol = 1.0f)
  {
    if (soundLib.ContainsKey(name) == false)
    {
      print("Sound library does not contain " + name + ".");
    }

    AudioProps props = new AudioProps(soundLib[name]);
    props.vol = vol;
    PlaySound(props);
  }

  public void PlaySound(AudioProps props)
  {
    if (isMute) return;

    // Look for a free audio source
    foreach (AudioSource source in audioSources)
    {
      if (source.isPlaying) continue;
      ApplyProperties(source, props);
      source.volume = props.vol;
      source.Play();
      return;
    }

    // No free audio sources; create and use a new one
    AudioSource src = gameObject.AddComponent<AudioSource>();
    ApplyProperties(src, props);
    src.volume = props.vol;
    src.Play();
    audioSources.Add(src);
  }


// Private 
  void ApplyProperties(AudioSource src, AudioProps props)
  {
    src.clip = props.clip;
    src.pitch = props.pitch;
  }

}
