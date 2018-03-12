using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AltMapDisplay : MonoBehaviour {
	public GameObject[] shapes;
	public GameObject[] mats;
	private AltMapGenerator mapGenerator;
	//public float score;
	public PlayerMovement player;
	public Text scoreText;
	//private float highScore;
	public Text highScoreText;

	// Use this for initialization
	void Start () {
		
		//print (highScore);
		player.highScore = PlayerPrefs.GetFloat("High Score");
		highScoreText.text = "High Score: " + player.highScore;
		Color newColor = new Color( Random.value, Random.value, Random.value, 1.0f );
		GameObject mat = GameObject.Find ("DeathPrefab");
		mat.gameObject.GetComponent<Renderer>().material.color = newColor;


		mapGenerator = GetComponent<AltMapGenerator> ();
		for (int r = 1; r < mapGenerator.mapRows-1; r++) {
			for (int c = 1; c < mapGenerator.mapColumns - 1; c++) {
				string ch = mapGenerator.map [r, c].ToString();
				int charPos = mapGenerator.boxCharacters.IndexOf (ch);

				if (charPos < 0) {
					continue;
				}

				// Debug.Log ("Character at " + r + "," + c + " = " + mapGenerator.map [r, c] + " (" + charPos + ")");
				Instantiate (shapes [charPos], new Vector3 (r * 3, 0, c * 3), shapes[charPos].transform.rotation);
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (player.speed > 0) {
			player.score += Time.deltaTime;
			scoreText.text = "Score: " + player.score;
		}
		//print (score);
	}


}
