
using UnityEngine;
using System.Text;
using System.Collections.Generic;

public partial class GameController : MonoBehaviour, IPUCode {

	public RoadGenerator roadGenerator;
	private MeshHelper roadMesh, roadMesh2, roadMesh3, roadMesh4, roadMesh5, roadMesh6, roadMesh7, roadMesh8, roadMesh9;

	public GameObject player;
	public PlayerVisual playerVisual;

	public List<GameObject> enemies = new List<GameObject>();
	public List<GameObject> turnips = new List<GameObject>();
	public List<GameObject> ketchupSpills = new List<GameObject>();

	public GameObject KetchupAnim;
	public GameObject KetchupSpill;
	public GameObject Turnip;

	public void Start() {

		roadGenerator = new RoadGenerator();

		roadGenerator.GenerateRoadMap ();

		CreateTiledRoad (roadGenerator);

		CreateScenary ();

		CreatePlayerObject ();

		AddEnemyOfType1 ();
		AddEnemyOfType1 ();


		NumberOfKetchupUses = 10;

		NumberOfCollectedTurnips = 0;
		for (int i = 0; i < 10; i++) {
			AddTurnip ();
		}

		NotificationCenter.addObserver (this, "UnconventionalWeaponActivate", null, (args, name) => {
			if(NumberOfKetchupUses > 0){
				GameObject localKetchupAnim = GameObject.Instantiate(KetchupAnim, player.transform.position, Quaternion.Euler(new Vector3(0,45,0))) as GameObject;
				localKetchupAnim.SetActive(true);
				LeanTween.delayedCall(1.0f, () => {
					GameObject.DestroyImmediate(localKetchupAnim);
				});

				Vector3 pos = player.transform.position;
				pos.y = 2;
				GameObject localKetchupSpill = GameObject.Instantiate(KetchupSpill, pos, Quaternion.Euler(new Vector3(90,320,0))) as GameObject;
				localKetchupSpill.SetActive(true);
				ketchupSpills.Add(localKetchupSpill);

				NumberOfKetchupUses--;
			}
		});
	}

	public void CreateScenary() {
		GameObject scenary = new GameObject ("Scenary");
		scenary.AddComponent<ScenaryVisual> ();
	}

	public void AddTurnip() {
		GameObject localTurnip = GameObject.Instantiate(Turnip, player.transform.position, Quaternion.Euler(new Vector3(60,45,0))) as GameObject;
		localTurnip.SetActive(true);

		FindRandomSpotForTurnip (localTurnip);

		turnips.Add (localTurnip);

		AddEdgeIndicator (player, localTurnip, "Game/turnip_indicator", 32);
	}

	public void AddEnemyOfType1() {
		GameObject enemy = new GameObject ("Enemy1");
		enemy.AddComponent<Enemy1Visual> ();

		FindRandomSpotForCar(enemy);

		enemies.Add (enemy);

		AddEdgeIndicator (player, enemy, "Game/car_indicator", 48);
	}

	public void CreatePlayerObject() {
		player = new GameObject ("Player");
		playerVisual = player.AddComponent<PlayerVisual> ();

		player.transform.transform.localEulerAngles = new Vector3 (0, 45, 0);
		
		Camera.main.transform.SetParent (player.transform, false);

		FindRandomSpotForCar(player);
	}

	public void FindRandomSpotForCar(GameObject car) {
		CarController controller = car.GetComponent<CarController> ();

		// Find a random, valid road space to put the player on
		for (int i = 0; i < 10000; i++) {
			int randX = Random.Range (1, RoadGenerator.roadDimensions - 1);
			int randY = Random.Range (1, RoadGenerator.roadDimensions - 1);

			if(controller.MovePlayerToTile (randX, randY)){
				// Cannot be too close to the player
				if (car != player && Vector3.Distance (car.transform.position, player.transform.position) < 4096) {
					continue;
				}
				break;
			}
		}
	}

	public void FindRandomSpotForTurnip(GameObject newTurnip) {
		
		// Find a random, valid road space to put the player on
		for (int i = 0; i < 10000; i++) {
			int randX = Random.Range (1, RoadGenerator.roadDimensions - 1);
			int randY = Random.Range (1, RoadGenerator.roadDimensions - 1);

			if(roadGenerator.roadMap [randX, randY] == 1){
				// Cannot be too close to any other turnips
				foreach (GameObject turnip in turnips) {
					if (Vector3.Distance (turnip.transform.position, newTurnip.transform.position) < 2048) {
						continue;
					}
				}

				newTurnip.transform.position = new Vector3 ((randX * 128), 32, (randY * 128));

				break;
			}
		}
	}
	
	public void CreateTiledRoad(RoadGenerator roadGenerator) {

		if (roadMesh != null) {
			roadMesh.Clear ();
		}
		if (roadMesh2 != null) {
			roadMesh2.Clear ();
		}
		if (roadMesh3 != null) {
			roadMesh3.Clear ();
		}
		if (roadMesh4 != null) {
			roadMesh4.Clear ();
		}
		if (roadMesh5 != null) {
			roadMesh5.Clear ();
		}
		if (roadMesh6 != null) {
			roadMesh6.Clear ();
		}
		if (roadMesh7 != null) {
			roadMesh7.Clear ();
		}
		if (roadMesh8 != null) {
			roadMesh8.Clear ();
		}
		if (roadMesh9 != null) {
			roadMesh9.Clear ();
		}

		roadMesh = new MeshHelper (RoadGenerator.roadDimensions * RoadGenerator.roadDimensions, "Game/tiles", "Mobile/Diffuse");
		roadMesh2 = new MeshHelper (RoadGenerator.roadDimensions * RoadGenerator.roadDimensions, "Game/tiles", "Mobile/Diffuse");
		roadMesh3 = new MeshHelper (RoadGenerator.roadDimensions * RoadGenerator.roadDimensions, "Game/tiles", "Mobile/Diffuse");
		roadMesh4 = new MeshHelper (RoadGenerator.roadDimensions * RoadGenerator.roadDimensions, "Game/tiles", "Mobile/Diffuse");
		roadMesh5 = new MeshHelper (RoadGenerator.roadDimensions * RoadGenerator.roadDimensions, "Game/tiles", "Mobile/Diffuse");
		roadMesh6 = new MeshHelper (RoadGenerator.roadDimensions * RoadGenerator.roadDimensions, "Game/tiles", "Mobile/Diffuse");
		roadMesh7 = new MeshHelper (RoadGenerator.roadDimensions * RoadGenerator.roadDimensions, "Game/tiles", "Mobile/Diffuse");
		roadMesh8 = new MeshHelper (RoadGenerator.roadDimensions * RoadGenerator.roadDimensions, "Game/tiles", "Mobile/Diffuse");
		roadMesh9 = new MeshHelper (RoadGenerator.roadDimensions * RoadGenerator.roadDimensions, "Game/tiles", "Mobile/Diffuse");

		for (int y = 0; y < RoadGenerator.roadDimensions; y++) {
			for (int x = 0; x < RoadGenerator.roadDimensions; x++) {
				
				int roadTile = roadGenerator.roadTilesMap [x, y];
				float tileX = (roadTile % 4) * 0.25f;
				float tileY = ((3 - roadTile / 4)) * 0.25f;

				roadMesh.AddQuad (
					new Vector2 (128, 128),
					new Vector3 (x * 128, 0, y * 128), 
					new Vector4 (tileX + 0.01f, tileY + 0.01f, tileX + 0.24f, tileY + 0.24f),
					Color.white,
					true);


				roadTile = 15;
				tileX = (roadTile % 4) * 0.25f;
				tileY = ((3 - roadTile / 4)) * 0.25f;

				roadMesh2.AddQuad (
					new Vector2 (128, 128),
					new Vector3 (x * 128, 0, y * 128), 
					new Vector4 (tileX + 0.01f, tileY + 0.01f, tileX + 0.24f, tileY + 0.24f),
					Color.white,
					true);

				roadMesh3.AddQuad (
					new Vector2 (128, 128),
					new Vector3 (x * 128, 0, y * 128), 
					new Vector4 (tileX + 0.01f, tileY + 0.01f, tileX + 0.24f, tileY + 0.24f),
					Color.white,
					true);

				roadMesh4.AddQuad (
					new Vector2 (128, 128),
					new Vector3 (x * 128, 0, y * 128), 
					new Vector4 (tileX + 0.01f, tileY + 0.01f, tileX + 0.24f, tileY + 0.24f),
					Color.white,
					true);

				roadMesh5.AddQuad (
					new Vector2 (128, 128),
					new Vector3 (x * 128, 0, y * 128), 
					new Vector4 (tileX + 0.01f, tileY + 0.01f, tileX + 0.24f, tileY + 0.24f),
					Color.white,
					true);

				roadMesh6.AddQuad (
					new Vector2 (128, 128),
					new Vector3 (x * 128, 0, y * 128), 
					new Vector4 (tileX + 0.01f, tileY + 0.01f, tileX + 0.24f, tileY + 0.24f),
					Color.white,
					true);

				roadMesh7.AddQuad (
					new Vector2 (128, 128),
					new Vector3 (x * 128, 0, y * 128), 
					new Vector4 (tileX + 0.01f, tileY + 0.01f, tileX + 0.24f, tileY + 0.24f),
					Color.white,
					true);
				
				roadMesh8.AddQuad (
					new Vector2 (128, 128),
					new Vector3 (x * 128, 0, y * 128), 
					new Vector4 (tileX + 0.01f, tileY + 0.01f, tileX + 0.24f, tileY + 0.24f),
					Color.white,
					true);

				roadMesh9.AddQuad (
					new Vector2 (128, 128),
					new Vector3 (x * 128, 0, y * 128), 
					new Vector4 (tileX + 0.01f, tileY + 0.01f, tileX + 0.24f, tileY + 0.24f),
					Color.white,
					true);
			}
		}

		roadMesh.Commit ();
		roadMesh2.Commit ();
		roadMesh3.Commit ();
		roadMesh4.Commit ();
		roadMesh5.Commit ();
		roadMesh6.Commit ();
		roadMesh7.Commit ();
		roadMesh8.Commit ();
		roadMesh9.Commit ();

		roadMesh2.gameObject.transform.localPosition = new Vector3 (RoadGenerator.roadDimensions * -128, 0, 0);
		roadMesh3.gameObject.transform.localPosition = new Vector3 (0, 0, RoadGenerator.roadDimensions * -128);
		roadMesh4.gameObject.transform.localPosition = new Vector3 (RoadGenerator.roadDimensions * 128, 0, 0);
		roadMesh5.gameObject.transform.localPosition = new Vector3 (0, 0, RoadGenerator.roadDimensions * 128);
		roadMesh6.gameObject.transform.localPosition = new Vector3 (RoadGenerator.roadDimensions * -128, 0, RoadGenerator.roadDimensions * 128);
		roadMesh7.gameObject.transform.localPosition = new Vector3 (RoadGenerator.roadDimensions * 128, 0, RoadGenerator.roadDimensions * -128);
		roadMesh8.gameObject.transform.localPosition = new Vector3 (RoadGenerator.roadDimensions * 128, 0, RoadGenerator.roadDimensions * 128);
		roadMesh9.gameObject.transform.localPosition = new Vector3 (RoadGenerator.roadDimensions * -128, 0, RoadGenerator.roadDimensions * -128);


		roadMesh.gameObject.AddComponent<BoxCollider> ();

		Rigidbody body = roadMesh.gameObject.AddComponent<Rigidbody> ();
		body.isKinematic = true;
	}
}
