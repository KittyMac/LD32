
using UnityEngine;
using System.Text;
using System.Collections.Generic;

public partial class GameController : MonoBehaviour, IPUCode {

	public int NumberOfCollectedTurnips = 0;

	public PUGameObject EdgeOfScreenIndicators;

	public void CollectTurnip(GameObject turnip) {

		NumberOfCollectedTurnips++;

		GameObject.Destroy (turnip);

		turnips.Remove (turnip);
	}

	public void AddEdgeIndicator(GameObject from, GameObject to, string imagePath) {
		PUEdgeIndicator i = new PUEdgeIndicator ();
		i.SetFrame (0, 0, 64, 64, 0.5f, 0.5f, "bottom,left");
		i.resourcePath = imagePath;
		i.from = from;
		i.to = to;
		i.LoadIntoPUGameObject (EdgeOfScreenIndicators);
	}

	public void Update() {

		// Solve the distance graph; this is use by the AI to know how to move towards the player
		SolveDistanceGraphForPlayer ();

		// Does the player collide with an enemy?
		Vector3 playerPos = player.gameObject.transform.position;


		List<GameObject> ketchupSpillsToRemove = new List<GameObject> ();


		// Player car vs turnips
		foreach (GameObject turnip in new List<GameObject>(turnips)) {
			float d = Vector3.Distance (turnip.transform.position, playerPos);
			if (d < 98) {
				CollectTurnip (turnip);
			}
		}

		foreach (GameObject enemy in enemies) {
			Vector3 enemyPos = enemy.gameObject.transform.position;

			// Enemy vs player car
			if (Vector3.Distance (playerPos, enemyPos) < 78) {
				Debug.Log ("GAME OVER!");
			}

			// Test enemy cars vs ketchup spills
			foreach (GameObject spill in new List<GameObject>(ketchupSpills)) {
				float d = Vector3.Distance (spill.gameObject.transform.position, enemyPos);
				if (d < 78) {
					enemy.SendMessage ("SpinOut");

					ketchupSpillsToRemove.Add (spill);
				}
			}
		}

		foreach (GameObject spill in ketchupSpillsToRemove) {
			ketchupSpills.Remove (spill);
			LeanTween.alpha (spill, 0.0f, 3.0f).setDestroyOnComplete (true);
		}

	}

}
