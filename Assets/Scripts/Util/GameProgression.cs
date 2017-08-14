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
  //BIN_TOOL,
  //LUNCH,
  //EATER_TOOL,
  //DINNER,
  //IMPATIENT_MON,
  START_RANK,
  TURNTABLE_TOOL,
  //PICKY_MON,
  NUM_MILESTONES
}

public enum CONSEC_DAYS
{
  ZERO,
  TIMED,
  PICKY,
  THIRD_INGREDIENT,
  NUM_CONSEC
}

public static class GameProgression {
  public static int[] rankReq = new int[(int)RANKS.NUM_MILESTONES - 1]
  {
    //100,  // bin
    //400,  // lunch
    //300,  // eater
    //1200, // dinner
    //400, // impatient
    300, // turntable
    //1500, // picky 
  };


  public static string[] dayFlavText = new string[(int)CONSEC_DAYS.NUM_CONSEC]
  {
    "First day on the job",
    "Impatient lizards",
    "Picky plankton",
    "Eat your vegetables"
  };

  public static string[] eventFlavText = new string[(int)MONSTER_EVENT.NUM_EVENTS]
{
    "First day on the job",
    "feed 30 monsters!",
    "Main event",
    "feed monsters to add time",
};
}
