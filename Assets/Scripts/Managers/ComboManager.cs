using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum COMBO_TYPE
{
  DOUBLE,
  TRIPLE,
  QUAD
}

public class ComboManager : MonoBehaviour {

  public GameObject comboObj;
  public TextMesh combo_text, combo_inner, multi_text, multi_inner;
  public float openSpeed, openDuration, closeSpeed;
  public float[] comboMultiplier;

  private Vector3 originalScale;
  private int comboCount, i = 0;

	// Use this for initialization
	void Start () {
    comboCount = 0;
    originalScale = comboObj.transform.localScale;
    comboObj.SetActive(true);
    comboObj.transform.localScale = new Vector3(0, 0, 0);
  }

  // Update is called once per frame
  void Update()
  {
    if (Input.GetKeyDown(KeyCode.Space))
    {
      i = ++i % 3;
      TriggerCombo((COMBO_TYPE)i);
    }
  }

  public int GetComboCount()
  {
    return comboCount;
  }

  public float GetComboMultiplier()
  {
    if (comboCount <= 1) return 1.0f;
    return comboMultiplier[comboCount - 2];
  }

  public void AddComboCount()
  {
    ++comboCount;
    if (comboCount > 1)
    {
      TriggerCombo((COMBO_TYPE)comboCount - 2);
    }
  }

  public void ResetComboCount()
  {
    comboCount = 0;
  }

  void TriggerCombo(COMBO_TYPE type)
  {
    switch (type)
    {
      case COMBO_TYPE.DOUBLE:
        combo_text.text = "double!";
        combo_inner.text = "double!";
        multi_text.text = "x1.5";
        multi_inner.text = "x1.5";
        break;
      case COMBO_TYPE.TRIPLE:
        combo_text.text = "triple!";
        combo_inner.text = "triple!";
        multi_text.text = "x2.0";
        multi_inner.text = "x2.0";
        break;
      case COMBO_TYPE.QUAD:
        combo_text.text = "quad!";
        combo_inner.text = "quad!";
        multi_text.text = "x3.0";
        multi_inner.text = "x3.0";
        break;
    }

    LeanTween.cancel(comboObj);
    LeanTween.cancel(gameObject);

    // animate cam
    GameFeel.ZoomCamera(gameObject, -0.1f, 0.05f, 0.25f, 0.25f);
    GameFeel.ShakeCameraRandom(0.1f, -0.1f, 12, 0.25f);

    // animate combo sign
    comboObj.transform.localScale = originalScale - new Vector3(0, originalScale.y, 0);
    LeanTween.scaleY(comboObj, originalScale.y, openSpeed).setEase(LeanTweenType.easeInOutQuad);
    comboObj.GetComponentInChildren<ParticleSystem>().Play();

    LeanTween.delayedCall(gameObject, openSpeed + openDuration, () =>
    {
      LeanTween.scaleY(comboObj, 0.0f, closeSpeed).setEase(LeanTweenType.easeInOutQuad);
    });
  }
}
