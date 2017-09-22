using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SocialPlatforms;
using GooglePlayGames;

public enum SOCIALBOARD
{
  EARNINGS,
  NUM_BOARDS
};

public class SocialManager : MonoBehaviour {

  // Social API stuff
  public ILeaderboard earningLeaderboard;

  private GameObject earningsText;
  private GameObject debugText;
  private string toDisplay = "";

  // Use this for initialization
  void Start () {
    earningLeaderboard = null;
#if UNITY_ANDROID

    // Activate the Google Play Games platform
    PlayGamesPlatform.Activate();

#endif

    Login();
    CreateLeaderboard();
    InitSocial();
  }

  // Login/authenticate local user
  void Login()
  {
    // This call needs to be made before we can proceed to other calls in the Social API
    Social.localUser.Authenticate(ProcessAuthentication);
  }

  // This function gets called when Authenticate completes
  // Note that if the operation is successful, Social.localUser will contain data from the server. 
  void ProcessAuthentication(bool success)
  {
    if (success)
    {
      Debug.Log("Authenticated");
      // debug
      toDisplay = "Username: " + Social.localUser.userName +
      "\nUser ID: " + Social.localUser.id +
      "\nIsUnderage: " + Social.localUser.underage + "\n";
    }
    else
    {
      toDisplay += "Failed to autheticate\n";
      Debug.Log("Failed to authenticate");
    }
  }

  // Create leaderboard
  void CreateLeaderboard()
  {
    string boardID = "";
#if UNITY_ANDROID
    // earnings leaderboards
    boardID = GPGLeaderboardManager.leaderboard_highest_earnings;

#elif UNITY_IOS
    // boardID = SOCIALBOARD.EARNINGS.ToString();

#endif

    // Create leaderboard instance
    earningLeaderboard = Social.CreateLeaderboard();
    earningLeaderboard.id = boardID;
  }

  // Load leaderboard
  public void LoadLeaderboard()
  {
    if (earningLeaderboard == null) return;
    // After loading scores how are the scores stored?

    // Load scores
    earningLeaderboard.LoadScores(result =>
    {
      toDisplay += "Received " + earningLeaderboard.scores.Length.ToString() + " scores\n";
      Debug.Log("Received " + earningLeaderboard.scores.Length + " scores");
      //ShowLeaderboard();
      DisplayLeaderboard();
    });
  }

  void ShowLeaderboard()
  {
    if (Social.localUser.authenticated)
      Social.ShowLeaderboardUI();
  }

  // Display leaderboard
  void DisplayLeaderboard()
  {
    if (earningLeaderboard == null) return;

    if (earningsText)
    {
      string earningDisplay = "";
      earningDisplay += earningLeaderboard.userScope + "\n";

      for (int i = 0; i < earningLeaderboard.scores.Length; ++i)
      {
        earningDisplay += earningLeaderboard.scores[i].rank.ToString() + ": ";
        earningDisplay += earningLeaderboard.scores[i].userID + " ";
        earningDisplay += earningLeaderboard.scores[i].value.ToString();
        earningDisplay += "\n";
      }

      earningsText.GetComponent<Text>().text = earningDisplay;
    }

    if (debugText)
    {
      debugText.GetComponent<Text>().text = toDisplay;
    }
  }

  // Report score
  public void ReportScore(long score, SOCIALBOARD boardType = SOCIALBOARD.EARNINGS)
  {
    string boardID = "";
    switch (boardType)
    {
      case SOCIALBOARD.EARNINGS:
        boardID = GPGLeaderboardManager.leaderboard_highest_earnings;
        break;
    }

    Social.ReportScore(score, boardID, success =>
    {
      toDisplay += success ? "Reported score successfully" : "Failed to report score";
      toDisplay += "\n";

      Debug.Log(success ? "Reported score successfully" : "Failed to report score");
    });

    LoadLeaderboard();
  }

  void Update()
  {
    if (Input.GetKeyDown(KeyCode.R))
    {
      ReportScore(9001 + (long)Time.time);
    }
  }

  public void InitSocial()
  {
    earningsText = GameObject.Find("global_leader_names");
    debugText = GameObject.Find("debug_names");
  }

}
