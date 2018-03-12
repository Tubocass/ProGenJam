using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]

public class Cell : MonoBehaviour 
{
	public Vector3 location{get{return transform.position;}set{transform.position = value;}}
	public Sprite sprite{set{GetComponent<SpriteRenderer>().sprite = value;}}
}
