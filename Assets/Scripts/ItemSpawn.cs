using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSpawn : MonoBehaviour {

  public int itemNum;
  public GameObject itemBlockPrefab, itemPrefab;
  public ItemScript itemObj;
  public int curCooldown;
  public Text cooldownText;
  public ItemInfo info;

  private bool hasItem;

  public void RemoveItem()
  {
    curCooldown = GameManager.Instance.turnCounter;
    hasItem = false;
  }

  // Use this for initialization
  void Start () {
    GameManager.Instance.InitItem(this, itemNum);
    SpawnItem();
    curCooldown = 0;
  }
	
	// Update is called once per frame
	void Update () {
    UpdateCooldown();

  }

  void UpdateCooldown()
  {
    int curTurn = GameManager.Instance.turnCounter;

    if (!hasItem)
    {
      int cooldownTime = info.itemCooldown - (curTurn - curCooldown);
      cooldownText.text = cooldownTime.ToString();
      UpdateStateFeedback(false);  // feedback cannot use

      // able to use again
      if (curTurn - curCooldown >= info.itemCooldown)
      {
        hasItem = true;

        if (itemObj.isStaticUse) itemObj.ToggleCanUse(true);
        else SpawnItem();
      }
    }
    else
    {
      cooldownText.text = "";
      UpdateStateFeedback(true);  // feedback can use
    }
  }

  void UpdateStateFeedback(bool flag)
  {
    if (itemObj == null) return;

    if (!flag)
    {
      SpriteRenderer[] sprites = itemObj.GetComponentsInChildren<SpriteRenderer>();
      foreach (SpriteRenderer renderer in sprites)
      {
        Color curColor = renderer.color;
        curColor.a = 0.25f;
        renderer.color = curColor;

        // turn off animations
        renderer.GetComponent<Animator>().enabled = false;
      }
    }
    else
    {
      SpriteRenderer[] sprites = itemObj.GetComponentsInChildren<SpriteRenderer>();
      foreach (SpriteRenderer renderer in sprites)
      {
        Color curColor = renderer.color;
        curColor.a = 1.0f;
        renderer.color = curColor;

        renderer.GetComponent<Animator>().enabled = true;
      }
    }
  }

  void SpawnItem()
  {
    hasItem = true;
    int layout = 0;
    itemObj = Instantiate(itemBlockPrefab, transform).GetComponent<ItemScript>();
    BlockBehaviour blockScript = itemObj.GetComponent<BlockBehaviour>();

    switch (info.type)
    {
      case ITEM_TYPE.BIN:
        // Bins have no layout so it stays 0
        GenerateItemSingle(blockScript, transform);

        ObjectFactory.InitializeItem(blockScript.childenObjs[0], info.type);
        break;
      case ITEM_TYPE.EATER:
        // Get random layout 
        layout = Random.Range(0, ObjectFactory.blockLayouts.Length);
        blockScript.layout = ObjectFactory.blockLayouts[layout];

        GenerateItemBlock(blockScript, transform);
        foreach (GameObject child in blockScript.childenObjs)
        {
          ObjectFactory.InitializeItem(child, info.type);
        }
        break;
    }

    // Initialize item script
    //ItemScript itemScript = itemObj.GetComponent<ItemScript>();
    itemObj.SetItemType(info.type);
  }

  void GenerateItemSingle(BlockBehaviour parent, Transform t)
  {
    Vector2 ingredientSize = t.localScale;

    // Create core of ingredient 
    GameObject ingredientHolder = new GameObject("Holder");
    ingredientHolder.transform.position = t.position;
    ingredientHolder.transform.localScale = ingredientSize;
    ingredientHolder.transform.SetParent(parent.transform);

    GameObject ingredientObj = Instantiate(itemPrefab, ingredientHolder.transform);
    parent.childenObjs.Add(ingredientObj);
  }

  void GenerateItemBlock(BlockBehaviour parent, Transform t)
  {
    float maxLayout = ObjectFactory.maxLayout;
    Vector2 ingredientSize = t.localScale;
    Vector2 newScale = ingredientSize / maxLayout;

    // Create core of ingredient 
    GameObject ingredientHolder = new GameObject("Holder");
    ingredientHolder.transform.position = t.position + new Vector3(-newScale.x, newScale.y, 0.0f) / maxLayout;
    ingredientHolder.transform.localScale = newScale;
    ingredientHolder.transform.SetParent(parent.transform);

    GameObject ingredientObj = Instantiate(itemPrefab, ingredientHolder.transform);
    parent.childenObjs.Add(ingredientObj);

    // Create ingredient layout 
    foreach (Vector2 vec in parent.layout)
    {
      // Initialize new ingredient 
      Vector3 offset = Vector2.Scale(vec, newScale);

      GameObject ingredientHolder2 = new GameObject("Holder");
      ingredientHolder2.transform.position = ingredientObj.transform.position + offset;
      ingredientHolder2.transform.localScale = newScale;
      ingredientHolder2.transform.SetParent(parent.transform);

      // Create ingredients within connection 
      GameObject newIngredient = Instantiate(itemPrefab, ingredientHolder2.transform);
      parent.childenObjs.Add(newIngredient);
    }
  }
}
