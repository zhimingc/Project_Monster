using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TIME_OF_DAY
{
  MORNING,
  AFTERNOON,
  NIGHT
}

public class BackgroundManager : MonoBehaviour {

  // Time of day
  public TIME_OF_DAY timeOfDay;
  public TIME_OF_DAY overrideTime;

  // Cloud ambient parameters
  public GameObject cloudObj, cloudShadowObj;
  public float[] cloudTimerLimits;
  public float[] cloudPosLimits;  // xMin, xMax, yMin, yMax
  public float[] cloudSizeLimits; // Compute for X, then min to X for Y
  public float cloudShadowOffset; // Diff from groundY + offset
  public int numStartClouds;
  public float cloudSpeed;

  private float cloudTimer;
  private GameObject bgGround, bgSky, bgSun;
  private List<GameObject> clouds;
  private Vector3[] sunPositions;

  public string[] groundColors, skyColors, cloudColors, shadowColors;

  void Awake()
  {
    // wake up game manager
    GameManager.Instance.WakeUp();
  }

  // Use this for initialization
  void Start()
  {
    clouds = new List<GameObject>();

    // init colors
    groundColors = new string[3]
    {
      "#b2d395ff", "#d39c95ff", "#a495d3ff"
    };
    skyColors = new string[3]
    {
      "#cfe3c4ff", "#e3d3c4ff", "#c4c4e3ff"
    };
    cloudColors = new string[3]
    {
      "#feeeeeff", "#eefef4ff", "#eef0feff"
    };
    shadowColors = new string[3]
    {
      "#95c36dff", "#b97779ff", "#826dc3ff"
    };

    // Get bg objects
    bgGround = GameObject.Find("bg_ground");
    bgSky = GameObject.Find("bg_sky");
    bgSun = GameObject.Find("bg_sun");

    // positions for the sun
    sunPositions = new Vector3[3];
    sunPositions[0] = new Vector3(-4, 7, 0);
    sunPositions[1] = new Vector3(0, 8.5f, 0);
    sunPositions[2] = new Vector3(4, 7, 0);

    ChangeTimeState((int)overrideTime);
    SpawnStartClouds();
  }
	
  void SpawnStartClouds()
  {
    foreach (GameObject obj in clouds)
    {
      obj.GetComponent<CloudScript>().DestroyAll();
    }
    clouds.Clear();

    int tmpNum = numStartClouds;
    while (--tmpNum >= 0)
    {
      float xPos = Random.Range(cloudPosLimits[0], cloudPosLimits[1] - 2.0f);
      SpawnCloud(xPos);
    }
  }

	// Update is called once per frame
	void Update () {
		//if (Input.GetKeyDown(KeyCode.C))
  //  {
  //    SpawnCloud();
  //  }

    if (Input.GetKeyDown(KeyCode.C))
    {
      SpawnStartClouds();
    }

    UpdateCloudSpawner();
  }

  public void ChangeTimeState(int newState)
  {
    timeOfDay = (TIME_OF_DAY) newState;
    if (timeOfDay >= TIME_OF_DAY.NIGHT) timeOfDay = TIME_OF_DAY.NIGHT;

    UpdateBGProperties();
    UpdateAllClouds();
  }

  public void RemoveCloud(GameObject cloudToRemove)
  {
    clouds.Remove(cloudToRemove);
  }

  void UpdateCloudSpawner()
  {
    cloudTimer -= Time.deltaTime;
    if (cloudTimer <= 0.0f)
    {
      SpawnCloud();
      cloudTimer = Random.Range(cloudTimerLimits[0], cloudTimerLimits[1]);
    }
  }

  GameObject SpawnCloud()
  {
    return SpawnCloud(cloudPosLimits[0]);
  }

  GameObject SpawnCloud(float xPos)
  {
    GameObject obj = Instantiate(cloudObj);

    // Random pos
    float randY = Random.Range(cloudPosLimits[2], cloudPosLimits[3]);
    obj.transform.position = new Vector3(xPos, randY, 0);

    // Random size
    float randXScale = Random.Range(cloudSizeLimits[0], cloudSizeLimits[1]);
    float randYScale = Random.Range(cloudSizeLimits[0], randXScale);
    obj.transform.localScale = new Vector3(randXScale, randYScale, 1.0f);

    // Cloud behaviour
    CloudScript cloud = obj.GetComponent<CloudScript>();
    cloud.SpawnShadow(cloudShadowObj, transform.position.y + cloudShadowOffset);
    cloud.xLimit = cloudPosLimits[1];
    cloud.speed = cloudSpeed;

    // Add to list
    clouds.Add(obj);

    return obj;
  }

  public void UpdateSunPosition(float progress)
  {
    Vector3 toMove = new Vector3();

    switch (timeOfDay)
    {
      case TIME_OF_DAY.MORNING:
        toMove = progress * (sunPositions[1] - sunPositions[0]);
        LeanTween.move(bgSun, sunPositions[0] + toMove, 0.5f);
        break;
      case TIME_OF_DAY.AFTERNOON:
        toMove = progress * (sunPositions[2] - sunPositions[1]);
        LeanTween.move(bgSun, sunPositions[1] + toMove, 0.5f);
        break;
      case TIME_OF_DAY.NIGHT:
        //toMove = progress * (sunPositions[2] - sunPositions[1]);
        //LeanTween.move(bgSun, sunPositions[1] + toMove, 0.5f);
        break;
    }
  }

  void UpdateBGProperties()
  {
    Utility.SetColorFromHex(bgGround, groundColors[(int)timeOfDay]);
    Utility.SetColorFromHex(bgSky, skyColors[(int)timeOfDay]);
    Utility.SetColorFromHex(bgSun, cloudColors[(int)timeOfDay]);

    UpdateCloudColor(cloudObj, cloudShadowObj);
  }

  void UpdateAllClouds()
  {
    foreach (GameObject cloud in clouds)
    {
      GameObject shadow = cloud.GetComponent<CloudScript>().cloudShadow;
      UpdateCloudColor(cloud, shadow);
    }
  }

  public void UpdateCloudColor(GameObject cloud, GameObject shadow)
  {
    Utility.SetColorFromHex(cloud, cloudColors[(int)timeOfDay]);
    Utility.SetColorFromHex(shadow, shadowColors[(int)timeOfDay]);
  }

  public void ChangeSignColors(GameObject sign, DAY_STATE state)
  {
    Image[] backings = sign.GetComponentsInChildren<Image>();
    Text text = sign.GetComponentInChildren<Text>();
    ButtonBehaviour[] buttons = sign.GetComponentsInChildren<ButtonBehaviour>();
    text.color = Utility.GetColorFromHex(shadowColors[(int)state]);
    backings[0].color = Utility.GetColorFromHex(shadowColors[(int)state]);
    backings[1].color = Utility.GetColorFromHex(skyColors[(int)state]);
    foreach(ButtonBehaviour button in buttons)
    {
      button.ChangeButtonColors((int)state, skyColors, shadowColors);
    }
  }
}
