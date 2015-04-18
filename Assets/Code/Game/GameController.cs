
using UnityEngine;
using System.Text;

public partial class GameController : MonoBehaviour, IPUCode {

	public PUText DebugPlayerPos;

	private RoadGenerator roadGenerator;
	private MeshHelper roadMesh;

	private GameObject player;
	private PlayerVisual playerVisual;
	
	public void Start() {

		roadGenerator = new RoadGenerator();

		roadGenerator.GenerateRoadMap ();

		CreateTiledRoad (roadGenerator);

		CreatePlayerObject ();
	}

	public void CreatePlayerObject() {
		player = new GameObject ("Player");
		playerVisual = player.AddComponent<PlayerVisual> ();

		player.transform.transform.localEulerAngles = new Vector3 (0, 45, 0);
		
		Camera.main.transform.SetParent (player.transform, false);

		// Find a random, valid road space to put the player on
		for (int i = 0; i < 10000; i++) {
			int randX = Random.Range (1, RoadGenerator.roadDimensions - 1);
			int randY = Random.Range (1, RoadGenerator.roadDimensions - 1);

			if(MovePlayerToTile (randX, randY)){
				break;
			}
		}
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

		BoxCollider collider = roadMesh.gameObject.AddComponent<BoxCollider> ();

		Rigidbody body = roadMesh.gameObject.AddComponent<Rigidbody> ();
		body.isKinematic = true;
	}
}
