using UnityEngine;
using System.Collections;

public class AltMapDisplay : MonoBehaviour {
	public GameObject[] shapes;
	private AltMapGenerator mapGenerator;

	// Use this for initialization
	void Start () {
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
	
	}
}
