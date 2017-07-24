using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RANKS
{
  BIN_TOOL,
  LUNCH,
  EATER_TOOL,
  DINNER,
  IMPATIENT_MON,
  TURNTABLE_TOOL,
  PICKY_MON,
  NUM_MILESTONES
}

public static class GameProgression {
  public static int[] rankReq = new int[(int)RANKS.NUM_MILESTONES]
  {
    200,  // bin
    400,  // lunch
    800,  // eater
    1200, // dinner
    2000, // impatient
    3000, // turntable
    4500, // picky 
  };
	
}
