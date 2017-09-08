using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class RequestParameters
{
  public RequestParameters()
  {
    ingCountRange = new int[] { 1, 3 };
    topBreadProbability = 2;
  }

  public int[] ingCountRange;
  public int topBreadProbability;

  // Weighted distributions (out of a 100)
  //public int[] sauceDist = new int[] { 33, 33, 34 };
  public int[] sauceDist = new int[] { 50, 50 };
  public int[] waresDist = new int[] { 50, 50 };
  public int[] ingredientDist = new int[] { 50, 50 };
  //public int[] ingredientDist = new int[] { 33, 33, 33 };
  public int weightSwing = 10;  // swings the weight of the other distributions by this amt

  // Contract parameters
  public bool timerContractOn;
  public float timerMax;
};

[System.Serializable]
public class Request
{
  public Request()
  {
    ingredients = new List<INGREDIENT_TYPE>();
    sauce = SAUCE_TYPE.EMPTY;
  }

  public bool IsSameAs(Request lhs)
  {
    if (ingredients.Count != lhs.ingredients.Count) return false;
    if (sauce != lhs.sauce || gridType != lhs.gridType) return false;

    for (int i = 0; i < ingredients.Count; ++i)
    {
      if (ingredients[i] != lhs.ingredients[i])
        return false;
    }

    return true;
  }

  public List<INGREDIENT_TYPE> ingredients;
  public SAUCE_TYPE sauce;
  public GRID_TYPE gridType;
  public MONSTER_TYPE monsterType;
  public MonsterTypeParams typeParams;
  public Color chairColor;
};

// debug data
public class Debug_MonsterManager
{
  public bool force_nextMonster;
  public MONSTER_TYPE next_type;
}

public class MonsterManager : MonoBehaviour {

  public RequestParameters rp = new RequestParameters();
  public List<MonsterRequest> requestBoxes;
  public float maxTimer, minTimer;
  public int speedUpInterval;

  // controlled monster spawning
  public int garbageMonsterSpawnRate;
  public int monsterSpawnCounter;

  private float currentTimer;
  //private Vector3 timerPos, timerScale;
  private IngredientManager ingredientMan;
  private List<GameObject> reserveMonsters;

  // for debugging
  private Debug_MonsterManager debug_data;
  private int breaker = 0;

  void Awake()
  {
    debug_data = new Debug_MonsterManager();
    ingredientMan = GameObject.Find("ingredient_manager").GetComponent<IngredientManager>();
  }

  // Use this for initialization
  void Start () {
    // Get/Generate monster request prefabs
    requestBoxes = new List<MonsterRequest>();
    reserveMonsters = new List<GameObject>();
    requestBoxes.Add(GameObject.Find("monster_request_0").GetComponent<MonsterRequest>());
    requestBoxes.Add(GameObject.Find("monster_request_1").GetComponent<MonsterRequest>());
    requestBoxes.Add(GameObject.Find("monster_request_2").GetComponent<MonsterRequest>());
    GameObject monsterReq = Resources.Load<GameObject>("Prefabs/Gameplay/monster_holder");
    reserveMonsters.Add(Instantiate(monsterReq));
    reserveMonsters.Add(Instantiate(monsterReq));
    reserveMonsters.Add(Instantiate(monsterReq));
    foreach (GameObject mon in reserveMonsters) mon.GetComponentInChildren<MonsterAnimation>().Hide();

    // update random parameters
    UpdateMonsterRP();

    // reset monster popularity
    GameManager.Instance.gameData.pop_monsters = new float[(int)MONSTER_TYPE.NUM_TYPES];
    GameManager.Instance.gameData.pop_monsters[0] = 100.0f;

    // Init request boxes
    foreach (MonsterRequest box in requestBoxes)
    {
      Request req = GenerateRandomRequest();

      box.InitStack();
      box.SetRequest(req);

      // apply any grid effects
      GameManager.Instance.gridMan.MonsterAffectGrid(box.request);
    }

    monsterSpawnCounter = 0;
  }
	
	// Update is called once per frame
	void Update () {
    // debug testing
    if (Input.GetKeyDown(KeyCode.T))
    {
      debug_data.force_nextMonster = true;
      debug_data.next_type = MONSTER_TYPE.TIMED;
      ServeMonsterRequest(null, requestBoxes[0]);
    }
    if (Input.GetKeyDown(KeyCode.P))
    {
      debug_data.force_nextMonster = true;
      debug_data.next_type = MONSTER_TYPE.PICKY;
      ServeMonsterRequest(null, requestBoxes[0]);
    }
  }

  public void UpdateMonsterRP()
  {
    // update rp for contracts
    rp.timerContractOn = GameManager.Instance.CheckForContract(CONTRACT_TYPE.TIMER);
  }

  void UpdateTimerText()
  {
    string timerString = currentTimer.ToString("0.0");
    GameObject.Find("timer_text").GetComponent<Text>().text = timerString;
  }


  public void ToggleMonsterRequests(bool flag)
  {
    foreach (MonsterRequest box in requestBoxes)
    {
      box.monsterObj.SetActive(flag);
      box.gameObject.SetActive(flag);
    }
  }

  public void CheckRequestMetAll()
  {
    var grid = GameManager.Instance.gridMan.grid;

    // set all grids to cannot serve first
    for (int x = 0; x < grid.Count; ++x)
    {
      for (int y = 0; y < grid[x].Count; ++y)
      {
        GridScript gs = grid[x][y].GetComponent<GridScript>();
        gs.SetCanServe(false);
      }
    }

    foreach (MonsterRequest obj in requestBoxes)
    {
      CheckRequestMet(grid, obj);
    }
  }

  public void CheckRequestMet(List<List<GameObject>> grid, MonsterRequest reqBox)
  {
    Request req = reqBox.request;
    reqBox.monsterObj.GetComponent<MonsterBody>().ToggleFeedSign(false);
    //lhs.ingredients.Remove(INGREDIENT_TYPE.EATER);

    for (int x = 0; x < grid.Count; ++x)
    {
      for (int y = 0; y < grid[x].Count; ++y)
      {
        GridScript gs = grid[x][y].GetComponent<GridScript>();
        List<INGREDIENT_TYPE> gridIngredients = new List<INGREDIENT_TYPE>(gs.ingredientStack);
        gridIngredients.RemoveAll((INGREDIENT_TYPE type) => { return type == INGREDIENT_TYPE.EATER; });

        // Check specific monster requirements
        if (!SpecificMonsterCheck(req, gs)) continue;

        // Check if grid request is met
        if (req.gridType != gs.gridType) continue;
        
        // Check if sauce request is met
        if (req.sauce != gs.sauceType) continue;
        
        // Check if ingredients request is met
        if (req.ingredients.Count != gridIngredients.Count) continue;
        bool same = true;
        for (int i = 0; i < req.ingredients.Count; ++i)
        {
          if (req.ingredients[i] != gridIngredients[i]) same = false;
        }
        if (!same) continue;

        //ServeMonsterRequest(gs, reqBox);
        reqBox.monsterObj.GetComponent<MonsterBody>().ToggleFeedSign(true);
        gs.SetCanServe(true, reqBox, reqBox.request.chairColor);
      }
    }
  }

  // returns false if check fails
  bool SpecificMonsterCheck(Request monReq, GridScript gs)
  {
    switch (monReq.monsterType)
    {
      case MONSTER_TYPE.PICKY:
        if (monReq.typeParams.specificGrid != gs) return false;
        break;
    }

    return true;
  }

  public void ServeMonsterRequest(GridScript gs, MonsterRequest reqBox)
  {
    // update combo man with another combo
    GameManager.Instance.comboMan.AddComboCount();

    // apply any effects onto timer
    float timeAdded = 0.0f;
    GameManager.Instance.dayMan.OnServeTimeEffect(ref timeAdded);

    // Increase score
    int scoreAdded = GameManager.Instance.AddScore();

    // add number served and check for shift change
    GameManager.Instance.AddNumServed(1);

    // update popularity of monsters
    GameManager.Instance.UpdateMonsterPopularity();

    // // request has been met
    if (gs != null)
    {
      gs.TriggerServed(reqBox);

      // update score text in grid
      gs.TriggerScoreText(scoreAdded);
    }

    // feedback for request met
    reqBox.monsterFbScript.PlayServedFeedback(scoreAdded, timeAdded);
    GameFeel.ShakeCameraRandom(new Vector3(0.05f, 0.05f, 0.0f), new Vector3(-0.05f, -0.05f, 0.0f), 4, 0.2f);
    PlayEatingSound();

    // Animate monsters in/out
    reqBox.monsterObj.GetComponent<MonsterAnimation>().ForceMoveComplete();
    Vector3 monsPos = reqBox.monsterObj.GetComponent<MonsterAnimation>().origin;
    foreach (GameObject reserve in reserveMonsters)
    {
      MonsterAnimation anim = reserve.GetComponentInChildren<MonsterAnimation>();
      if (anim.isAnimating()) continue;
      anim.InitSprite(reqBox.request);
      anim.MoveOutFrom(monsPos);
      break;
    }

    //Vector3 moveIn = reqBox.transform.position;
    Vector3 moveIn = monsPos;
    moveIn.x = -7;
    reqBox.monsterObj.GetComponent<MonsterAnimation>().MoveInFrom(moveIn);

    // get the next request
    UpdateRequestMet(reqBox);

    // update serve ability of grids;
    CheckRequestMetAll();
  }

  public void AddSauceToAllRequests()
  {
    for (int i = 0; i < requestBoxes.Count; ++i)
    {
      Request req = requestBoxes[i].request;
      req.sauce = (SAUCE_TYPE)GenerateWithDist(rp.sauceDist);
      SwingWeights((int)req.sauce, rp.sauceDist);

      requestBoxes[i].SetRequest(req);
    }

  }

  void PlayEatingSound()
  {
    AudioProps clip = new AudioProps();

    clip = GameManager.Instance.SFX().GetAudio("nom");
    clip.pitch = Random.Range(0.7f, 0.9f);

    GameManager.Instance.SFX().PlaySound(clip);
  }

  void UpdateRequestMet(MonsterRequest box)
  {
    int index = requestBoxes.IndexOf(box);
    requestBoxes[index].request = new Request();

    // generate request
    ++monsterSpawnCounter;
    Request newReq = GenerateRandomRequest();

    // update monsters
    requestBoxes[index].SetRequest(newReq);

    // apply any grid effects
    newReq.chairColor = requestBoxes[index].request.chairColor;
    GameManager.Instance.gridMan.MonsterAffectGrid(newReq);
  }

  void GenMonsterType(out Request req)
  {
    req = new Request();

    // Generate monster type
    req.monsterType = RollMonsterType();

    if (debug_data.force_nextMonster)
    {
      req.monsterType = debug_data.next_type;
    }

    switch (req.monsterType)
    {
      case MONSTER_TYPE.TIMED:
        req.typeParams.maxTimer = rp.timerMax;
        req.typeParams.curTimer = rp.timerMax;
        break;
    }
  }

  MONSTER_TYPE RollMonsterType()
  {
    int roll = Random.Range(0, 100);
    MONSTER_TYPE ret = MONSTER_TYPE.NORMAL;
    float odds = 0;

    // hard spawn garbage monster
    if (monsterSpawnCounter > 0 &&
      monsterSpawnCounter % garbageMonsterSpawnRate == 0)
    {
      return MONSTER_TYPE.GARBAGE;
    }

    for (int i = 0; i < (int)MONSTER_TYPE.NUM_TYPES; ++i)
    {
      odds += GameManager.Instance.gameData.pop_monsters[i];
      if (roll <= odds)
      {
        ret = (MONSTER_TYPE)i;
        break;
      }
    }

    return ret;
  }
  

  Request GenerateRandomRequest()
  {
    Request req = new Request();

    // Determine monster type
    GenMonsterType(out req);

    if (req.monsterType == MONSTER_TYPE.GARBAGE)
    {
      return req;
    }

    // Add bot bread
    req.ingredients.Add(INGREDIENT_TYPE.BREAD);

    // Add ingredients
    //int numIngredients = Random.Range(rp.ingCountRange[0], rp.ingCountRange[1]);
    int numIngredients = GenerateWithDist(rp.ingredientDist);
    SwingWeights(numIngredients, rp.ingredientDist);
    numIngredients += 1;

    for (int i = 0; i < numIngredients; ++i)
    {
      // Start from 1 to exclude BREAD
      INGREDIENT_TYPE randIngredient = (INGREDIENT_TYPE)Random.Range(1, ingredientMan.numberOfIngredients);
      req.ingredients.Add(randIngredient);
    }

    // Add top bread by chance
    int addTopBread = Random.Range(0, rp.topBreadProbability);
    if (addTopBread == 0) req.ingredients.Add(INGREDIENT_TYPE.BREAD);

    // Add sauce if it is lunch or dinner time
    if (GameManager.Instance.dayMan.IsOrPastShift(DAY_STATE.LUNCH))
    {
      //req.sauce = (SAUCE_TYPE)Random.Range(0, (int)SAUCE_TYPE.NUM_SAUCE);
      req.sauce = (SAUCE_TYPE)GenerateWithDist(rp.sauceDist);
      SwingWeights((int)req.sauce, rp.sauceDist);
    }

    // Add grid type if it is dinner time
    if (GameManager.Instance.dayMan.IsOrPastShift(DAY_STATE.DINNER))
    {
      //req.gridType = (GRID_TYPE)Random.Range(0, (int)GRID_TYPE.NUM_GRID);
      req.gridType = (GRID_TYPE)GenerateWithDist(rp.waresDist);
      SwingWeights((int)req.gridType, rp.waresDist);
    }

    // Reroll if the request is the same as any existing request
    foreach (MonsterRequest existing in requestBoxes)
    {
      if (++breaker > 100) return req;

      //if (existing.request.IsSameAs(req))
      if (req.IsSameAs(existing.request))
      {
        req = GenerateRandomRequest();
        break;
      }
    }

    breaker = 0;
    return req;
  }

  // Swings weights depending on outcome
  void SwingWeights(int outcome, int[] dist)
  {
    int singleSwing = rp.weightSwing / (dist.Length - 1);

    // Decrease outcome change
    dist[outcome] -= rp.weightSwing;

    // Increate other distributions
    for (int i = 0; i < dist.Length; ++i)
    {
      if (i == outcome) continue;
      dist[i] += singleSwing;
    }
  }

  // Generates an index depending on given distribution (out of 100)
  int GenerateWithDist(int[] distribution)
  {
    int i = 0;
    int total = 0;
    int random = Random.Range(0, 100);

    for (; i < distribution.Length; ++i)
    {
      total += distribution[i];
      if (random < total) break;
    }

    return i;
  }

  void ResetRequestTimer()
  {
    int score = GameManager.Instance.scoreMan.numServed;
    if (score > 0 && score % speedUpInterval == 0)
    {
      maxTimer -= 5.0f;
      maxTimer = Mathf.Max(maxTimer, minTimer);
    }
    currentTimer = maxTimer;
  }

  // recall set request for any updates to the requests
  // e.g. picky monster updated by dinner
  public void UpdateMonsterRequests()
  {
    foreach(MonsterRequest req in requestBoxes)
    {
      req.UpdateRequest();
    }
  }
}
