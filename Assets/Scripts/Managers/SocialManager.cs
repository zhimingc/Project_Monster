using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SocialPlatforms;
using GooglePlayGames;
using GooglePlayGames.BasicApi;

public enum SOCIALBOARD
{
  EARNINGS,
  NUM_BOARDS
};


public class SocialManager : MonoBehaviour {

  // Social API stuff
  public ILeaderboard earningLeaderboard;
  public string playerName;

  private GameObject earningsText;
  private GameObject debugText;
  private string toDisplay = "";
  string earningDisplay = "";
  List<string> userIds;

  // Use this for initialization
  void Start () {
    earningLeaderboard = null;
#if UNITY_ANDROID

    // Activate the Google Play Games platform
    PlayGamesPlatform.Activate();

#endif

    Login();
    //InitSocial();
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
      playerName = Social.localUser.userName;

      // debug
      toDisplay = "Username: " + Social.localUser.userName +
      "\nID: " + Social.localUser.id + "\n";

      CreateLeaderboard();
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

    earningLeaderboard = PlayGamesPlatform.Instance.CreateLeaderboard();
    earningLeaderboard.id = boardID;
    earningLeaderboard.LoadScores(ok =>
    {
      if (ok)
      {
        //LoadUsersAndDisplay(earningLeaderboard);
      }
      else {
        Debug.Log("Error retrieving leaderboardi");
      }
    });

#elif UNITY_IOS
    // boardID = SOCIALBOARD.EARNINGS.ToString();

    // Create leaderboard instance
    earningLeaderboard = Social.CreateLeaderboard();
    earningLeaderboard.id = boardID;

#endif

    LoadLeaderboard();
  }

  // Load leaderboard
  public void LoadLeaderboard()
  {
    if (!Social.localUser.authenticated || 
      earningLeaderboard == null) return;
    // After loading scores how are the scores stored?

#if UNITY_ANDROID
    //earningDisplay = "";
    PlayGamesPlatform.Instance.LoadScores(
        GPGLeaderboardManager.leaderboard_highest_earnings,
        LeaderboardStart.PlayerCentered,
        10,
        LeaderboardCollection.Public,
        LeaderboardTimeSpan.AllTime,
        (data) =>
        {
          toDisplay += "Leaderboard data valid: " + data.Valid;
          toDisplay += "\n approx:" + data.ApproximateCount + " have " + data.Scores.Length;
          toDisplay += "\n";
          DisplayLeaderboard();

          //UpdateLeaderboard_Android(data);

          LoadUsersAndDisplay(data);
          DisplayLeaderboard();

          foreach (IScore score in data.Scores)
          {
            earningDisplay += score.rank.ToString() + ". ";
            earningDisplay += score.userID + " ";
            earningDisplay += score.value + "\n";
          }

          DisplayLeaderboard();
        });
#elif UNITY_IOS

#endif
    // Load scores
    //earningLeaderboard.LoadScores(result =>
    //{
    //  toDisplay += "Received " + earningLeaderboard.scores.Length.ToString() + " scores\n";
    //  Debug.Log("Received " + earningLeaderboard.scores.Length + " scores");
    //  //ShowLeaderboard();
    //  DisplayLeaderboard();
    //});
  }

  void ShowLeaderboard()
  {
    if (Social.localUser.authenticated)
      Social.ShowLeaderboardUI();
  }

#if UNITY_ANDROID
  void UpdateLeaderboard_Android(LeaderboardScoreData lb)
  {
    //int playerRank = lb.PlayerScore.rank;
    int playerRank = 0;

    // start from two ranks up
    playerRank -= 2;
    for (int i = 0; i < 5; ++i)
    {
      int rank = playerRank + i;
      if (rank < 0) continue;

      IScore score = lb.Scores[rank];
      earningDisplay += score.rank.ToString() + ". ";
      earningDisplay += userIds[rank] + " ";
      earningDisplay += score.value + "\n";
    }
  }
#endif

  // Display leaderboard
  void DisplayLeaderboard()
  {
    if (earningLeaderboard == null) return;

    if (earningsText)
    {
      //earningsText.GetComponent<Text>().text = earningDisplay;
    }

    if (debugText)
    {
      //debugText.GetComponent<Text>().text = toDisplay;
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

    CreateLeaderboard();
  }

  private IUserProfile FindUser(IUserProfile[] users, string userid)
  {
    foreach (IUserProfile user in users)
    {
      if (user.id == userid)
      {
        return user;
      }
    }
    return null;
  }

#if UNITY_ANDROID
  internal void LoadUsersAndDisplay(LeaderboardScoreData lb)
  {
    // get the user ids
    userIds = new List<string>();

    foreach (IScore score in lb.Scores)
    {
      userIds.Add(score.userID);
    }

    // load the profiles and display (or in this case, log)
    Social.LoadUsers(userIds.ToArray(), (users) =>
    {
      toDisplay += "Leaderboard loading: " + lb.Title + " count = " +
          lb.Scores.Length + "\n";
      foreach (IScore score in lb.Scores)
      {
        IUserProfile user = FindUser(users, score.userID);
        toDisplay += score.formattedValue + " by " +
            (string)(
                (user != null) ? user.userName : "**unk_" + score.userID + "**");
        toDisplay += "\n";

        DisplayLeaderboard();
      }
    });
  }
#endif

}
