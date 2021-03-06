
using UnityEngine;
using System.Text;
using System.Collections.Generic;

public partial class GameController : MonoBehaviour, IPUCode {

	static public int LevelNumber = 1;
	static public int PlayerScore = 0;
	static public int NumberOfTurnipsThisLevel = 10;


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

	public GameObject Bubble0;
	public GameObject Bubble1;
	public GameObject Bubble2;

	public PURawImage Vignette;
	public PUColor BlackCover;
	public PUText LevelDesc;
	public PUText PlayerScoreField;

	public bool GameIsOver = false;


	static public void StartNewGame() {
		LevelNumber = 1;
		PlayerScore = 0;
		NumberOfTurnipsThisLevel = 10;

		Application.LoadLevel ("01_Game");
	}

	static public void AdvanceNextLevel() {
		LevelNumber++;
		NumberOfTurnipsThisLevel = 10 + LevelNumber * 2;

		Application.LoadLevel ("01_Game");
	}

	public void AnimateIntro() {
		BlackCover.CheckCanvasGroup ();
		BlackCover.canvasGroup.alpha = 1.0f;
		LeanTween.alpha (BlackCover.gameObject, 0.0f, 0.44f);

		string levelDesc = string.Format("[b1]Level {0}[/b1]\n[p2]Collect All Turnips\nAvoid Selling Hot Dogs[/p2]", LevelNumber);
		LevelDesc.text.text = PlanetUnityStyle.ReplaceStyleTags (levelDesc);

		UpdateScoreField ();
	}

	public void EndIntro() {

		BlackCover.unload ();

		Vignette.CheckCanvasGroup ();
		Vignette.canvasGroup.alpha = 1.0f;
		LeanTween.alpha (Vignette.gameObject, 0.0f, 0.44f).setDestroyOnComplete (true);
	}

	public void Start() {

		GameIsOver = false;

		roadGenerator = new RoadGenerator();

		roadGenerator.GenerateRoadMap (LevelNumber);

		CreateTiledRoad (roadGenerator);

		CreateScenary ();

		CreatePlayerObject ();


		NumberOfKetchupUses = 10;

		NumberOfCollectedTurnips = 0;
		for (int i = 0; i < NumberOfTurnipsThisLevel; i++) {
			AddTurnip ();
		}

		NotificationCenter.addObserver (this, "SpawnAllEnemies", null, (args, name) => {
			if(enemies.Count == 0){
				for(int i = 0; i < (LevelNumber); i++){
					AddEnemyOfType1 ();
				}

				EndIntro();
			}
		});

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

		AnimateIntro ();
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

				Vector3 newCarPosition = new Vector3 ((randX * 128), 64, (randY * 128));
				float d = Vector3.Distance (newCarPosition, player.transform.position);

				if (car != player && d < 3648) {
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
				newTurnip.transform.position = new Vector3 ((randX * 128), 32, (randY * 128));

				bool spotIsGood = true;
				foreach (GameObject turnip in turnips) {
					float d = Vector3.Distance (turnip.transform.position, newTurnip.transform.position);
					if (d < 1024) {
						spotIsGood = false;
						break;
					}
				}

				if (!spotIsGood) {
					continue;
				}

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

		roadMesh = new MeshHelper (RoadGenerator.roadDimensions * RoadGenerator.roadDimensions, "Game/tiles", "Unlit/Texture");
		roadMesh2 = new MeshHelper (RoadGenerator.roadDimensions * RoadGenerator.roadDimensions, "Game/tiles", "Unlit/Texture");
		roadMesh3 = new MeshHelper (RoadGenerator.roadDimensions * RoadGenerator.roadDimensions, "Game/tiles", "Unlit/Texture");
		roadMesh4 = new MeshHelper (RoadGenerator.roadDimensions * RoadGenerator.roadDimensions, "Game/tiles", "Unlit/Texture");
		roadMesh5 = new MeshHelper (RoadGenerator.roadDimensions * RoadGenerator.roadDimensions, "Game/tiles", "Unlit/Texture");
		roadMesh6 = new MeshHelper (RoadGenerator.roadDimensions * RoadGenerator.roadDimensions, "Game/tiles", "Unlit/Texture");
		roadMesh7 = new MeshHelper (RoadGenerator.roadDimensions * RoadGenerator.roadDimensions, "Game/tiles", "Unlit/Texture");
		roadMesh8 = new MeshHelper (RoadGenerator.roadDimensions * RoadGenerator.roadDimensions, "Game/tiles", "Unlit/Texture");
		roadMesh9 = new MeshHelper (RoadGenerator.roadDimensions * RoadGenerator.roadDimensions, "Game/tiles", "Unlit/Texture");

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
