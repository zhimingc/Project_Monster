using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;


public class highscore : MonoBehaviour {

  //private WWW www;
  private string url = "http://nomossgames.com/monsterkitchenscore/score.php?secret=sdhfiuaef89shdf";
	
  // Use this for initialization
//	IEnumerator Start()
//	{
//		//www = new WWW(url);
//		//yield return www;
////		print (www.data);

//		//StartCoroutine(GetScores(9));
//	}

  public void GetScoresWrapper(int num)
  {
    StartCoroutine(GetScores(num));
  }

  public IEnumerator GetScores(int num)
	{
    WWW www = new WWW(url);
    yield return www;

		TextMesh text = GetComponent<TextMesh>();

		// check for errors
		if (www.error == null)
		{
			//var outputstring = "";
			var json = JSON.Parse(www.text);

      string[] nameArr = new string[10];
      float[] scoreArr = new float[10];

			for (var i=0;i<10;i++)
      {
        nameArr[i] = json[i]["name"];
        scoreArr[i] = json[i]["bestscore"];
				//outputstring += json[i]["name"] + "  - " + json[i]["bestscore"] + '\n';
			}

      GameManager.Instance.scoreMan.DownloadedLeaderboard(nameArr, scoreArr, num);

		} else {
      string errorString = "WWW Error: " + www.error + "\nError loading leaderboards";
			Debug.Log("WWW Error: "+ www.error);
      GameManager.Instance.scoreMan.DisplayGetError(errorString);
    }    
	}

	void PostAndGet(string name, float score, int getNum)
  {
    // Post scores
    StartCoroutine(GameManager.Instance.scoreMan.PostScores(name, score));

    //while (GameManager.Instance.scoreMan.download == null)
    //{
    //  //yield return new WaitForEndOfFrame();
    //}

    //// Get scores
    //yield return GetScores(getNum);
  }
}
