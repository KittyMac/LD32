
using UnityEngine;
using System.Text;

public partial class GameController : MonoBehaviour, IPUCode {


	const int kMapUnreachable = -1;
	const int kMapMaxDistance = 30000;

	short[] tileMap = new short[RoadGenerator.roadDimensions * RoadGenerator.roadDimensions];

	static int[,] directionTable1 = 
	{
		{  -1,  0 },
		{  0, -1 },
		{  1, 0 },
		{  0, 1 },
		{  -1,  1 },
		{  -1, -1 },
		{  1, -1 },
		{  1, 1 }
	};

	private short MAP_LOCAL(int X, int Y) {
		return tileMap[(int)((Y)*RoadGenerator.roadDimensions+(X))];
	}

	private void SET_MAP_LOCAL(int X, int Y, short V) {
		tileMap [(int)((Y) * RoadGenerator.roadDimensions + (X))] = V;
	}

	protected bool GTMDistanceTransformForward() {
		int h, v;
		int direction;
		int neighbor_x;
		int neighbor_y;
		bool changed = false;
		short current_tile_value;
		short neighbor_tile_value;

		for (v = 1; v < RoadGenerator.roadDimensions - 1; v++) {
			for (h = 1; h < RoadGenerator.roadDimensions - 1; h++) {
				current_tile_value = MAP_LOCAL (h, v);

				if (current_tile_value >= 0) {
					for (direction = 0; direction < 4; direction++) {
						neighbor_x = h + directionTable1 [direction, 0];
						neighbor_y = v + directionTable1 [direction, 1];

						neighbor_tile_value = MAP_LOCAL (neighbor_x, neighbor_y);

						if (neighbor_tile_value >= 0 && neighbor_tile_value + 1 < MAP_LOCAL (h, v)) {
							SET_MAP_LOCAL (h, v, (short)(neighbor_tile_value + 1));
							changed = true;
						}
					}
				}
			}
		}

		return changed;
	}

	protected bool GTMDistanceTransformBackward() {
		int h, v;
		int direction;
		int neighbor_x;
		int neighbor_y;
		bool changed = false;
		short current_tile_value;
		short neighbor_tile_value;

		for (v = RoadGenerator.roadDimensions - 2; v > 0; v--) {
			for (h = RoadGenerator.roadDimensions - 2; h > 0; h--) {
				current_tile_value = MAP_LOCAL (h, v);

				if (current_tile_value >= 0) {
					for (direction = 4; direction < 8; direction++) {
						neighbor_x = h + directionTable1 [direction, 0];
						neighbor_y = v + directionTable1 [direction, 1];

						neighbor_tile_value = MAP_LOCAL (neighbor_x, neighbor_y);

						if (neighbor_tile_value >= 0 && neighbor_tile_value + 1 < MAP_LOCAL (h, v)) {
							SET_MAP_LOCAL (h, v, (short)(neighbor_tile_value + 1));
							changed = true;
						}
					}
				}
			}
		}

		return changed;
	}

	protected void BeginSolveSystemGraph() {
		for (int i = 0; i < tileMap.Length; i++) {
			tileMap [i] = kMapUnreachable;
		}
	}

	protected void EndSolveSystemGraph() {
		bool changed;

		do {
			changed = false;

			if(GTMDistanceTransformForward())
			{
				changed = true;
			}

			if(GTMDistanceTransformBackward())
			{
				changed = true;
			}

		}while(changed);
	}


	public void SolveDistanceGraphForPlayer() {

		BeginSolveSystemGraph ();

		for (int y = 0; y < RoadGenerator.roadDimensions; y++) {
			for (int x = 0; x < RoadGenerator.roadDimensions; x++) {

				int roadTile = roadGenerator.roadMap [x, y];
				if (roadTile == 1) {
					SET_MAP_LOCAL (x, y, kMapMaxDistance);
				}
			}
		}

		int tileX, tileY;
		playerVisual.PlayerTile(out tileX, out tileY);
		SET_MAP_LOCAL (tileX, tileY, 0);


		EndSolveSystemGraph ();

		//PrintGraph ();
	}

	public void PrintGraph() {
		StringBuilder sb = new StringBuilder ();

		for (int v = 0; v < RoadGenerator.roadDimensions; v++) {
			for (int h = 0; h < RoadGenerator.roadDimensions; h++) {
				short current_tile_value = MAP_LOCAL (h, v);

				sb.AppendFormat ("{0},", current_tile_value);
			}

			sb.AppendFormat ("\n");
		}

		Debug.Log (sb);
	}

	public short GetDistanceGraphValue(int tileX, int tileY) {
		return MAP_LOCAL (tileX, tileY);
	}

}
