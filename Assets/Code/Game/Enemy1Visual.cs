
using UnityEngine;
using System.Text;

public class Enemy1Visual : CarController {

	MeshHelper mesh;

	float bubbleWait = 2.0f;

	public override float MaximumSpeed() {
		return 610;
	}

	public void UpdateGeometry(Vector3 velocity) {
		if (mesh != null) {
			mesh.Clear ();
		}

		mesh = new MeshHelper (1, "Game/car1", "LD32/TruckBillboard");


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

		mesh.Commit (true);
	}


	public void Update() {

		bubbleWait -= Time.deltaTime;
		if (bubbleWait < 0) {
			bubbleWait = Random.Range (5, 10);

			SpawnChatBubble ();
		}

		playerSpeed = MaximumSpeed ();

		HandleCarAI();
		UpdatePlayer ();
	}

	public override void UpdatePlayerVisuals() {
		transform.position = playerPosition;
		UpdateGeometry (playerVector);
	}

	public void SpawnChatBubble() {
		int bubbleN = Random.Range (0, 2);
		GameObject bubble = gameController.Bubble0;

		switch (bubbleN) {
		case 0:
			bubble = gameController.Bubble0;
			break;
		case 1:
			bubble = gameController.Bubble1;
			break;
		case 2:
			bubble = gameController.Bubble2;
			break;
		}

		GameObject bubbleClone = GameObject.Instantiate (bubble, new Vector3 (0, 0, 0), Quaternion.Euler(60, 45, 0)) as GameObject;
		bubbleClone.gameObject.SetActive (true);
		bubbleClone.transform.SetParent (gameObject.transform, false);

		LeanTween.delayedCall (3.0f, () => {
			GameObject.DestroyImmediate(bubbleClone);
		});
	}

}
