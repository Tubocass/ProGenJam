using UnityEngine;
using System.Collections;

public class AltMapGenerator : MonoBehaviour {
	public int mapRows = 5;
	public int mapColumns = 10;

	public char[,] map;

	public string boxCharacters;
	private string[] boxCharacterUpFriends;
	private string[] boxCharacterDownFriends;
	private string[] boxCharacterLeftFriends;
	private string[] boxCharacterRightFriends;

	// Use this for initialization
	void Awake () {
		InitializeBoxCharacters ();
		InitializeMap ();
		DisplayMap ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void DisplayMap() {
		string output = "";
		for (int r = 0; r < mapRows; r++) {
			for (int c = 0; c < mapColumns; c++) {
				output += map [r, c];
			}
			output += "\n";
		}
		Debug.Log (output);
	}

	private void InitializeMap() {
		map = new char[mapRows, mapColumns];

		// Put 'X's in top and bottom rows.
		for (int c = 0; c < mapColumns; c++) {
			map [0, c] = 'X';
			map [mapRows - 1, c] = 'X';
		}

		// Put 'X's in the left and right columns.
		for (int r = 0; r < mapRows; r++) {
			map [r, 0] = 'X';
			map [r, mapColumns - 1] = 'X';
		}

		// Set 'O' for the other map spaces (which means 'free').
		for (int r = 1; r < mapRows - 1; r++) {
			for (int c = 1; c < mapColumns - 1; c++) {
				map [r, c] = 'O';
			}
		}

		Random.seed = System.DateTime.Now.Millisecond;
		// map [1, 1] = '@';
		// map [1, 2] = '@';
		// map [2, 1] = '@';
		// map [2, 2] = '@';

		for (int r = 1; r < mapRows - 1; r++) {
			for (int c = 1; c < mapColumns - 1; c++) {

				if (map [r, c] == '@') {
					continue;
				}

				string validCharacters = GetValidBoxCharacters (r, c);
				map [r, c] = validCharacters [Random.Range (0, validCharacters.Length)];
			}
		}
	}


	private string GetValidBoxCharacters(int row, int column) {
		string validCharacters = "";

		for (int i = 0; i < boxCharacters.Length; i++) {
			char ch = boxCharacters [i];

			if (
				boxCharacterLeftFriends [i].Contains (map [row, column - 1].ToString ()) &&
				boxCharacterRightFriends [i].Contains (map [row, column + 1].ToString ()) &&
				boxCharacterUpFriends [i].Contains (map [row - 1, column].ToString ()) &&
				boxCharacterDownFriends [i].Contains (map [row + 1, column].ToString ()))
			{
				validCharacters += ch.ToString ();
			}
		}

		if (validCharacters.Length == 0) {
			validCharacters = "O";
		}

		return validCharacters;
	}


	private void InitializeBoxCharacters() {
		boxCharacters = "─│┌┐└┘├┤┬┴┼"; 
		boxCharacterUpFriends = new string[boxCharacters.Length];
		boxCharacterDownFriends = new string[boxCharacters.Length];
		boxCharacterLeftFriends = new string[boxCharacters.Length];
		boxCharacterRightFriends = new string[boxCharacters.Length];

		boxCharacterLeftFriends [0] = "O─┌└├┬┴┼"; //    ─
		boxCharacterLeftFriends [1] = "O│┐┘┤X"; //     │
		boxCharacterLeftFriends [2] = "O│┐┘┤X"; //     ┌
		boxCharacterLeftFriends [3] = "O─┌└├┬┴┼"; //    ┐
		boxCharacterLeftFriends [4] = "O│┐┘┤X"; //     └
		boxCharacterLeftFriends [5] = "O─┌└├┬┴┼"; //    ┘
		boxCharacterLeftFriends [6] = "O│┐┘┤X"; //      ├
		boxCharacterLeftFriends [7] = "O─┌└├┬┴┼"; //   ┤
		boxCharacterLeftFriends [8] = "O─┌└├┬┴┼"; //    ┬
		boxCharacterLeftFriends [9] = "O─┌└├┬┴┼"; //    ┴
		boxCharacterLeftFriends [10] = "O─┌└├┬┴┼"; //   ┼

		boxCharacterRightFriends [0] = "O─┐┘┤┬┴┼"; //    ─
		boxCharacterRightFriends [1] = "O│┌└├X"; //     │
		boxCharacterRightFriends [2] = "O─┐┘┤┬┴┼"; //   ┌
		boxCharacterRightFriends [3] = "O│┌└├X"; //      ┐
		boxCharacterRightFriends [4] = "O─┐┘┤┬┴┼"; //   └
		boxCharacterRightFriends [5] = "O│┌└├X"; //      ┘
		boxCharacterRightFriends [6] = "O─┐┘┤┬┴┼"; //   ├
		boxCharacterRightFriends [7] = "O│┌└├X"; //      ┤
		boxCharacterRightFriends [8] = "O─┐┘┤┬┴┼"; //    ┬
		boxCharacterRightFriends [9] = "O─┐┘┤┬┴┼"; //    ┴
		boxCharacterRightFriends [10] = "O─┐┘┤┬┴┼"; //   ┼

		boxCharacterUpFriends [0] = "O─└┘┴X"; //       ─
		boxCharacterUpFriends [1] = "O│┌┐├┤┬┼"; //      │
		boxCharacterUpFriends [2] = "O─└┘┴X"; //        ┌
		boxCharacterUpFriends [3] = "O─└┘┴X"; //        ┐
		boxCharacterUpFriends [4] = "O│┌┐├┤┬┼"; //     └
		boxCharacterUpFriends [5] = "O│┌┐├┤┬┼"; //     ┘
		boxCharacterUpFriends [6] = "O│┌┐├┤┬┼"; //      ├
		boxCharacterUpFriends [7] = "O│┌┐├┤┬┼"; //      ┤
		boxCharacterUpFriends [8] = "O─└┘┴X"; //        ┬
		boxCharacterUpFriends [9] = "O│┌┐├┤┬┼"; //     ┴
		boxCharacterUpFriends [10] = "O│┌┐├┤┬┼"; //     ┼

		boxCharacterDownFriends [0] = "O─┌┐┬X"; //       ─
		boxCharacterDownFriends [1] = "O│└┘├┤┴┼"; //      │
		boxCharacterDownFriends [2] = "O│└┘├┤┴┼"; //     ┌
		boxCharacterDownFriends [3] = "O│└┘├┤┴┼"; //     ┐
		boxCharacterDownFriends [4] = "O─┌┐┬X"; //        └
		boxCharacterDownFriends [5] = "O─┌┐┬X"; //        ┘
		boxCharacterDownFriends [6] = "O│└┘├┤┴┼"; //      ├
		boxCharacterDownFriends [7] = "O│└┘├┤┴┼"; //      ┤
		boxCharacterDownFriends [8] = "O│└┘├┤┴┼"; //     ┬
		boxCharacterDownFriends [9] = "O─┌┐┬X"; //        ┴
		boxCharacterDownFriends [10] = "O│└┘├┤┴┼"; //     ┼
	}

}
