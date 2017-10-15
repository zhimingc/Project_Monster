using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class submitbutton : MonoBehaviour {

	private static string url = "http://nomossgames.com/monsterkitchenscore/score.php?secret=sdhfiuaef89shdf";

	public void OnClick(){
		print ("Hit me baby one more time");
		StartCoroutine(go());


	}

	// Use this for initialization
//	void Start () {
//		
//	}

	string highscore_url = url;
	string playName = "Player 1";
	int score = 20;

	// Use this for initialization
	IEnumerator go () {
		// Create a form object for sending high score data to the server
		WWWForm form = new WWWForm();
		// Assuming the perl script manages high scores for different games

		// The name of the player submitting the scores
		form.AddField( "name", playName );
		// The score
		form.AddField( "score", score );

		form.AddField( "secret", "sdhfiuaef89shdf");

		// Create a download object
		WWW download = new WWW( highscore_url, form );

		// Wait until the download is done
		yield return download;

		if(!string.IsNullOrEmpty(download.error)) {
			print( "Error downloading: " + download.error );
		} else {
			// show the highscores
			Debug.Log(download.text);
		}

  }
	
	// Update is called once per frame
	void Update () {
		
	}
}
