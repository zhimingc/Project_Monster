using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterRequest : MonoBehaviour {

  public GameObject ingredientObj;
  public List<INGREDIENT_TYPE> request;

  private List<GameObject> ingredientStackObjs;

  // Use this for initialization
  void Start () {

  }
	
	// Update is called once per frame
	void Update () {
		
	}

  public void SetRequest(List<INGREDIENT_TYPE> req)
  {
    request = req;

    // reset ingredient stack obj
    for (int i = 0; i < ingredientStackObjs.Count; ++i)
    {
      ingredientStackObjs[i].GetComponent<SpriteRenderer>().color = Color.grey;
    }

    // Set the display according to ingredient
    for (int i = 0; i < req.Count; ++i)
    {
      IngredientFactory.InitializeIngredient(ingredientStackObjs[i], req[i]);
    }
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
