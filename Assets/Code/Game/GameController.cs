
using UnityEngine;
using System.Text;

public class GameController : MonoBehaviour, IPUCode {

	private RoadGenerator roadGenerator;
	private MeshHelper roadMesh;
	
	public void Start() {

		roadGenerator = new RoadGenerator();

		roadGenerator.GenerateRoadMap ();

		CreateTiledRoad (roadGenerator);

	}
	
	public void CreateTiledRoad(RoadGenerator roadGenerator) {

		if (roadMesh != null) {
			roadMesh.Clear ();
		}

		roadMesh = new MeshHelper (RoadGenerator.roadDimensions * RoadGenerator.roadDimensions, "Game/tiles", "Mobile/Diffuse");

		for (int y = 0; y < RoadGenerator.roadDimensions; y++) {
			for (int x = 0; x < RoadGenerator.roadDimensions; x++) {
				
				int roadTile = roadGenerator.roadTilesMap [x, y];
				float tileX = roadTile % 4;
				float tileY = (3 - roadTile / 4);

				tileX *= 0.25f;
				tileY *= 0.25f;

				roadMesh.AddQuad (
					new Vector2 (128, 128),
					new Vector3 (x * 128, 0, y * 128), 
					new Vector4 (tileX, tileY, tileX + 0.25f, tileY + 0.25f),
					Color.white,
					true);
			}
		}

		roadMesh.Commit ();
	}
}
