using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockBehaviour : MonoBehaviour
{
  public bool beingDragged, isUsable;
  public Vector2[] layout;
  public Vector3 oldPos;
  public bool isReverseLayout;
  public List<GameObject> childrenObjs;

  protected PlayerScript playerScript;

  // UI for being dragged
  public Vector3 draggedScale, idleScale;
  protected LTDescr moveDescr;

  protected void Awake()
  {
    isReverseLayout = false;
    //isBigScale = false;
    playerScript = GameObject.Find("player").GetComponent<PlayerScript>();
    childrenObjs = new List<GameObject>();
  }

  // Use this for initialization
  protected void Start()
  {
    //isUsable = true;
    beingDragged = false;
    //draggedScale = transform.localScale;

    // initialize as idle scale
    ToggleUsability(isUsable);
    //transform.localScale = idleScale;
  }

  // Update is called once per frame
  protected void Update()
  {
    if (beingDragged)
    {
      DragUpdate();
    }
  }

  public void ToggleObjects(bool flag)
  {
    foreach (GameObject obj in childrenObjs)
      obj.GetComponent<SpriteRenderer>().enabled = flag;
  }

  public void SetIdleScale(float scaling)
  {
    idleScale = transform.localScale * scaling;
    draggedScale = transform.localScale;
  }

  public void SetIdleScale(Vector3 scaling)
  {
    idleScale = scaling;
  }

  protected void DragUpdate()
  {
    Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    mousePos.z = 0.0f;
    transform.position = mousePos;

    // Update border display 
    GetComponent<SpriteRenderer>().enabled = false;
  }

  public void StopDrag(bool deleteIngredient)
  {
    if (deleteIngredient == false)
    {
      GetComponent<BoxCollider2D>().enabled = true;
      //transform.position = oldPos;
      ReturnToOrigin();
      beingDragged = false;

      // return to idle scale when in queue 
      //transform.localScale = idleScale;
      ToggleScale();
      GetComponent<SpriteRenderer>().enabled = true;
    }
    else
    {
      Destroy(gameObject);
    }

  }

  protected void ReturnToOrigin()
  {
    if (moveDescr != null) LeanTween.cancel(moveDescr.id);
    moveDescr = LeanTween.move(gameObject, oldPos, 0.25f).setEase(LeanTweenType.easeOutQuad);
  }

  public void ToggleScale()
  {
    if (beingDragged)
    {
      //isBigScale = true;
      transform.localScale = idleScale;
      LeanTween.scale(gameObject, draggedScale, 0.25f).setEase(LeanTweenType.easeOutQuad);

      InteractSFX(true);
    }
    else
    {
      //isBigScale = false;
      transform.localScale = draggedScale;
      LeanTween.scale(gameObject, idleScale, 0.25f).setEase(LeanTweenType.easeOutQuad);

      InteractSFX(false);
    }
  }

  public void StartDrag()
  {
    beingDragged = true;
    GetComponent<BoxCollider2D>().enabled = false;

    // Update player script
    GameObject.Find("player").GetComponent<PlayerScript>().DragIngredientBlock(this);
    playerScript.SetPlayerState(PLAYER_STATE.DRAGGING);

    // Original scale when dragging
    ToggleScale();
  }

  public void InteractSFX(bool flag)
  {
    if (flag)
    {
      var clip = GameManager.Instance.SFX().GetAudio("upslide");
      clip.pitch = Random.Range(0.7f, 0.9f);
      GameManager.Instance.SFX().PlaySound(clip);
    }
    else
    {
      var clip = GameManager.Instance.SFX().GetAudio("downslide");
      clip.pitch = Random.Range(0.7f, 0.9f);
      GameManager.Instance.SFX().PlaySound(clip);
    }
  }

  public void ToggleUsability(bool flag)
  {
    isUsable = flag;
    if (isUsable == true)
    {
      GetComponent<BoxCollider2D>().enabled = true;
      transform.localScale = idleScale;
    }
    else
    {
      GetComponent<BoxCollider2D>().enabled = false;
      transform.localScale = idleScale * 0.75f;
    }
  }
}
