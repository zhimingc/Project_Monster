using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

  // Use this for initialization
  void Start()
  {
    clouds = new List<GameObject>();

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
    while (--numStartClouds >= 0)
    {
      float xPos = Random.Range(cloudPosLimits[0], cloudPosLimits[1] - 2.0f);
      SpawnCloud(xPos);
    }
  }

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.C))
    {
      SpawnCloud();
    }

    UpdateCloudSpawner();
  }

  public void ChangeTimeState(int newState)
  {
    timeOfDay = (TIME_OF_DAY) newState;

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
        bgSun.transform.position = sunPositions[0] + toMove;
        break;
      case TIME_OF_DAY.AFTERNOON:
        toMove = progress * (sunPositions[2] - sunPositions[1]);
        bgSun.transform.position = sunPositions[1] + toMove;
        break;
      //case TIME_OF_DAY.NIGHT:
      //  toMove = progress * (sunPositions[3] - sunPositions[2]);
      //  bgSun.transform.position = sunPositions[2] + toMove;
      //  break;
    }
  }

  void UpdateBGProperties()
  {
    switch(timeOfDay)
    {
      case TIME_OF_DAY.MORNING:
        //Utility.SetColorFromHex(bgGround, "#d3b295ff");
        Utility.SetColorFromHex(bgGround, "#b2d395ff");
        //Utility.SetColorFromHex(bgSky, "#e3dec4ff");
        Utility.SetColorFromHex(bgSky, "#cfe3c4ff");
        Utility.SetColorFromHex(bgSun, "#feeeeeff");

        // sun position
        bgSun.transform.position = sunPositions[0];
        break;
      case TIME_OF_DAY.AFTERNOON:
        Utility.SetColorFromHex(bgGround, "#d39c95ff");
        Utility.SetColorFromHex(bgSky, "#e3d3c4ff");
        Utility.SetColorFromHex(bgSun, "#eefef4ff");

        // sun position
        bgSun.transform.position = sunPositions[1];
        break;
      case TIME_OF_DAY.NIGHT:
        Utility.SetColorFromHex(bgGround, "#a495d3ff");
        Utility.SetColorFromHex(bgSky, "#c4c4e3ff");
        Utility.SetColorFromHex(bgSun, "#eef0feff");

        // sun position
        bgSun.transform.position = sunPositions[2];
        break;
    }

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
    switch (timeOfDay)
    {
      case TIME_OF_DAY.MORNING:
        Utility.SetColorFromHex(cloud, "#feeeeeff");
        //Utility.SetColorFromHex(shadow, "#c3956dff");
        Utility.SetColorFromHex(shadow, "#95c36dff");
        break;
      case TIME_OF_DAY.AFTERNOON:
        Utility.SetColorFromHex(cloud, "#eefef4ff");
        Utility.SetColorFromHex(shadow, "#b97779ff");
        break;
      case TIME_OF_DAY.NIGHT:
        Utility.SetColorFromHex(cloud, "#eef0feff");
        Utility.SetColorFromHex(shadow, "#826dc3ff");
        break;
    }
  }


}
