using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterRequest : MonoBehaviour {

  public GameObject ingredientObj, speechBubble, monsterObj;
  public Request request;

  private List<GameObject> ingredientStackObjs;
  private Vector3 originScale;

  // Use this for initialization
  void Start () {
    originScale = transform.localScale;
  }
	
	// Update is called once per frame
	void Update () {
		
	}

  public void ToggleSpeechBubble(bool flag)
  {
    speechBubble.GetComponent<SpriteRenderer>().enabled = flag;
    if (flag == true)
    {
      transform.localScale = new Vector3(0, 0, 0);
      LeanTween.delayedCall(0.5f, () => { LeanTween.scale(gameObject, originScale, 0.5f); });
    }
  }

  public void SetRequest(Request req)
  {
    request = req;

    // reset ingredient stack obj
    for (int i = 0; i < ingredientStackObjs.Count; ++i)
    {
      Sprite sprite = Resources.Load<Sprite>("Sprites/ingredient_side");
      ingredientStackObjs[i].GetComponent<SpriteRenderer>().sprite = sprite;
    }

    // Set the display according to ingredient
    for (int i = 0; i < req.ingredients.Count; ++i)
    {
      IngredientFactory.InitializeIngredientSide(ingredientStackObjs[i], req.ingredients[i]);

      if (i != 0 && req.ingredients[i] == INGREDIENT_TYPE.BREAD)
      {
        Sprite sprite = Resources.Load<Sprite>("Sprites/bread_dark_side_top");
        ingredientStackObjs[i].GetComponent<SpriteRenderer>().sprite = sprite;
      }
    }

    // Set display according to sauce type
    IngredientFactory.InitializeSauce(gameObject, req.sauce);

    // Set display according to grid type
    IngredientFactory.InitializeGrid(gameObject, req.gridType);
  }

  public void InitStack()
  {
    ingredientStackObjs = new List<GameObject>();

    for (int i = 0; i < 4; ++i)
    {
      GameObject sideObj = Instantiate(ingredientObj, transform.position, Quaternion.identity);
      Vector3 localScale = transform.localScale;
      localScale = Vector3.Scale(localScale, new Vector3(0.4f, 0.5f, 1.0f));
      sideObj.transform.SetParent(transform);
      sideObj.transform.localScale = localScale;
      sideObj.transform.localPosition += new Vector3(0, -transform.localScale.y / 3.0f + i * localScale.y / 2.5f, 0);

      ingredientStackObjs.Add(sideObj);
    }
  }


}
