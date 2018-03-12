//using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProGenJam
{
	public class CustomGen: MonoBehaviour
	{
		public enum TileType
		{
			Wall, Floor,
		}
		public int length, width; 
		public bool useRandomSeed;
		public string seed;
		[Range(0,100)]
		public int randomFillPercent;
		[Range(0,10)]
		public int smoothing;
		public Sprite[] floorTiles;                           // An array of floor tile prefabs.
		public Sprite[] wallTiles;                            // An array of wall tile prefabs.
		public Sprite[] outerWallTiles;                       // An array of outer wall tile prefabs.
		private GameObject boardHolder;
		int[,] cells2D;
		List<Cell> cells = new List<Cell>();

		void Start()
		{
			boardHolder = new GameObject("BoardHolder");
			InstantiateTiles();
			Generate();
		}
		public void Generate () 
		{
			cells2D = new int[width,length];
			RandomFillMap();
			for(int x=0;x<smoothing;x++)
				SmoothMap();
			FillTiles();
		}	
		void Update() {
			if (Input.GetMouseButtonDown(0)) {
				Generate();
			}
		}
		void RandomFillMap()
		{
			//Vector3 loc = Vector3.zero, size = new Vector3 (1,1,1);
			if (useRandomSeed) {
				seed = Time.time.ToString();
			}

			//System.Random pseudoRandom = new System.Random(seed.GetHashCode());
			for (int x = 0; x<width; x++)
			{
				for (int y = 0; y<length; y++)
				{
					if(x==0||x==width-1||y==0||y==length-1)//Border
					{
						cells2D[x,y] = 2;
					}else{
						//if(x==width/2&&z==length/2)//center
						//{}
						float p = Random.value*100;//Mathf.PerlinNoise(Random.value,0)*100;
						Debug.Log(p);
						cells2D[x,y] = (p < randomFillPercent)? 1: 0;
						Debug.Log(cells2D[x,y]);
					}
				}
			}
		}
		bool IsInMapRange(int x, int y) 
		{
			return x >= 1 && x < width-1 && y >= 1 && y < length-1;
		}
		void SmoothMap() 
		{
			for (int x = 1; x < width-1; x ++) {
				for (int y = 1; y < length-1; y ++) {
					int neighbourWallTiles = GetSurroundingWallCount(x,y);

					if (neighbourWallTiles > 4)
						cells2D[x,y] = 1;
					else if (neighbourWallTiles < 4)
						cells2D[x,y] = 0;

				}
			}
		}
		int GetSurroundingWallCount(int gridX, int gridY) 
		{
			int wallCount = 0;
			for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX ++) {
				for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY ++) {
					if (IsInMapRange(neighbourX,neighbourY)) {
						if (neighbourX != gridX || neighbourY != gridY) {
							wallCount += cells2D[neighbourX,neighbourY];
						}
					}
					else {
						wallCount ++;
					}
				}
			}

			return wallCount;
		}
		void InstantiateTiles()
		{
			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < length; j++)
				{
					GameObject go = new GameObject();
					go.AddComponent<Cell>();
					Cell cell = go.GetComponent<Cell>();
					cell.location = new Vector3(i,j);
					cells.Add(cell);
				}
			}
		}
		void FillTiles ()
		{
			int c = 0;
			// Go through all the tiles in the jagged array...
			for (int i = 0; i < cells2D.GetLength(0); i++)
			{
				for (int j = 0; j < cells2D.GetLength(1); j++)
				{
					cells[c].sprite = floorTiles[UnityEngine.Random.Range(0, floorTiles.Length)];

					if (cells2D[i,j] == 2)
					{
						cells[c].sprite = outerWallTiles[UnityEngine.Random.Range(0, outerWallTiles.Length)];
					}
					// If the tile type is Wall...
					if (cells2D[i,j] == 1)
					{
						cells[c].sprite = wallTiles[UnityEngine.Random.Range(0, wallTiles.Length)];
					}
					c++;
				}
			}
		}

	}
}
