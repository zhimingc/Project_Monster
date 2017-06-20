using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientScript : MonoBehaviour {

  public INGREDIENT_TYPE type;
  //public List<Connection> connects = new List<Connection>();

  public void InitializeIngredientScript(INGREDIENT_TYPE _type, Vector3 _size)
  {
    type = _type;
    transform.localScale = _size;
    IngredientFactory.InitializeIngredient(gameObject, type);
  }

  // Use this for initialization
  void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
