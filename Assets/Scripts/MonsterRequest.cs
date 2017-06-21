using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterRequest : MonoBehaviour {

  public GameObject ingredientObj;
  public Request request;

  private List<GameObject> ingredientStackObjs;

  // Use this for initialization
  void Start () {

  }
	
	// Update is called once per frame
	void Update () {
		
	}

  public void SetRequest(Request req)
  {
    request = req;

    // reset ingredient stack obj
    for (int i = 0; i < ingredientStackObjs.Count; ++i)
    {
      ingredientStackObjs[i].GetComponent<SpriteRenderer>().color = Color.grey;
    }

    // Set the display according to ingredient
    for (int i = 0; i < req.ingredients.Count; ++i)
    {
      IngredientFactory.InitializeIngredient(ingredientStackObjs[i], req.ingredients[i]);
    }

    // Set display according to sauce type
    IngredientFactory.InitializeSauce(gameObject, req.sauce);
  }

  public void InitStack()
  {
    ingredientStackObjs = new List<GameObject>();

    for (int i = 0; i < 4; ++i)
    {
      GameObject sideObj = Instantiate(ingredientObj, transform.position, Quaternion.identity);
      Vector3 localScale = transform.localScale;
      localScale = Vector3.Scale(localScale, new Vector3(0.8f, 0.08f, 1.0f));
      sideObj.transform.localScale = localScale;
      sideObj.transform.position += new Vector3(0, -transform.localScale.y / 3.0f + i * localScale.y * 2.0f, 0);
      sideObj.transform.SetParent(transform);
      sideObj.GetComponent<SpriteRenderer>().color = Color.grey;

      ingredientStackObjs.Add(sideObj);
    }
  }


}
