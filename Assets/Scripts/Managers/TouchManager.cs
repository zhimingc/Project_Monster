using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TouchManager : MonoBehaviour
{
  //TODO convert physics -> physics2D and raycastHit -> raycastHit2D so that it works for 2Dcollider rather than mesh.
  public LayerMask touchInputMask;
  private List<GameObject> touchList = new List<GameObject>();
  private GameObject[] touchesOld;
  private RaycastHit hit;
  private Camera cam;

  void Awake()
  {
    GameManager.Instance.WakeUp();
    cam = Camera.main;
    hit = new RaycastHit();
  }

  // Update is called once per frame
  void Update()
  {
    if (Input.GetMouseButton(0) || Input.GetMouseButtonDown(0) || Input.GetMouseButtonUp(0))
    {
      touchesOld = new GameObject[touchList.Count];
      touchList.CopyTo(touchesOld);
      touchList.Clear();

      //Ray ray = cam.ScreenPointToRay(Input.mousePosition);
      Vector2 origin = cam.ScreenToWorldPoint(Input.mousePosition);
      RaycastHit2D hit2D = Physics2D.Raycast(origin, Vector2.zero, Mathf.Infinity, touchInputMask);

      if (hit2D.collider != null)
      {
        GameObject recipient = hit2D.transform.gameObject;
        touchList.Add(recipient);

        if (Input.GetMouseButton(0))
        {
          recipient.SendMessage("OnTouchStay", hit.point, SendMessageOptions.DontRequireReceiver);
        }

        if (Input.GetMouseButtonDown(0))
        {
          recipient.SendMessage("OnTouchDown", hit.point, SendMessageOptions.DontRequireReceiver);
        }

        if (Input.GetMouseButtonUp(0))
        {
          recipient.SendMessage("OnTouchUp", hit.point, SendMessageOptions.DontRequireReceiver);
        }
      }

      foreach (GameObject g in touchesOld)
      {
        if (!touchList.Contains(g))
        {
          g.SendMessage("OnTouchExit", hit.point, SendMessageOptions.DontRequireReceiver);
        }
      }
    }

    if (Input.touchCount > 0)
    {
      touchesOld = new GameObject[touchList.Count];
      touchList.CopyTo(touchesOld);
      touchList.Clear();

      foreach (Touch touch in Input.touches)
      {
        Ray ray = cam.ScreenPointToRay(touch.position);
        RaycastHit2D hit2D = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, touchInputMask);

        if (hit2D.collider != null)
        {
          GameObject recipient = hit2D.transform.gameObject;
          touchList.Add(recipient);

          switch (touch.phase)
          {
            case TouchPhase.Began:
              recipient.SendMessage("OnTouchDown", hit.point, SendMessageOptions.DontRequireReceiver);
              break;

            case TouchPhase.Ended:
              recipient.SendMessage("OnTouchUp", hit.point, SendMessageOptions.DontRequireReceiver);
              break;
             
            case TouchPhase.Stationary:
              recipient.SendMessage("OnTouchStay", hit.point, SendMessageOptions.DontRequireReceiver);
              break;
              
            case TouchPhase.Canceled:
              recipient.SendMessage("OnTouchExit", hit.point, SendMessageOptions.DontRequireReceiver);

              break;
          }
        }
      }

      foreach (GameObject g in touchesOld)
      {
        if (!touchList.Contains(g))
        {
          g.SendMessage("OnTouchExit", hit.point, SendMessageOptions.DontRequireReceiver);
        }
      }
    }
  }
}