
using UnityEngine;
using System.Text;

public class PlayerVisual : CarController {

	MeshHelper mesh;

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

	public void HandlePlayerControls() {
		// Movement by AWSD keyboard
		if (Input.GetKey ("w")) {
			// Speed up
			playerSpeed += 50.0f * Time.deltaTime;
			if (playerSpeed > 600.0f) {
				playerSpeed = 600.0f;
			}
		} else if (Input.GetKey ("s")) {
			// Slow down
			playerSpeed -= 200.0f * Time.deltaTime;
			if (playerSpeed < 0) {
				playerSpeed = 0;
			}
		}

		// Handle player turning
		// 0) Find my current tile
		// 1) If the tile to my left is road, allow a turn
		// 2) If the tile to my right is a road, allow a turn
		if (Input.GetKey ("a")) {
			if (CanPlayerTurnLeft()) {
				playerVector = playerVector.RotateLeftAboutY ();
				MarkPlayerTurned ();
			}
		} else if (Input.GetKey ("d")) {
			if (CanPlayerTurnRight()) {
				playerVector = playerVector.RotateRightAboutY ();
				MarkPlayerTurned ();
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
		Camera.main.transform.localPosition = new Vector3 (0, 200, -200);

		Camera.main.transform.LookAt (transform.position, new Vector3 (0, 1, 0));
	}
	
}
