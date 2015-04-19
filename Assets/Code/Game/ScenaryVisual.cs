
using UnityEngine;
using System.Text;
using System.IO;

public class ScenaryVisual : MonoBehaviour {

	MeshHelper mesh;

	protected GameController gameController;
	protected RoadGenerator roadGenerator;

	public void CheckRoadGenerator() {
		if (roadGenerator == null) {
			gameController = PUCode.GetSingletonByName<GameController> ();
			roadGenerator = gameController.roadGenerator;
		}
	}

	public void AddScenary(int x, int y, Sprite sprite) {

		Rect UVs = sprite.rect;
		UVs.x /= sprite.texture.width;
		UVs.width /= sprite.texture.width;
		UVs.y /= sprite.texture.height;
		UVs.height /= sprite.texture.height;

		float scaleW = UVs.width * sprite.texture.width;
		float scaleH = UVs.height * sprite.texture.height;

		Vector3 pos = new Vector3 (x * 128, 0, y * 128);


		MeshQuad quad = new MeshQuad (
			                pos,
			                pos,
			                pos,
			                pos,
			                new Vector2 (UVs.x + UVs.width, UVs.y + UVs.height),
			                new Vector2 (UVs.x, UVs.y + UVs.height),
			                new Vector2 (UVs.x + UVs.width, UVs.y),
			                new Vector2 (UVs.x, UVs.y),
			                new Vector2 (scaleW, scaleH),
			                new Vector2 (scaleW, scaleH),
			                new Vector2 (scaleW, scaleH),
			                new Vector2 (scaleW, scaleH),
			                new Vector3 (-0.77f, -1.54f, 0),
			                new Vector3 (0.77f, -1.54f, 0),
			                new Vector3 (-0.77f, 0.0f, 0),
			                new Vector3 (0.77f, 0.0f, 0),
			                Color.white,
			                Color.white,
			                Color.white,
			                Color.white
		                );

		mesh.AddQuad (quad);

	}

	public void Start() {

		CheckRoadGenerator ();

		if (mesh != null) {
			mesh.Clear ();
		}

		Sprite[] allSprites = Resources.LoadAll<Sprite>(Path.GetDirectoryName("Game/scenary/scenary"));


		int nQuads = 0;
		for (int y = RoadGenerator.roadDimensions-1; y >= 0; y--) {
			for (int x = RoadGenerator.roadDimensions-1; x >= 0; x--) {

				int roadTile = roadGenerator.roadMap [x, y];
				if (roadTile > 1) {
					nQuads++;
				}
			}
		}

		mesh = new MeshHelper (nQuads, "Game/scenary/scenary", "LD32/Billboard");

		for (int y = RoadGenerator.roadDimensions-1; y >= 0; y--) {
			for (int x = RoadGenerator.roadDimensions-1; x >= 0; x--) {

				int roadTile = roadGenerator.roadMap [x, y];
				if (roadTile > 1) {
					AddScenary (x, y, allSprites [roadTile - 2]);
				}
			}
		}




		mesh.Commit (true);
	}
	
}
