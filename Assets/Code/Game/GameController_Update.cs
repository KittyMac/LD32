
using UnityEngine;
using System.Text;

public partial class GameController : MonoBehaviour, IPUCode {

	private float playerSpeed = 600;
	private Vector3 playerVector = new Vector3(1, 0, 0);
	private Vector3 playerPosition = new Vector3(0, 64, 0);

	private int lastTurnTileX, lastTurnTileY;

	public void Update() {

		UpdatePlayer ();
		UpdateCameraPosition ();

	}

	public bool MovePlayerToTile(int x, int y){
		if (roadGenerator.roadMap [x, y] == 1) {
			playerPosition = new Vector3 ((x * 128), 64, (y * 128));
			return true;
		}
		return false;
	}

	public void PlayerTile(out int x, out int y) {
		float fx = playerPosition.x / 128;
		float fy = playerPosition.z / 128;

		x = Mathf.RoundToInt (fx);
		y = Mathf.RoundToInt (fy);
	}

	public void MarkPlayerTurned() {
		PlayerTile (out lastTurnTileX, out lastTurnTileY);
	}

	public bool CanPlayerTurnLeft() {
		int tileX, tileY;
		PlayerTile (out tileX, out tileY);

		if (tileX == lastTurnTileX && tileY == lastTurnTileY) {
			return false;
		}

		Vector3 leftVector = playerVector.RotateLeftAboutY ();
		return (roadGenerator.roadMap [Mathf.RoundToInt(tileX + leftVector.x), Mathf.RoundToInt(tileY + leftVector.z)] == 1);
	}

	public bool CanPlayerTurnRight() {
		int tileX, tileY;
		PlayerTile (out tileX, out tileY);

		if (tileX == lastTurnTileX && tileY == lastTurnTileY) {
			return false;
		}

		Vector3 leftVector = playerVector.RotateRightAboutY ();
		return (roadGenerator.roadMap [Mathf.RoundToInt(tileX + leftVector.x), (int)(tileY + leftVector.z)] == 1);
	}

	public bool IsPlayerOnARoad() {
		int tileX, tileY;
		PlayerTile (out tileX, out tileY);
		return (roadGenerator.roadMap [tileX, tileY] == 1);
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

	public void ValidatePlayerPosition() {
		int tileX, tileY;
		int roadTile;

		PlayerTile (out tileX, out tileY);
		roadTile = roadGenerator.roadMap [tileX, tileY];
		DebugPlayerPos.text.text = string.Format ("({0}, {1}) :: {2}", tileX, tileY, roadTile);

		// We want to slowly drift the play back to the center of the road they are on...
		if (playerVector.x > 0.1f || playerVector.x < -0.1f) {
			float target = (tileY * 128);
			playerPosition.z += (target - playerPosition.z) * 0.123f;
		}

		if (playerVector.z > 0.1f || playerVector.z < -0.1f) {
			float target = (tileX * 128);
			playerPosition.x += (target - playerPosition.x) * 0.123f;
		}
	}

	public void UpdatePlayer () {

		HandlePlayerControls ();
		ValidatePlayerPosition ();

		Vector3 oldPlayerPosition = playerPosition;

		// Check to see if the player is in danger of running off of the road
		playerPosition += playerVector * 48.0f;
		if (IsPlayerOnARoad () == false) {
			playerPosition = oldPlayerPosition;

			if (CanPlayerTurnLeft ()) {
				playerVector = playerVector.RotateLeftAboutY();
				MarkPlayerTurned ();
			} else if (CanPlayerTurnRight ()) {
				playerVector = playerVector.RotateRightAboutY();
				MarkPlayerTurned ();
			} else {
				playerVector = playerVector.RotateLeftAboutY();
				playerVector = playerVector.RotateLeftAboutY();
				MarkPlayerTurned ();
			}
		}
		playerPosition = oldPlayerPosition;

		// Update the player visuals
		playerPosition += playerVector * playerSpeed * Time.deltaTime;

		player.transform.position = playerPosition;
		playerVisual.UpdateGeometry (playerVector);
	}


	public void UpdateCameraPosition () {
		Vector3 pos = player.transform.position;
		pos.y += 600;
		pos.z -= 600;
		Camera.main.transform.localPosition = new Vector3 (0, 200, -200);

		Camera.main.transform.LookAt (player.transform.position, new Vector3 (0, 1, 0));
	}
}
