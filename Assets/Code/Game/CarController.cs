
using UnityEngine;
using System.Text;

public class CarController : NotificationBehaviour {

	protected float playerSpeed = 600;
	protected Vector3 playerVector = new Vector3(1, 0, 0);
	protected Vector3 playerPosition = new Vector3(0, 64, 0);

	protected int lastTurnTileX, lastTurnTileY;

	protected RoadGenerator roadGenerator;

	public void CheckRoadGenerator() {
		if (roadGenerator == null) {
			roadGenerator = PUCode.GetSingletonByName<GameController> ().roadGenerator;
		}
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
	
	public void ValidatePlayerPosition() {
		int tileX, tileY;
		PlayerTile (out tileX, out tileY);

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

		UpdatePlayerVisuals ();
	}

	public virtual void UpdatePlayerVisuals() {
		
	}
}
