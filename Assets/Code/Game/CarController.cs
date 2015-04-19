
using UnityEngine;
using System.Text;

public class CarController : NotificationBehaviour {

	protected float playerSpeed = 0;
	protected Vector3 playerVector = new Vector3(1, 0, 0);
	protected Vector3 playerPosition = new Vector3(0, 64, 0);

	protected int lastTurnTileX, lastTurnTileY;

	protected GameController gameController;
	protected RoadGenerator roadGenerator;

	protected float SpinningOutTimer = 0;

	public virtual float MaximumSpeed() {
		return 600;
	}

	public void CheckRoadGenerator() {
		if (roadGenerator == null) {
			gameController = PUCode.GetSingletonByName<GameController> ();
			roadGenerator = gameController.roadGenerator;
		}
	}

	public void SpinOut() {
		SpinningOutTimer = 4.0f;
	}

	public bool MovePlayerToTile(int x, int y){
		CheckRoadGenerator ();

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

	public bool CanPlayerTurnLeft(out short distanceValue) {
		int tileX, tileY;
		PlayerTile (out tileX, out tileY);

		if (tileX == lastTurnTileX && tileY == lastTurnTileY) {
			distanceValue = 32767;
			return false;
		}

		CheckRoadGenerator ();

		Vector3 leftVector = playerVector.RotateLeftAboutY ();
		tileX = Mathf.RoundToInt(tileX + leftVector.x);
		tileY = Mathf.RoundToInt(tileY + leftVector.z);

		distanceValue = gameController.GetDistanceGraphValue (tileX, tileY);

		if (roadGenerator.roadMap [tileX, tileY] == 1) {
			return true;
		}

		distanceValue = 32767;
		return false;
	}

	public bool CanPlayerTurnRight(out short distanceValue) {
		int tileX, tileY;
		PlayerTile (out tileX, out tileY);

		if (tileX == lastTurnTileX && tileY == lastTurnTileY) {
			distanceValue = 32767;
			return false;
		}

		CheckRoadGenerator ();

		Vector3 leftVector = playerVector.RotateRightAboutY ();
		tileX = Mathf.RoundToInt(tileX + leftVector.x);
		tileY = Mathf.RoundToInt(tileY + leftVector.z);

		distanceValue = gameController.GetDistanceGraphValue (tileX, tileY);

		if (roadGenerator.roadMap [tileX, tileY] == 1) {
			return true;
		}

		distanceValue = 32767;
		return false;
	}

	public bool IsPlayerOnARoad() {
		int tileX, tileY;
		PlayerTile (out tileX, out tileY);

		CheckRoadGenerator ();

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
		short distanceValue;

		// Check to see if the player is in danger of running off of the road
		playerPosition += playerVector * 48.0f;
		if (IsPlayerOnARoad () == false) {
			playerPosition = oldPlayerPosition;

			lastTurnTileX = lastTurnTileY = 99999;

			if (CanPlayerTurnLeft (out distanceValue)) {
				playerVector = playerVector.RotateLeftAboutY();
				MarkPlayerTurned ();
			} else if (CanPlayerTurnRight (out distanceValue)) {
				playerVector = playerVector.RotateRightAboutY();
				MarkPlayerTurned ();
			} else {
				playerVector = playerVector.RotateLeftAboutY();
				playerVector = playerVector.RotateLeftAboutY();
				MarkPlayerTurned ();
			}
		}
		playerPosition = oldPlayerPosition;

		if (SpinningOutTimer <= 0) {
			// Update the player visuals
			playerPosition += playerVector * playerSpeed * Time.deltaTime;
		} else {
			// car is spinning out of control...
			SpinningOutTimer -= Time.deltaTime;

			playerVector = playerVector.RotateLeftAboutY();
		}

		UpdatePlayerVisuals ();
	}

	public virtual void UpdatePlayerVisuals() {
		
	}

	public virtual void HandleCarAI() {
		// 0) If I can turn left, what is the distance graph value?
		// 1) If I can turn right, what is the distance graph value?
		// 2) If I can go straight, what is the distance graph value?

		// 3) 90% chance I choose to go down the path with the least distance graph value

		short leftDistanceValue;
		short rightDistanceValue;
		short straightDistanceValue = 32767;

		CanPlayerTurnLeft (out leftDistanceValue);
		CanPlayerTurnRight (out rightDistanceValue);

		Vector3 oldPlayerPosition = playerPosition;
		int tileX, tileY;
		playerPosition += playerVector * 48.0f;
		if (IsPlayerOnARoad () == true) {
			PlayerTile (out tileX, out tileY);
			straightDistanceValue = gameController.GetDistanceGraphValue (tileX, tileY);
		}
		playerPosition = oldPlayerPosition;

		if (leftDistanceValue < rightDistanceValue && leftDistanceValue < straightDistanceValue) {
			if (Random.Range (0, 100) < 90) {
				playerVector = playerVector.RotateLeftAboutY ();
			}
			MarkPlayerTurned ();
		}
		if (rightDistanceValue < leftDistanceValue && rightDistanceValue < straightDistanceValue) {
			if (Random.Range (0, 100) < 90) {
				playerVector = playerVector.RotateRightAboutY ();
			}
			MarkPlayerTurned ();
		}
	}
}
