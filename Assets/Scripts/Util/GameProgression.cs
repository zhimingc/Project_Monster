using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// the unlocks for consecutive days
public enum DAYS
{
  BREAKFAST, // day 1
  LUNCH,     // lunch has been unlocked
  DINNER,    // dinner has been unlocked
}

public enum RANKS
{
  BIN_TOOL,
  //LUNCH,
  EATER_TOOL,
  //DINNER,
  IMPATIENT_MON,
  TURNTABLE_TOOL,
  PICKY_MON,
  NUM_MILESTONES
}

public static class GameProgression {
  public static int[] rankReq = new int[(int)RANKS.NUM_MILESTONES]
  {
    100,  // bin
    //400,  // lunch
    200,  // eater
    //1200, // dinner
    400, // impatient
    800, // turntable
    1500, // picky 
  };
	
}
