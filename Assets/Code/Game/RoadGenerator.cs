
using UnityEngine;
using System.Text;

public class RoadGenerator {

	public const int roadDimensions = 20;

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

	public void LoadTrackFile(string fileName) {
		string trackString = PlanetUnityResourceCache.GetTextFile (fileName);

		int index = 0;
		foreach (char c in trackString) {
			if (c == '0' || c == '1') {
				int tileX = index % roadDimensions;
				int tileY = ((roadDimensions-1) - index / roadDimensions);

				if (c == '0') {
					roadMap [tileX, tileY] = 0;
				} else {
					roadMap [tileX, tileY] = 1;
				}

				index++;
			}
		}
	}

	public void GenerateRoadMap() {

		roadTilesMap = new byte[roadDimensions,roadDimensions];
		roadMap = new byte[roadDimensions,roadDimensions];

		// 0 for grass, 1 for roads.

		// Start with all grass
		for (int y = 0; y < roadDimensions; y++) {
			for (int x = 0; x < roadDimensions; x++) {
				roadMap [x, y] = 0;
			}
		}

		LoadTrackFile("Game/track0");

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

					if (x == 1 && y == 1) {
						Debug.Log (string.Format ("{0} {1} {2} {3}", topRoad, bottomRoad, leftRoad, rightRoad));
					}



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
