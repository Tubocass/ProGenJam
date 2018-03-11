using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MapGenerator : MonoBehaviour {

	public int width;
	public int height;
	public string seed;
	public bool useRandomSeed;

	[Range(0, 100)]
	public int randomFillPercent;

	int[,] map;

	void Start () {
		GenerateMap ();
	}
	void Update() {
		if (Input.GetMouseButtonDown (0)) {
			GenerateMap ();
		}
	}

	void GenerateMap() {
		map = new int[width,height];
		RandomFillMap ();

		//using 5, but can plug in different values to get different outputs with smoothing
		for (int i = 0; i < 5; i++) {
			SmoothMap ();
		}
	}

	void RandomFillMap() {
		if(useRandomSeed) {
			seed = Time.time.ToString();
		}
			
		//this will return a unique HashCode for the seed
		System.Random pseudoRandom = new System.Random (seed.GetHashCode());

		for (int x = 0; x < width; x++) { 
			for(int y = 0; y < height; y++) { 
				//this makes sure the edges of the map are all walls
				// '1' = a wall
				if (x == 0 || x == width-1 || y == 0 || y == height - 1) {
					map[x,y] = 1;
				} else {
					float p = Mathf.PerlinNoise(pseudoRandom.Next(0,100)*0.69f,pseudoRandom.Next(0,100)*0.69f)*100;
					//Debug.Log(p);
					map[x,y] = (p < randomFillPercent)? 1: 0; //(pseudoRandom.Next(0,100)
				}
			}
		}
	}

	//function to smooth the map with iterations
	void SmoothMap() {
		//looking at every tile
		for (int x = 0; x < width; x++) { 
			for(int y = 0; y < height; y++) {
				int neighborWallTiles = GetSurrondingWallCount (x,y);

				if (neighborWallTiles > 4)
					map [x, y] = 1;
				else if (neighborWallTiles < 4)
					map [x, y] = 0;
			}
		}
	}

	//how many neighboring tiles are walls
	int GetSurrondingWallCount (int gridX, int gridY) {
		int wallCount = 0;
		for ( int neighborX = gridX - 1; neighborX <= gridX + 1; neighborX++) {
			for(int neighborY = gridY - 1; neighborY <= gridY + 1; neighborY ++) {
				//check to make sure we are safe inside the map and not on an edge tile
				if(neighborX >=0 && neighborX < width && neighborY >= 0 && neighborY < height) {
				//make sure you're not looking at the current tile	
				if (neighborX != gridX || neighborY != gridY) {
					//if passed, increment wallcount	
					wallCount += map[neighborX, neighborY];
					}
				}
				else {
					//happens if you were on an edge tile
					//encourages the growth of walls around the edge of the map
					wallCount++;
				}
			}
		}
		return wallCount;
	}
		
	//function that draws the 'wall' cubes with a size of 1
	void OnDrawGizmos() {
		if (map != null) {
			for (int x = 0; x < width; x++) { 
				for (int y = 0; y < height; y++) { 
					Gizmos.color = (map[x, y] == 1) ? Color.black : Color.white;
					Vector3 pos = new Vector3 (-width / 2 + x + .5f, 0, -height / 2 + y + .5f);
					Gizmos.DrawCube (pos, Vector3.one);
				}
			}
		}
	}
}

