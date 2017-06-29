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

    GenerateSauce();
    isCoolingDown = false;
  }
	
	// Update is called once per frame
	void Update () {
    UpdateCooldown();
    UpdateStateFeedback();
  }

  public void SauceMouseUp()
  {
    if (!isCoolingDown && playerScript.playerState == PLAYER_STATE.DRAGGING)
    {
      //if (Input.GetMouseButtonUp(0) && playerScript.hoveredGrid != null)
      if (InputMan.OnUp() && playerScript.hoveredGrid != null)
        {
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
    if (InputMan.OnDown() &&
      !isCoolingDown && playerScript.playerState == PLAYER_STATE.IDLE)
    {
      sauceObj.GetComponent<IngredientBlock>().StartDrag();
      // Set delegate to determine mouse up behaviour
      playerScript.SetMouseUpDel(SauceMouseUp);
    }
  }

  void OnMouseExit()
  {
    OnTouchExit();
  }

  public void OnTouchExit()
  {
    if (!isCoolingDown && playerScript.playerState == PLAYER_STATE.IDLE)
    {
      playerScript.ResetMouseUpDel();
    }
  }

  void UpdateCooldown()
  {
    if (!isCoolingDown && sauceObj == null)
    {
      isCoolingDown = true;
      curCooldown = GameManager.Instance.turnCounter;
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
      GetComponent<SpriteRenderer>().color = Color.green;
      sauceObj.GetComponent<SpriteRenderer>().color = Color.green;
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
    sauceObj.transform.parent = transform;

    sauceObj.GetComponent<BoxCollider2D>().enabled = false;
  }

  
}
