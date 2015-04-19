
using UnityEngine;
using System.Text;

public class PlayerVisual : CarController {

	MeshHelper mesh;

	protected const float KetchupDelay = 1;
	protected float KetchupTimer = KetchupDelay;

	public void UpdateGeometry(Vector3 velocity) {
		if (mesh != null) {
			mesh.Clear ();
		}

		mesh = new MeshHelper (1, "Game/truck", "LD32/TruckBillboard");


		float tileX = 0.0f;
		float tileY = 0.0f;

		if (velocity.x > 0.1f) {
			tileX = 0.0f;
			tileY = 0.5f;
		} else if (velocity.x < -0.1f) {
			tileX = 0.5f;
			tileY = 0.0f;
		} else if (velocity.z > 0.1f) {
			tileX = 0.0f;
			tileY = 0.0f;
		} else{
			tileX = 0.5f;
			tileY = 0.5f;
		}




		MeshQuad quad = new MeshQuad (
			transform.position,
			transform.position,
			transform.position,
			transform.position,
			new Vector2 (tileX+0.5f, tileY+0.5f),
			new Vector2 (tileX+0.0f, tileY+0.5f),
			new Vector2 (tileX+0.5f, tileY+0.0f),
			new Vector2 (tileX+0.0f, tileY+0.0f),
			new Vector2 (128, 0),
			new Vector2 (128, 0),
			new Vector2 (128, 0),
			new Vector2 (128, 0),
			new Vector3 (-0.77f, -0.77f, 0),
			new Vector3 (0.77f, -0.77f, 0),
			new Vector3 (-0.77f, 0.77f, 0),
			new Vector3 (0.77f, 0.77f, 0),
			Color.white,
			Color.white,
			Color.white,
			Color.white
		);

		mesh.AddQuad (quad);

		mesh.Commit ();
	}




	public void Update() {
		HandlePlayerControls ();
		UpdatePlayer ();
		UpdateCameraPosition ();
	}

	public bool CheckPlayerSpeed() {
		if ((int)playerSpeed == 0) {
			playerSpeed = MaximumSpeed ();
			NotificationCenter.postNotification (null, "SpawnAllEnemies");
			return false;
		}
		return true;
	}

	public bool MovePlayerInDirection(int x, int y){
		short d;

		// PLAYER CANNOT GO OPPOSITE DIRECTION THEY ARE ALREADY GOING
		float dot = Vector3.Dot(playerVector.normalized, new Vector3(x, 0, y).normalized);
		if (dot < -0.9f || dot > 0.9f) {
			return false;
		}

		if (CanPlayerGo (x, y, out d)) {
			playerVector = new Vector3 (x, 0, y);
			MarkPlayerTurned ();
			return true;
		}
		return false;
	}

	public void HandlePlayerControls() {

		if (KetchupTimer > 0) {
			KetchupTimer -= Time.deltaTime;
		} else {
			if (Input.GetKey (KeyCode.Space)) {
				NotificationCenter.postNotification (null, "UnconventionalWeaponActivate");
				KetchupTimer = KetchupDelay;
			}
		}


		if (Input.GetKey ("a") || Input.GetKey (KeyCode.LeftArrow)) {
			if (CheckPlayerSpeed ()) {
				if (!MovePlayerInDirection (-1, 0)) {
					MovePlayerInDirection (0, 1);
				}
			}
		} else if (Input.GetKey ("d") || Input.GetKey (KeyCode.RightArrow)) {
			if (CheckPlayerSpeed ()) {
				if (!MovePlayerInDirection (1, 0)) {
					MovePlayerInDirection (0, -1);
				}
			}
		} else if (Input.GetKey ("w") || Input.GetKey (KeyCode.UpArrow)) {
			if (CheckPlayerSpeed ()) {
				if (!MovePlayerInDirection (0, 1)) {
					MovePlayerInDirection (1, 0);
				}
			}
		} else if (Input.GetKey ("s") || Input.GetKey (KeyCode.DownArrow)) {
			if (CheckPlayerSpeed ()) {
				if (!MovePlayerInDirection (0, -1)) {
					MovePlayerInDirection (-1, 0);
				}
			}
		}
	}

	public override void UpdatePlayerVisuals() {
		transform.position = playerPosition;
		UpdateGeometry (playerVector);
	}

	public void UpdateCameraPosition () {
		Vector3 pos = transform.position;
		pos.y += 600;
		pos.z -= 600;
		Camera.main.transform.localPosition = new Vector3 (0, 700, -700);

		Camera.main.transform.LookAt (transform.position, new Vector3 (0, 1, 0));
	}
	
}
