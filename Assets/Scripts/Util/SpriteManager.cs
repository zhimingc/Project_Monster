using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteManager : MonoBehaviour {

  public List<List<Sprite>> monsterSprites;

  // Use this for initialization
  void Start () {
    monsterSprites = new List<List<Sprite>>();

    // basic
    monsterSprites.Add(new List<Sprite>());
    monsterSprites[0].Add(Resources.Load<Sprite>("Sprites/monster_basic_0"));
    monsterSprites[0].Add(Resources.Load<Sprite>("Sprites/monster_basic_1"));
    monsterSprites[0].Add(Resources.Load<Sprite>("Sprites/monster_basic_2"));
    monsterSprites[0].Add(Resources.Load<Sprite>("Sprites/monster_basic_3"));

    // impatient
    monsterSprites.Add(new List<Sprite>());
    monsterSprites[1].Add(Resources.Load<Sprite>("Sprites/monster_impatient_0"));
    monsterSprites[1].Add(Resources.Load<Sprite>("Sprites/monster_impatient_1"));
    monsterSprites[1].Add(Resources.Load<Sprite>("Sprites/monster_impatient_2"));
    monsterSprites[1].Add(Resources.Load<Sprite>("Sprites/monster_impatient_3"));

    // picky
    monsterSprites.Add(new List<Sprite>());
    monsterSprites[2].Add(Resources.Load<Sprite>("Sprites/monster_rude_0"));
    monsterSprites[2].Add(Resources.Load<Sprite>("Sprites/monster_rude_1"));
    monsterSprites[2].Add(Resources.Load<Sprite>("Sprites/monster_rude_2"));
    monsterSprites[2].Add(Resources.Load<Sprite>("Sprites/monster_rude_3"));

    // greedy
    monsterSprites.Add(new List<Sprite>());
    monsterSprites[3].Add(Resources.Load<Sprite>("Sprites/monster_greedy_0"));
    monsterSprites[3].Add(Resources.Load<Sprite>("Sprites/monster_greedy_1"));
    monsterSprites[3].Add(Resources.Load<Sprite>("Sprites/monster_greedy_2"));
    monsterSprites[3].Add(Resources.Load<Sprite>("Sprites/monster_greedy_3"));
  }

  // Update is called once per frame
  void Update () {
		
	}
}
