using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class JMapGenerator : MonoBehaviour {

	public int width;			//Width of Map
	public int height;			//Height of Map
	public string seed;			
	public bool useRandomSeed;

	[Range(0, 100)]
	public int randomFillPercent;

	int[,] map;


	public IntRange numRooms = new IntRange (3, 10);
	public IntRange roomWidth = new IntRange (3, 10);         
	public IntRange roomHeight = new IntRange (3, 10);


	//Runs once at Runtime
	void Start () {
		GenerateMap ();
	}


	void Update() {
		//click right mouse button to generate new PCG Map
		if (Input.GetMouseButtonDown (0)) {
			GenerateMap ();
		}
	}







	void GenerateMap() {
		map = new int[width, height];
		RandomFillMap ();


		//using 5; Different values will produce different outputs with smoothing
		for (int i = 0; i < 5; i++) {
			SmoothMap ();
		}

		ProcessMap ();

		//sets the border of the map to walls
		int borderSize = 1;
		int[,] borderedMap = new int[width + borderSize * 2,height + borderSize * 2];

		for (int x = 0; x < borderedMap.GetLength(0); x ++) {
			for (int y = 0; y < borderedMap.GetLength(1); y ++) {
				if (x >= borderSize && x < width + borderSize && y >= borderSize && y < height + borderSize) {
					borderedMap[x,y] = map[x-borderSize,y-borderSize];
				}
				else {
					borderedMap[x,y] =1;
				}
			}
		} 
	}




	//gets the entire wall region and places it in a list and checks it against a size threshold. 
	void ProcessMap() {
		List<List<Coord>> wallRegions = GetRegions (1);
		int wallThresholdSize = 10;

		foreach (List<Coord> wallRegion in wallRegions) {
			if (wallRegion.Count < wallThresholdSize) {
				foreach (Coord tile in wallRegion) {
					map[tile.tileX,tile.tileY] = 0;
				}
			}
		}

		//same idea as above but with rooms
		List<List<Coord>> roomRegions = GetRegions (0);
		int roomThresholdSize = 100;
		List<Room> survivingRooms = new List<Room> ();

		foreach (List<Coord> roomRegion in roomRegions) {
			if (roomRegion.Count < roomThresholdSize) {
				foreach (Coord tile in roomRegion) {
					map[tile.tileX,tile.tileY] = 1;
				}
			}
			else {
				//keeps rooms that pass and stores them in List
				survivingRooms.Add (new Room (roomRegion, map));
			}
		}

		//sort survivingRooms list by roomSize
		survivingRooms.Sort();
		foreach (Room r in survivingRooms) {
			print (r.roomSize);
		}
		ConnectClosestRooms (survivingRooms);
	}



	//connects the rooms that are closest to each other
	//need to adjust so it find the shortest connection distance
	void ConnectClosestRooms(List<Room> allRooms) {

		int bestDistance = 0;
		Coord bestTileA = new Coord ();
		Coord bestTileB = new Coord ();
		Room bestRoomA = new Room();
		Room bestRoomB = new Room();
		bool possibleConnectionFound = false;

		Debug.Log ((int)allRooms.Count);

		foreach(Room roomA in allRooms) {
			
			possibleConnectionFound = false;

			foreach(Room roomB in allRooms) {
				if (roomA == roomB) {
					continue;
				}
				if(roomA.isConnected(roomB)) {
					possibleConnectionFound = false;
					break;
				}

				for(int tileIndexA = 0; tileIndexA < roomA.edgeTiles.Count; tileIndexA ++) {
					for(int tileIndexB = 0;tileIndexB < roomB.edgeTiles.Count; tileIndexB++) {
						Coord tileA = roomA.edgeTiles [tileIndexA];
						Coord tileB = roomB.edgeTiles [tileIndexB];
						int distanceBetweenRooms = (int)(Mathf.Pow (tileA.tileX - tileB.tileX, 2) + Mathf.Pow (tileA.tileX - tileB.tileX, 2));

						if(distanceBetweenRooms < bestDistance || !possibleConnectionFound) {
							bestDistance = distanceBetweenRooms;
							possibleConnectionFound = true;
							bestTileA = tileA;
							bestTileB = tileB;
							bestRoomA = roomA;
							bestRoomB = roomB;
						}
					}
				}
			}

			//this Creates a passage way between the rooms that are closest to each other
			if (possibleConnectionFound) {
				CreatePassage (bestRoomA, bestRoomB, bestTileA, bestTileB);
					

			}
		}
	}

	//function and math for creating the passage
	void CreatePassage(Room roomA, Room roomB, Coord tileA, Coord tileB) {
		Room.ConnectRooms (roomA, roomB);
		Debug.DrawLine(CoordToWorldPoint (tileA), CoordToWorldPoint (tileB), Color.green, .5f);

		List<Coord> line = GetLine (tileA, tileB);
		foreach (Coord c in line) {
			DrawCircle(c,1);
		}
	}

	void DrawCircle(Coord c, int r) {
		for (int x = -r; x <= r; x++) {
			for (int y = -r; y <= r; y++) {
				if (x*x + y*y <= r*r) {
					int drawX = c.tileX + x;
					int drawY = c.tileY + y;
					if (IsInMapRange(drawX, drawY)) {
						map[drawX,drawY] = 0;
					}
				}
			}
		}
	}

	List<Coord> GetLine(Coord from, Coord to) {
		List<Coord> line = new List<Coord> ();

		int x = from.tileX;
		int y = from.tileY;

		int dx = to.tileX - from.tileX;
		int dy = to.tileY - from.tileY;

		bool inverted = false;
		int step = Math.Sign (dx);
		int gradientStep = Math.Sign (dy);

		int longest = Mathf.Abs (dx);
		int shortest = Mathf.Abs (dy);

		if (longest < shortest) {
			inverted = true;
			longest = Mathf.Abs(dy);
			shortest = Mathf.Abs(dx);

			step = Math.Sign (dy);
			gradientStep = Math.Sign (dx);
		}

		int gradientAccumulation = longest / 2;
		for (int i =0; i < longest; i ++) {
			line.Add(new Coord(x,y));

			if (inverted) {
				y += step;
			}
			else {
				x += step;
			}

			gradientAccumulation += shortest;
			if (gradientAccumulation >= longest) {
				if (inverted) {
					x += gradientStep;
				}
				else {
					y += gradientStep;
				}
				gradientAccumulation -= longest;
			}
		}

		return line;
	}



	//sets the coordinate to a world point to compare to
	Vector3 CoordToWorldPoint(Coord tile) {
		return new Vector3 (-width / 2 + .5f + tile.tileX, 2, -height / 2 + .5f + tile.tileY);

	}

	//produces regions of a given tileType
	List<List<Coord>> GetRegions(int tileType) {
		List<List<Coord>> regions = new List<List<Coord>> ();
		int[,] mapFlags = new int[width,height];

		for (int x = 0; x < width; x ++) {
			for (int y = 0; y < height; y ++) {
				if (mapFlags[x,y] == 0 && map[x,y] == tileType) {
					List<Coord> newRegion = GetRegionTiles(x,y);
					regions.Add(newRegion);

					foreach (Coord tile in newRegion) {
						mapFlags[tile.tileX, tile.tileY] = 1;
					}
				}
			}
		}

		return regions;
	}

	List<Coord> GetRegionTiles(int startX, int startY) {
		List<Coord> tiles = new List<Coord> ();
		int[,] mapFlags = new int[width,height];
		int tileType = map [startX, startY];

		Queue<Coord> queue = new Queue<Coord> ();
		queue.Enqueue (new Coord (startX, startY));
		mapFlags [startX, startY] = 1;

		while (queue.Count > 0) {
			Coord tile = queue.Dequeue();
			tiles.Add(tile);

			for (int x = tile.tileX - 1; x <= tile.tileX + 1; x++) {
				for (int y = tile.tileY - 1; y <= tile.tileY + 1; y++) {
					if (IsInMapRange(x,y) && (y == tile.tileY || x == tile.tileX)) {
						if (mapFlags[x,y] == 0 && map[x,y] == tileType) {
							mapFlags[x,y] = 1;
							queue.Enqueue(new Coord(x,y));
						}
					}
				}
			}
		}

		return tiles;
	}




	//function to make sure we are on the map
	bool IsInMapRange(int x, int y) {
		return x >= 0 && x < width && y >= 0 && y < height;
	}




		

	void RandomFillMap() {
		if(useRandomSeed) {
			seed = Time.time.ToString();
		}
			
		//this will return a unique HashCode for the seed
		System.Random pseudoRandomGen = new System.Random (seed.GetHashCode());

		for (int x = 0; x < width; x++) { 
			for(int y = 0; y < height; y++) { 
				//this makes sure the edges of the map are all walls
				// '1' = a wall
				if (x == 0 || x == width-1 || y == 0 || y == height - 1) {
					map[x,y] = 1;
				} else {
				map[x, y] = (pseudoRandomGen.Next (0, 100) < randomFillPercent)? 1 : 0;
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
				//check to make sure we are safely inside the map and not on an edge tile
				if(IsInMapRange(neighborX,neighborY)) {
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

	//stores coordinates of current tile you're looking at
	struct Coord {
		public int tileX;
		public int tileY;

		public Coord(int x, int y) {
			tileX = x;
			tileY = y;
		}
	}






	//will store all info about our room
	class Room : IComparable<Room> {


		public List<Coord> tiles;							//list of coord to store all the tiles in room
		public List<Coord> edgeTiles;						//list of all tiles that form the edge of the room
		public List<Room> connectedRooms;					//rooms that share a common passageway
		public int roomSize;								//how many tiles the room is made up of

		public Room() {
		}


		public Room(List<Coord> roomTiles, int[,] map) {
			tiles = roomTiles;
			roomSize = tiles.Count;
			connectedRooms = new List<Room>();

			edgeTiles = new List<Coord>();
			foreach(Coord tile in tiles) {
				for(int x = tile.tileX-1; x<=tile.tileX+1; x++) {
					for(int y = tile.tileY-1; y<=tile.tileY+1; y++) {
						if(x == tile.tileX || y == tile.tileY) {
							if(map[x,y] == 1) {
								edgeTiles.Add(tile);
							}
						}
					}
				}
			}
		}



		public static void ConnectRooms(Room roomA, Room roomB) {
			roomA.connectedRooms.Add (roomB);
			roomB.connectedRooms.Add (roomA);
		}

		public bool isConnected(Room otherRoom) {
			return connectedRooms.Contains(otherRoom);
		}

		public int CompareTo(Room otherRoom) {
			return otherRoom.roomSize.CompareTo (roomSize);
		}
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
