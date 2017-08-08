using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SauceScript : MonoBehaviour {

  public bool isCoolingDown;
  public SAUCE_TYPE sauceType;
  public GameObject sauceObj;
  public int curCooldown;
  public Text cooldownText;

  private int maxCooldown;
  private IngredientManager ingredientMan;
  private PlayerScript playerScript;

  // Use this for initialization
  void Start()
  {
    playerScript = GameObject.Find("player").GetComponent<PlayerScript>();
    ingredientMan = GameObject.Find("ingredient_manager").GetComponent<IngredientManager>();
    maxCooldown = GetComponentInParent<SauceManager>().maxCooldown;
    isCoolingDown = false;

    cooldownText.enabled = false;
    GenerateSauce();
    UpdateStateFeedback();
  }
	
	// Update is called once per frame
	void Update () {
    UpdateCooldown();
  }

  public void SauceMouseUp()
  {
    //if (Input.GetMouseButtonUp(0) && playerScript.hoveredGrid != null)
    if (InputMan.OnUp() && playerScript.hoveredGrid != null)
    {
      if (GameManager.Instance.dayMan.toggleTimedShiftFeature)
      {
        GenerateSauce();
      }
      else
      {
        GameManager.Instance.IncrementTurnCounter();

        // turn off cooldown if shifts are timed
        isCoolingDown = true;

        // Set sauce cooldown
        curCooldown = GameManager.Instance.turnCounter;
      }

    }
  }

  void OnMouseOver()
  {
    OnTouchStay();
  }

  public void OnTouchStay()
  {
    //if (Input.GetMouseButtonDown(0) &&
    if (InputMan.OnDown() && playerScript.playerState == PLAYER_STATE.IDLE)
    {
      sauceObj.GetComponent<IngredientBlock>().StartDrag();
      // Set delegate to determine mouse up behaviour
      playerScript.SetMouseUpDel(gameObject, SauceMouseUp);
    }
  }

  void UpdateCooldown()
  {
    if (GameManager.Instance.dayMan.toggleTimedShiftFeature)
    {
      sauceObj.GetComponent<BoxCollider2D>().enabled = false;
      return;
    }

    // THIS PIECE OF CODE IS DOING THE JOB OF MOUSEUPEVENT
    if (!isCoolingDown && sauceObj == null)
    {
      GameManager.Instance.IncrementTurnCounter();
      isCoolingDown = true;
      curCooldown = GameManager.Instance.turnCounter;
      cooldownText.enabled = true;
    }

    if (isCoolingDown)
    {
      int curTurn = GameManager.Instance.turnCounter;
      int cooldownTime = maxCooldown - (curTurn - curCooldown);
      cooldownText.text = cooldownTime.ToString();

      if (curTurn - curCooldown >= maxCooldown)
      {
        isCoolingDown = false;
        GenerateSauce();
        UpdateStateFeedback();
      }
    }
    else
    {
      cooldownText.text = "";
    }
  }

  void UpdateStateFeedback()
  {
    if (isCoolingDown)
    {
      GetComponent<SpriteRenderer>().color = Color.red;
    }
    else
    {
      //Color sauceColor = sauceObj.GetComponentsInChildren<SpriteRenderer>()[1].color;
      //GetComponent<SpriteRenderer>().color = sauceColor;
      //sauceObj.GetComponent<SpriteRenderer>().color = sauceColor;
    }
  }

  void GenerateSauce()
  {
    // Get random layout 
    int layout = ingredientMan.GenerateLayout();
    sauceObj = ingredientMan.GenerateIngredient(INGREDIENT_TYPE.SAUCE, sauceType, layout, transform);
    sauceObj.transform.position = transform.position;
    sauceObj.GetComponent<IngredientBlock>().SetIdleScale(new Vector3(1.0f, 1.0f, 1.0f));
    sauceObj.GetComponent<IngredientBlock>().oldPos = transform.position;
    sauceObj.GetComponent<IngredientBlock>().isUsable = true;
    sauceObj.transform.parent = transform;
    sauceObj.GetComponent<SpriteRenderer>().enabled = false;

    // color grid
    ObjectFactory.InitializeSaucePlate(sauceObj, sauceType);
  }

  
}
