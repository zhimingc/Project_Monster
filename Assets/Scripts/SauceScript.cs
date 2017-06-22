using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SauceScript : MonoBehaviour {

  public SAUCE_TYPE sauceType;
  public GameObject sauceObj;
  public int maxCooldown, curCooldown;
  public Text cooldownText;

  private IngredientManager ingredientMan;

	// Use this for initialization
	void Start () {
    ingredientMan = GameObject.Find("ingredient_manager").GetComponent<IngredientManager>();
    GenerateSauce();
  }
	
	// Update is called once per frame
	void Update () {
		
	}

  void GenerateSauce()
  {
    // Get random layout 
    int layout = ingredientMan.GenerateLayout();
    sauceObj = ingredientMan.GenerateIngredient(INGREDIENT_TYPE.SAUCE, sauceType, layout, transform);
    sauceObj.transform.position = transform.position;
  }

  
}
