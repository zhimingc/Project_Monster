using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

/*

  Analytics to catch:
  1. Length of session
  2. Number of plays per session
  3. Average length of play
  4. Average result per play
  5. Number of unlocks

*/
public enum ANALYTICS
{
  LENGTH_SESSION,
  NUM_PLAY,
  AVG_PLAYLENGTH,
  AVG_RESULT,
  NUM_UNLOCKS,
  NUM_PLAYRESULT,
  LENGTH_PLAY
}

public enum ANALYTICS_EVENT
{
  PLAY_END,
  SESSION_END,
  PLAYER_UNLOCK
}

public class AnalyticsManager : MonoBehaviour {

  public float length_session, num_playResult, length_play;
  public int num_unlocks, num_play;

  private bool trackPlayLength;
  private float curPlayLength, totalResult, totalPlayLength;

	// Use this for initialization
	void Start () {
    length_session = 0.0f;
    num_playResult = 0.0f;
    length_play = 0.0f;
    num_unlocks = 0;
    num_play = 0;

    totalResult = 0.0f;
    totalPlayLength = 0.0f;
  }

  public void ToggleAnalysis(bool toggle, ANALYTICS a_type)
  {
    switch (a_type)
    {
      case ANALYTICS.LENGTH_PLAY:
        if (toggle)
        {
          trackPlayLength = true;
          curPlayLength = 0.0f;
        }
        else
        {
          trackPlayLength = false;
          UpdateAnalytics(a_type, curPlayLength);
        }
        break;
    }
  }

  public void UpdateAnalytics(ANALYTICS a_type, float data)
  {
    switch (a_type)
    {
      //case ANALYTICS.AVG_PLAYLENGTH:
      //  totalPlayLength += data;
      //  break;
      //case ANALYTICS.AVG_RESULT:
      //  totalResult += data;
      //  break;
      case ANALYTICS.NUM_PLAY:
        num_play = (int)data;
        break;
      case ANALYTICS.NUM_UNLOCKS:
        num_unlocks = (int)data;
        break;
      case ANALYTICS.NUM_PLAYRESULT:
        num_playResult = data;
        break;
      case ANALYTICS.LENGTH_PLAY:
        length_play = data;
        break;
    }
  }

  public void PostAnalytics(ANALYTICS_EVENT eventType)
  {
    switch (eventType)
    {
      case ANALYTICS_EVENT.PLAY_END:
        Analytics.CustomEvent("Play end", new Dictionary<string, object>
        {
          { "Play length", length_play },
          { "Play result", num_playResult },
        });
        break;
      case ANALYTICS_EVENT.SESSION_END:
        Analytics.CustomEvent("Session end", new Dictionary<string, object>
        {
          //{ "Avg play length", num_play == 0 ? 0 : totalPlayLength / num_play },
          //{ "Avg play result", num_play == 0 ? 0 : totalResult / num_play },
          { "Session minutes", length_session },
          { "Number of plays", num_play },
        });

        ResetSession();
        break;
      case ANALYTICS_EVENT.PLAYER_UNLOCK:
        Analytics.CustomEvent("Player unlock", new Dictionary<string, object>
        {
          { "Number of unlocks", num_unlocks },
        });
        break;
    }

  }

  void ResetSession()
  {
    length_session = 0.0f;
    num_playResult = 0.0f;
    length_play = 0.0f;
    num_unlocks = 0;
    num_play = 0;

    totalResult = 0.0f;
    totalPlayLength = 0.0f;
  }

  // Update is called once per frame
  void Update()
  {
    if (Input.GetKeyDown(KeyCode.A))
    {
      PostAnalytics(ANALYTICS_EVENT.SESSION_END);
    }

    // Update session length
    length_session = Time.time / 60.0f;

    if (trackPlayLength)
    {
      curPlayLength += Time.deltaTime;
    }
  }
}
