
using UnityEngine;
using System.Text;
using System.IO;

public class RoadGenerator {

	public const int roadDimensions = 100;

	public byte[,] roadTilesMap;
	public byte[,] roadMap;

	public void PrintMap() {

		StringBuilder sb = new StringBuilder ();

		for (int y = 0; y < roadDimensions; y++) {
			for (int x = 0; x < roadDimensions; x++) {
				sb.AppendFormat ("{0}", roadMap [x, y]);
			}
			sb.AppendFormat ("\n");
		}

		Debug.Log (sb);
	}

	public void LoadTrackFile(int gameLevel) {

		int levelIdx = (gameLevel-1) % 4;
		string levelFilePath = string.Format ("Game/maps/map{0}", levelIdx);

		string trackString = PlanetUnityResourceCache.GetTextFile (levelFilePath);

		Sprite[] allSprites = Resources.LoadAll<Sprite>(Path.GetDirectoryName("Game/scenary/scenary"));
		int numScenary = allSprites.Length;

		int index = 0;
		foreach (char c in trackString) {

			if (c >= '0' && c <= '1') {
				int tileX = index % roadDimensions;
				int tileY = ((roadDimensions-1) - index / roadDimensions);

				roadMap [tileX, tileY] = (byte)(c - '0');

				index++;
			}else if (c >= '2' && c <= '9') {
				int tileX = index % roadDimensions;
				int tileY = ((roadDimensions-1) - index / roadDimensions);

				roadMap [tileX, tileY] = (byte)((c - '0') + 2);

				index++;
			}else if (c == '?') {
				int tileX = index % roadDimensions;
				int tileY = ((roadDimensions-1) - index / roadDimensions);

				roadMap [tileX, tileY] = (byte)(Random.Range (0, numScenary) + 2);

				index++;
			}
		}

		// Add random scenary
		for (int i = 0; i < 400; i++) {
			int randX = Random.Range (5, 95);
			int randY = Random.Range (5, 95);
			byte type = (byte)(Random.Range (0, numScenary) + 2);


			if (roadMap [randX, randY] == 0) {

				// Check all neighbors, we need grass all around us
				bool allGrass = true;
				for (int x = -1; x <= 1; x++) {
					for (int y = -1; y <= 1; y++) {
						if (roadMap [randX+x, randY+y] != 0) {
							allGrass = false;
						}
					}
				}

				// I must be within 2 of a road
				bool hasRoad = false;
				for (int x = -2; x <= 2; x++) {
					for (int y = -2; y <= 2; y++) {
						if (roadMap [randX+x, randY+y] != 0) {
							hasRoad = true;
						}
					}
				}

				if (allGrass && hasRoad) {
					roadMap [randX, randY] = type;
				}
			}
		}

	}

	public void GenerateRoadMap(int level) {

		roadTilesMap = new byte[roadDimensions,roadDimensions];
		roadMap = new byte[roadDimensions,roadDimensions];

		// 0 for grass, 1 for roads.

		// Start with all grass
		for (int y = 0; y < roadDimensions; y++) {
			for (int x = 0; x < roadDimensions; x++) {
				roadMap [x, y] = 0;
			}
		}

		LoadTrackFile(level);

		// Convert all 0 and 1 to road tiles
		for (int y = 0; y < roadDimensions; y++) {
			for (int x = 0; x < roadDimensions; x++) {
				
				bool topRoad = false;
				bool leftRoad = false;
				bool rightRoad = false;
				bool bottomRoad = false;



				if (y < roadDimensions-1) {
					topRoad = (roadMap [x, y + 1] == 1);
				}
				if (y >= 1) {
					bottomRoad = (roadMap [x, y - 1] == 1);
				}

				if (x >= 1) {
					leftRoad = (roadMap [x - 1, y] == 1);
				}
				if (x < roadDimensions-1) {
					rightRoad = (roadMap [x + 1, y] == 1);
				}

				// Default to grass
				roadTilesMap [x, y] = 15;

				if (roadMap [x, y] == 1) {

					if (topRoad == false && bottomRoad == true && leftRoad == false && rightRoad == false) {
						roadTilesMap [x, y] = 14;
					}

					if (topRoad == false && bottomRoad == false && leftRoad == true && rightRoad == false) {
						roadTilesMap [x, y] = 13;
					}

					if (topRoad == true && bottomRoad == false && leftRoad == false && rightRoad == false) {
						roadTilesMap [x, y] = 12;
					}

					if (topRoad == false && bottomRoad == false && leftRoad == false && rightRoad == true) {
						roadTilesMap [x, y] = 11;
					}

					if (topRoad == false && bottomRoad == true && leftRoad == false && rightRoad == true) {
						roadTilesMap [x, y] = 10;
					}

					if (topRoad == false && bottomRoad == true && leftRoad == true && rightRoad == false) {
						roadTilesMap [x, y] = 9;
					}

					if (topRoad == true && bottomRoad == false && leftRoad == true && rightRoad == false) {
						roadTilesMap [x, y] = 8;
					}

					if (topRoad == true && bottomRoad == false && leftRoad == false && rightRoad == true) {
						roadTilesMap [x, y] = 7;
					}

					if (topRoad == true && bottomRoad == true && leftRoad == false && rightRoad == true) {
						roadTilesMap [x, y] = 6;
					}

					if (topRoad == true && bottomRoad == true && leftRoad == true && rightRoad == false) {
						roadTilesMap [x, y] = 5;
					}

					if (topRoad == false && bottomRoad == true && leftRoad == true && rightRoad == true) {
						roadTilesMap [x, y] = 4;
					}

					if (topRoad == true && bottomRoad == false && leftRoad == true && rightRoad == true) {
						roadTilesMap [x, y] = 3;
					}

					if (topRoad == true && bottomRoad == true && leftRoad == true && rightRoad == true) {
						roadTilesMap [x, y] = 2;
					}

					if (topRoad == false && bottomRoad == false && leftRoad == true && rightRoad == true) {
						roadTilesMap [x, y] = 1;
					}

					if (topRoad == true && bottomRoad == true && leftRoad == false && rightRoad == false) {
						roadTilesMap [x, y] = 0;
					}
				}

			}
		}
	}
	
}
