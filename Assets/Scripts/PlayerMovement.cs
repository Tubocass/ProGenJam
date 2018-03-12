using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

	public float speed;
	private Vector3 dir;
	public float score = 0;
	public float highScore = 0;




	// Use this for initialization
	void Start () {

		dir = Vector3.zero;
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown (KeyCode.UpArrow)) {
			dir = Vector3.forward;

		} 

		if (Input.GetKeyDown(KeyCode.DownArrow)) {
			dir = Vector3.back;
		} 

		if (Input.GetKeyDown(KeyCode.LeftArrow)) {
			dir = Vector3.left;
		} 

		if(Input.GetKeyDown(KeyCode.RightArrow)) {
			dir = Vector3.right;
		}

			float amountToMove = speed * Time.deltaTime;

			transform.Translate (dir * amountToMove);
	}

	void OnTriggerEnter(Collider col)
	{
		if(col.gameObject.tag == "Platform")
		{
			print ("collides");
			if (score > highScore) {
				PlayerPrefs.SetFloat ("High Score", score);
				//print(PlayerPrefs.GetInt
				print ("score before death was: " + score);
			}
			Application.LoadLevel (Application.loadedLevel);
		}
	}
}
