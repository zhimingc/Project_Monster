using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterFeedback : MonoBehaviour {

  public GameObject scoreText, timeText;
  public GameObject particlesObj;
  public Vector3 origin;

  // Use this for initialization
  void Start()
  {
    timeText.SetActive(false);
    scoreText.SetActive(false);
    origin = transform.localPosition;
  }

  public void PlayParticles()
  {
    particlesObj.GetComponent<ParticleSystem>().Stop();
    particlesObj.GetComponent<ParticleSystem>().Play();
  }

  public void PlayServedFeedback(int score, float time)
  {
    LeanTween.cancel(gameObject);
    transform.localPosition = origin;
    scoreText.SetActive(true);
    timeText.SetActive(true);

    scoreText.GetComponent<TextMesh>().text = "$" + score.ToString();
    timeText.GetComponent<TextMesh>().text = "+" + time.ToString("0.0");

    LeanTween.moveLocalY(gameObject, origin.y + 0.5f, 1.5f);
    LeanTween.delayedCall(gameObject, 1.5f, () =>
    {
      timeText.SetActive(false);
    });
    LeanTween.delayedCall(gameObject, 1.5f, () =>
    {
      scoreText.SetActive(false);
    });

  }
}
