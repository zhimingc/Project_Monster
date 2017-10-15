using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum COMBO_TYPE
{
  SINGLE,
  DOUBLE,
  TRIPLE,
  QUAD
}

public class ComboManager : MonoBehaviour {

  //public GameObject comboObj;
  public TextMesh combo_text, combo_inner, multi_text, multi_inner;
  public float openSpeed, openDuration, closeSpeed;
  public float[] comboMultiplier;

  //private Vector3 originalScale;
  private int comboCount, i = 0;

	// Use this for initialization
	void Start () {
    comboCount = 0;
    //originalScale = comboObj.transform.localScale;
    //comboObj.SetActive(true);
    //comboObj.transform.localScale = new Vector3(0, 0, 0);
  }

  // Update is called once per frame
  void Update()
  {
    if (Input.GetKeyDown(KeyCode.Space))
    {
      i = ++i % 3;
      //TriggerCombo((COMBO_TYPE)i);
    }
  }

  public int GetComboCount()
  {
    return comboCount;
  }

  public float GetComboMultiplier()
  {
    if (comboCount - 2 >= comboMultiplier.Length) return 1.0f;

    if (comboCount <= 1) return 1.0f;
    return comboMultiplier[comboCount - 2];
  }

  public void AddComboCount()
  {
    ++comboCount;
    //if (comboCount > 1)
    //{
    //  TriggerCombo((COMBO_TYPE)comboCount - 2);
    //}
  }

  public void ResetComboCount()
  {
    comboCount = 0;
  }

  public void TriggerCombo(GameObject comboObj)
  {
    if (comboCount < 2) return;

    TextMesh[] texts = comboObj.GetComponentsInChildren<TextMesh>();

    COMBO_TYPE type = (COMBO_TYPE)(comboCount - 1);
    switch (type)
    {
      case COMBO_TYPE.DOUBLE:
        texts[0].text = "double";
        texts[1].text = "double";
        texts[2].text = "x" + comboMultiplier[0].ToString("0.0");
        multi_inner.text = "x" + comboMultiplier[0].ToString();
        break;
      case COMBO_TYPE.TRIPLE:
        texts[0].text = "triple";
        texts[1].text = "triple";
        texts[2].text = "x" + comboMultiplier[1].ToString("0.0");
        multi_inner.text = "x" + comboMultiplier[1].ToString();
        break;
      case COMBO_TYPE.QUAD:
        texts[0].text = "quad";
        texts[1].text = "quad";
        texts[2].text = "x" + comboMultiplier[2].ToString("0.0");
        multi_inner.text = "x" + comboMultiplier[2].ToString();
        break;
    }

    LeanTween.cancel(comboObj);
    //LeanTween.cancel(gameObject);

    // animate cam
    GameFeel.ZoomCamera(gameObject, -0.1f, 0.05f, 0.25f, 0.25f);
    GameFeel.ShakeCameraRandom(0.1f, -0.1f, 12, 0.25f);

    // animate combo sign
    Vector3 originalScale = comboObj.transform.localScale;
    originalScale.y = originalScale.x + 0.01f;
    comboObj.gameObject.SetActive(true);

    comboObj.transform.localScale = originalScale - new Vector3(0, originalScale.y, 0);
    LeanTween.scaleY(comboObj, originalScale.y, openSpeed).setEase(LeanTweenType.easeInOutQuad);
    //comboObj.GetComponentInChildren<ParticleSystem>().Play();

    LeanTween.delayedCall(gameObject, openSpeed + openDuration, () =>
    {
      LeanTween.scaleY(comboObj, 0.0f, closeSpeed).setEase(LeanTweenType.easeInOutQuad);
    });
  }
}
