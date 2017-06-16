using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientManager : MonoBehaviour {

  public GameObject block, center, connector; 
  public List<GameObject> ingredientList; 
 
  private float spacing; 
  private Vector2 gridBlockSize; 
 
  // Ingredient block layouts 
  private int maxLayout = 2; 
  private Vector2[][] blockLayouts = new Vector2[][] 
  {
    new Vector2[] { },                      // single block
    new Vector2[] { new Vector2( 1, 0 ) },  // double horizontal 
    new Vector2[] { new Vector2( 0, -1 ) },  // double vertical 
  }; 
 
 
  // Use this for initialization 
  void Start () { 
    spacing = GameObject.Find("grid_manager").GetComponent<GridManager>().spacing;
    gridBlockSize = GameObject.Find("grid_manager").GetComponent<GridManager>().gridBlockSize;
 
  }
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Z)) 
    { 
      GameObject newIngredient = RandomizeIngredient(); 
      AddIngredientToList(newIngredient); 
    }
	}

  GameObject RandomizeIngredient() 
  { 
    // Get random type 
    INGREDIENT_TYPE type = (INGREDIENT_TYPE)Random.Range(0, (int)INGREDIENT_TYPE.NUM_INGREDIENTS); 
 
    // Get random layout 
    int layout = Random.Range(0, blockLayouts.Length); 
 
    // Generate ingredient 
    return GenerateIngredient(type, layout); 
  }

  void AddIngredientToList(GameObject ingredient) 
  { 
    // Offset existing ingredients 
    foreach (GameObject obj in ingredientList) 
    { 
      obj.transform.position += new Vector3(0, (-gridBlockSize.y - spacing) * maxLayout, 0); 
    } 
 
    // Add ingredient to list 
    ingredientList.Add(ingredient); 
  }

  GameObject GenerateIngredient(INGREDIENT_TYPE type, int layout) 
  { 
    GameObject parent = Instantiate(block); 
    IngredientBlock blockScript = parent.GetComponent<IngredientBlock>();
    blockScript.layout = blockLayouts[layout];

    Vector2 ingredientSize = gridBlockSize;
    Vector2 newScale = (ingredientSize + new Vector2(spacing, spacing)) * maxLayout; 
    parent.transform.localScale = newScale; 
    parent.transform.position = transform.position + new Vector3(newScale.x, -newScale.y, 0.0f) / (maxLayout * 2.0f); 
 
    // Create core of ingredient 
    GameObject ingredientObj = Instantiate(center, transform.position, Quaternion.identity); 
    Ingredient result = new Ingredient(type, ingredientObj, ingredientSize); 
    result.obj.transform.SetParent(parent.transform); 
    blockScript.AddIngredient(ingredientObj); 
 
    // Create ingredient layout 
    foreach (Vector2 vec in blockLayouts[layout]) 
    { 
      Connection con = new Connection(); 
 
      // Create ingredients within connection 
      GameObject newIngredient = Instantiate(center, transform.position, Quaternion.identity); 
      con.ingredient = new Ingredient[2] { result, new Ingredient(type, newIngredient, ingredientSize) }; 
 
      // Initialize new ingredient 
      Vector3 offset = Vector2.Scale(vec, ingredientSize + new Vector2(spacing, spacing)); 
      con.ingredient[1].obj.transform.position += offset; 
      con.ingredient[1].connects.Add(con); 
 
      result.connects.Add(con); 
      con.ingredient[1].obj.transform.SetParent(parent.transform); 
      blockScript.AddIngredient(newIngredient); 
    } 
 
    return parent; 
  }
}
