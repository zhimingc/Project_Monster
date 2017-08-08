using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ConsecutiveManager : MonoBehaviour {

  int currentChangeIndex;

	// Use this for initialization
	void Start () {
    currentChangeIndex = 0;

  }
	
	// Update is called once per frame
	void Update () {
		
	}

  public void ApplyNewDayChanges(int dayIndex)
  {
    GameManager.Instance.gameData.newMonsterIndex = -1;

    if (currentChangeIndex == dayIndex) return;
    currentChangeIndex = dayIndex;

    float[] pop_monsters = GameManager.Instance.gameData.pop_monsters;
    int FIRST_TIME_PERCENTAGE = 33;

    switch (dayIndex)
    {
      case 1:  // add timed monsters
        pop_monsters[0] -= FIRST_TIME_PERCENTAGE;
        pop_monsters[1] = FIRST_TIME_PERCENTAGE;
        GameManager.Instance.gameData.newMonsterIndex = 1;
        break;
      case 2:  // add picky monsters
        pop_monsters[0] -= FIRST_TIME_PERCENTAGE;
        pop_monsters[2] = FIRST_TIME_PERCENTAGE;
        GameManager.Instance.gameData.newMonsterIndex = 2;
        break;
      case 3:  // add third ingredient
        GameManager.Instance.gameData.num_ingredients = 2;
        break;
    }

    // update game data
    GameManager.Instance.gameData.pop_monsters = pop_monsters;
  }
}
