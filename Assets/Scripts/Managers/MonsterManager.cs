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
  public bool isTimerOn;
};

[System.Serializable]
public class Request
{
  public Request()
  {
    ingredients = new List<INGREDIENT_TYPE>();
    sauce = SAUCE_TYPE.EMPTY;
  }

  public List<INGREDIENT_TYPE> ingredients;
  public SAUCE_TYPE sauce;
  public GRID_TYPE gridType;
};

public class MonsterManager : MonoBehaviour {

  public RequestParameters rp = new RequestParameters();
  public List<MonsterRequest> requestBoxes;
  public float maxTimer, minTimer;
  public int speedUpInterval;

  // Hack
  //public int score = 0;

  private float currentTimer;
  private GameObject timerDisplay;
  private Vector3 timerPos, timerScale;
  private IngredientManager ingredientMan;

  void Awake()
  {
    ingredientMan = GameObject.Find("ingredient_manager").GetComponent<IngredientManager>();
    timerDisplay = GameObject.Find("timer_bar");
    timerPos = timerDisplay.transform.position;
    timerScale = timerDisplay.transform.localScale;
  }

  // Use this for initialization
  void Start () {
    requestBoxes = new List<MonsterRequest>();
    requestBoxes.Add(GameObject.Find("monster_request_0").GetComponent<MonsterRequest>());
    requestBoxes.Add(GameObject.Find("monster_request_1").GetComponent<MonsterRequest>());
    requestBoxes.Add(GameObject.Find("monster_request_2").GetComponent<MonsterRequest>());

    // Init request boxes
    foreach (MonsterRequest box in requestBoxes)
    {
      Request req = GenerateRandomRequest();

      box.InitStack();
      box.SetRequest(req);
    }

    // Init request timer
    if (!rp.isTimerOn)
    {
      GameObject.Find("timer_text").GetComponent<Text>().enabled = false;
      timerDisplay.GetComponent<SpriteRenderer>().enabled = false;
    }
    ResetRequestTimer();
  }
	
	// Update is called once per frame
	void Update () {
    // Timer for requests
    if (rp.isTimerOn) UpdateRequestTimer();

    if (Input.GetKeyDown(KeyCode.R))
    {
      foreach (MonsterRequest box in requestBoxes)
      {
        Request req = GenerateRandomRequest();
        box.SetRequest(req);
      }
    }
	}

  void UpdateTimerText()
  {
    string timerString = currentTimer.ToString("0.0");
    GameObject.Find("timer_text").GetComponent<Text>().text = timerString;
  }


  public void ToggleMonsterRequests(bool flag)
  {
    foreach (MonsterRequest box in requestBoxes) box.gameObject.SetActive(flag);
  }

  public void CheckRequestsMet(List<List<GameObject>> grid)
  {
    Request req = requestBoxes[0].GetComponent<MonsterRequest>().request;

    for (int x = 0; x < grid.Count; ++x)
    {
      for (int y = 0; y < grid[x].Count; ++y)
      {
        GridScript gs = grid[x][y].GetComponent<GridScript>();

        // Check if grid request is met
        if (req.gridType != gs.gridType) continue;
        
        // Check if sauce request is met
        if (req.sauce != gs.sauceType) continue;
        
        // Check if ingredients request is met
        if (req.ingredients.Count != gs.ingredientStack.Count) continue;
        bool same = true;
        for (int i = 0; i < req.ingredients.Count; ++i)
        {
          if (req.ingredients[i] != gs.ingredientStack[i]) same = false;
        }
        if (!same) continue;

        // request has been met
        gs.ClearStack();
        AdvanceRequests();

        // Increase score
        GameManager.Instance.AddScore(1);

        // Continue recursively to check for any other completes
        CheckRequestsMet(grid);
        return;
      }
    }
  }

  void AdvanceRequests()
  {
    for (int i = 0; i < requestBoxes.Count - 1; ++i)
    {
      requestBoxes[i].SetRequest(requestBoxes[i + 1].request);
    }

    requestBoxes[requestBoxes.Count - 1].SetRequest(GenerateRandomRequest());

    // Reset the timer for next request
    ResetRequestTimer();
  }

  Request GenerateRandomRequest()
  {
    Request req = new Request();

    // Add bot bread
    req.ingredients.Add(INGREDIENT_TYPE.BREAD);

    // Add ingredients
    int numIngredients = Random.Range(rp.ingCountRange[0], rp.ingCountRange[1]);
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
    if (GameManager.Instance.dayMan.FutureShiftCheck(DAY_STATE.LUNCH))
    {
      req.sauce = (SAUCE_TYPE)Random.Range(0, (int)SAUCE_TYPE.NUM_SAUCE);
    }

    // Add grid type if it is dinner time
    if (GameManager.Instance.dayMan.FutureShiftCheck(DAY_STATE.DINNER))
    {
      req.gridType = (GRID_TYPE)Random.Range(0, (int)GRID_TYPE.NUM_GRID);
    }

    return req;
  }

  void ResetRequestTimer()
  {
    int score = GameManager.Instance.scoreMan.score;
    if (score > 0 && score % speedUpInterval == 0)
    {
      maxTimer -= 5.0f;
      maxTimer = Mathf.Max(maxTimer, minTimer);
    }
    currentTimer = maxTimer;

    // Reset timer display
    timerDisplay.transform.position = timerPos;
    timerDisplay.transform.localScale = timerScale;
  }

  void UpdateRequestTimer()
  {
    currentTimer -= Time.deltaTime;
    UpdateTimerText();

    // Player loses if they run out of time
    if (currentTimer <= 0.0f)
    {
      GameManager.Instance.SetGameState(GAME_STATE.LOSE);
    }
    else
    {
      // Update timer display
      float offsetAmt = (1 - (currentTimer / maxTimer)) * timerScale.y;
      timerDisplay.transform.position = timerPos - new Vector3(0, offsetAmt / 2.0f, 0);
      timerDisplay.transform.localScale = timerScale - new Vector3(0, offsetAmt, 0);
    }
  }
}
